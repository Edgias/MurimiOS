import { DecimalPipe } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { AgriculturalInput, ApiClient, CropZone, Farm, FarmField, GrowingSeason, InputStockLot, LivestockSubject, VeterinaryMedicine, VeterinaryMedicineStockLot } from './core/api-client';
import { SyncService } from './offline/sync.service';

@Component({
  selector: 'app-root',
  imports: [DecimalPipe, FormsModule],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  protected readonly sync = inject(SyncService);
  private readonly api = inject(ApiClient);

  protected tenantId = this.api.tenantId;
  protected farms: Farm[] = [];
  protected fields: FarmField[] = [];
  protected seasons: GrowingSeason[] = [];
  protected cropZones: CropZone[] = [];
  protected inputs: AgriculturalInput[] = [];
  protected stockLots: InputStockLot[] = [];
  protected subjects: LivestockSubject[] = [];
  protected medicines: VeterinaryMedicine[] = [];
  protected medicineStockLots: VeterinaryMedicineStockLot[] = [];
  protected busy = false;
  protected notice = '';

  protected farmDraft = { name: 'Mupfumi Farm', code: 'MUPFUMI', countryCode: 'ZW', timeZone: 'Africa/Harare' };
  protected fieldDraft = {
    farmId: '',
    name: 'North Field',
    validFrom: new Date().toISOString().slice(0, 10),
    coordinates: '31.0000,-17.8000\n31.0020,-17.8000\n31.0020,-17.8020\n31.0000,-17.8020'
  };
  protected seasonDraft = {
    farmId: '',
    name: `Main ${new Date().getFullYear()}`,
    startsOn: new Date().toISOString().slice(0, 10),
    endsOn: new Date(new Date().setMonth(new Date().getMonth() + 6)).toISOString().slice(0, 10),
    status: 2
  };
  protected cropZoneDraft = { seasonId: '', fieldId: '', boundaryVersionId: '', name: 'North maize', crop: 'Maize', variety: '' };
  protected inputDraft = { farmId: '', name: 'Compound D fertiliser', category: 'Fertiliser', unit: 'kg' };
  protected stockLotDraft = { inputId: '', lotCode: `INPUT-${new Date().getFullYear()}-001`, receivedOn: new Date().toISOString().slice(0, 10), expiresOn: '', initialQuantity: 1000 };
  protected inputApplicationDraft = { cropZoneId: '', stockLotId: '', quantity: 50, notes: '' };
  protected harvestDraft = { cropZoneId: '', lotCode: `HARVEST-${new Date().getFullYear()}-001`, quantity: 1000, unit: 'kg', notes: '' };
  protected evidenceDraft = { cropZoneId: '', latitude: undefined as number | undefined, longitude: undefined as number | undefined, accuracyMetres: undefined as number | undefined };
  protected evidenceFile?: File;
  protected evidenceFileName = '';
  protected evidenceLocationLabel = 'No GPS position attached';
  protected traceSummary = '';
  protected subjectDraft = { farmId: '', kind: 1, name: 'Animal 001', species: 'Cattle', officialIdentifier: '' };
  protected operationDraft = { cropZoneId: '', operationType: 'Scouting', notes: '' };
  protected measurementDraft = { subjectId: '', metric: 'Weight', value: 0, unit: 'kg' };
  protected medicineDraft = { farmId: '', name: 'Oxytetracycline', activeIngredient: 'Oxytetracycline', unit: 'ml', meatWithdrawalDays: 28, milkWithdrawalHours: 168 };
  protected medicineStockDraft = { medicineId: '', batchNumber: `VET-${new Date().getFullYear()}-001`, receivedOn: new Date().toISOString().slice(0, 10), expiresOn: '', initialQuantity: 500 };
  protected treatmentDraft = { subjectId: '', medicineStockLotId: '', dose: 10, route: 'Intramuscular', notes: '' };
  protected dispatchDraft = { subjectId: '', purpose: 2, destination: 'Licensed abattoir', overrideReason: '' };
  protected dispatchStatus = '';

  async ngOnInit(): Promise<void> {
    await this.sync.initialize();
    await this.refresh();
  }

  protected async saveTenant(): Promise<void> {
    if (!this.isGuid(this.tenantId)) {
      this.notice = 'Enter a valid tenant UUID.';
      return;
    }
    this.api.setTenant(this.tenantId);
    this.farms = [];
    this.fields = [];
    this.seasons = [];
    this.cropZones = [];
    this.inputs = [];
    this.stockLots = [];
    this.medicines = [];
    this.medicineStockLots = [];
    this.fieldDraft.farmId = '';
    this.seasonDraft.farmId = '';
    this.cropZoneDraft.seasonId = '';
    this.cropZoneDraft.fieldId = '';
    this.inputDraft.farmId = '';
    this.stockLotDraft.inputId = '';
    this.medicineStockDraft.medicineId = '';
    this.treatmentDraft.medicineStockLotId = '';
    this.inputApplicationDraft.stockLotId = '';
    this.evidenceDraft.cropZoneId = '';
    this.operationDraft.cropZoneId = '';
    this.notice = 'Tenant workspace changed.';
    await this.sync.initialize();
    await this.refresh();
  }

  protected async refresh(): Promise<void> {
    if (!navigator.onLine) {
      this.notice = 'Offline: queued work remains available. Reopen while online to refresh master data.';
      return;
    }
    this.busy = true;
    try {
      this.farms = await this.api.listFarms();
      this.subjects = await this.api.listLivestockSubjects();
      if (!this.fieldDraft.farmId && this.farms[0]) {
        this.fieldDraft.farmId = this.farms[0].id;
        this.seasonDraft.farmId = this.farms[0].id;
        this.subjectDraft.farmId = this.farms[0].id;
        this.inputDraft.farmId = this.farms[0].id;
        this.medicineDraft.farmId = this.farms[0].id;
      }
      if (!this.measurementDraft.subjectId && this.subjects[0]) {
        this.measurementDraft.subjectId = this.subjects[0].id;
        this.treatmentDraft.subjectId = this.subjects[0].id;
        this.dispatchDraft.subjectId = this.subjects[0].id;
      }
      await this.refreshFields();
      await this.refreshSeasons();
      await this.refreshInventory();
      await this.refreshMedicines();
      this.notice = 'Workspace data refreshed.';
    } catch (error) {
      this.notice = this.describeError(error);
    } finally {
      this.busy = false;
    }
  }

  protected async createFarm(): Promise<void> {
    await this.runOnline(async () => {
      const farm = await this.api.createFarm(this.farmDraft);
      this.farms = [...this.farms, farm];
      this.fieldDraft.farmId = farm.id;
      this.seasonDraft.farmId = farm.id;
      this.subjectDraft.farmId = farm.id;
      this.inputDraft.farmId = farm.id;
      this.notice = `Created ${farm.name}.`;
    });
  }

  protected async createField(): Promise<void> {
    await this.runOnline(async () => {
      const exterior = this.fieldDraft.coordinates
        .split(/\r?\n/)
        .map(line => line.split(',').map(value => Number(value.trim())))
        .filter(pair => pair.length === 2 && pair.every(Number.isFinite))
        .map(([longitude, latitude]) => ({ longitude, latitude }));
      const field = await this.api.createField(this.fieldDraft.farmId, {
        name: this.fieldDraft.name,
        validFrom: this.fieldDraft.validFrom,
        exterior,
        source: 'drawn'
      });
      this.fields = [...this.fields, field];
      this.cropZoneDraft.fieldId = field.id;
      this.cropZoneDraft.boundaryVersionId = field.boundaryId;
      this.notice = `Created ${field.name}; server-computed area ${field.areaHa.toFixed(2)} ha.`;
    });
  }

  protected async createSeason(): Promise<void> {
    await this.runOnline(async () => {
      const season = await this.api.createSeason(this.seasonDraft.farmId, {
        name: this.seasonDraft.name,
        startsOn: this.seasonDraft.startsOn,
        endsOn: this.seasonDraft.endsOn,
        status: Number(this.seasonDraft.status)
      });
      this.seasons = [season, ...this.seasons];
      this.cropZoneDraft.seasonId = season.id;
      this.notice = `Created production season ${season.name}.`;
    });
  }

  protected async createCropZone(): Promise<void> {
    await this.runOnline(async () => {
      const field = this.fields.find(item => item.id === this.cropZoneDraft.fieldId);
      if (!field) throw new Error('Select a field before creating a crop zone.');
      const cropZone = await this.api.createCropZone(this.cropZoneDraft.seasonId, {
        fieldId: field.id,
        boundaryVersionId: field.boundaryId,
        name: this.cropZoneDraft.name,
        crop: this.cropZoneDraft.crop,
        variety: this.cropZoneDraft.variety || null,
        exterior: null
      });
      this.cropZones = [...this.cropZones, cropZone];
      this.operationDraft.cropZoneId = cropZone.id;
      this.inputApplicationDraft.cropZoneId = cropZone.id;
      this.harvestDraft.cropZoneId = cropZone.id;
      this.evidenceDraft.cropZoneId = cropZone.id;
      this.notice = `Created ${cropZone.name} at ${cropZone.areaHa.toFixed(2)} ha.`;
    });
  }

  protected async createInput(): Promise<void> {
    await this.runOnline(async () => {
      const input = await this.api.createInput(this.inputDraft.farmId, {
        name: this.inputDraft.name,
        category: this.inputDraft.category,
        unit: this.inputDraft.unit
      });
      this.inputs = [...this.inputs, input];
      this.stockLotDraft.inputId = input.id;
      this.notice = `Created inventory item ${input.name}.`;
    });
  }

  protected async createStockLot(): Promise<void> {
    await this.runOnline(async () => {
      const lot = await this.api.createStockLot(this.stockLotDraft.inputId, {
        lotCode: this.stockLotDraft.lotCode,
        receivedOn: this.stockLotDraft.receivedOn,
        expiresOn: this.stockLotDraft.expiresOn || null,
        initialQuantity: Number(this.stockLotDraft.initialQuantity)
      });
      this.stockLots = [lot, ...this.stockLots];
      this.inputApplicationDraft.stockLotId = lot.id;
      this.notice = `Received stock lot ${lot.lotCode}: ${lot.quantityOnHand} ${lot.unit}.`;
    });
  }

  protected async createSubject(): Promise<void> {
    await this.runOnline(async () => {
      const subject = await this.api.createLivestockSubject(this.subjectDraft);
      this.subjects = [...this.subjects, subject];
      this.measurementDraft.subjectId = subject.id;
      this.treatmentDraft.subjectId = subject.id;
      this.dispatchDraft.subjectId = subject.id;
      this.notice = `Created livestock ${subject.kind === 1 ? 'individual' : 'group'} ${subject.name}.`;
    });
  }

  protected async queueOperation(): Promise<void> {
    await this.sync.queue('RecordCropZoneOperation', {
      cropZoneId: this.operationDraft.cropZoneId,
      operationType: this.operationDraft.operationType,
      occurredAt: new Date().toISOString(),
      notes: this.operationDraft.notes || null
    });
    this.notice = 'Field operation saved to the durable device outbox.';
  }

  protected async queueInputApplication(): Promise<void> {
    await this.sync.queue('ApplyInputToCropZone', {
      cropZoneId: this.inputApplicationDraft.cropZoneId,
      stockLotId: this.inputApplicationDraft.stockLotId,
      quantity: Number(this.inputApplicationDraft.quantity),
      occurredAt: new Date().toISOString(),
      notes: this.inputApplicationDraft.notes || null
    });
    this.notice = 'Input application saved to the durable device outbox.';
  }

  protected async queueHarvest(): Promise<void> {
    await this.sync.queue('RecordHarvestLot', {
      cropZoneId: this.harvestDraft.cropZoneId,
      lotCode: this.harvestDraft.lotCode,
      quantity: Number(this.harvestDraft.quantity),
      unit: this.harvestDraft.unit,
      harvestedAt: new Date().toISOString(),
      notes: this.harvestDraft.notes || null
    });
    this.notice = 'Harvest lot saved to the durable device outbox.';
  }

  protected selectEvidenceFile(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file = input.files?.[0];
    if (!file) return;
    if (file.size > 20 * 1024 * 1024) {
      this.notice = 'Photo evidence cannot exceed 20 MB.';
      input.value = '';
      return;
    }
    this.evidenceFile = file;
    this.evidenceFileName = `${file.name} · ${(file.size / 1024 / 1024).toFixed(2)} MB`;
  }

  protected async captureLocation(): Promise<void> {
    if (!navigator.geolocation) {
      this.notice = 'GPS location is not available on this device.';
      return;
    }

    try {
      const position = await new Promise<GeolocationPosition>((resolve, reject) =>
        navigator.geolocation.getCurrentPosition(resolve, reject, {
          enableHighAccuracy: true,
          timeout: 15_000,
          maximumAge: 30_000
        }));
      this.evidenceDraft.latitude = position.coords.latitude;
      this.evidenceDraft.longitude = position.coords.longitude;
      this.evidenceDraft.accuracyMetres = position.coords.accuracy;
      this.evidenceLocationLabel = `${position.coords.latitude.toFixed(5)}, ${position.coords.longitude.toFixed(5)} · ±${Math.round(position.coords.accuracy)} m`;
    } catch {
      this.notice = 'The GPS position could not be captured. You may still save the photo without it.';
    }
  }

  protected async queueEvidence(): Promise<void> {
    if (!this.evidenceFile || !this.evidenceDraft.cropZoneId) return;
    const location = this.evidenceDraft.latitude !== undefined && this.evidenceDraft.longitude !== undefined
      ? {
          latitude: this.evidenceDraft.latitude,
          longitude: this.evidenceDraft.longitude,
          accuracyMetres: this.evidenceDraft.accuracyMetres
        }
      : undefined;
    await this.sync.queueMedia(this.evidenceDraft.cropZoneId, this.evidenceFile, location);
    this.evidenceFile = undefined;
    this.evidenceFileName = '';
    this.notice = 'Photo and GPS evidence saved to the resumable media outbox.';
  }

  protected async inspectTrace(): Promise<void> {
    if (!this.inputApplicationDraft.stockLotId || !navigator.onLine) return;
    try {
      const trace = await this.api.getStockLotTrace(this.inputApplicationDraft.stockLotId);
      this.traceSummary = `${trace.stockLotCode}: ${trace.quantityOnHand} ${trace.unit} remaining · ${trace.applications.length} applications · ${trace.relatedHarvestLots.length} related harvest lots`;
    } catch (error) {
      this.notice = this.describeError(error);
    }
  }

  protected async downloadProductionReport(): Promise<void> {
    const cropZone = this.cropZones.find(zone => zone.id === this.operationDraft.cropZoneId);
    if (!cropZone) {
      this.notice = 'Select a crop zone before exporting its report.';
      return;
    }

    await this.runOnline(async () => {
      const report = await this.api.downloadProductionReport(cropZone.id);
      const url = URL.createObjectURL(report);
      const link = document.createElement('a');
      link.href = url;
      link.download = `${this.fileSlug(cropZone.name)}-production-report.pdf`;
      link.click();
      setTimeout(() => URL.revokeObjectURL(url), 0);
      this.notice = `Downloaded the audited production and evidence report for ${cropZone.name}.`;
    });
  }

  protected async queueMeasurement(): Promise<void> {
    await this.sync.queue('RecordLivestockMeasurement', {
      subjectId: this.measurementDraft.subjectId,
      metric: this.measurementDraft.metric,
      value: Number(this.measurementDraft.value),
      unit: this.measurementDraft.unit,
      occurredAt: new Date().toISOString()
    });
    this.notice = 'Livestock measurement saved to the durable device outbox.';
  }

  protected async createMedicine(): Promise<void> {
    await this.runOnline(async () => {
      const medicine = await this.api.createMedicine(this.medicineDraft.farmId, {
        name: this.medicineDraft.name,
        activeIngredient: this.medicineDraft.activeIngredient,
        unit: this.medicineDraft.unit,
        meatWithdrawalDays: Number(this.medicineDraft.meatWithdrawalDays),
        milkWithdrawalHours: Number(this.medicineDraft.milkWithdrawalHours)
      });
      this.medicines = [...this.medicines, medicine];
      this.medicineStockDraft.medicineId = medicine.id;
      this.notice = `Created veterinary medicine ${medicine.name}.`;
    });
  }

  protected async createMedicineStockLot(): Promise<void> {
    await this.runOnline(async () => {
      const lot = await this.api.createMedicineStockLot(this.medicineStockDraft.medicineId, {
        batchNumber: this.medicineStockDraft.batchNumber,
        receivedOn: this.medicineStockDraft.receivedOn,
        expiresOn: this.medicineStockDraft.expiresOn || null,
        initialQuantity: Number(this.medicineStockDraft.initialQuantity)
      });
      this.medicineStockLots = [lot, ...this.medicineStockLots];
      this.treatmentDraft.medicineStockLotId = lot.id;
      this.notice = `Received medicine batch ${lot.batchNumber}.`;
    });
  }

  protected async queueTreatment(): Promise<void> {
    await this.sync.queue('RecordLivestockTreatment', {
      subjectId: this.treatmentDraft.subjectId,
      medicineStockLotId: this.treatmentDraft.medicineStockLotId,
      dose: Number(this.treatmentDraft.dose),
      route: this.treatmentDraft.route,
      administeredAt: new Date().toISOString(),
      notes: this.treatmentDraft.notes || null
    });
    this.notice = 'Treatment saved offline; withdrawal dates will be calculated from the recorded medicine policy during sync.';
  }

  protected async checkDispatchEligibility(): Promise<void> {
    if (!this.dispatchDraft.subjectId) return;
    await this.runOnline(async () => {
      const result = await this.api.getDispatchEligibility(
        this.dispatchDraft.subjectId,
        Number(this.dispatchDraft.purpose),
        new Date().toISOString());
      this.dispatchStatus = result.eligible
        ? 'Eligible: no active withdrawal hold for this dispatch purpose.'
        : `Blocked until ${new Date(result.blockedUntil!).toLocaleString()} by ${result.blockingTreatmentIds.length} treatment record(s).`;
      this.notice = this.dispatchStatus;
    });
  }

  protected async createDispatch(): Promise<void> {
    await this.runOnline(async () => {
      await this.api.createLivestockDispatch({
        subjectId: this.dispatchDraft.subjectId,
        purpose: Number(this.dispatchDraft.purpose),
        dispatchedAt: new Date().toISOString(),
        destination: this.dispatchDraft.destination,
        overrideReason: this.dispatchDraft.overrideReason || null
      });
      this.dispatchStatus = 'Dispatch recorded.';
      this.notice = this.dispatchStatus;
    });
  }

  protected async refreshFields(): Promise<void> {
    if (!this.fieldDraft.farmId || !navigator.onLine) {
      this.fields = [];
      return;
    }
    this.fields = await this.api.listFields(this.fieldDraft.farmId);
    if (this.fields[0]) {
      this.cropZoneDraft.fieldId = this.fields[0].id;
      this.cropZoneDraft.boundaryVersionId = this.fields[0].boundaryId;
    }
  }

  protected async selectFarm(farmId: string): Promise<void> {
    this.fieldDraft.farmId = farmId;
    this.seasonDraft.farmId = farmId;
    this.inputDraft.farmId = farmId;
    this.medicineDraft.farmId = farmId;
    await this.refreshFields();
    await this.refreshSeasons();
    await this.refreshInventory();
    await this.refreshMedicines();
  }

  protected selectField(fieldId: string): void {
    const field = this.fields.find(item => item.id === fieldId);
    if (field) this.cropZoneDraft.boundaryVersionId = field.boundaryId;
  }

  protected async refreshSeasons(): Promise<void> {
    if (!this.seasonDraft.farmId || !navigator.onLine) {
      this.seasons = [];
      this.cropZones = [];
      return;
    }
    this.seasons = await this.api.listSeasons(this.seasonDraft.farmId);
    if (this.seasons[0]) this.cropZoneDraft.seasonId = this.seasons[0].id;
    await this.refreshCropZones();
  }

  protected async refreshCropZones(): Promise<void> {
    if (!this.cropZoneDraft.seasonId || !navigator.onLine) {
      this.cropZones = [];
      return;
    }
    this.cropZones = await this.api.listCropZones(this.cropZoneDraft.seasonId);
    this.operationDraft.cropZoneId = this.cropZones[0]?.id ?? '';
    this.inputApplicationDraft.cropZoneId = this.cropZones[0]?.id ?? '';
    this.harvestDraft.cropZoneId = this.cropZones[0]?.id ?? '';
    this.evidenceDraft.cropZoneId = this.cropZones[0]?.id ?? '';
  }

  protected async refreshInventory(): Promise<void> {
    if (!this.inputDraft.farmId || !navigator.onLine) {
      this.inputs = [];
      this.stockLots = [];
      return;
    }
    this.inputs = await this.api.listInputs(this.inputDraft.farmId);
    if (this.inputs[0]) this.stockLotDraft.inputId = this.inputs[0].id;
    await this.refreshStockLots();
  }

  protected async refreshStockLots(): Promise<void> {
    if (!this.stockLotDraft.inputId || !navigator.onLine) {
      this.stockLots = [];
      return;
    }
    this.stockLots = await this.api.listStockLots(this.stockLotDraft.inputId);
    this.inputApplicationDraft.stockLotId = this.stockLots[0]?.id ?? '';
  }

  protected async refreshMedicines(): Promise<void> {
    if (!this.medicineDraft.farmId || !navigator.onLine) {
      this.medicines = [];
      this.medicineStockLots = [];
      return;
    }
    this.medicines = await this.api.listMedicines(this.medicineDraft.farmId);
    if (this.medicines[0]) this.medicineStockDraft.medicineId = this.medicines[0].id;
    await this.refreshMedicineStockLots();
  }

  protected async refreshMedicineStockLots(): Promise<void> {
    if (!this.medicineStockDraft.medicineId || !navigator.onLine) {
      this.medicineStockLots = [];
      return;
    }
    this.medicineStockLots = await this.api.listMedicineStockLots(this.medicineStockDraft.medicineId);
    this.treatmentDraft.medicineStockLotId = this.medicineStockLots[0]?.id ?? '';
  }

  private async runOnline(action: () => Promise<void>): Promise<void> {
    if (!navigator.onLine) {
      this.notice = 'This master-data action needs a connection. Production records can still be queued offline.';
      return;
    }
    this.busy = true;
    try {
      await action();
    } catch (error) {
      this.notice = this.describeError(error);
    } finally {
      this.busy = false;
    }
  }

  private describeError(error: unknown): string {
    return error instanceof Error ? error.message : 'The request could not be completed.';
  }

  private isGuid(value: string): boolean {
    return /^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$/i.test(value);
  }

  private fileSlug(value: string): string {
    return value.trim().toLowerCase().replace(/[^a-z0-9]+/g, '-').replace(/^-|-$/g, '') || 'crop-zone';
  }
}
