# Backend refactoring inventory — contracts and boundaries

Review aid, not a semantic architecture-test result. Source paths/lines identify declarations at inventory time (2026-09-04). Consumer lists use identifier scanning and exclude tests; they can include comments, properties or identically named types. Ambiguous names are explicitly flagged. Method-body extraction and compiler-resolved dependency checks remain necessary before moving slices. No production Dispatcher use-case handlers exist yet, so these are endpoint-era dependencies, not violations between existing Application slices.

## Confirmed endpoint helper coupling

| Caller | Callee | Proposed treatment |
| --- | --- | --- |
| `API/Platform/AnalyticsEndpoints.cs:70,73,76,81` | `AnalyticsMaturityEndpoints.CurrentFacts` | Move current-fact selection behind an analytics query abstraction; keep projection-version/deletion filtering unchanged. |
| `API/Platform/PublicEnterpriseEndpoints.cs:141` | `AnalyticsMaturityEndpoints.CurrentFacts` | Public API must consume the same query contract, not another endpoint/handler class. |
| `API/Platform/LivestockStatutoryEndpoints.cs:59,61,67` | `LivestockExchangeEndpoints.ScotlandCattleMovementProfileKey`, `EnsureDefaultsAsync` | Extract shared statutory-profile/default-installation contract; preserve tenant-local defaults and save semantics. |

`PlatformEndpoints` calls other endpoint registration methods, and `ComplianceEndpoints:39` registers audit-pack endpoints. These are composition dependencies, not cross-use-case business calls. Do not remove registrations while breaking helper coupling.

## Runtime/application-facing abstractions

Use-case-facing ports belong in Application; infrastructure-private maintenance ports need not all move there. The table deliberately separates scope from placement approval.

| Declaration | Current owner | Non-test consumer files | Proposed placement |
| --- | --- | --- | --- |
| `IAggregateRoot` | `Domain/SharedKernel/IAggregateRoot.cs:5` | `Domain/Entities/YieldMeasurement.cs`<br>`Domain/Entities/WorkItemSubCategory.cs`<br>`Domain/Entities/WorkItemStatus.cs`<br>`Domain/Entities/WorkItemCategory.cs`<br>`Domain/Entities/WorkItem.cs`<br>`Domain/Entities/Warehouse.cs`<br>`Domain/Entities/UnitMeasurement.cs`<br>`Domain/Entities/UnitGroup.cs`<br>`Domain/Entities/Tax.cs`<br>`Domain/Entities/Supplier.cs`<br>`Domain/Entities/SoilType.cs`<br>`Domain/Entities/SeasonStatus.cs`<br>`Domain/Entities/Season.cs`<br>`Domain/Entities/SalesOrderAggregate/SalesOrder.cs`<br>`Domain/Entities/SalesInvoiceNote.cs`<br>`Domain/Entities/SalesInvoiceAggregate/SalesInvoice.cs`<br>`Domain/Entities/QuotationAggregate/Quotation.cs`<br>`Domain/Entities/PurchaseInvoiceAggregate/PurchaseInvoice.cs`<br>`Domain/Entities/PriceList.cs`<br>`Domain/Entities/OwnershipType.cs`<br>`Domain/Entities/NumberSequence.cs`<br>`Domain/Entities/Location.cs`<br>`Domain/Entities/Loan.cs`<br>`Domain/Entities/LivestockProductionAggregate/Livestock.cs`<br>`Domain/Entities/LivestockGroup.cs`<br>`Domain/Entities/ItemCategory.cs`<br>`Domain/Entities/Item.cs`<br>`Domain/Entities/FieldMeasurement.cs`<br>`Domain/Entities/Field.cs`<br>`Domain/Entities/EquipmentCategory.cs`<br>`Domain/Entities/Equipment.cs`<br>`Domain/Entities/Customer.cs`<br>`Domain/Entities/Currency.cs`<br>`Domain/Entities/CropVariety.cs`<br>`Domain/Entities/CropUnit.cs`<br>`Domain/Entities/CropProductionAggregate/CropProduction.cs`<br>`Domain/Entities/CropCategory.cs`<br>`Domain/Entities/Crop.cs`<br>`Domain/Entities/Contact.cs`<br>`Domain/Entities/BinType.cs`<br>`Domain/Entities/Bin.cs`<br>`Domain/Entities/Asset.cs` | Domain/Common |
| `IRequestActorContext` | `API/Access/RequestActorContext.cs:6` | `API/Access/AccessEndpoints.cs`<br>`API/Access/OrganisationScopeAccess.cs`<br>`API/Tenancy/RequestTelemetryMiddleware.cs`<br>`API/Tenancy/IdentitySecurityMiddleware.cs`<br>`API/Platform/AnalyticsMaturityEndpoints.cs`<br>`API/Platform/AnalyticsEndpoints.cs`<br>`API/Platform/IdentitySecurityEndpoints.cs`<br>`API/Platform/GeospatialImportEndpoints.cs`<br>`API/Platform/FinanceEndpoints.cs`<br>`API/Platform/FieldIntelligenceEndpoints.cs`<br>`API/Platform/FarmDigitalTwinEndpoints.cs`<br>`API/Platform/EudrEndpoints.cs`<br>`API/Platform/EpcisEndpoints.cs`<br>`API/Platform/EnterpriseConsolidationEndpoints.cs`<br>`API/Platform/CropPlanningEndpoints.cs`<br>`API/Platform/CorrectionEndpoints.cs`<br>`API/Platform/ConnectedAgricultureEndpoints.cs`<br>`API/Platform/ComplianceEndpoints.cs`<br>`API/Platform/ComplianceAuditPackEndpoints.cs`<br>`API/Platform/MasterDataImportEndpoints.cs`<br>`API/Platform/LivestockStatutoryEndpoints.cs`<br>`API/Platform/LivestockProductionEndpoints.cs`<br>`API/Platform/LivestockExchangeEndpoints.cs`<br>`API/Platform/ProviderIntegrationEndpoints.cs`<br>`API/Platform/NotificationEndpoints.cs`<br>`API/Platform/PublicEnterpriseEndpoints.cs`<br>`API/Platform/SaasControlPlaneEndpoints.cs`<br>`API/Platform/SupplierCompletionEndpoints.cs`<br>`API/Program.cs`<br>`API/Platform/WorkOrderEndpoints.cs`<br>`API/Platform/SyncCommandProcessor.cs`<br>`API/Platform/SupplyNetworkEndpoints.cs`<br>`API/Platform/SupplierWorkflowEndpoints.cs` | Application/Common/Abstractions (implementation stays API) |
| `ITenantContext` | `Domain/Platform/Tenancy/ITenantContext.cs:3` | `tools/SeedData/Program.cs`<br>`Worker/WorkerTenantContext.cs`<br>`Worker/Program.cs`<br>`API/Access/AccessEndpoints.cs`<br>`Infrastructure/Platform/TenantConnectionInterceptor.cs`<br>`API/Reporting/ProductionReportEndpoints.cs`<br>`Infrastructure/Platform/PlatformDbContext.cs`<br>`API/Tenancy/SaasTenantLifecycleMiddleware.cs`<br>`API/Tenancy/RequestTelemetryMiddleware.cs`<br>`API/Tenancy/RequestQuotaMiddleware.cs`<br>`API/Tenancy/HttpTenantContext.cs`<br>`Infrastructure/Platform/Auditing/AuditSaveChangesInterceptor.cs`<br>`API/Platform/AnalyticsMaturityEndpoints.cs`<br>`API/Platform/IdentitySecurityEndpoints.cs`<br>`API/Platform/GeospatialImportEndpoints.cs`<br>`API/Platform/FinanceEndpoints.cs`<br>`API/Platform/FieldIntelligenceEndpoints.cs`<br>`API/Platform/FarmDigitalTwinEndpoints.cs`<br>`API/Platform/EvidenceEndpoints.cs`<br>`API/Platform/EudrEndpoints.cs`<br>`API/Platform/EpcisEndpoints.cs`<br>`API/Platform/EnterpriseConsolidationEndpoints.cs`<br>`API/Platform/CropPlanningEndpoints.cs`<br>`API/Platform/CorrectionEndpoints.cs`<br>`API/Platform/ConnectedAgricultureEndpoints.cs`<br>`API/Platform/ComplianceEndpoints.cs`<br>`API/Platform/ComplianceAuditPackEndpoints.cs`<br>`API/Platform/MasterDataImportEndpoints.cs`<br>`API/Platform/LivestockStatutoryEndpoints.cs`<br>`API/Platform/LivestockRecordsEndpoints.cs`<br>`API/Platform/LivestockProductionEndpoints.cs`<br>`API/Platform/LivestockHealthEndpoints.cs`<br>`API/Platform/LivestockExchangeEndpoints.cs`<br>`API/Platform/ProviderIntegrationEndpoints.cs`<br>`API/Platform/NotificationEndpoints.cs`<br>`API/Platform/PublicEnterpriseEndpoints.cs`<br>`API/Platform/SupplierCompletionEndpoints.cs`<br>`API/Program.cs`<br>`API/Platform/WorkOrderEndpoints.cs`<br>`API/Platform/SyncCommandProcessor.cs`<br>`API/Platform/SupplyNetworkEndpoints.cs`<br>`API/Platform/SupplierWorkflowEndpoints.cs`<br>`Infrastructure/Platform/DesignTimePlatformDbContextFactory.cs` | Application/Common/Abstractions |
| `IAnalyticsMaintenanceStore` | `Infrastructure/Platform/Analytics/AnalyticsMaintenanceStore.cs:7` | `Worker/Program.cs`<br>`Worker/AnalyticsMaintenanceWorker.cs` | Review orchestration ownership first; may remain an Infrastructure-private port |
| `IWebhookStore` | `Infrastructure/Platform/Messaging/WebhookDispatching.cs:19` | `Worker/Program.cs` | Review orchestration ownership first; may remain an Infrastructure-private port |
| `IOutboxStore` | `Infrastructure/Platform/Messaging/OutboxDispatching.cs:16` | `Worker/Program.cs`<br>`Infrastructure/Platform/Messaging/PostgresOutboxStore.cs` | Review orchestration ownership first; may remain an Infrastructure-private port |
| `IIntegrationEventPublisher` | `Infrastructure/Platform/Messaging/OutboxDispatching.cs:41` | `Worker/Program.cs`<br>`Infrastructure/Platform/Messaging/IntegrationEventPublishers.cs` | Review orchestration ownership first; may remain an Infrastructure-private port |
| `IMediaMalwareScanner` | `Infrastructure/Platform/Media/MediaMalwareScanning.cs:17` | `API/Platform/EvidenceEndpoints.cs`<br>`API/Program.cs` | Application/Common/Abstractions (split contracts/results from adapters) |
| `IMediaStorage` | `Infrastructure/Platform/Media/LocalMediaStorage.cs:7` | `Worker/Program.cs`<br>`Worker/ConnectedDataLifecycleWorker.cs`<br>`Infrastructure/Platform/Compliance/ComplianceAuditPackGenerator.cs`<br>`API/Platform/GeospatialImportEndpoints.cs`<br>`API/Platform/EvidenceEndpoints.cs`<br>`API/Platform/ConnectedAgricultureEndpoints.cs`<br>`API/Platform/ComplianceAuditPackEndpoints.cs`<br>`API/Platform/MasterDataImportEndpoints.cs`<br>`API/Program.cs` | Application/Common/Abstractions (split contracts/results from adapters) |
| `IMediaStorageHealthProbe` | `Infrastructure/Platform/Media/LocalMediaStorage.cs:68` | `Infrastructure/Platform/Observability/PlatformHealthChecks.cs`<br>`API/Program.cs` | Infrastructure/Observability or Storage (keep internal boundary) |
| `ITenantMigrationControlStore` | `Infrastructure/Platform/Enterprise/TenantMigrationExecution.cs:36` | `Worker/TenantMigrationWorker.cs`<br>`Worker/Program.cs` | Review orchestration ownership first; may remain an Infrastructure-private port |
| `IScheduledIngestionStore` | `Infrastructure/Platform/Integrations/ScheduledIngestionStore.cs:7` | `Worker/ProviderIngestionWorker.cs`<br>`Worker/Program.cs` | Review orchestration ownership first; may remain an Infrastructure-private port |
| `IProviderAdapter` | `Infrastructure/Platform/Integrations/ProviderAdapterContracts.cs:108` | `Worker/ProviderIngestionWorker.cs`<br>`Worker/Program.cs`<br>`Infrastructure/Platform/Integrations/SentinelHubSatelliteAdapter.cs`<br>`Infrastructure/Platform/Integrations/OpenMeteoWeatherAdapter.cs`<br>`Infrastructure/Platform/Integrations/EuPesticidesRegulatoryAdapter.cs`<br>`Infrastructure/Platform/Integrations/CanonicalRestAdapters.cs` | Application/Common/Abstractions (split contracts/results from adapters) |
| `IMqttSubscriptionStore` | `Infrastructure/Platform/Integrations/MqttSubscriptionStore.cs:11` | `Worker/Program.cs`<br>`Worker/MqttSubscriptionWorker.cs` | Review orchestration ownership first; may remain an Infrastructure-private port |
| `IConnectedDataLifecycleStore` | `Infrastructure/Platform/Integrations/ConnectedDataLifecycleStore.cs:7` | `Worker/Program.cs`<br>`Worker/ConnectedDataLifecycleWorker.cs` | Review orchestration ownership first; may remain an Infrastructure-private port |
| `IAuditActorContext` | `Domain/Platform/Auditing/IAuditActorContext.cs:3` | `API/Access/RequestActorContext.cs`<br>`Infrastructure/Platform/Auditing/AuditSaveChangesInterceptor.cs`<br>`API/Program.cs` | Application/Common/Abstractions |
| `ISalesOrderService` | `Domain/Interfaces/ISalesOrderService.cs:5` | `Infrastructure/Services/SalesOrderService.cs` | Application/Common/Abstractions |
| `ISalesInvoiceService` | `Domain/Interfaces/ISalesInvoiceService.cs:5` | `Infrastructure/Services/SalesInvoiceService.cs` | Application/Common/Abstractions |
| `IQuotationService` | `Domain/Interfaces/IQuotationService.cs:5` | `Infrastructure/Services/QuotationService.cs` | Application/Common/Abstractions |
| `IMessagingService` | `Domain/Interfaces/IMessagingService.cs:6` | No lexical consumers outside declaration file | Application/Common/Abstractions |
| `IFileSystem` | `Domain/Interfaces/IFileSystem.cs:5` | No lexical consumers outside declaration file | Application/Common/Abstractions |
| `IAppLogger` | `Domain/Interfaces/IAppLogger.cs:7` | `Infrastructure/Logging/LoggerAdapter.cs`<br>`Infrastructure/Dependencies.cs` | Application/Common/Abstractions |

Also required before extraction: a persistence/transaction boundary for PlatformDbContext-backed use cases, a separate legacy persistence boundary if legacy endpoints are migrated, and a scope-access abstraction replacing direct use of API's OrganisationScopeAccess. Do not introduce an ASP.NET-dependent ICurrentUser or collapse IRequestActorContext/IAuditActorContext in a way that changes resolution or auditing semantics.

## API DTO/record catalogue

Catalogue includes feature-local DTOs as well as globally shared contracts; not every row is globally shared. Files with no external consumer may still share a type between multiple methods in that file. For split DTOs preserve public simple names and JSON fields, and use per-use-case namespaces to avoid collisions. Keep framework-bound transport models in API if copying them to Application would bring HTTP types with them.

| Type | Declaration | Other production files containing identifier | Name ambiguity / placement |
| --- | --- | --- | --- |
| `BaseRequest` | `API/BaseRequest.cs:3` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `AssetResponse` | `API/Assets/AssetResponse.cs:3` | `API/Assets/AssetMapper.cs`<br>`API/Assets/AssetEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `AssetRequest` | `API/Assets/AssetRequest.cs:3` | `API/Assets/AssetMapper.cs`<br>`API/Assets/AssetEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `AssetModel` | `API/Assets/AssetModel.cs:5` | `API/Assets/AssetResponse.cs`<br>`API/Assets/AssetRequest.cs`<br>`API/Assets/AssetEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `CreateMembershipRequest` | `API/Access/AccessEndpoints.cs:186` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `UpdateMembershipAccessRequest` | `API/Access/AccessEndpoints.cs:195` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `MembershipResponse` | `API/Access/AccessEndpoints.cs:201` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `AuditEventResponse` | `API/Access/AuditEndpoints.cs:62` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ResolvedOrganisationScope` | `API/Access/OrganisationScopeAccess.cs:6` | `API/Access/AccessEndpoints.cs`<br>`API/Access/AuditEndpoints.cs`<br>`API/Access/OrganisationEndpoints.cs`<br>`API/Reporting/ProductionReportEndpoints.cs`<br>`API/Platform/AnalyticsMaturityEndpoints.cs`<br>`API/Platform/AnalyticsEndpoints.cs`<br>`API/Platform/FinanceEndpoints.cs`<br>`API/Platform/FieldIntelligenceEndpoints.cs`<br>`API/Platform/EudrEndpoints.cs`<br>`API/Platform/EnterpriseConsolidationEndpoints.cs`<br>`API/Platform/CorrectionEndpoints.cs`<br>`API/Platform/MasterDataImportEndpoints.cs`<br>`API/Platform/LivestockExchangeEndpoints.cs`<br>`API/Platform/ProviderIntegrationEndpoints.cs`<br>`API/Platform/PlatformEndpoints.cs`<br>`API/Platform/PublicEnterpriseEndpoints.cs`<br>`API/Platform/WorkOrderEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `CreateOrganisationNodeRequest` | `API/Access/OrganisationEndpoints.cs:105` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `OrganisationNodeResponse` | `API/Access/OrganisationEndpoints.cs:110` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `YieldMeasurementResponse` | `API/YieldMeasurements/YieldMeasurementResponse.cs:5` | `API/YieldMeasurements/YieldMeasurementMapper.cs`<br>`API/YieldMeasurements/YieldMeasurementEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `YieldMeasurementRequest` | `API/YieldMeasurements/YieldMeasurementRequest.cs:3` | `API/YieldMeasurements/YieldMeasurementMapper.cs`<br>`API/YieldMeasurements/YieldMeasurementEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `YieldMeasurementModel` | `API/YieldMeasurements/YieldMeasurementModel.cs:5` | `API/YieldMeasurements/YieldMeasurementResponse.cs`<br>`API/YieldMeasurements/YieldMeasurementRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `SeasonResponse` | `API/Seasons/SeasonResponse.cs:5` | `API/Seasons/SeasonMapper.cs`<br>`API/Seasons/SeasonEndpoints.cs`<br>`API/Platform/PlatformEndpoints.cs`<br>`API/Platform/PlatformContracts.cs` | AMBIGUOUS: same simple name is declared elsewhere; resolve by namespace before moving. |
| `SeasonRequest` | `API/Seasons/SeasonRequest.cs:3` | `API/Seasons/SeasonMapper.cs`<br>`API/Seasons/SeasonEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `SeasonModel` | `API/Seasons/SeasonModel.cs:5` | `API/Seasons/SeasonResponse.cs`<br>`API/Seasons/SeasonRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `WorkItemSubCategoryResponse` | `API/WorkItemSubCategories/WorkItemSubCategoryResponse.cs:5` | `API/WorkItemSubCategories/WorkItemSubCategoryMapper.cs`<br>`API/WorkItemSubCategories/WorkItemSubCategoryEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `WorkItemSubCategoryRequest` | `API/WorkItemSubCategories/WorkItemSubCategoryRequest.cs:3` | `API/WorkItemSubCategories/WorkItemSubCategoryMapper.cs`<br>`API/WorkItemSubCategories/WorkItemSubCategoryEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `WorkItemSubCategoryModel` | `API/WorkItemSubCategories/WorkItemSubCategoryModel.cs:5` | `API/WorkItemSubCategories/WorkItemSubCategoryResponse.cs`<br>`API/WorkItemSubCategories/WorkItemSubCategoryRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `FieldResponse` | `API/Fields/FieldResponse.cs:5` | `API/Fields/FieldMapper.cs`<br>`API/Fields/FieldEndpoints.cs`<br>`API/Platform/PlatformEndpoints.cs`<br>`API/Platform/PlatformContracts.cs` | AMBIGUOUS: same simple name is declared elsewhere; resolve by namespace before moving. |
| `FieldRequest` | `API/Fields/FieldRequest.cs:3` | `API/Fields/FieldMapper.cs`<br>`API/Fields/FieldEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `FieldModel` | `API/Fields/FieldModel.cs:5` | `API/Fields/FieldResponse.cs`<br>`API/Fields/FieldRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `CropVarietyResponse` | `API/CropVarieties/CropVarietyResponse.cs:3` | `API/CropVarieties/CropVarietyMapper.cs`<br>`API/CropVarieties/CropVarietyEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `CropVarietyRequest` | `API/CropVarieties/CropVarietyRequest.cs:3` | `API/CropVarieties/CropVarietyMapper.cs`<br>`API/CropVarieties/CropVarietyEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `CropVarietyModel` | `API/CropVarieties/CropVarietyModel.cs:5` | `API/CropVarieties/CropVarietyResponse.cs`<br>`API/CropVarieties/CropVarietyRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `WorkItemStatusResponse` | `API/WorkItemStatuses/WorkItemStatusResponse.cs:5` | `API/WorkItemStatuses/WorkItemStatusMapper.cs`<br>`API/WorkItemStatuses/WorkItemStatusEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `WorkItemStatusRequest` | `API/WorkItemStatuses/WorkItemStatusRequest.cs:3` | `API/WorkItemStatuses/WorkItemStatusMapper.cs`<br>`API/WorkItemStatuses/WorkItemStatusEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `WorkItemStatusModel` | `API/WorkItemStatuses/WorkItemStatusModel.cs:5` | `API/WorkItemStatuses/WorkItemStatusResponse.cs`<br>`API/WorkItemStatuses/WorkItemStatusRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `FieldMeasurementResponse` | `API/FieldMeasurements/FieldMeasurementResponse.cs:3` | `API/FieldMeasurements/FieldMeasurementMapper.cs`<br>`API/FieldMeasurements/FieldMeasurementEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `FieldMeasurementRequest` | `API/FieldMeasurements/FieldMeasurementRequest.cs:3` | `API/FieldMeasurements/FieldMeasurementMapper.cs`<br>`API/FieldMeasurements/FieldMeasurementEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `FieldMeasurementModel` | `API/FieldMeasurements/FieldMeasurementModel.cs:5` | `API/FieldMeasurements/FieldMeasurementResponse.cs`<br>`API/FieldMeasurements/FieldMeasurementRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `WorkItemResponse` | `API/WorkItems/WorkItemResponse.cs:5` | `API/WorkItems/WorkItemMapper.cs`<br>`API/WorkItems/WorkItemEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `WorkItemRequest` | `API/WorkItems/WorkItemRequest.cs:3` | `API/WorkItems/WorkItemMapper.cs`<br>`API/WorkItems/WorkItemEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `WorkItemModel` | `API/WorkItems/WorkItemModel.cs:5` | `API/WorkItems/WorkItemResponse.cs`<br>`API/WorkItems/WorkItemRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `ProductionReportData` | `API/Reporting/ProductionReportPdfBuilder.cs:6` | `API/Reporting/ProductionReportEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `ProductionOperationRow` | `API/Reporting/ProductionReportPdfBuilder.cs:22` | `API/Reporting/ProductionReportEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `InputApplicationRow` | `API/Reporting/ProductionReportPdfBuilder.cs:24` | `API/Reporting/ProductionReportEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `HarvestLotRow` | `API/Reporting/ProductionReportPdfBuilder.cs:32` | `API/Reporting/ProductionReportEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `EvidenceRow` | `API/Reporting/ProductionReportPdfBuilder.cs:34` | `API/Reporting/ProductionReportEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `CropCategoryResponse` | `API/CropCategories/CropCategoryResponse.cs:3` | `API/CropCategories/CropCategoryMapper.cs`<br>`API/CropCategories/CropCategoryEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `CropCategoryRequest` | `API/CropCategories/CropCategoryRequest.cs:3` | `API/CropCategories/CropCategoryMapper.cs`<br>`API/CropCategories/CropCategoryEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `CropCategoryModel` | `API/CropCategories/CropCategoryModel.cs:5` | `API/CropCategories/CropCategoryResponse.cs`<br>`API/CropCategories/CropCategoryRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `EquipmentResponse` | `API/Equipments/EquipmentResponse.cs:3` | `API/Equipments/EquipmentMapper.cs`<br>`API/Equipments/EquipmentEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `EquipmentRequest` | `API/Equipments/EquipmentRequest.cs:3` | `API/Equipments/EquipmentMapper.cs`<br>`API/Equipments/EquipmentEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `EquipmentModel` | `API/Equipments/EquipmentModel.cs:5` | `API/Equipments/EquipmentResponse.cs`<br>`API/Equipments/EquipmentRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `WorkItemCategoryResponse` | `API/WorkItemCategories/WorkItemCategoryResponse.cs:5` | `API/WorkItemCategories/WorkItemCategoryMapper.cs`<br>`API/WorkItemCategories/WorkItemCategoryEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `WorkItemCategoryRequest` | `API/WorkItemCategories/WorkItemCategoryRequest.cs:3` | `API/WorkItemCategories/WorkItemCategoryMapper.cs`<br>`API/WorkItemCategories/WorkItemCategoryEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `WorkItemCategoryModel` | `API/WorkItemCategories/WorkItemCategoryModel.cs:5` | `API/WorkItemCategories/WorkItemCategoryResponse.cs`<br>`API/WorkItemCategories/WorkItemCategoryRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `BinTypeResponse` | `API/BinTypes/BinTypeResponse.cs:5` | `API/BinTypes/BinTypeMapper.cs`<br>`API/BinTypes/BinTypeEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `BinTypeRequest` | `API/BinTypes/BinTypeRequest.cs:3` | `API/BinTypes/BinTypeMapper.cs`<br>`API/BinTypes/BinTypeEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `BinTypeModel` | `API/BinTypes/BinTypeModel.cs:5` | `API/BinTypes/BinTypeResponse.cs`<br>`API/BinTypes/BinTypeRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `CropUnitResponse` | `API/CropUnits/CropUnitResponse.cs:3` | `API/CropUnits/CropUnitMapper.cs`<br>`API/CropUnits/CropUnitEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `CropUnitRequest` | `API/CropUnits/CropUnitRequest.cs:3` | `API/CropUnits/CropUnitMapper.cs`<br>`API/CropUnits/CropUnitEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `CropUnitModel` | `API/CropUnits/CropUnitModel.cs:5` | `API/CropUnits/CropUnitResponse.cs`<br>`API/CropUnits/CropUnitRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `EquipmentCategoryResponse` | `API/EquipmentCategories/EquipmentCategoryResponse.cs:3` | `API/EquipmentCategories/EquipmentCategoryMapper.cs`<br>`API/EquipmentCategories/EquipmentCategoryEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `EquipmentCategoryRequest` | `API/EquipmentCategories/EquipmentCategoryRequest.cs:3` | `API/EquipmentCategories/EquipmentCategoryMapper.cs`<br>`API/EquipmentCategories/EquipmentCategoryEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `EquipmentCategoryModel` | `API/EquipmentCategories/EquipmentCategoryModel.cs:5` | `API/EquipmentCategories/EquipmentCategoryResponse.cs`<br>`API/EquipmentCategories/EquipmentCategoryRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `CropResponse` | `API/Crops/CropResponse.cs:5` | `API/Crops/CropMapper.cs`<br>`API/Crops/CropEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `CropRequest` | `API/Crops/CropRequest.cs:3` | `API/Crops/CropMapper.cs`<br>`API/Crops/CropEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `CropModel` | `API/Crops/CropModel.cs:5` | `API/Crops/CropResponse.cs`<br>`API/Crops/CropRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `CustomerResponse` | `API/Customers/CustomerResponse.cs:5` | `API/Customers/CustomerMapper.cs`<br>`API/Customers/CustomerEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `CustomerRequest` | `API/Customers/CustomerRequest.cs:3` | `API/Customers/CustomerMapper.cs`<br>`API/Customers/CustomerEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `CustomerModel` | `API/Customers/CustomerModel.cs:5` | `API/Customers/CustomerResponse.cs`<br>`API/Customers/CustomerRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `SoilTypeResponse` | `API/SoilTypes/SoilTypeResponse.cs:3` | `API/SoilTypes/SoilTypeMapper.cs`<br>`API/SoilTypes/SoilTypeEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `SoilTypeRequest` | `API/SoilTypes/SoilTypeRequest.cs:3` | `API/SoilTypes/SoilTypeMapper.cs`<br>`API/SoilTypes/SoilTypeEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `SoilTypeModel` | `API/SoilTypes/SoilTypeModel.cs:5` | `API/SoilTypes/SoilTypeResponse.cs`<br>`API/SoilTypes/SoilTypeRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `CurrencyResponse` | `API/Currencies/CurrencyResponse.cs:3` | `API/Currencies/CurrencyMapper.cs`<br>`API/Currencies/CurrencyEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `CurrencyRequest` | `API/Currencies/CurrencyRequest.cs:3` | `API/Currencies/CurrencyMapper.cs`<br>`API/Currencies/CurrencyEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `CurrencyModel` | `API/Currencies/CurrencyModel.cs:5` | `API/Currencies/CurrencyResponse.cs`<br>`API/Currencies/CurrencyRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `CropProductionResponse` | `API/CropProductions/CropProductionResponse.cs:3` | `API/CropProductions/CropProductionMapper.cs`<br>`API/CropProductions/CropProductionEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `CropProductionRequest` | `API/CropProductions/CropProductionRequest.cs:3` | `API/CropProductions/CropProductionMapper.cs`<br>`API/CropProductions/CropProductionEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `CropProductionModel` | `API/CropProductions/CropProductionModel.cs:5` | `API/CropProductions/CropProductionResponse.cs`<br>`API/CropProductions/CropProductionRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `SeasonStatusResponse` | `API/SeasonStatuses/SeasonStatusResponse.cs:5` | `API/SeasonStatuses/SeasonStatusMapper.cs`<br>`API/SeasonStatuses/SeasonStatusEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `SeasonStatusRequest` | `API/SeasonStatuses/SeasonStatusRequest.cs:3` | `API/SeasonStatuses/SeasonStatusMapper.cs`<br>`API/SeasonStatuses/SeasonStatusEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `SeasonStatusModel` | `API/SeasonStatuses/SeasonStatusModel.cs:5` | `API/SeasonStatuses/SeasonStatusResponse.cs`<br>`API/SeasonStatuses/SeasonStatusRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `NumberSequenceResponse` | `API/NumberSequences/NumberSequenceResponse.cs:3` | `API/NumberSequences/NumberSequenceMapper.cs`<br>`API/NumberSequences/NumberSequenceEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `NumberSequenceRequest` | `API/NumberSequences/NumberSequenceRequest.cs:3` | `API/NumberSequences/NumberSequenceMapper.cs`<br>`API/NumberSequences/NumberSequenceEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `NumberSequenceModel` | `API/NumberSequences/NumberSequenceModel.cs:5` | `API/NumberSequences/NumberSequenceResponse.cs`<br>`API/NumberSequences/NumberSequenceRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `LoanResponse` | `API/Loans/LoanResponse.cs:5` | `API/Loans/LoanMapper.cs`<br>`API/Loans/LoanEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `LoanRequest` | `API/Loans/LoanRequest.cs:3` | `API/Loans/LoanMapper.cs`<br>`API/Loans/LoanEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `LoanModel` | `API/Loans/LoanModel.cs:5` | `API/Loans/LoanResponse.cs`<br>`API/Loans/LoanRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `LocationResponse` | `API/Locations/LocationResponse.cs:5` | `API/Locations/LocationMapper.cs`<br>`API/Locations/LocationEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `LocationRequest` | `API/Locations/LocationRequest.cs:3` | `API/Locations/LocationMapper.cs`<br>`API/Locations/LocationEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `LocationModel` | `API/Locations/LocationModel.cs:5` | `API/Locations/LocationResponse.cs`<br>`API/Locations/LocationRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `ItemCategoryResponse` | `API/ItemCategories/ItemCategoryResponse.cs:3` | `API/ItemCategories/ItemCategoryMapper.cs`<br>`API/ItemCategories/ItemCategoryEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `ItemCategoryRequest` | `API/ItemCategories/ItemCategoryRequest.cs:3` | `API/ItemCategories/ItemCategoryMapper.cs`<br>`API/ItemCategories/ItemCategoryEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `ItemCategoryModel` | `API/ItemCategories/ItemCategoryModel.cs:5` | `API/ItemCategories/ItemCategoryResponse.cs`<br>`API/ItemCategories/ItemCategoryRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `CreateBenchmarkCohortRequest` | `API/Platform/AnalyticsMaturityEndpoints.cs:185` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `AnalyticsApprovalRequest` | `API/Platform/AnalyticsMaturityEndpoints.cs:188` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreatePredictiveModelRequest` | `API/Platform/AnalyticsMaturityEndpoints.cs:189` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateKpiDefinitionRequest` | `API/Platform/AnalyticsEndpoints.cs:112` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateAnalyticsProjectionRequest` | `API/Platform/AnalyticsEndpoints.cs:113` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateBenchmarkEnrollmentRequest` | `API/Platform/AnalyticsEndpoints.cs:114` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateAnalyticsScheduleRequest` | `API/Platform/AnalyticsEndpoints.cs:115` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ResumeAnalyticsScheduleRequest` | `API/Platform/AnalyticsEndpoints.cs:116` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateAnalyticsRetentionPolicyRequest` | `API/Platform/AnalyticsEndpoints.cs:117` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `OwnershipTypeResponse` | `API/OwnershipTypes/OwnershipTypeResponse.cs:3` | `API/OwnershipTypes/OwnershipTypeMapper.cs`<br>`API/OwnershipTypes/OwnershipTypeEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `OwnershipTypeRequest` | `API/OwnershipTypes/OwnershipTypeRequest.cs:3` | `API/OwnershipTypes/OwnershipTypeMapper.cs`<br>`API/OwnershipTypes/OwnershipTypeEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `OwnershipTypeModel` | `API/OwnershipTypes/OwnershipTypeModel.cs:5` | `API/OwnershipTypes/OwnershipTypeResponse.cs`<br>`API/OwnershipTypes/OwnershipTypeRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `SecurityReasonRequest` | `API/Platform/IdentitySecurityEndpoints.cs:143` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateApiRateLimitPolicyRequest` | `API/Platform/IdentitySecurityEndpoints.cs:144` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateCredentialRotationPolicyRequest` | `API/Platform/IdentitySecurityEndpoints.cs:145` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateRecoveryRequest` | `API/Platform/IdentitySecurityEndpoints.cs:146` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `VerifyRecoveryRequest` | `API/Platform/IdentitySecurityEndpoints.cs:147` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ApplyGeospatialImportRequest` | `API/Platform/GeospatialImportEndpoints.cs:286` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ApplyGeospatialImportFeature` | `API/Platform/GeospatialImportEndpoints.cs:287` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `GeospatialImportJobResponse` | `API/Platform/GeospatialImportEndpoints.cs:288` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateCostCentreRequest` | `API/Platform/FinanceEndpoints.cs:657` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CostCentreResponse` | `API/Platform/FinanceEndpoints.cs:659` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `FinanceAllocationRequest` | `API/Platform/FinanceEndpoints.cs:663` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateFinanceTransactionRequest` | `API/Platform/FinanceEndpoints.cs:665` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ReverseFinanceTransactionRequest` | `API/Platform/FinanceEndpoints.cs:670` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `FinanceAllocationResponse` | `API/Platform/FinanceEndpoints.cs:672` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `FinanceTransactionResponse` | `API/Platform/FinanceEndpoints.cs:674` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `RecordLabourCostRequest` | `API/Platform/FinanceEndpoints.cs:681` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `LabourCostResponse` | `API/Platform/FinanceEndpoints.cs:685` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `RecordMachineryCostRequest` | `API/Platform/FinanceEndpoints.cs:690` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `MachineryCostResponse` | `API/Platform/FinanceEndpoints.cs:694` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CostCentreProfitabilityResponse` | `API/Platform/FinanceEndpoints.cs:699` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `FarmProfitabilityResponse` | `API/Platform/FinanceEndpoints.cs:703` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `EnterpriseProfitabilityResponse` | `API/Platform/FinanceEndpoints.cs:707` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `EnterpriseConsolidationSummary` | `API/Platform/FinanceEndpoints.cs:710` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ImportAccountingRequest` | `API/Platform/FinanceEndpoints.cs:714` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ImportAccountingLineRequest` | `API/Platform/FinanceEndpoints.cs:717` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ReconcileAccountingRequest` | `API/Platform/FinanceEndpoints.cs:720` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `AccountingStatementLineResponse` | `API/Platform/FinanceEndpoints.cs:721` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `AccountingImportResponse` | `API/Platform/FinanceEndpoints.cs:725` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `IngestWeatherObservationsRequest` | `API/Platform/FieldIntelligenceEndpoints.cs:686` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `WeatherObservationItem` | `API/Platform/FieldIntelligenceEndpoints.cs:688` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `IngestWeatherForecastsRequest` | `API/Platform/FieldIntelligenceEndpoints.cs:692` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `WeatherForecastItem` | `API/Platform/FieldIntelligenceEndpoints.cs:694` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `IngestSatelliteStatisticsRequest` | `API/Platform/FieldIntelligenceEndpoints.cs:698` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `SatelliteStatisticItem` | `API/Platform/FieldIntelligenceEndpoints.cs:700` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateScoutingObservationRequest` | `API/Platform/FieldIntelligenceEndpoints.cs:703` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateFieldRiskRuleRequest` | `API/Platform/FieldIntelligenceEndpoints.cs:706` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `AlertDecisionRequest` | `API/Platform/FieldIntelligenceEndpoints.cs:711` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateAlertWorkOrderRequest` | `API/Platform/FieldIntelligenceEndpoints.cs:712` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `WeatherObservationResponse` | `API/Platform/FieldIntelligenceEndpoints.cs:716` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `WeatherForecastResponse` | `API/Platform/FieldIntelligenceEndpoints.cs:721` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `FieldWeatherResponse` | `API/Platform/FieldIntelligenceEndpoints.cs:726` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `SatelliteFieldStatisticResponse` | `API/Platform/FieldIntelligenceEndpoints.cs:728` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ScoutingObservationResponse` | `API/Platform/FieldIntelligenceEndpoints.cs:733` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `FieldRiskRuleResponse` | `API/Platform/FieldIntelligenceEndpoints.cs:737` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `FieldRiskAssessmentResponse` | `API/Platform/FieldIntelligenceEndpoints.cs:743` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `EvaluateFieldRisksResponse` | `API/Platform/FieldIntelligenceEndpoints.cs:747` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `FieldInsightAlertResponse` | `API/Platform/FieldIntelligenceEndpoints.cs:750` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `FieldMapLayerResponse` | `API/Platform/FieldIntelligenceEndpoints.cs:757` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `InsightFeedFreshnessResponse` | `API/Platform/FieldIntelligenceEndpoints.cs:761` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `FarmIntelligenceOverviewResponse` | `API/Platform/FieldIntelligenceEndpoints.cs:765` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `AlertWorkOrderResponse` | `API/Platform/FieldIntelligenceEndpoints.cs:768` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateLandTenureRequest` | `API/Platform/FarmDigitalTwinEndpoints.cs:414` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CloseLandTenureRequest` | `API/Platform/FarmDigitalTwinEndpoints.cs:417` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateInfrastructureAssetRequest` | `API/Platform/FarmDigitalTwinEndpoints.cs:418` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ChangeInfrastructureStatusRequest` | `API/Platform/FarmDigitalTwinEndpoints.cs:421` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `DecommissionInfrastructureRequest` | `API/Platform/FarmDigitalTwinEndpoints.cs:422` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateSoilZoneRequest` | `API/Platform/FarmDigitalTwinEndpoints.cs:423` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `AddSoilZoneBoundaryRequest` | `API/Platform/FarmDigitalTwinEndpoints.cs:426` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateSoilSampleRequest` | `API/Platform/FarmDigitalTwinEndpoints.cs:428` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateSoilSampleResultRequest` | `API/Platform/FarmDigitalTwinEndpoints.cs:432` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `SoilSampleCommandRequest` | `API/Platform/FarmDigitalTwinEndpoints.cs:434` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateEudrCaseRequest` | `API/Platform/EudrEndpoints.cs:505` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `AddEudrPlotRequest` | `API/Platform/EudrEndpoints.cs:509` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `RecordEudrScreeningRequest` | `API/Platform/EudrEndpoints.cs:511` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `RecordEudrRiskDecisionRequest` | `API/Platform/EudrEndpoints.cs:515` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `AssembleEudrStatementRequest` | `API/Platform/EudrEndpoints.cs:517` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `SubmitEudrStatementRequest` | `API/Platform/EudrEndpoints.cs:518` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CancelEudrCaseRequest` | `API/Platform/EudrEndpoints.cs:519` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateLegalCompanyRequest` | `API/Platform/EnterpriseConsolidationEndpoints.cs:223` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CloseLegalCompanyRequest` | `API/Platform/EnterpriseConsolidationEndpoints.cs:226` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateEliminationRuleRequest` | `API/Platform/EnterpriseConsolidationEndpoints.cs:227` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ApproveEliminationRuleRequest` | `API/Platform/EnterpriseConsolidationEndpoints.cs:230` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateEnterpriseConsolidationRequest` | `API/Platform/EnterpriseConsolidationEndpoints.cs:231` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateCropPlanRequest` | `API/Platform/CropPlanningEndpoints.cs:686` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ReplaceCropPlanDraftRequest` | `API/Platform/CropPlanningEndpoints.cs:688` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CropPlanAllocationDraft` | `API/Platform/CropPlanningEndpoints.cs:690` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `PlannedCropOperationDraft` | `API/Platform/CropPlanningEndpoints.cs:693` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CropPlanResourceDraft` | `API/Platform/CropPlanningEndpoints.cs:695` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `RejectCropPlanRequest` | `API/Platform/CropPlanningEndpoints.cs:698` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateCropPlanRevisionRequest` | `API/Platform/CropPlanningEndpoints.cs:699` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CompareCropPlansRequest` | `API/Platform/CropPlanningEndpoints.cs:700` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateProductApplicationRuleRequest` | `API/Platform/CropPlanningEndpoints.cs:701` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `RecordCropPlanFinancialEntryRequest` | `API/Platform/CropPlanningEndpoints.cs:704` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CropPlanSummaryResponse` | `API/Platform/CropPlanningEndpoints.cs:708` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CropPlanDetailResponse` | `API/Platform/CropPlanningEndpoints.cs:713` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CropPlanAllocationResponse` | `API/Platform/CropPlanningEndpoints.cs:715` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `PlannedCropOperationResponse` | `API/Platform/CropPlanningEndpoints.cs:720` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CropPlanResourceBudgetResponse` | `API/Platform/CropPlanningEndpoints.cs:722` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ProductApplicationRuleResponse` | `API/Platform/CropPlanningEndpoints.cs:726` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CropPlanFinancialEntryResponse` | `API/Platform/CropPlanningEndpoints.cs:730` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CropRotationResponse` | `API/Platform/CropPlanningEndpoints.cs:733` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CropPlanOperationVarianceResponse` | `API/Platform/CropPlanningEndpoints.cs:736` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CropPlanResourceVarianceResponse` | `API/Platform/CropPlanningEndpoints.cs:737` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CropPlanAllocationVarianceResponse` | `API/Platform/CropPlanningEndpoints.cs:740` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CropPlanVarianceResponse` | `API/Platform/CropPlanningEndpoints.cs:746` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CropPlanComparisonResponse` | `API/Platform/CropPlanningEndpoints.cs:752` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CorrectFieldOperationRequest` | `API/Platform/CorrectionEndpoints.cs:232` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CorrectLivestockMeasurementRequest` | `API/Platform/CorrectionEndpoints.cs:241` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CorrectionResponse` | `API/Platform/CorrectionEndpoints.cs:251` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateEquipmentRequest` | `API/Platform/ConnectedAgricultureEndpoints.cs:324` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateConnectedDeviceRequest` | `API/Platform/ConnectedAgricultureEndpoints.cs:325` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateSensorChannelRequest` | `API/Platform/ConnectedAgricultureEndpoints.cs:326` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `IngestSensorReadingItem` | `API/Platform/ConnectedAgricultureEndpoints.cs:327` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `IngestSensorReadingsRequest` | `API/Platform/ConnectedAgricultureEndpoints.cs:328` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `IngestMachineryTelemetryRequest` | `API/Platform/ConnectedAgricultureEndpoints.cs:329` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateMachineryTaskFileRequest` | `API/Platform/ConnectedAgricultureEndpoints.cs:330` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateBusinessDocumentRequest` | `API/Platform/ConnectedAgricultureEndpoints.cs:331` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ExchangeCommandRequest` | `API/Platform/ConnectedAgricultureEndpoints.cs:332` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `RegisterDerivedMediaRequest` | `API/Platform/ConnectedAgricultureEndpoints.cs:333` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `DerivedMediaHoldRequest` | `API/Platform/ConnectedAgricultureEndpoints.cs:335` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateComplianceStandardRequest` | `API/Platform/ComplianceEndpoints.cs:421` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateComplianceControlRequest` | `API/Platform/ComplianceEndpoints.cs:423` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateComplianceRequirementRequest` | `API/Platform/ComplianceEndpoints.cs:425` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateComplianceAssessmentRequest` | `API/Platform/ComplianceEndpoints.cs:427` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `EvaluateComplianceControlRequest` | `API/Platform/ComplianceEndpoints.cs:429` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateCorrectiveActionRequest` | `API/Platform/ComplianceEndpoints.cs:431` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CorrectiveActionCommandRequest` | `API/Platform/ComplianceEndpoints.cs:432` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateComplianceAuditPackRequest` | `API/Platform/ComplianceAuditPackEndpoints.cs:214` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateStatutoryMovementDocumentRequest` | `API/Platform/LivestockStatutoryEndpoints.cs:346` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `SubmitStatutoryMovementDocumentRequest` | `API/Platform/LivestockStatutoryEndpoints.cs:347` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `StatutoryMovementSnapshot` | `API/Platform/LivestockStatutoryEndpoints.cs:348` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `StatutoryMovementDocumentResponse` | `API/Platform/LivestockStatutoryEndpoints.cs:351` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `StatutorySubmissionAttemptResponse` | `API/Platform/LivestockStatutoryEndpoints.cs:358` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateCropFeedTransferRequest` | `API/Platform/LivestockStatutoryEndpoints.cs:361` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CropFeedTransferResponse` | `API/Platform/LivestockStatutoryEndpoints.cs:364` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateLivestockHoldingRequest` | `API/Platform/LivestockRecordsEndpoints.cs:300` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `LivestockHoldingResponse` | `API/Platform/LivestockRecordsEndpoints.cs:301` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateLivestockIdentifierRequest` | `API/Platform/LivestockRecordsEndpoints.cs:302` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `RetireLivestockIdentifierRequest` | `API/Platform/LivestockRecordsEndpoints.cs:303` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `LivestockIdentifierResponse` | `API/Platform/LivestockRecordsEndpoints.cs:304` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateLivestockGroupMembershipRequest` | `API/Platform/LivestockRecordsEndpoints.cs:305` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `LeaveLivestockGroupMembershipRequest` | `API/Platform/LivestockRecordsEndpoints.cs:306` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `LivestockGroupMembershipResponse` | `API/Platform/LivestockRecordsEndpoints.cs:307` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateLivestockMovementRequest` | `API/Platform/LivestockRecordsEndpoints.cs:308` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `LivestockMovementResponse` | `API/Platform/LivestockRecordsEndpoints.cs:309` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateLivestockHealthEventRequest` | `API/Platform/LivestockRecordsEndpoints.cs:310` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `LivestockHealthEventResponse` | `API/Platform/LivestockRecordsEndpoints.cs:311` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateLivestockFeedBatchRequest` | `API/Platform/LivestockRecordsEndpoints.cs:312` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `LivestockFeedBatchResponse` | `API/Platform/LivestockRecordsEndpoints.cs:313` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateLivestockFeedAllocationRequest` | `API/Platform/LivestockRecordsEndpoints.cs:314` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `LivestockFeedAllocationResponse` | `API/Platform/LivestockRecordsEndpoints.cs:315` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `LivestockHistoryItem` | `API/Platform/LivestockRecordsEndpoints.cs:316` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateBiologicalProfileRequest` | `API/Platform/LivestockProductionEndpoints.cs:418` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateOffspringLinkRequest` | `API/Platform/LivestockProductionEndpoints.cs:419` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateReproductionEventRequest` | `API/Platform/LivestockProductionEndpoints.cs:421` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateMilkRecordRequest` | `API/Platform/LivestockProductionEndpoints.cs:425` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateGrazingPlanRequest` | `API/Platform/LivestockProductionEndpoints.cs:427` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateRationLineRequest` | `API/Platform/LivestockProductionEndpoints.cs:429` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateRationPlanRequest` | `API/Platform/LivestockProductionEndpoints.cs:431` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `LivestockPerformanceResponse` | `API/Platform/LivestockProductionEndpoints.cs:433` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateLivestockRegulatoryProfileRequest` | `API/Platform/LivestockExchangeEndpoints.cs:448` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateLivestockExchangeSubmissionRequest` | `API/Platform/LivestockExchangeEndpoints.cs:452` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `SubmitLivestockExchangeRequest` | `API/Platform/LivestockExchangeEndpoints.cs:453` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `AcknowledgeLivestockExchangeRequest` | `API/Platform/LivestockExchangeEndpoints.cs:454` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `RegulatoryDeadlineResponse` | `API/Platform/LivestockExchangeEndpoints.cs:456` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateIntegrationConnectionRequest` | `API/Platform/ProviderIntegrationEndpoints.cs:590` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `UpdateIntegrationConnectionRequest` | `API/Platform/ProviderIntegrationEndpoints.cs:592` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `RotateIntegrationCredentialRequest` | `API/Platform/ProviderIntegrationEndpoints.cs:593` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `DisableIntegrationConnectionRequest` | `API/Platform/ProviderIntegrationEndpoints.cs:595` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateIntegrationFeedRequest` | `API/Platform/ProviderIntegrationEndpoints.cs:596` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `EnqueueIngestionJobRequest` | `API/Platform/ProviderIntegrationEndpoints.cs:599` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ClaimIngestionJobRequest` | `API/Platform/ProviderIntegrationEndpoints.cs:601` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `IngestionRecordRequest` | `API/Platform/ProviderIntegrationEndpoints.cs:602` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ReceiveIngestionRecordsRequest` | `API/Platform/ProviderIntegrationEndpoints.cs:604` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CompleteIngestionJobRequest` | `API/Platform/ProviderIntegrationEndpoints.cs:605` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `FailIngestionJobRequest` | `API/Platform/ProviderIntegrationEndpoints.cs:607` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ReplayIngestionJobRequest` | `API/Platform/ProviderIntegrationEndpoints.cs:609` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `RejectIngestionRecordRequest` | `API/Platform/ProviderIntegrationEndpoints.cs:611` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `IntegrationConnectionResponse` | `API/Platform/ProviderIntegrationEndpoints.cs:613` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `IntegrationConnectionDetailResponse` | `API/Platform/ProviderIntegrationEndpoints.cs:619` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `IntegrationCredentialResponse` | `API/Platform/ProviderIntegrationEndpoints.cs:622` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `IntegrationFeedResponse` | `API/Platform/ProviderIntegrationEndpoints.cs:626` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `IngestionJobResponse` | `API/Platform/ProviderIntegrationEndpoints.cs:632` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ClaimedIngestionJobResponse` | `API/Platform/ProviderIntegrationEndpoints.cs:638` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `IngestionAttemptResponse` | `API/Platform/ProviderIntegrationEndpoints.cs:639` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `IngestionJobDetailResponse` | `API/Platform/ProviderIntegrationEndpoints.cs:642` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ReceiveIngestionRecordsResponse` | `API/Platform/ProviderIntegrationEndpoints.cs:644` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `IngestionRecordResponse` | `API/Platform/ProviderIntegrationEndpoints.cs:645` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateFarmRequest` | `API/Platform/PlatformContracts.cs:9` | `API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `FarmResponse` | `API/Platform/PlatformContracts.cs:17` | `API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `CoordinateRequest` | `API/Platform/PlatformContracts.cs:25` | `API/Platform/FarmDigitalTwinEndpoints.cs`<br>`API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `CreateFieldRequest` | `API/Platform/PlatformContracts.cs:27` | `API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `FieldResponse` | `API/Platform/PlatformContracts.cs:35` | `API/Fields/FieldResponse.cs`<br>`API/Fields/FieldMapper.cs`<br>`API/Fields/FieldEndpoints.cs`<br>`API/Platform/PlatformEndpoints.cs` | AMBIGUOUS: same simple name is declared elsewhere; resolve by namespace before moving. |
| `CreateSeasonRequest` | `API/Platform/PlatformContracts.cs:43` | `API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `SeasonResponse` | `API/Platform/PlatformContracts.cs:50` | `API/Seasons/SeasonResponse.cs`<br>`API/Seasons/SeasonMapper.cs`<br>`API/Seasons/SeasonEndpoints.cs`<br>`API/Platform/PlatformEndpoints.cs` | AMBIGUOUS: same simple name is declared elsewhere; resolve by namespace before moving. |
| `CreateCropZoneRequest` | `API/Platform/PlatformContracts.cs:58` | `API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `CropZoneResponse` | `API/Platform/PlatformContracts.cs:67` | `API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `FieldOperationResponse` | `API/Platform/PlatformContracts.cs:77` | `API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `CreateAgriculturalInputRequest` | `API/Platform/PlatformContracts.cs:85` | `API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `AgriculturalInputResponse` | `API/Platform/PlatformContracts.cs:87` | `API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `CreateInputStockLotRequest` | `API/Platform/PlatformContracts.cs:89` | `API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `InputStockLotResponse` | `API/Platform/PlatformContracts.cs:95` | `API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `HarvestLotResponse` | `API/Platform/PlatformContracts.cs:108` | `API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `InputApplicationTraceResponse` | `API/Platform/PlatformContracts.cs:120` | `API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `HarvestTraceResponse` | `API/Platform/PlatformContracts.cs:129` | `API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `StockLotTraceResponse` | `API/Platform/PlatformContracts.cs:140` | `API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `InitiateEvidenceUploadRequest` | `API/Platform/PlatformContracts.cs:150` | `API/Platform/EvidenceEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `EvidenceUploadResponse` | `API/Platform/PlatformContracts.cs:162` | `API/Platform/EvidenceEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `EvidenceUploadOffsetResponse` | `API/Platform/PlatformContracts.cs:170` | `API/Platform/EvidenceEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `EvidenceResponse` | `API/Platform/PlatformContracts.cs:172` | `API/Platform/EvidenceEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `CreateLivestockSubjectRequest` | `API/Platform/PlatformContracts.cs:185` | `API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `LivestockSubjectResponse` | `API/Platform/PlatformContracts.cs:193` | `API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `CreateVeterinaryMedicineRequest` | `API/Platform/PlatformContracts.cs:201` | `API/Platform/LivestockHealthEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `VeterinaryMedicineResponse` | `API/Platform/PlatformContracts.cs:208` | `API/Platform/LivestockHealthEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `CreateVeterinaryMedicineStockLotRequest` | `API/Platform/PlatformContracts.cs:218` | `API/Platform/LivestockHealthEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `VeterinaryMedicineStockLotResponse` | `API/Platform/PlatformContracts.cs:224` | `API/Platform/LivestockHealthEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `LivestockTreatmentResponse` | `API/Platform/PlatformContracts.cs:237` | `API/Platform/LivestockHealthEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `DispatchEligibilityResponse` | `API/Platform/PlatformContracts.cs:254` | `API/Platform/LivestockHealthEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `CreateLivestockDispatchRequest` | `API/Platform/PlatformContracts.cs:262` | `API/Platform/LivestockHealthEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `LivestockDispatchResponse` | `API/Platform/PlatformContracts.cs:269` | `API/Platform/LivestockHealthEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `SyncPushRequest` | `API/Platform/PlatformContracts.cs:279` | `API/Platform/PlatformEndpoints.cs`<br>`API/Platform/SyncCommandProcessor.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `SyncCommandEnvelope` | `API/Platform/PlatformContracts.cs:281` | `API/Platform/SyncCommandProcessor.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `RecordFieldOperationPayload` | `API/Platform/PlatformContracts.cs:289` | `API/Platform/SyncCommandProcessor.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `RecordCropZoneOperationPayload` | `API/Platform/PlatformContracts.cs:296` | `API/Platform/SyncCommandProcessor.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `ApplyInputToCropZonePayload` | `API/Platform/PlatformContracts.cs:302` | `API/Platform/SyncCommandProcessor.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `RecordHarvestLotPayload` | `API/Platform/PlatformContracts.cs:309` | `API/Platform/SyncCommandProcessor.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `RecordLivestockMeasurementPayload` | `API/Platform/PlatformContracts.cs:317` | `API/Platform/SyncCommandProcessor.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `RecordLivestockTreatmentPayload` | `API/Platform/PlatformContracts.cs:324` | `API/Platform/SyncCommandProcessor.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `RecordLivestockMovementPayload` | `API/Platform/PlatformContracts.cs:332` | `API/Platform/SyncCommandProcessor.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `RecordLivestockHealthEventPayload` | `API/Platform/PlatformContracts.cs:341` | `API/Platform/SyncCommandProcessor.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `AllocateLivestockFeedPayload` | `API/Platform/PlatformContracts.cs:350` | `API/Platform/SyncCommandProcessor.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `StartWorkOrderPayload` | `API/Platform/PlatformContracts.cs:358` | `API/Platform/SyncCommandProcessor.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `SyncWorkOrderResultPayload` | `API/Platform/PlatformContracts.cs:360` | `API/Platform/SyncCommandProcessor.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `CompleteWorkOrderPayload` | `API/Platform/PlatformContracts.cs:362` | `API/Platform/SyncCommandProcessor.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `SyncCommandResult` | `API/Platform/PlatformContracts.cs:368` | `API/Platform/SyncCommandProcessor.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `SyncPushResponse` | `API/Platform/PlatformContracts.cs:376` | `API/Platform/SyncCommandProcessor.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `SyncGapResponse` | `API/Platform/PlatformContracts.cs:378` | `API/Platform/SyncCommandProcessor.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `SyncChangeResponse` | `API/Platform/PlatformContracts.cs:380` | `API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `SyncPullResponse` | `API/Platform/PlatformContracts.cs:388` | `API/Platform/PlatformEndpoints.cs` | Global platform DTO bundle: allocate to owning use case, not a new global Contracts folder. |
| `CreateNotificationRequest` | `API/Platform/NotificationEndpoints.cs:204` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `UpdateNotificationPreferenceRequest` | `API/Platform/NotificationEndpoints.cs:214` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `NotificationPreferenceResponse` | `API/Platform/NotificationEndpoints.cs:215` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `NotificationInboxResponse` | `API/Platform/NotificationEndpoints.cs:216` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `NotificationResponse` | `API/Platform/NotificationEndpoints.cs:217` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `UpdateRegionalSettingsRequest` | `API/Platform/RegionalFoundationEndpoints.cs:213` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `RegionalSettingsResponse` | `API/Platform/RegionalFoundationEndpoints.cs:224` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `MeasurementUnitResponse` | `API/Platform/RegionalFoundationEndpoints.cs:237` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ConvertMeasurementRequest` | `API/Platform/RegionalFoundationEndpoints.cs:245` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ConvertedMeasurementResponse` | `API/Platform/RegionalFoundationEndpoints.cs:246` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateReferenceCatalogueRequest` | `API/Platform/RegionalFoundationEndpoints.cs:255` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ReferenceCatalogueResponse` | `API/Platform/RegionalFoundationEndpoints.cs:264` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateReferenceCatalogueItemRequest` | `API/Platform/RegionalFoundationEndpoints.cs:275` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ReferenceCatalogueItemResponse` | `API/Platform/RegionalFoundationEndpoints.cs:283` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreatePublicApiClientRequest` | `API/Platform/PublicEnterpriseEndpoints.cs:355` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `RotateCredentialRequest` | `API/Platform/PublicEnterpriseEndpoints.cs:356` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateWebhookRequest` | `API/Platform/PublicEnterpriseEndpoints.cs:357` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateDisclosurePolicyRequest` | `API/Platform/PublicEnterpriseEndpoints.cs:358` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `IssueTransparencyTokenRequest` | `API/Platform/PublicEnterpriseEndpoints.cs:359` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateApprovalPolicyRequest` | `API/Platform/PublicEnterpriseEndpoints.cs:360` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateDelegatedApprovalRequest` | `API/Platform/PublicEnterpriseEndpoints.cs:361` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ApprovalDecisionRequest` | `API/Platform/PublicEnterpriseEndpoints.cs:362` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateTenantPlacementRequest` | `API/Platform/PublicEnterpriseEndpoints.cs:363` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateTenantMigrationRequest` | `API/Platform/PublicEnterpriseEndpoints.cs:364` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `FailTenantMigrationRequest` | `API/Platform/PublicEnterpriseEndpoints.cs:365` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ProvisionTenantAccountRequest` | `API/Platform/SaasControlPlaneEndpoints.cs:284` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `LifecycleReasonRequest` | `API/Platform/SaasControlPlaneEndpoints.cs:285` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateSaasPlanRequest` | `API/Platform/SaasControlPlaneEndpoints.cs:286` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreatePlanEntitlementRequest` | `API/Platform/SaasControlPlaneEndpoints.cs:287` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateTenantSubscriptionRequest` | `API/Platform/SaasControlPlaneEndpoints.cs:288` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `SubscriptionCommandRequest` | `API/Platform/SaasControlPlaneEndpoints.cs:289` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `RecordUsageRequest` | `API/Platform/SaasControlPlaneEndpoints.cs:290` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateBillingAccountRequest` | `API/Platform/SaasControlPlaneEndpoints.cs:291` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateBillingInvoiceRequest` | `API/Platform/SaasControlPlaneEndpoints.cs:292` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `AssessSupplierRiskRequest` | `API/Platform/SupplierCompletionEndpoints.cs:343` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `IssueSupplierSelfServiceRequest` | `API/Platform/SupplierCompletionEndpoints.cs:344` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateContractPriceRuleRequest` | `API/Platform/SupplierCompletionEndpoints.cs:345` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ReconcileContractRequest` | `API/Platform/SupplierCompletionEndpoints.cs:348` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateWorkOrderRequest` | `API/Platform/WorkOrderEndpoints.cs:570` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `UpdateWorkOrderDraftRequest` | `API/Platform/WorkOrderEndpoints.cs:574` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `AssignWorkOrderRequest` | `API/Platform/WorkOrderEndpoints.cs:576` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `AddWorkOrderResourceRequest` | `API/Platform/WorkOrderEndpoints.cs:577` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `WorkOrderResultRequest` | `API/Platform/WorkOrderEndpoints.cs:580` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CompleteWorkOrderRequest` | `API/Platform/WorkOrderEndpoints.cs:581` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ReasonRequest` | `API/Platform/WorkOrderEndpoints.cs:582` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CorrectWorkOrderRequest` | `API/Platform/WorkOrderEndpoints.cs:583` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `MaterializePlanRequest` | `API/Platform/WorkOrderEndpoints.cs:585` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `WorkOrderSummaryResponse` | `API/Platform/WorkOrderEndpoints.cs:587` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `WorkOrderDetailResponse` | `API/Platform/WorkOrderEndpoints.cs:592` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `WorkOrderAssignmentResponse` | `API/Platform/WorkOrderEndpoints.cs:599` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `WorkOrderResourceResponse` | `API/Platform/WorkOrderEndpoints.cs:601` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `WorkOrderHistoryResponse` | `API/Platform/WorkOrderEndpoints.cs:605` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `WorkOrderResultResponse` | `API/Platform/WorkOrderEndpoints.cs:607` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `OperationalCalendarItem` | `API/Platform/WorkOrderEndpoints.cs:609` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `OperationalCalendarResponse` | `API/Platform/WorkOrderEndpoints.cs:612` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `TransformationInput` | `API/Platform/TraceabilityEndpoints.cs:442` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `TransformationOutput` | `API/Platform/TraceabilityEndpoints.cs:443` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateTransformationRequest` | `API/Platform/TraceabilityEndpoints.cs:444` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `StorageLotResponse` | `API/Platform/TraceabilityEndpoints.cs:447` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `TransformationResponse` | `API/Platform/TraceabilityEndpoints.cs:449` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `DeliverySourceLine` | `API/Platform/TraceabilityEndpoints.cs:450` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateDeliveryRequest` | `API/Platform/TraceabilityEndpoints.cs:451` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `ReceiveDeliveryRequest` | `API/Platform/TraceabilityEndpoints.cs:453` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `DeliveryResponse` | `API/Platform/TraceabilityEndpoints.cs:454` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `TraceEventResponse` | `API/Platform/TraceabilityEndpoints.cs:456` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `MassBalanceResponse` | `API/Platform/TraceabilityEndpoints.cs:459` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `TraceObjectReference` | `API/Platform/TraceabilityEndpoints.cs:461` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `LineageEdge` | `API/Platform/TraceabilityEndpoints.cs:462` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `LineageGraph` | `API/Platform/TraceabilityEndpoints.cs:464` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateRecallRequest` | `API/Platform/TraceabilityEndpoints.cs:466` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CloseRecallRequest` | `API/Platform/TraceabilityEndpoints.cs:468` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `RecallResponse` | `API/Platform/TraceabilityEndpoints.cs:469` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `RecallImpactResponse` | `API/Platform/TraceabilityEndpoints.cs:471` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateQualityAssessmentRequest` | `API/Platform/TraceabilityEndpoints.cs:473` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `QualityAssessmentResponse` | `API/Platform/TraceabilityEndpoints.cs:475` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `SyncProcessingOutcome` | `API/Platform/SyncCommandProcessor.cs:858` | `API/Platform/PlatformEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `CreateSupplyRelationshipRequest` | `API/Platform/SupplyNetworkEndpoints.cs:273` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `RevokeSupplyAccessRequest` | `API/Platform/SupplyNetworkEndpoints.cs:274` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `SupplyRelationshipResponse` | `API/Platform/SupplyNetworkEndpoints.cs:275` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateDataGrantRequest` | `API/Platform/SupplyNetworkEndpoints.cs:278` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `DataGrantResponse` | `API/Platform/SupplyNetworkEndpoints.cs:282` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CrossTenantAccessAuditResponse` | `API/Platform/SupplyNetworkEndpoints.cs:288` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateSupplierInvitationRequest` | `API/Platform/SupplierWorkflowEndpoints.cs:304` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `AcceptSupplierInvitationRequest` | `API/Platform/SupplierWorkflowEndpoints.cs:305` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateHostedSupplierRequest` | `API/Platform/SupplierWorkflowEndpoints.cs:306` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `SupplierCommandRequest` | `API/Platform/SupplierWorkflowEndpoints.cs:307` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateSupplyContractRequest` | `API/Platform/SupplierWorkflowEndpoints.cs:308` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateContractAllocationRequest` | `API/Platform/SupplierWorkflowEndpoints.cs:311` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `AllocationCommandRequest` | `API/Platform/SupplierWorkflowEndpoints.cs:313` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `CreateSupplierEvidenceRequestRequest` | `API/Platform/SupplierWorkflowEndpoints.cs:314` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `SupplierEvidenceCommandRequest` | `API/Platform/SupplierWorkflowEndpoints.cs:316` | None outside source file | Feature/transport-local: split only with consuming use cases. |
| `BinResponse` | `API/Bins/BinResponse.cs:3` | `API/Bins/BinMapper.cs`<br>`API/Bins/BinEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `BinRequest` | `API/Bins/BinRequest.cs:3` | `API/Bins/BinMapper.cs`<br>`API/Bins/BinEndpoints.cs` | Feature/transport-local: split only with consuming use cases. |
| `BinModel` | `API/Bins/BinModel.cs:5` | `API/Bins/BinResponse.cs`<br>`API/Bins/BinRequest.cs` | Feature/transport-local: split only with consuming use cases. |
| `BaseResponse` | `API/BaseResponse.cs:3` | None outside source file | Feature/transport-local: split only with consuming use cases. |

## HTTP-bound endpoint files

These are explicitly detected HTTP identifiers in files that register routes. Route counts are literal MapGet/Post/Put/Delete/Patch calls, not runtime route inventory: group prefixes, feature flags, MapHealthChecks and dynamic registrations are not expanded. Results named as record properties in PlatformContracts and SyncCommandProcessor are not evidence of ASP.NET dependence and are excluded here. This is a boundary checklist, not a complete transitive call graph.

| Endpoint source | Literal route registrations | HTTP identifiers | Extraction boundary |
| --- | --- | --- | --- |
| `API/Assets/AssetEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Access/AccessEndpoints.cs` | 4 | `IResult`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Access/AuditEndpoints.cs` | 1 | `IResult`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Access/OrganisationEndpoints.cs` | 2 | `IResult`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/YieldMeasurements/YieldMeasurementEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Seasons/SeasonEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/WorkItemSubCategories/WorkItemSubCategoryEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Fields/FieldEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/WorkItemStatuses/WorkItemStatusEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/FieldMeasurements/FieldMeasurementEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/WorkItems/WorkItemEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/CropCategories/CropCategoryEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Reporting/ProductionReportEndpoints.cs` | 1 | `HttpContext`, `IResult`, `Results`, `RouteGroupBuilder`, `TypedResults` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/CropVarieties/CropVarietyEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Equipments/EquipmentEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/WorkItemCategories/WorkItemCategoryEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/BinTypes/BinTypeEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/CropUnits/CropUnitEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/EquipmentCategories/EquipmentCategoryEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Crops/CropEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Customers/CustomerEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/SoilTypes/SoilTypeEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Currencies/CurrencyEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/CropProductions/CropProductionEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/SeasonStatuses/SeasonStatusEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/NumberSequences/NumberSequenceEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Loans/LoanEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Locations/LocationEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/ItemCategories/ItemCategoryEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Platform/AnalyticsMaturityEndpoints.cs` | 12 | `IResult`, `Results`, `RouteGroupBuilder` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Platform/AnalyticsEndpoints.cs` | 21 | `IResult`, `Results`, `RouteGroupBuilder` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/OwnershipTypes/OwnershipTypeEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Platform/IdentitySecurityEndpoints.cs` | 15 | `HttpContext`, `IEndpointRouteBuilder`, `IResult`, `Results`, `RouteGroupBuilder` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/GeospatialImportEndpoints.cs` | 5 | `HttpRequest`, `IResult`, `Results`, `RouteGroupBuilder` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/FinanceEndpoints.cs` | 13 | `HttpContext`, `IResult`, `Results`, `RouteGroupBuilder`, `TypedResults` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/FieldIntelligenceEndpoints.cs` | 20 | `HttpContext`, `IResult`, `Results`, `RouteGroupBuilder`, `TypedResults` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/FarmDigitalTwinEndpoints.cs` | 16 | `HttpContext`, `IResult`, `Results`, `RouteGroupBuilder` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/EvidenceEndpoints.cs` | 5 | `HttpRequest`, `IResult`, `Results`, `RouteGroupBuilder`, `TypedResults` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/EudrEndpoints.cs` | 11 | `HttpContext`, `IResult`, `Results`, `RouteGroupBuilder` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/EpcisEndpoints.cs` | 5 | `HttpRequest`, `HttpResponse`, `IResult`, `Results`, `RouteGroupBuilder`, `TypedResults` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/EnterpriseConsolidationEndpoints.cs` | 9 | `HttpContext`, `IResult`, `Results`, `RouteGroupBuilder` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/CropPlanningEndpoints.cs` | 15 | `HttpContext`, `IResult`, `Results`, `RouteGroupBuilder`, `TypedResults` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/CorrectionEndpoints.cs` | 3 | `IResult`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Platform/ConnectedAgricultureEndpoints.cs` | 22 | `HttpContext`, `IResult`, `Results`, `RouteGroupBuilder` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/ComplianceEndpoints.cs` | 19 | `HttpContext`, `IResult`, `Results`, `RouteGroupBuilder` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/ComplianceAuditPackEndpoints.cs` | 6 | `HttpContext`, `IResult`, `Results`, `RouteGroupBuilder` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/MasterDataImportEndpoints.cs` | 8 | `HttpRequest`, `IResult`, `Results`, `RouteGroupBuilder` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/LivestockStatutoryEndpoints.cs` | 7 | `IResult`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Platform/LivestockRecordsEndpoints.cs` | 17 | `IResult`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Platform/LivestockProductionEndpoints.cs` | 14 | `IResult`, `Results`, `RouteGroupBuilder` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Platform/LivestockHealthEndpoints.cs` | 7 | `HttpContext`, `IResult`, `Results`, `RouteGroupBuilder`, `TypedResults` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/LivestockExchangeEndpoints.cs` | 10 | `IResult`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Platform/ProviderIntegrationEndpoints.cs` | 26 | `HttpContext`, `IResult`, `Results`, `RouteGroupBuilder`, `TypedResults` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/PlatformEndpoints.cs` | 20 | `IEndpointRouteBuilder`, `IResult`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Platform/NotificationEndpoints.cs` | 5 | `IResult`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Platform/RegionalFoundationEndpoints.cs` | 8 | `IResult`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Platform/PublicEnterpriseEndpoints.cs` | 37 | `HttpContext`, `IEndpointRouteBuilder`, `IResult`, `Results`, `RouteGroupBuilder` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/SaasControlPlaneEndpoints.cs` | 19 | `IResult`, `Results`, `RouteGroupBuilder` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Platform/SupplierCompletionEndpoints.cs` | 12 | `HttpContext`, `IEndpointRouteBuilder`, `IResult`, `Results`, `RouteGroupBuilder` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/WorkOrderEndpoints.cs` | 19 | `HttpContext`, `IResult`, `Results`, `RouteGroupBuilder`, `TypedResults` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/TraceabilityEndpoints.cs` | 13 | `IResult`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |
| `API/Platform/SupplyNetworkEndpoints.cs` | 11 | `HttpContext`, `IResult`, `Results`, `RouteGroupBuilder`, `TypedResults` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Platform/SupplierWorkflowEndpoints.cs` | 16 | `HttpContext`, `IResult`, `Results`, `RouteGroupBuilder` | REVIEW: separate HTTP metadata/streams/authentication/header handling from use-case inputs/results before extraction. |
| `API/Bins/BinEndpoints.cs` | 5 | `IEndpointRouteBuilder`, `Results`, `RouteGroupBuilder`, `TypedResults` | Keep registration/results in API; move only framework-independent orchestration. |

## Shared domain-name collisions

| Name | Current declarations | Safe proposed distinction |
| --- | --- | --- |
| `LivestockTreatment` | `Domain/Entities/LivestockProductionAggregate/LivestockTreatment.cs`; `Domain/Platform/Livestock/LivestockTreatment.cs` | Keep legacy `Domain.LivestockProduction` separate from active `Domain.LivestockTreatment`; never merge types or rename classes. |
| `InvoicedItem` | `Domain/Entities/PurchaseInvoiceAggregate/InvoicedItem.cs`; `Domain/Entities/SalesInvoiceAggregate/InvoicedItem.cs` | Keep `Domain.PurchaseInvoice` and `Domain.SalesInvoice` namespaces separate. |

API's legacy and active `FieldResponse`/`SeasonResponse` also share simple names. Identifier scans alone cannot decide which one a reference uses.

