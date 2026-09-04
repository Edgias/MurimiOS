using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;


using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise;
using CancelMigrationEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CancelMigration.CancelMigrationEndpoint;
using ChangeApprovalRequestEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ChangeApprovalRequest.ChangeApprovalRequestEndpoint;
using ChangeClientEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ChangeClient.ChangeClientEndpoint;
using ChangeDisclosurePolicyEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ChangeDisclosurePolicy.ChangeDisclosurePolicyEndpoint;
using ChangeWebhookEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ChangeWebhook.ChangeWebhookEndpoint;
using ClosePlacementEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ClosePlacement.ClosePlacementEndpoint;
using CreateApprovalPolicyEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateApprovalPolicy.CreateApprovalPolicyEndpoint;
using CreateApprovalRequestEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateApprovalRequest.CreateApprovalRequestEndpoint;
using CreateClientEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateClient.CreateClientEndpoint;
using CreateDisclosurePolicyEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateDisclosurePolicy.CreateDisclosurePolicyEndpoint;
using CreateMigrationEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateMigration.CreateMigrationEndpoint;
using CreatePlacementEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreatePlacement.CreatePlacementEndpoint;
using CreateWebhookEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateWebhook.CreateWebhookEndpoint;
using DecideApprovalEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.DecideApproval.DecideApprovalEndpoint;
using FailMigrationEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.FailMigration.FailMigrationEndpoint;
using GetEnterpriseApprovalPoliciesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.GetEnterpriseApprovalPolicies.GetEnterpriseApprovalPoliciesEndpoint;
using GetGroupAccessLogEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.GetGroupAccessLog.GetGroupAccessLogEndpoint;
using GetGroupClientsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.GetGroupClients.GetGroupClientsEndpoint;
using GetGroupWebhooksEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.GetGroupWebhooks.GetGroupWebhooksEndpoint;
using GetGroupWebhooksSubscriptionIdGuidDeliveriesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.GetGroupWebhooksSubscriptionIdGuidDeliveries.GetGroupWebhooksSubscriptionIdGuidDeliveriesEndpoint;
using GetRetailAccessLogEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.GetRetailAccessLog.GetRetailAccessLogEndpoint;
using GetRetailPoliciesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.GetRetailPolicies.GetRetailPoliciesEndpoint;
using GetRetailTokensEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.GetRetailTokens.GetRetailTokensEndpoint;
using IssueTransparencyTokenEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.IssueTransparencyToken.IssueTransparencyTokenEndpoint;
using ListApprovalRequestsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ListApprovalRequests.ListApprovalRequestsEndpoint;
using ListMigrationJobsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ListMigrationJobs.ListMigrationJobsEndpoint;
using ListPlacementsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ListPlacements.ListPlacementsEndpoint;
using PostEnterpriseApprovalPoliciesPolicyIdGuidRetireEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PostEnterpriseApprovalPoliciesPolicyIdGuidRetire.PostEnterpriseApprovalPoliciesPolicyIdGuidRetireEndpoint;
using PostGroupCredentialsCredentialIdGuidRevokeEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PostGroupCredentialsCredentialIdGuidRevoke.PostGroupCredentialsCredentialIdGuidRevokeEndpoint;
using PostGroupWebhookDeliveriesDeliveryIdGuidReplayEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PostGroupWebhookDeliveriesDeliveryIdGuidReplay.PostGroupWebhookDeliveriesDeliveryIdGuidReplayEndpoint;
using PostRetailTokensTokenIdGuidRevokeEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PostRetailTokensTokenIdGuidRevoke.PostRetailTokensTokenIdGuidRevokeEndpoint;
using PublicAnalyticsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicAnalytics.PublicAnalyticsEndpoint;
using PublicFarmsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicFarms.PublicFarmsEndpoint;
using PublicHarvestsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicHarvests.PublicHarvestsEndpoint;
using PublicTraceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicTrace.PublicTraceEndpoint;
using ResolveTransparencyEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ResolveTransparency.ResolveTransparencyEndpoint;
using RotateClientCredentialEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.RotateClientCredential.RotateClientCredentialEndpoint;


public static class PublicEnterpriseEndpoints
{
    public static void MapPublicEnterpriseEndpoints(IEndpointRouteBuilder endpoints, RouteGroupBuilder api)
    {
        MapPublicApiManagement(api); MapRetailManagement(api); MapEnterprise(api);
        RouteGroupBuilder publicApi = endpoints.MapGroup("/api/public/v1").WithTags("Public API").AllowAnonymous();
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicFarms.PublicFarmsEndpoint.Map(publicApi);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicHarvests.PublicHarvestsEndpoint.Map(publicApi);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicTrace.PublicTraceEndpoint.Map(publicApi);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicAnalytics.PublicAnalyticsEndpoint.Map(publicApi);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ResolveTransparency.ResolveTransparencyEndpoint.Map(publicApi);
    }

    private static void MapPublicApiManagement(RouteGroupBuilder api)
    {
        RouteGroupBuilder group = api.MapGroup("/public-api").WithTags("Public API management");
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.GetGroupClients.GetGroupClientsEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateClient.CreateClientEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ChangeClient.ChangeClientEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.RotateClientCredential.RotateClientCredentialEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PostGroupCredentialsCredentialIdGuidRevoke.PostGroupCredentialsCredentialIdGuidRevokeEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.GetGroupAccessLog.GetGroupAccessLogEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.GetGroupWebhooks.GetGroupWebhooksEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateWebhook.CreateWebhookEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ChangeWebhook.ChangeWebhookEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.GetGroupWebhooksSubscriptionIdGuidDeliveries.GetGroupWebhooksSubscriptionIdGuidDeliveriesEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PostGroupWebhookDeliveriesDeliveryIdGuidReplay.PostGroupWebhookDeliveriesDeliveryIdGuidReplayEndpoint.Map(group);
    }

    private static void MapRetailManagement(RouteGroupBuilder api)
    {
        RouteGroupBuilder retail = api.MapGroup("/retail-transparency").WithTags("Retail transparency");
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.GetRetailPolicies.GetRetailPoliciesEndpoint.Map(retail);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateDisclosurePolicy.CreateDisclosurePolicyEndpoint.Map(retail);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ChangeDisclosurePolicy.ChangeDisclosurePolicyEndpoint.Map(retail);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.GetRetailTokens.GetRetailTokensEndpoint.Map(retail);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.IssueTransparencyToken.IssueTransparencyTokenEndpoint.Map(retail);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PostRetailTokensTokenIdGuidRevoke.PostRetailTokensTokenIdGuidRevokeEndpoint.Map(retail);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.GetRetailAccessLog.GetRetailAccessLogEndpoint.Map(retail);
    }

    private static void MapEnterprise(RouteGroupBuilder api)
    {
        RouteGroupBuilder enterprise = api.MapGroup("/enterprise").WithTags("Enterprise");
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.GetEnterpriseApprovalPolicies.GetEnterpriseApprovalPoliciesEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateApprovalPolicy.CreateApprovalPolicyEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PostEnterpriseApprovalPoliciesPolicyIdGuidRetire.PostEnterpriseApprovalPoliciesPolicyIdGuidRetireEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ListApprovalRequests.ListApprovalRequestsEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateApprovalRequest.CreateApprovalRequestEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.DecideApproval.DecideApprovalEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ChangeApprovalRequest.ChangeApprovalRequestEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ListPlacements.ListPlacementsEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreatePlacement.CreatePlacementEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ClosePlacement.ClosePlacementEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ListMigrationJobs.ListMigrationJobsEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateMigration.CreateMigrationEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CancelMigration.CancelMigrationEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.FailMigration.FailMigrationEndpoint.Map(enterprise);
    }
}
