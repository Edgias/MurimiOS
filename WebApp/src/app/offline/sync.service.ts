import { HttpClient } from '@angular/common/http';
import { Injectable, signal } from '@angular/core';
import { firstValueFrom } from 'rxjs';
import { ApiClient } from '../core/api-client';
import { OfflineStore, QueuedCommand, QueuedMedia } from './offline-store';

interface PushResult { commandId: string; status: 'Applied' | 'Duplicate' | 'Rejected'; errorCode?: string; errorMessage?: string; }
interface PushResponse { expectedNextSequence: number; results: PushResult[]; }
interface PullResponse { checkpoint: number; hasMore: boolean; changes: { sequence: number; payload: unknown }[]; }
interface UploadSessionResponse { uploadId: string; evidenceId: string; offset: number; chunkSize: number; completed: boolean; }
interface UploadOffsetResponse { offset: number; }

@Injectable({ providedIn: 'root' })
export class SyncService {
  readonly online = signal(navigator.onLine);
  readonly syncing = signal(false);
  readonly pendingCount = signal(0);
  readonly message = signal('');
  readonly lastSyncLabel = signal(localStorage.getItem('murimios.lastSync') ?? 'Never');

  private readonly store = new OfflineStore();

  constructor(private readonly http: HttpClient, private readonly api: ApiClient) {
    window.addEventListener('online', () => {
      this.online.set(true);
      void this.synchronize();
    });
    window.addEventListener('offline', () => {
      this.online.set(false);
      this.message.set('Offline mode active. New work will remain safely queued on this device.');
    });
  }

  async initialize(): Promise<void> {
    const tenantId = this.api.tenantId;
    await this.store.initializeIdentity(tenantId);
    await this.refreshCount(tenantId);
    if (navigator.onLine) void this.synchronize();
  }

  async queue(type: string, payload: unknown): Promise<void> {
    const tenantId = this.api.tenantId;
    await this.store.enqueue(tenantId, type, payload);
    await this.refreshCount(tenantId);
    this.message.set('Work saved on this device.');
    if (navigator.onLine) void this.synchronize();
  }

  async queueMedia(
    cropZoneId: string,
    file: File,
    location?: { latitude: number; longitude: number; accuracyMetres?: number }
  ): Promise<void> {
    const tenantId = this.api.tenantId;
    await this.store.enqueueMedia(tenantId, cropZoneId, file, location);
    await this.refreshCount(tenantId);
    this.message.set('Photo evidence saved on this device.');
    if (navigator.onLine) void this.synchronize();
  }

  async synchronize(): Promise<void> {
    if (!navigator.onLine || this.syncing()) return;
    const tenantId = this.api.tenantId;
    this.syncing.set(true);
    this.message.set('Synchronizing device outbox…');

    try {
      const pending = await this.store.pending(tenantId);
      if (pending.length > 0) await this.push(tenantId, pending);
      await this.uploadMedia(tenantId);
      await this.pull(tenantId);
      const label = new Date().toLocaleString();
      localStorage.setItem('murimios.lastSync', label);
      this.lastSyncLabel.set(label);
      this.message.set('Synchronization complete.');
    } catch (error) {
      this.message.set(error instanceof Error ? error.message : 'Synchronization paused; queued work is still safe.');
    } finally {
      this.syncing.set(false);
      await this.refreshCount(this.api.tenantId);
    }
  }

  private async push(tenantId: string, commands: QueuedCommand[]): Promise<void> {
    const response = await firstValueFrom(this.http.post<PushResponse>(
      `${this.api.baseUrl}/sync/push`,
      { deviceId: commands[0].deviceId, commands },
      { headers: this.api.headers(tenantId) }
    ));

    for (const result of response.results) {
      if (result.status === 'Applied' || result.status === 'Duplicate') {
        await this.store.remove(result.commandId);
      } else {
        await this.store.reject(result.commandId, result.errorCode, result.errorMessage);
      }
    }
  }

  private async pull(tenantId: string): Promise<void> {
    let checkpoint = await this.store.getCheckpoint(tenantId);
    let hasMore = true;
    while (hasMore) {
      const response = await firstValueFrom(this.http.get<PullResponse>(
        `${this.api.baseUrl}/sync/pull`,
        { params: { checkpoint, limit: 200 }, headers: this.api.headers(tenantId) }
      ));
      await this.store.saveChanges(tenantId, response.changes, response.checkpoint);
      checkpoint = response.checkpoint;
      hasMore = response.hasMore;
    }
  }

  private async uploadMedia(tenantId: string): Promise<void> {
    const items = await this.store.pendingMedia(tenantId);
    for (const media of items) {
      await this.uploadOne(tenantId, media);
    }
  }

  private async uploadOne(tenantId: string, media: QueuedMedia): Promise<void> {
    const session = await firstValueFrom(this.http.post<UploadSessionResponse>(
      `${this.api.baseUrl}/media/uploads`,
      {
        evidenceId: media.evidenceId,
        cropZoneId: media.cropZoneId,
        fileName: media.fileName,
        contentType: media.contentType,
        byteSize: media.byteSize,
        sha256: media.sha256,
        capturedAt: media.capturedAt,
        latitude: media.latitude ?? null,
        longitude: media.longitude ?? null,
        accuracyMetres: media.accuracyMetres ?? null
      },
      { headers: this.api.headers(tenantId) }
    ));

    if (session.completed) {
      await this.store.removeMedia(media.evidenceId);
      return;
    }

    let offset = session.offset;
    await this.store.saveMediaProgress(media.evidenceId, session.uploadId, offset);
    while (offset < media.byteSize) {
      const chunk = media.blob.slice(offset, Math.min(offset + session.chunkSize, media.byteSize));
      const headers = this.api.headers(tenantId)
        .set('Content-Type', 'application/octet-stream')
        .set('Upload-Offset', offset.toString());
      const response = await firstValueFrom(this.http.put<UploadOffsetResponse>(
        `${this.api.baseUrl}/media/uploads/${session.uploadId}`,
        chunk,
        { headers }
      ));
      offset = response.offset;
      await this.store.saveMediaProgress(media.evidenceId, session.uploadId, offset);
    }

    await firstValueFrom(this.http.post(
      `${this.api.baseUrl}/media/uploads/${session.uploadId}/complete`,
      {},
      { headers: this.api.headers(tenantId) }
    ));
    await this.store.removeMedia(media.evidenceId);
  }

  private async refreshCount(tenantId: string): Promise<void> {
    const [commands, media] = await Promise.all([
      this.store.pendingCount(tenantId),
      this.store.mediaPendingCount(tenantId)
    ]);
    this.pendingCount.set(commands + media);
  }
}
