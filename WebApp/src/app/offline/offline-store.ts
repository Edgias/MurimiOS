export type CommandStatus = 'Pending' | 'Rejected';

export interface QueuedCommand {
  commandId: string;
  tenantId: string;
  deviceId: string;
  sequence: number;
  type: string;
  schemaVersion: 1;
  capturedAt: string;
  payload: unknown;
  status: CommandStatus;
  errorCode?: string;
  errorMessage?: string;
}

interface Setting { key: string; value: string | number; }
interface StoredChange { id: string; tenantId: string; sequence: number; payload: unknown; }

export interface QueuedMedia {
  evidenceId: string;
  tenantId: string;
  cropZoneId: string;
  fileName: string;
  contentType: string;
  byteSize: number;
  sha256: string;
  capturedAt: string;
  latitude?: number;
  longitude?: number;
  accuracyMetres?: number;
  blob: Blob;
  uploadId?: string;
  offset: number;
}

export class OfflineStore {
  private readonly database = this.open();

  async initializeIdentity(tenantId: string): Promise<{ deviceId: string; nextSequence: number }> {
    const db = await this.database;
    const deviceKey = `deviceId:${tenantId}`;
    const sequenceKey = `nextSequence:${tenantId}`;
    const device = await this.getSetting(deviceKey);
    const sequence = await this.getSetting(sequenceKey);
    const deviceId = device?.value.toString() ?? crypto.randomUUID();
    const nextSequence = Number(sequence?.value ?? 1);

    if (!device) {
      await this.complete(db.transaction('settings', 'readwrite'), transaction => {
        transaction.objectStore('settings').put({ key: deviceKey, value: deviceId } satisfies Setting);
        transaction.objectStore('settings').put({ key: sequenceKey, value: nextSequence } satisfies Setting);
      });
    }

    return { deviceId, nextSequence };
  }

  async enqueue(tenantId: string, type: string, payload: unknown): Promise<QueuedCommand> {
    const db = await this.database;
    const identity = await this.initializeIdentity(tenantId);
    const command: QueuedCommand = {
      commandId: crypto.randomUUID(),
      tenantId,
      deviceId: identity.deviceId,
      sequence: identity.nextSequence,
      type,
      schemaVersion: 1,
      capturedAt: new Date().toISOString(),
      payload,
      status: 'Pending'
    };

    await this.complete(db.transaction(['commands', 'settings'], 'readwrite'), transaction => {
      transaction.objectStore('commands').add(command);
      transaction.objectStore('settings').put({
        key: `nextSequence:${tenantId}`,
        value: command.sequence + 1
      } satisfies Setting);
    });
    return command;
  }

  async pending(tenantId: string, limit = 100): Promise<QueuedCommand[]> {
    const all = await this.allCommands();
    return all
      .filter(item => item.tenantId === tenantId && item.status === 'Pending')
      .sort((a, b) => a.sequence - b.sequence)
      .slice(0, limit);
  }

  async pendingCount(tenantId: string): Promise<number> {
    return (await this.pending(tenantId, Number.MAX_SAFE_INTEGER)).length;
  }

  async enqueueMedia(
    tenantId: string,
    cropZoneId: string,
    file: File,
    location?: { latitude: number; longitude: number; accuracyMetres?: number }
  ): Promise<QueuedMedia> {
    const db = await this.database;
    const digest = await crypto.subtle.digest('SHA-256', await file.arrayBuffer());
    const sha256 = Array.from(new Uint8Array(digest), value => value.toString(16).padStart(2, '0')).join('');
    const media: QueuedMedia = {
      evidenceId: crypto.randomUUID(),
      tenantId,
      cropZoneId,
      fileName: file.name || `evidence-${Date.now()}.jpg`,
      contentType: file.type || 'image/jpeg',
      byteSize: file.size,
      sha256,
      capturedAt: new Date().toISOString(),
      latitude: location?.latitude,
      longitude: location?.longitude,
      accuracyMetres: location?.accuracyMetres,
      blob: file,
      offset: 0
    };

    await this.complete(db.transaction('media', 'readwrite'), transaction => {
      transaction.objectStore('media').add(media);
    });
    return media;
  }

  async pendingMedia(tenantId: string): Promise<QueuedMedia[]> {
    const db = await this.database;
    const media = await this.request<QueuedMedia[]>(db.transaction('media').objectStore('media').getAll());
    return media.filter(item => item.tenantId === tenantId).sort((a, b) => a.capturedAt.localeCompare(b.capturedAt));
  }

  async mediaPendingCount(tenantId: string): Promise<number> {
    return (await this.pendingMedia(tenantId)).length;
  }

  async saveMediaProgress(evidenceId: string, uploadId: string, offset: number): Promise<void> {
    const db = await this.database;
    const store = db.transaction('media').objectStore('media');
    const media = await this.request<QueuedMedia | undefined>(store.get(evidenceId));
    if (!media) return;
    media.uploadId = uploadId;
    media.offset = offset;
    await this.complete(db.transaction('media', 'readwrite'), transaction => {
      transaction.objectStore('media').put(media);
    });
  }

  async removeMedia(evidenceId: string): Promise<void> {
    const db = await this.database;
    await this.complete(db.transaction('media', 'readwrite'), transaction => {
      transaction.objectStore('media').delete(evidenceId);
    });
  }

  async remove(commandId: string): Promise<void> {
    const db = await this.database;
    await this.complete(db.transaction('commands', 'readwrite'), transaction => transaction.objectStore('commands').delete(commandId));
  }

  async reject(commandId: string, errorCode?: string, errorMessage?: string): Promise<void> {
    const db = await this.database;
    const command = await this.getCommand(commandId);
    if (!command) return;
    command.status = 'Rejected';
    command.errorCode = errorCode;
    command.errorMessage = errorMessage;
    await this.complete(db.transaction('commands', 'readwrite'), transaction => transaction.objectStore('commands').put(command));
  }

  async getCheckpoint(tenantId: string): Promise<number> {
    const setting = await this.getSetting(`checkpoint:${tenantId}`);
    return Number(setting?.value ?? 0);
  }

  async saveChanges(tenantId: string, changes: Omit<StoredChange, 'id' | 'tenantId'>[], checkpoint: number): Promise<void> {
    const db = await this.database;
    await this.complete(db.transaction(['changes', 'settings'], 'readwrite'), transaction => {
      const store = transaction.objectStore('changes');
      for (const change of changes) {
        store.put({ ...change, id: `${tenantId}:${change.sequence}`, tenantId } satisfies StoredChange);
      }
      transaction.objectStore('settings').put({ key: `checkpoint:${tenantId}`, value: checkpoint } satisfies Setting);
    });
  }

  private async allCommands(): Promise<QueuedCommand[]> {
    const db = await this.database;
    return this.request(db.transaction('commands').objectStore('commands').getAll());
  }

  private async getCommand(commandId: string): Promise<QueuedCommand | undefined> {
    const db = await this.database;
    return this.request(db.transaction('commands').objectStore('commands').get(commandId));
  }

  private async getSetting(key: string): Promise<Setting | undefined> {
    const db = await this.database;
    return this.request(db.transaction('settings').objectStore('settings').get(key));
  }

  private request<T>(request: IDBRequest<T>): Promise<T> {
    return new Promise((resolve, reject) => {
      request.onsuccess = () => resolve(request.result);
      request.onerror = () => reject(request.error);
    });
  }

  private complete(transaction: IDBTransaction, action: (transaction: IDBTransaction) => void): Promise<void> {
    return new Promise((resolve, reject) => {
      action(transaction);
      transaction.oncomplete = () => resolve();
      transaction.onerror = () => reject(transaction.error);
    });
  }

  private open(): Promise<IDBDatabase> {
    return new Promise((resolve, reject) => {
      const request = indexedDB.open('murimios-field', 3);
      request.onupgradeneeded = event => {
        const db = request.result;
        const oldVersion = (event as IDBVersionChangeEvent).oldVersion;
        if (!db.objectStoreNames.contains('commands')) {
          const commands = db.createObjectStore('commands', { keyPath: 'commandId' });
          commands.createIndex('tenantDeviceSequence', ['tenantId', 'deviceId', 'sequence'], { unique: true });
          commands.createIndex('status', 'status');
        } else if (oldVersion < 2) {
          const commands = request.transaction!.objectStore('commands');
          if (commands.indexNames.contains('deviceSequence')) commands.deleteIndex('deviceSequence');
          commands.createIndex('tenantDeviceSequence', ['tenantId', 'deviceId', 'sequence'], { unique: true });
          const legacyTenantId = localStorage.getItem('murimios.tenantId') ?? '11111111-1111-1111-1111-111111111111';
          commands.openCursor().onsuccess = event => {
            const cursor = (event.target as IDBRequest<IDBCursorWithValue | null>).result;
            if (!cursor) return;
            cursor.update({ ...cursor.value, tenantId: legacyTenantId });
            cursor.continue();
          };
        }
        if (!db.objectStoreNames.contains('settings')) db.createObjectStore('settings', { keyPath: 'key' });
        if (oldVersion < 2 && db.objectStoreNames.contains('changes')) db.deleteObjectStore('changes');
        if (!db.objectStoreNames.contains('changes')) db.createObjectStore('changes', { keyPath: 'id' });
        if (!db.objectStoreNames.contains('media')) {
          const media = db.createObjectStore('media', { keyPath: 'evidenceId' });
          media.createIndex('tenantCapturedAt', ['tenantId', 'capturedAt']);
        }
      };
      request.onsuccess = () => resolve(request.result);
      request.onerror = () => reject(request.error);
    });
  }
}
