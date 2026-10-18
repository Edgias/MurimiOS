import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { firstValueFrom } from 'rxjs';

export interface Farm { id: string; name: string; code: string; countryCode: string; timeZone: string; }
export interface FarmField { id: string; farmId: string; name: string; boundaryId: string; areaHa: number; validFrom: string; }
export interface GrowingSeason { id: string; farmId: string; name: string; startsOn: string; endsOn: string; status: number; }
export interface CropZone { id: string; seasonId: string; fieldId: string; boundaryVersionId: string; name: string; crop: string; variety?: string; areaHa: number; }
export interface AgriculturalInput { id: string; farmId: string; name: string; category: string; unit: string; }
export interface InputStockLot { id: string; inputId: string; lotCode: string; receivedOn: string; expiresOn?: string; initialQuantity: number; quantityOnHand: number; unit: string; }
export interface StockLotTrace { stockLotId: string; stockLotCode: string; input: string; unit: string; quantityOnHand: number; applications: unknown[]; relatedHarvestLots: unknown[]; }
export interface LivestockSubject { id: string; farmId: string; kind: number; name: string; species: string; officialIdentifier?: string; }
export interface VeterinaryMedicine { id: string; farmId: string; name: string; activeIngredient: string; unit: string; meatWithdrawalDays: number; milkWithdrawalHours: number; }
export interface VeterinaryMedicineStockLot { id: string; medicineId: string; batchNumber: string; receivedOn: string; expiresOn?: string; initialQuantity: number; quantityOnHand: number; unit: string; }
export interface DispatchEligibility { subjectId: string; purpose: number; evaluatedAt: string; eligible: boolean; blockedUntil?: string; blockingTreatmentIds: string[]; }

@Injectable({ providedIn: 'root' })
export class ApiClient {
  readonly baseUrl = 'http://localhost:5000/api/v1';
  tenantId = localStorage.getItem('murimios.tenantId') ?? '11111111-1111-1111-1111-111111111111';

  constructor(private readonly http: HttpClient) {}

  setTenant(tenantId: string): void {
    this.tenantId = tenantId;
    localStorage.setItem('murimios.tenantId', tenantId);
  }

  listFarms(): Promise<Farm[]> {
    return firstValueFrom(this.http.get<Farm[]>(`${this.baseUrl}/farms`, { headers: this.headers() }));
  }

  createFarm(request: Omit<Farm, 'id'>): Promise<Farm> {
    return firstValueFrom(this.http.post<Farm>(`${this.baseUrl}/farms`, request, { headers: this.headers() }));
  }

  listFields(farmId: string): Promise<FarmField[]> {
    return firstValueFrom(this.http.get<FarmField[]>(`${this.baseUrl}/farms/${farmId}/fields`, { headers: this.headers() }));
  }

  createField(farmId: string, request: object): Promise<FarmField> {
    return firstValueFrom(this.http.post<FarmField>(`${this.baseUrl}/farms/${farmId}/fields`, request, { headers: this.headers() }));
  }

  listSeasons(farmId: string): Promise<GrowingSeason[]> {
    return firstValueFrom(this.http.get<GrowingSeason[]>(`${this.baseUrl}/farms/${farmId}/seasons`, { headers: this.headers() }));
  }

  createSeason(farmId: string, request: object): Promise<GrowingSeason> {
    return firstValueFrom(this.http.post<GrowingSeason>(`${this.baseUrl}/farms/${farmId}/seasons`, request, { headers: this.headers() }));
  }

  listCropZones(seasonId: string): Promise<CropZone[]> {
    return firstValueFrom(this.http.get<CropZone[]>(`${this.baseUrl}/seasons/${seasonId}/crop-zones`, { headers: this.headers() }));
  }

  createCropZone(seasonId: string, request: object): Promise<CropZone> {
    return firstValueFrom(this.http.post<CropZone>(`${this.baseUrl}/seasons/${seasonId}/crop-zones`, request, { headers: this.headers() }));
  }

  listInputs(farmId: string): Promise<AgriculturalInput[]> {
    return firstValueFrom(this.http.get<AgriculturalInput[]>(`${this.baseUrl}/farms/${farmId}/inputs`, { headers: this.headers() }));
  }

  createInput(farmId: string, request: object): Promise<AgriculturalInput> {
    return firstValueFrom(this.http.post<AgriculturalInput>(`${this.baseUrl}/farms/${farmId}/inputs`, request, { headers: this.headers() }));
  }

  listStockLots(inputId: string): Promise<InputStockLot[]> {
    return firstValueFrom(this.http.get<InputStockLot[]>(`${this.baseUrl}/inputs/${inputId}/stock-lots`, { headers: this.headers() }));
  }

  createStockLot(inputId: string, request: object): Promise<InputStockLot> {
    return firstValueFrom(this.http.post<InputStockLot>(`${this.baseUrl}/inputs/${inputId}/stock-lots`, request, { headers: this.headers() }));
  }

  getStockLotTrace(stockLotId: string): Promise<StockLotTrace> {
    return firstValueFrom(this.http.get<StockLotTrace>(`${this.baseUrl}/stock-lots/${stockLotId}/trace`, { headers: this.headers() }));
  }

  downloadProductionReport(cropZoneId: string): Promise<Blob> {
    return firstValueFrom(this.http.get(
      `${this.baseUrl}/reports/crop-zones/${cropZoneId}/production.pdf`,
      { headers: this.headers(), responseType: 'blob' }
    ));
  }

  listLivestockSubjects(): Promise<LivestockSubject[]> {
    return firstValueFrom(this.http.get<LivestockSubject[]>(`${this.baseUrl}/livestock/subjects`, { headers: this.headers() }));
  }

  createLivestockSubject(request: object): Promise<LivestockSubject> {
    return firstValueFrom(this.http.post<LivestockSubject>(`${this.baseUrl}/livestock/subjects`, request, { headers: this.headers() }));
  }

  listMedicines(farmId: string): Promise<VeterinaryMedicine[]> {
    return firstValueFrom(this.http.get<VeterinaryMedicine[]>(`${this.baseUrl}/farms/${farmId}/medicines`, { headers: this.headers() }));
  }

  createMedicine(farmId: string, request: object): Promise<VeterinaryMedicine> {
    return firstValueFrom(this.http.post<VeterinaryMedicine>(`${this.baseUrl}/farms/${farmId}/medicines`, request, { headers: this.headers() }));
  }

  listMedicineStockLots(medicineId: string): Promise<VeterinaryMedicineStockLot[]> {
    return firstValueFrom(this.http.get<VeterinaryMedicineStockLot[]>(`${this.baseUrl}/medicines/${medicineId}/stock-lots`, { headers: this.headers() }));
  }

  createMedicineStockLot(medicineId: string, request: object): Promise<VeterinaryMedicineStockLot> {
    return firstValueFrom(this.http.post<VeterinaryMedicineStockLot>(`${this.baseUrl}/medicines/${medicineId}/stock-lots`, request, { headers: this.headers() }));
  }

  getDispatchEligibility(subjectId: string, purpose: number, at: string): Promise<DispatchEligibility> {
    return firstValueFrom(this.http.get<DispatchEligibility>(
      `${this.baseUrl}/livestock/subjects/${subjectId}/dispatch-eligibility`,
      { params: { purpose, at }, headers: this.headers() }
    ));
  }

  createLivestockDispatch(request: object): Promise<object> {
    return firstValueFrom(this.http.post(`${this.baseUrl}/livestock/dispatches`, request, { headers: this.headers() }));
  }

  headers(tenantId = this.tenantId): HttpHeaders {
    return new HttpHeaders({ 'X-Tenant-Id': tenantId });
  }
}
