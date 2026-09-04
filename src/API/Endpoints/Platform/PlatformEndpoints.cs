using EdgIQ.MurimiOS.API.Endpoints.Analytics;
using EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity;
using EdgIQ.MurimiOS.API.Endpoints.Compliance;
using EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture;
using EdgIQ.MurimiOS.API.Endpoints.Correction;
using EdgIQ.MurimiOS.API.Endpoints.CropPlanning;
using EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation;
using EdgIQ.MurimiOS.API.Endpoints.Epcis;
using EdgIQ.MurimiOS.API.Endpoints.Eudr;
using EdgIQ.MurimiOS.API.Endpoints.Evidence;
using EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin;
using EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence;
using EdgIQ.MurimiOS.API.Endpoints.Finance;
using EdgIQ.MurimiOS.API.Endpoints.GeospatialImport;
using EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity;
using EdgIQ.MurimiOS.API.Endpoints.LivestockExchange;
using EdgIQ.MurimiOS.API.Endpoints.LivestockHealth;
using EdgIQ.MurimiOS.API.Endpoints.LivestockProduction;
using EdgIQ.MurimiOS.API.Endpoints.LivestockRecords;
using EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory;
using EdgIQ.MurimiOS.API.Endpoints.MasterDataImport;
using EdgIQ.MurimiOS.API.Endpoints.Notification;
using EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration;
using EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise;
using EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation;
using EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane;
using EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion;
using EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow;
using EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork;
using EdgIQ.MurimiOS.API.Endpoints.Traceability;
using EdgIQ.MurimiOS.API.Endpoints.WorkOrder;
using EdgIQ.MurimiOS.API.Endpoints.Access;
using EdgIQ.MurimiOS.API.Endpoints.Audit;
using EdgIQ.MurimiOS.API.Endpoints.Organisation;

using EdgIQ.MurimiOS.API.Endpoints.ProductionReport;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;

namespace EdgIQ.MurimiOS.API.Endpoints.Platform;
using GetFarmsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Farms.GetFarms.GetFarmsEndpoint;
using GetFarmsFarmIdGuidFieldsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Farms.GetFarmsFarmIdGuidFields.GetFarmsFarmIdGuidFieldsEndpoint;
using PostFarmsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Farms.PostFarms.PostFarmsEndpoint;
using PostFarmsFarmIdGuidFieldsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Farms.PostFarmsFarmIdGuidFields.PostFarmsFarmIdGuidFieldsEndpoint;
using GetApiCropZonesCropZoneIdGuidHarvestLotsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Inventory.GetApiCropZonesCropZoneIdGuidHarvestLots.GetApiCropZonesCropZoneIdGuidHarvestLotsEndpoint;
using GetApiFarmsFarmIdGuidInputsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Inventory.GetApiFarmsFarmIdGuidInputs.GetApiFarmsFarmIdGuidInputsEndpoint;
using GetApiInputsInputIdGuidStockLotsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Inventory.GetApiInputsInputIdGuidStockLots.GetApiInputsInputIdGuidStockLotsEndpoint;
using GetApiStockLotsStockLotIdGuidTraceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Inventory.GetApiStockLotsStockLotIdGuidTrace.GetApiStockLotsStockLotIdGuidTraceEndpoint;
using PostApiFarmsFarmIdGuidInputsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Inventory.PostApiFarmsFarmIdGuidInputs.PostApiFarmsFarmIdGuidInputsEndpoint;
using PostApiInputsInputIdGuidStockLotsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Inventory.PostApiInputsInputIdGuidStockLots.PostApiInputsInputIdGuidStockLotsEndpoint;
using GetLivestockSubjectsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Livestock.GetLivestockSubjects.GetLivestockSubjectsEndpoint;
using PostLivestockSubjectsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Livestock.PostLivestockSubjects.PostLivestockSubjectsEndpoint;
using GetApiPlatformEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Platform.GetApiPlatform.GetApiPlatformEndpoint;
using GetApiCropZonesCropZoneIdGuidOperationsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Production.GetApiCropZonesCropZoneIdGuidOperations.GetApiCropZonesCropZoneIdGuidOperationsEndpoint;
using GetApiFarmsFarmIdGuidSeasonsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Production.GetApiFarmsFarmIdGuidSeasons.GetApiFarmsFarmIdGuidSeasonsEndpoint;
using GetApiSeasonsSeasonIdGuidCropZonesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Production.GetApiSeasonsSeasonIdGuidCropZones.GetApiSeasonsSeasonIdGuidCropZonesEndpoint;
using PostApiFarmsFarmIdGuidSeasonsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Production.PostApiFarmsFarmIdGuidSeasons.PostApiFarmsFarmIdGuidSeasonsEndpoint;
using PostApiSeasonsSeasonIdGuidCropZonesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Production.PostApiSeasonsSeasonIdGuidCropZones.PostApiSeasonsSeasonIdGuidCropZonesEndpoint;
using GetSyncPullEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Sync.GetSyncPull.GetSyncPullEndpoint;
using PostSyncPushEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush.PostSyncPushEndpoint;

using AccessEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.Access.AccessEndpoints;
using AuditEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.Audit.AuditEndpoints;
using OrganisationEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.Organisation.OrganisationEndpoints;
using AnalyticsEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.AnalyticsEndpoints;
using AnalyticsMaturityEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.AnalyticsMaturityEndpoints;
using ComplianceEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.ComplianceEndpoints;
using ConnectedAgricultureEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ConnectedAgricultureEndpoints;
using CorrectionEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.Correction.CorrectionEndpoints;
using CropPlanningEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CropPlanningEndpoints;
using EnterpriseConsolidationEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.EnterpriseConsolidationEndpoints;
using EpcisEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.Epcis.EpcisEndpoints;
using EudrEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.Eudr.EudrEndpoints;
using EvidenceEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.Evidence.EvidenceEndpoints;
using FarmDigitalTwinEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.FarmDigitalTwinEndpoints;
using FieldIntelligenceEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.FieldIntelligenceEndpoints;
using FinanceEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.Finance.FinanceEndpoints;
using GeospatialImportEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.GeospatialImportEndpoints;
using IdentitySecurityEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.IdentitySecurityEndpoints;
using LivestockExchangeEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.LivestockExchangeEndpoints;
using LivestockHealthEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.LivestockHealth.LivestockHealthEndpoints;
using LivestockProductionEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.LivestockProductionEndpoints;
using LivestockRecordsEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.LivestockRecordsEndpoints;
using LivestockStatutoryEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.LivestockStatutory.LivestockStatutoryEndpoints;
using MasterDataImportEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.MasterDataImportEndpoints;
using NotificationEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.Notification.NotificationEndpoints;
using ProviderIntegrationEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ProviderIntegrationEndpoints;
using PublicEnterpriseEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
using RegionalFoundationEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.RegionalFoundation.RegionalFoundationEndpoints;
using SaasControlPlaneEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.SaasControlPlaneEndpoints;
using SupplierCompletionEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.SupplierCompletionEndpoints;
using SupplierWorkflowEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.SupplierWorkflowEndpoints;
using SupplyNetworkEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.SupplyNetworkEndpoints;
using TraceabilityEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.Traceability.TraceabilityEndpoints;
using WorkOrderEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.WorkOrder.WorkOrderEndpoints;
using ProductionReportEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.ProductionReport.ProductionReportEndpoints;

public static class PlatformEndpoints
{
    public static IEndpointRouteBuilder MapPlatformEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder api = endpoints.MapGroup("/api/v1").RequireAuthorization();
        global::EdgIQ.MurimiOS.API.Endpoints.Platform.GetApiPlatform.GetApiPlatformEndpoint.Map(api);

        AccessEndpoints.MapAccessEndpoints(api);
        AuditEndpoints.MapAuditEndpoints(api);
        OrganisationEndpoints.MapOrganisationEndpoints(api);

        MapFarmEndpoints(api);
        FarmDigitalTwinEndpoints.Map(api);
        MapProductionEndpoints(api);
        MapInventoryEndpoints(api);
        EvidenceEndpoints.MapEvidenceEndpoints(api);
        ProductionReportEndpoints.MapProductionReportEndpoints(api);
        MapLivestockEndpoints(api);
        LivestockHealthEndpoints.MapLivestockHealthEndpoints(api);
        LivestockRecordsEndpoints.MapLivestockRecordsEndpoints(api);
        LivestockProductionEndpoints.MapLivestockProductionEndpoints(api);
        LivestockStatutoryEndpoints.MapLivestockStatutoryEndpoints(api);
        LivestockExchangeEndpoints.Map(api);
        CorrectionEndpoints.MapCorrectionEndpoints(api);
        NotificationEndpoints.MapNotificationEndpoints(api);
        RegionalFoundationEndpoints.MapRegionalFoundationEndpoints(api);
        TraceabilityEndpoints.MapTraceabilityEndpoints(api);
        EpcisEndpoints.Map(api);
        SupplyNetworkEndpoints.MapSupplyNetworkEndpoints(api);
        SupplierWorkflowEndpoints.MapSupplierWorkflowEndpoints(api);
        SupplierCompletionEndpoints.Map(endpoints, api);
        ComplianceEndpoints.MapComplianceEndpoints(api);
        EudrEndpoints.MapEudrEndpoints(api);
        CropPlanningEndpoints.MapCropPlanningEndpoints(api);
        FinanceEndpoints.MapFinanceEndpoints(api);
        EnterpriseConsolidationEndpoints.Map(api);
        WorkOrderEndpoints.MapWorkOrderEndpoints(api);
        ProviderIntegrationEndpoints.MapProviderIntegrationEndpoints(api);
        ConnectedAgricultureEndpoints.MapConnectedAgricultureEndpoints(api);
        AnalyticsEndpoints.MapAnalyticsEndpoints(api);
        AnalyticsMaturityEndpoints.Map(api);
        FieldIntelligenceEndpoints.MapFieldIntelligenceEndpoints(api);
        GeospatialImportEndpoints.MapGeospatialImportEndpoints(api);
        MasterDataImportEndpoints.Map(api);
        PublicEnterpriseEndpoints.MapPublicEnterpriseEndpoints(endpoints, api);
        SaasControlPlaneEndpoints.MapSaasControlPlaneEndpoints(api);
        IdentitySecurityEndpoints.MapIdentitySecurityEndpoints(endpoints, api);
        MapSyncEndpoints(api);
        return endpoints;
    }

    private static void MapInventoryEndpoints(RouteGroupBuilder api)
    {
        global::EdgIQ.MurimiOS.API.Endpoints.Inventory.GetApiFarmsFarmIdGuidInputs.GetApiFarmsFarmIdGuidInputsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.Inventory.PostApiFarmsFarmIdGuidInputs.PostApiFarmsFarmIdGuidInputsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.Inventory.GetApiInputsInputIdGuidStockLots.GetApiInputsInputIdGuidStockLotsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.Inventory.PostApiInputsInputIdGuidStockLots.PostApiInputsInputIdGuidStockLotsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.Inventory.GetApiStockLotsStockLotIdGuidTrace.GetApiStockLotsStockLotIdGuidTraceEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.Inventory.GetApiCropZonesCropZoneIdGuidHarvestLots.GetApiCropZonesCropZoneIdGuidHarvestLotsEndpoint.Map(api);
    }

    private static void MapProductionEndpoints(RouteGroupBuilder api)
    {
        global::EdgIQ.MurimiOS.API.Endpoints.Production.GetApiFarmsFarmIdGuidSeasons.GetApiFarmsFarmIdGuidSeasonsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.Production.PostApiFarmsFarmIdGuidSeasons.PostApiFarmsFarmIdGuidSeasonsEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.Production.GetApiSeasonsSeasonIdGuidCropZones.GetApiSeasonsSeasonIdGuidCropZonesEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.Production.PostApiSeasonsSeasonIdGuidCropZones.PostApiSeasonsSeasonIdGuidCropZonesEndpoint.Map(api);
        global::EdgIQ.MurimiOS.API.Endpoints.Production.GetApiCropZonesCropZoneIdGuidOperations.GetApiCropZonesCropZoneIdGuidOperationsEndpoint.Map(api);
    }

    private static void MapFarmEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder farms = api.MapGroup("/farms").WithTags("Farms");
        global::EdgIQ.MurimiOS.API.Endpoints.Farms.GetFarms.GetFarmsEndpoint.Map(farms);
        global::EdgIQ.MurimiOS.API.Endpoints.Farms.PostFarms.PostFarmsEndpoint.Map(farms);
        global::EdgIQ.MurimiOS.API.Endpoints.Farms.GetFarmsFarmIdGuidFields.GetFarmsFarmIdGuidFieldsEndpoint.Map(farms);
        global::EdgIQ.MurimiOS.API.Endpoints.Farms.PostFarmsFarmIdGuidFields.PostFarmsFarmIdGuidFieldsEndpoint.Map(farms);
    }

    private static void MapLivestockEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder livestock = api.MapGroup("/livestock").WithTags("Livestock");
        global::EdgIQ.MurimiOS.API.Endpoints.Livestock.GetLivestockSubjects.GetLivestockSubjectsEndpoint.Map(livestock);
        global::EdgIQ.MurimiOS.API.Endpoints.Livestock.PostLivestockSubjects.PostLivestockSubjectsEndpoint.Map(livestock);
    }

    private static void MapSyncEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder sync = api.MapGroup("/sync").WithTags("Sync");
        global::EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush.PostSyncPushEndpoint.Map(sync);
        global::EdgIQ.MurimiOS.API.Endpoints.Sync.GetSyncPull.GetSyncPullEndpoint.Map(sync);
    }
}
