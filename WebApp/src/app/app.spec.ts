import { signal } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { App } from './app';
import { ApiClient } from './core/api-client';
import { SyncService } from './offline/sync.service';

describe('App', () => {
  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [App],
      providers: [
        {
          provide: ApiClient,
          useValue: {
            tenantId: '11111111-1111-1111-1111-111111111111',
            listFarms: async () => [],
            listLivestockSubjects: async () => [],
            listFields: async () => [],
            listSeasons: async () => [],
            listCropZones: async () => [],
            listInputs: async () => [],
            listStockLots: async () => [],
            listMedicines: async () => [],
            listMedicineStockLots: async () => [],
            setTenant: () => undefined
          }
        },
        {
          provide: SyncService,
          useValue: {
            online: signal(false),
            syncing: signal(false),
            pendingCount: signal(0),
            message: signal(''),
            lastSyncLabel: signal('Never'),
            initialize: async () => undefined,
            synchronize: async () => undefined,
            queue: async () => undefined,
            queueMedia: async () => undefined
          }
        }
      ]
    }).compileComponents();
  });

  it('creates the application shell', () => {
    const fixture = TestBed.createComponent(App);
    const app = fixture.componentInstance;
    expect(app).toBeTruthy();
  });

  it('renders the platform proposition', () => {
    const fixture = TestBed.createComponent(App);
    fixture.detectChanges();
    const compiled = fixture.nativeElement as HTMLElement;
    expect(compiled.querySelector('h1')?.textContent).toContain('Every field, herd and buyer');
  });
});
