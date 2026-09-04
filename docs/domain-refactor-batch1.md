# Domain refactoring — first approved batch

Completed: 2026-09-04. Scope: structural backend changes only. This completes the first approved move batch, not all of Phase 2 or the complete vertical-slice refactor.

## Delivered

- Relocated `Domain/Domain.csproj` to `src/Domain/Domain.csproj` and updated solution, Application, test and SeedData project references.
- Moved 145 files: 137 remain in Domain and eight existing interfaces now live in `src/Application/Common/Abstractions`.
- Regrouped legacy aggregates, the shared foundation and unambiguous platform records. Domain and the moved interfaces now use folder-aligned `EdgIQ.MurimiOS.*` namespaces.
- Preserved public simple type names, method bodies, enum values and existing MediatR/NetTopologySuite package versions. No endpoint logic, persistence mappings, API contracts, PWA code or worker workflows were redesigned.
- Updated consumer imports and qualified references in Infrastructure, API, Worker, tools and tests. Local type aliases resolve the cases where a sibling aggregate namespace shares a class name.
- Enabled IDE0130 warnings for the migrated Domain/Application paths; migration diagnostics are explicitly exempted.
- Added four boundary tests covering interface ownership, Domain dependency direction, namespace prefixes and retained legacy event/distinct aggregate contracts.

## Eight moved contracts

`ITenantContext`, `IAuditActorContext`, `IAppLogger<T>`, `IFileSystem`, `IMessagingService`, `IQuotationService`, `ISalesOrderService`, and `ISalesInvoiceService` now belong to Application. Their names and signatures are unchanged. Domain does not reference Application. Existing request/worker implementations and test doubles were updated to use the new contract namespaces.

## Build-layout adjustment

The proposed `src/Domain/Bin/Bin.cs` destination collided with the SDK's case-insensitive `bin` exclusion on Windows. The actual location is `src/Domain/Bins/Bin.cs`, with namespace `EdgIQ.MurimiOS.Domain.Bins`; the class is still named `Bin`. No SDK output exclusions or entity mappings were changed. The final move manifest records this adjustment.

## Verification

| Check | Result |
| --- | --- |
| `dotnet build MurimiOS.sln --no-restore` (after restore for the relocated project) | Passed; zero warnings and errors. |
| `dotnet test MurimiOS.sln --no-build --no-restore` | 211 passed: 206 Platform + 5 Application. No failures. |
| Opt-in integration/load tests | 28 skipped; no test database was provisioned or mutated. |
| `dotnet ef migrations has-pending-model-changes --project Infrastructure/Infrastructure.csproj --startup-project API/API.csproj --context PlatformDbContext --no-build` | No pending model changes after namespace relocation. |
| Migration SHA-256 comparison | All 125 migration/snapshot files unchanged at their original paths. |
| Structural source comparison | All 497 pre-existing non-migration C# files match their archived bodies after normalizing namespace declarations, imports and qualified type references. This is a source-preservation check, not a substitute for runtime integration tests. |
| Move manifest | All 145 destinations exist; old source file paths are absent. |
| Git index | No staged changes remain. No commits were made. |

The structural verifier is `tools/Refactoring/Move-DomainBatch.ps1 -Verify`. Its default mode is a preflight for the original baseline; `-Apply` is one-shot and refuses to overwrite an existing baseline. Do not rerun the move operation on the relocated tree.

## Recovery and working-tree handling

Before editing, the batch captured source bytes, hashes and a move/type map under `output/refactoring/domain-batch-20260904`:

- `sources.zip`: original source/project/solution files, with repository-relative entry paths, including the frozen migrations.
- `baseline.json`: original SHA-256 hashes plus actual source/destination and namespace mappings.

This generated recovery directory is ignored by Git. It must not be committed or blindly extracted over subsequent user edits. Recovery should compare individual current files with their archived originals and reverse only the chosen batch changes. Existing build-output directories were not recursively deleted.

Tracked sources used `git mv`; untracked sources used validated, non-overwriting filesystem moves. Git's automatic staging of tracked renames was subsequently undone for those exact paths only, restoring the initial unstaged index state. No untracked user implementation was staged or committed.

## Remaining before Phase 2 is complete

- Thirty bundled files remain under `src/Domain/Platform`. Their namespace prefix is updated, but aggregate ownership and splitting remain deferred as approved.
- `Common/Enums.cs` still contains the legacy shared enum bundle; assign each enum to its owner without changing numeric values or attributes.
- The three legacy `Exceptions` files await throw/catch-site ownership review. No exception was deleted or behavior changed.
- `IRequestActorContext` and `OrganisationScopeAccess` still live in API. Their separation belongs to the subsequent application-boundary work.
- Domain retains MediatR 12.4.1 and NetTopologySuite 2.6.0 as explicitly approved. The legacy event pipeline is not converted to Dispatcher.
- Infrastructure/API/Worker source namespaces and physical project locations are still at their respective earlier refactoring stages; only their Domain/contract references changed here.
- Application use-case extraction, thin endpoints and the complete architecture test project are still pending.

Next: review and split the deferred Domain bundles by aggregate, including shared enum/exception ownership, then verify the same model, migration and behavior-preservation checks before extracting Application handlers.

## Actual file moves

Paths are repository-relative. The inventory documents remain the historical pre-move proposal; this is the completed first-batch register.

| Original path | Current path | Original Git status |
| --- | --- | --- |
| `Domain/Domain.csproj` | `src/Domain/Domain.csproj` | Tracked |
| `Domain/Entities/Address.cs` | `src/Domain/Common/Address.cs` | Tracked |
| `Domain/Entities/Asset.cs` | `src/Domain/Asset/Asset.cs` | Tracked |
| `Domain/Entities/Bin.cs` | `src/Domain/Bins/Bin.cs` | Tracked |
| `Domain/Entities/BinType.cs` | `src/Domain/BinType/BinType.cs` | Tracked |
| `Domain/Entities/Contact.cs` | `src/Domain/Contact/Contact.cs` | Tracked |
| `Domain/Entities/Crop.cs` | `src/Domain/Crop/Crop.cs` | Tracked |
| `Domain/Entities/CropCategory.cs` | `src/Domain/CropCategory/CropCategory.cs` | Tracked |
| `Domain/Entities/CropProductionAggregate/CropProduction.cs` | `src/Domain/CropProduction/CropProduction.cs` | Tracked |
| `Domain/Entities/CropProductionAggregate/CropProductionField.cs` | `src/Domain/CropProduction/CropProductionField.cs` | Tracked |
| `Domain/Entities/CropProductionAggregate/CropProductionVariety.cs` | `src/Domain/CropProduction/CropProductionVariety.cs` | Tracked |
| `Domain/Entities/CropUnit.cs` | `src/Domain/CropUnit/CropUnit.cs` | Tracked |
| `Domain/Entities/CropVariety.cs` | `src/Domain/CropVariety/CropVariety.cs` | Tracked |
| `Domain/Entities/Currency.cs` | `src/Domain/Currency/Currency.cs` | Tracked |
| `Domain/Entities/Customer.cs` | `src/Domain/Customer/Customer.cs` | Tracked |
| `Domain/Entities/Equipment.cs` | `src/Domain/Equipment/Equipment.cs` | Tracked |
| `Domain/Entities/EquipmentCategory.cs` | `src/Domain/EquipmentCategory/EquipmentCategory.cs` | Tracked |
| `Domain/Entities/Field.cs` | `src/Domain/Field/Field.cs` | Tracked |
| `Domain/Entities/FieldMeasurement.cs` | `src/Domain/FieldMeasurement/FieldMeasurement.cs` | Tracked |
| `Domain/Entities/Item.cs` | `src/Domain/Item/Item.cs` | Tracked |
| `Domain/Entities/ItemCategory.cs` | `src/Domain/ItemCategory/ItemCategory.cs` | Tracked |
| `Domain/Entities/LivestockGroup.cs` | `src/Domain/LivestockGroup/LivestockGroup.cs` | Tracked |
| `Domain/Entities/LivestockProductionAggregate/Livestock.cs` | `src/Domain/LivestockProduction/Livestock.cs` | Tracked |
| `Domain/Entities/LivestockProductionAggregate/LivestockFeeding.cs` | `src/Domain/LivestockProduction/LivestockFeeding.cs` | Tracked |
| `Domain/Entities/LivestockProductionAggregate/LivestockGrazing.cs` | `src/Domain/LivestockProduction/LivestockGrazing.cs` | Tracked |
| `Domain/Entities/LivestockProductionAggregate/LivestockInput.cs` | `src/Domain/LivestockProduction/LivestockInput.cs` | Tracked |
| `Domain/Entities/LivestockProductionAggregate/LivestockMeasurement.cs` | `src/Domain/LivestockProduction/LivestockMeasurement.cs` | Tracked |
| `Domain/Entities/LivestockProductionAggregate/LivestockPhoto.cs` | `src/Domain/LivestockProduction/LivestockPhoto.cs` | Tracked |
| `Domain/Entities/LivestockProductionAggregate/LivestockTreatment.cs` | `src/Domain/LivestockProduction/LivestockTreatment.cs` | Tracked |
| `Domain/Entities/Loan.cs` | `src/Domain/Loan/Loan.cs` | Tracked |
| `Domain/Entities/Location.cs` | `src/Domain/Location/Location.cs` | Tracked |
| `Domain/Entities/NumberSequence.cs` | `src/Domain/NumberSequence/NumberSequence.cs` | Tracked |
| `Domain/Entities/OwnershipType.cs` | `src/Domain/OwnershipType/OwnershipType.cs` | Tracked |
| `Domain/Entities/PriceList.cs` | `src/Domain/PriceList/PriceList.cs` | Tracked |
| `Domain/Entities/PurchaseInvoiceAggregate/InvoicedItem.cs` | `src/Domain/PurchaseInvoice/InvoicedItem.cs` | Tracked |
| `Domain/Entities/PurchaseInvoiceAggregate/PurchaseInvoice.cs` | `src/Domain/PurchaseInvoice/PurchaseInvoice.cs` | Tracked |
| `Domain/Entities/PurchaseInvoiceAggregate/PurchaseInvoiceItem.cs` | `src/Domain/PurchaseInvoice/PurchaseInvoiceItem.cs` | Tracked |
| `Domain/Entities/QuotationAggregate/ItemQuoted.cs` | `src/Domain/Quotation/ItemQuoted.cs` | Tracked |
| `Domain/Entities/QuotationAggregate/Quotation.cs` | `src/Domain/Quotation/Quotation.cs` | Tracked |
| `Domain/Entities/QuotationAggregate/QuotationItem.cs` | `src/Domain/Quotation/QuotationItem.cs` | Tracked |
| `Domain/Entities/SalesInvoiceAggregate/InvoicedItem.cs` | `src/Domain/SalesInvoice/InvoicedItem.cs` | Tracked |
| `Domain/Entities/SalesInvoiceAggregate/SalesInvoice.cs` | `src/Domain/SalesInvoice/SalesInvoice.cs` | Tracked |
| `Domain/Entities/SalesInvoiceAggregate/SalesInvoiceItem.cs` | `src/Domain/SalesInvoice/SalesInvoiceItem.cs` | Tracked |
| `Domain/Entities/SalesInvoiceNote.cs` | `src/Domain/SalesInvoiceNote/SalesInvoiceNote.cs` | Tracked |
| `Domain/Entities/SalesOrderAggregate/ItemOrdered.cs` | `src/Domain/SalesOrder/ItemOrdered.cs` | Tracked |
| `Domain/Entities/SalesOrderAggregate/SalesOrder.cs` | `src/Domain/SalesOrder/SalesOrder.cs` | Tracked |
| `Domain/Entities/SalesOrderAggregate/SalesOrderItem.cs` | `src/Domain/SalesOrder/SalesOrderItem.cs` | Tracked |
| `Domain/Entities/Season.cs` | `src/Domain/Season/Season.cs` | Tracked |
| `Domain/Entities/SeasonStatus.cs` | `src/Domain/SeasonStatus/SeasonStatus.cs` | Tracked |
| `Domain/Entities/SoilType.cs` | `src/Domain/SoilType/SoilType.cs` | Tracked |
| `Domain/Entities/Supplier.cs` | `src/Domain/Supplier/Supplier.cs` | Tracked |
| `Domain/Entities/Tax.cs` | `src/Domain/Tax/Tax.cs` | Tracked |
| `Domain/Entities/UnitGroup.cs` | `src/Domain/UnitGroup/UnitGroup.cs` | Tracked |
| `Domain/Entities/UnitMeasurement.cs` | `src/Domain/UnitMeasurement/UnitMeasurement.cs` | Tracked |
| `Domain/Entities/Warehouse.cs` | `src/Domain/Warehouse/Warehouse.cs` | Tracked |
| `Domain/Entities/WorkItem.cs` | `src/Domain/WorkItem/WorkItem.cs` | Tracked |
| `Domain/Entities/WorkItemCategory.cs` | `src/Domain/WorkItemCategory/WorkItemCategory.cs` | Tracked |
| `Domain/Entities/WorkItemStatus.cs` | `src/Domain/WorkItemStatus/WorkItemStatus.cs` | Tracked |
| `Domain/Entities/WorkItemSubCategory.cs` | `src/Domain/WorkItemSubCategory/WorkItemSubCategory.cs` | Tracked |
| `Domain/Entities/YieldMeasurement.cs` | `src/Domain/YieldMeasurement/YieldMeasurement.cs` | Tracked |
| `Domain/Events/LoanApprovedEvent.cs` | `src/Domain/Loan/Events/LoanApprovedEvent.cs` | Tracked |
| `Domain/Exceptions/DataStoreException.cs` | `src/Domain/Exceptions/DataStoreException.cs` | Tracked |
| `Domain/Exceptions/DuplicateFoundException.cs` | `src/Domain/Exceptions/DuplicateFoundException.cs` | Tracked |
| `Domain/Exceptions/MessagingException.cs` | `src/Domain/Exceptions/MessagingException.cs` | Tracked |
| `Domain/GlobalUsings.cs` | `src/Domain/GlobalUsings.cs` | Tracked |
| `Domain/Interfaces/IAppLogger.cs` | `src/Application/Common/Abstractions/IAppLogger.cs` | Tracked |
| `Domain/Interfaces/IFileSystem.cs` | `src/Application/Common/Abstractions/IFileSystem.cs` | Tracked |
| `Domain/Interfaces/IMessagingService.cs` | `src/Application/Common/Abstractions/IMessagingService.cs` | Tracked |
| `Domain/Interfaces/IQuotationService.cs` | `src/Application/Common/Abstractions/IQuotationService.cs` | Tracked |
| `Domain/Interfaces/ISalesInvoiceService.cs` | `src/Application/Common/Abstractions/ISalesInvoiceService.cs` | Tracked |
| `Domain/Interfaces/ISalesOrderService.cs` | `src/Application/Common/Abstractions/ISalesOrderService.cs` | Tracked |
| `Domain/Platform/Access/MembershipPermissionGrant.cs` | `src/Domain/TenantMembership/MembershipPermissionGrant.cs` | Untracked |
| `Domain/Platform/Access/PlatformPermissions.cs` | `src/Domain/TenantMembership/PlatformPermissions.cs` | Untracked |
| `Domain/Platform/Access/TenantMembership.cs` | `src/Domain/TenantMembership/TenantMembership.cs` | Untracked |
| `Domain/Platform/Analytics/AnalyticsMaturityModels.cs` | `src/Domain/Platform/Analytics/AnalyticsMaturityModels.cs` | Untracked |
| `Domain/Platform/Analytics/AnalyticsModels.cs` | `src/Domain/Platform/Analytics/AnalyticsModels.cs` | Untracked |
| `Domain/Platform/Auditing/AuditEvent.cs` | `src/Domain/AuditEvent/AuditEvent.cs` | Untracked |
| `Domain/Platform/Auditing/CorrectionRecord.cs` | `src/Domain/CorrectionRecord/CorrectionRecord.cs` | Untracked |
| `Domain/Platform/Auditing/IAuditActorContext.cs` | `src/Application/Common/Abstractions/IAuditActorContext.cs` | Untracked |
| `Domain/Platform/Compliance/ComplianceModels.cs` | `src/Domain/Platform/Compliance/ComplianceModels.cs` | Untracked |
| `Domain/Platform/Configuration/BusinessTime.cs` | `src/Domain/Common/BusinessTime.cs` | Untracked |
| `Domain/Platform/Configuration/MeasurementUnits.cs` | `src/Domain/Common/MeasurementUnits.cs` | Untracked |
| `Domain/Platform/Configuration/Money.cs` | `src/Domain/Common/Money.cs` | Untracked |
| `Domain/Platform/Configuration/ReferenceCatalogue.cs` | `src/Domain/ReferenceCatalogue/ReferenceCatalogue.cs` | Untracked |
| `Domain/Platform/Configuration/TenantRegionalSettings.cs` | `src/Domain/TenantRegionalSettings/TenantRegionalSettings.cs` | Untracked |
| `Domain/Platform/Eudr/EudrModels.cs` | `src/Domain/Platform/Eudr/EudrModels.cs` | Untracked |
| `Domain/Platform/Evidence/EvidenceAttachment.cs` | `src/Domain/EvidenceAttachment/EvidenceAttachment.cs` | Untracked |
| `Domain/Platform/Evidence/MediaUploadSession.cs` | `src/Domain/MediaUploadSession/MediaUploadSession.cs` | Untracked |
| `Domain/Platform/External/PublicEnterpriseModels.cs` | `src/Domain/Platform/External/PublicEnterpriseModels.cs` | Untracked |
| `Domain/Platform/Farms/DigitalTwinModels.cs` | `src/Domain/Platform/Farms/DigitalTwinModels.cs` | Untracked |
| `Domain/Platform/Farms/Farm.cs` | `src/Domain/Farm/Farm.cs` | Untracked |
| `Domain/Platform/Farms/FarmField.cs` | `src/Domain/FarmField/FarmField.cs` | Untracked |
| `Domain/Platform/Farms/FieldBoundaryVersion.cs` | `src/Domain/FarmField/FieldBoundaryVersion.cs` | Untracked |
| `Domain/Platform/FieldIntelligence/FieldIntelligenceModels.cs` | `src/Domain/Platform/FieldIntelligence/FieldIntelligenceModels.cs` | Untracked |
| `Domain/Platform/Finance/EnterpriseConsolidationModels.cs` | `src/Domain/Platform/Finance/EnterpriseConsolidationModels.cs` | Untracked |
| `Domain/Platform/Finance/FinanceModels.cs` | `src/Domain/Platform/Finance/FinanceModels.cs` | Untracked |
| `Domain/Platform/Finance/FinanceOpeningBalance.cs` | `src/Domain/FinanceOpeningBalance/FinanceOpeningBalance.cs` | Untracked |
| `Domain/Platform/Imports/GeospatialImportModels.cs` | `src/Domain/Platform/Imports/GeospatialImportModels.cs` | Untracked |
| `Domain/Platform/Imports/MasterDataImportModels.cs` | `src/Domain/Platform/Imports/MasterDataImportModels.cs` | Untracked |
| `Domain/Platform/Integrations/ConnectedAgricultureModels.cs` | `src/Domain/Platform/Integrations/ConnectedAgricultureModels.cs` | Untracked |
| `Domain/Platform/Integrations/ConnectedDataLifecycle.cs` | `src/Domain/Platform/Integrations/ConnectedDataLifecycle.cs` | Untracked |
| `Domain/Platform/Integrations/ProviderIngestionModels.cs` | `src/Domain/Platform/Integrations/ProviderIngestionModels.cs` | Untracked |
| `Domain/Platform/Inventory/AgriculturalInput.cs` | `src/Domain/AgriculturalInput/AgriculturalInput.cs` | Untracked |
| `Domain/Platform/Inventory/InputStockLot.cs` | `src/Domain/InputStockLot/InputStockLot.cs` | Untracked |
| `Domain/Platform/Inventory/InventoryMovement.cs` | `src/Domain/InventoryMovement/InventoryMovement.cs` | Untracked |
| `Domain/Platform/Livestock/LivestockDispatch.cs` | `src/Domain/LivestockDispatch/LivestockDispatch.cs` | Untracked |
| `Domain/Platform/Livestock/LivestockFeed.cs` | `src/Domain/Platform/Livestock/LivestockFeed.cs` | Untracked |
| `Domain/Platform/Livestock/LivestockHistory.cs` | `src/Domain/Platform/Livestock/LivestockHistory.cs` | Untracked |
| `Domain/Platform/Livestock/LivestockLaunchIntegrations.cs` | `src/Domain/Platform/Livestock/LivestockLaunchIntegrations.cs` | Untracked |
| `Domain/Platform/Livestock/LivestockMeasurementRecord.cs` | `src/Domain/LivestockMeasurementRecord/LivestockMeasurementRecord.cs` | Untracked |
| `Domain/Platform/Livestock/LivestockProductionModels.cs` | `src/Domain/Platform/Livestock/LivestockProductionModels.cs` | Untracked |
| `Domain/Platform/Livestock/LivestockRegistry.cs` | `src/Domain/Platform/Livestock/LivestockRegistry.cs` | Untracked |
| `Domain/Platform/Livestock/LivestockStatutory.cs` | `src/Domain/Platform/Livestock/LivestockStatutory.cs` | Untracked |
| `Domain/Platform/Livestock/LivestockSubject.cs` | `src/Domain/LivestockSubject/LivestockSubject.cs` | Untracked |
| `Domain/Platform/Livestock/LivestockTreatment.cs` | `src/Domain/LivestockTreatment/LivestockTreatment.cs` | Untracked |
| `Domain/Platform/Livestock/VeterinaryMedicine.cs` | `src/Domain/VeterinaryMedicine/VeterinaryMedicine.cs` | Untracked |
| `Domain/Platform/Livestock/VeterinaryMedicineStockLot.cs` | `src/Domain/VeterinaryMedicineStockLot/VeterinaryMedicineStockLot.cs` | Untracked |
| `Domain/Platform/Notifications/NotificationPreference.cs` | `src/Domain/NotificationPreference/NotificationPreference.cs` | Untracked |
| `Domain/Platform/Notifications/NotificationRecord.cs` | `src/Domain/NotificationRecord/NotificationRecord.cs` | Untracked |
| `Domain/Platform/Operations/FieldOperationRecord.cs` | `src/Domain/FieldOperationRecord/FieldOperationRecord.cs` | Untracked |
| `Domain/Platform/Operations/WorkOrderModels.cs` | `src/Domain/Platform/Operations/WorkOrderModels.cs` | Untracked |
| `Domain/Platform/Organisation/OrganisationNode.cs` | `src/Domain/OrganisationNode/OrganisationNode.cs` | Untracked |
| `Domain/Platform/Planning/CropPlanningModels.cs` | `src/Domain/Platform/Planning/CropPlanningModels.cs` | Untracked |
| `Domain/Platform/Production/CropZone.cs` | `src/Domain/CropZone/CropZone.cs` | Untracked |
| `Domain/Platform/Production/GrowingSeason.cs` | `src/Domain/GrowingSeason/GrowingSeason.cs` | Untracked |
| `Domain/Platform/Production/HarvestLot.cs` | `src/Domain/HarvestLot/HarvestLot.cs` | Untracked |
| `Domain/Platform/Reporting/ReportExportRecord.cs` | `src/Domain/ReportExportRecord/ReportExportRecord.cs` | Untracked |
| `Domain/Platform/Saas/SaasControlPlaneModels.cs` | `src/Domain/Platform/Saas/SaasControlPlaneModels.cs` | Untracked |
| `Domain/Platform/Security/IdentitySecurityModels.cs` | `src/Domain/Platform/Security/IdentitySecurityModels.cs` | Untracked |
| `Domain/Platform/SupplyNetwork/SupplierCompletionModels.cs` | `src/Domain/Platform/SupplyNetwork/SupplierCompletionModels.cs` | Untracked |
| `Domain/Platform/SupplyNetwork/SupplierWorkflowModels.cs` | `src/Domain/Platform/SupplyNetwork/SupplierWorkflowModels.cs` | Untracked |
| `Domain/Platform/SupplyNetwork/SupplyNetworkModels.cs` | `src/Domain/Platform/SupplyNetwork/SupplyNetworkModels.cs` | Untracked |
| `Domain/Platform/Tenancy/ITenantContext.cs` | `src/Application/Common/Abstractions/ITenantContext.cs` | Untracked |
| `Domain/Platform/Tenancy/TenantEntity.cs` | `src/Domain/Common/TenantEntity.cs` | Untracked |
| `Domain/Platform/Traceability/Delivery.cs` | `src/Domain/Delivery/Delivery.cs` | Untracked |
| `Domain/Platform/Traceability/EpcisInteropModels.cs` | `src/Domain/Platform/Traceability/EpcisInteropModels.cs` | Untracked |
| `Domain/Platform/Traceability/RecallAndQuality.cs` | `src/Domain/Platform/Traceability/RecallAndQuality.cs` | Untracked |
| `Domain/Platform/Traceability/StorageLot.cs` | `src/Domain/StorageLot/StorageLot.cs` | Untracked |
| `Domain/Platform/Traceability/TraceabilityModels.cs` | `src/Domain/Platform/Traceability/TraceabilityModels.cs` | Untracked |
| `Domain/SharedKernel/BaseDomainEvent.cs` | `src/Domain/Common/BaseDomainEvent.cs` | Tracked |
| `Domain/SharedKernel/BaseEntity.cs` | `src/Domain/Common/BaseEntity.cs` | Tracked |
| `Domain/SharedKernel/Enums.cs` | `src/Domain/Common/Enums.cs` | Tracked |
| `Domain/SharedKernel/Guard.cs` | `src/Domain/Common/Guard.cs` | Tracked |
| `Domain/SharedKernel/IAggregateRoot.cs` | `src/Domain/Common/IAggregateRoot.cs` | Tracked |
| `Domain/SharedKernel/IgnoreMemberAttribute.cs` | `src/Domain/Common/IgnoreMemberAttribute.cs` | Tracked |

