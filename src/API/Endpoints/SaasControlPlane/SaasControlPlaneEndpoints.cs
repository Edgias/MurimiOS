using System.Data;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane;
using AddEntitlementEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.AddEntitlement.AddEntitlementEndpoint;
using ChangeAccountEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ChangeAccount.ChangeAccountEndpoint;
using ChangeBillingAccountEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ChangeBillingAccount.ChangeBillingAccountEndpoint;
using ChangeInvoiceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ChangeInvoice.ChangeInvoiceEndpoint;
using ChangePlanEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ChangePlan.ChangePlanEndpoint;
using ChangeSubscriptionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ChangeSubscription.ChangeSubscriptionEndpoint;
using CreateBillingAccountEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreateBillingAccount.CreateBillingAccountEndpoint;
using CreateInvoiceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreateInvoice.CreateInvoiceEndpoint;
using CreateSubscriptionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreateSubscription.CreateSubscriptionEndpoint;
using EffectiveEntitlementsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.EffectiveEntitlements.EffectiveEntitlementsEndpoint;
using GetAccountEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.GetAccount.GetAccountEndpoint;
using ListBillingAccountsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ListBillingAccounts.ListBillingAccountsEndpoint;
using ListInvoicesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ListInvoices.ListInvoicesEndpoint;
using ListSubscriptionsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ListSubscriptions.ListSubscriptionsEndpoint;
using ListUsageEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ListUsage.ListUsageEndpoint;
using ProvisionAccountEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ProvisionAccount.ProvisionAccountEndpoint;
using RecordUsageEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.RecordUsage.RecordUsageEndpoint;


public static class SaasControlPlaneEndpoints
{
    public const string PlatformOperatorPolicy = "saas.platform-operator";

    public static void MapSaasControlPlaneEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder group = api.MapGroup("/saas").WithTags("SaaS control plane");
        global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.GetAccount.GetAccountEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ProvisionAccount.ProvisionAccountEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ChangeAccount.ChangeAccountEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ListPlans.ListPlansEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreatePlan.CreatePlanEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.AddEntitlement.AddEntitlementEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ChangePlan.ChangePlanEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ListSubscriptions.ListSubscriptionsEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreateSubscription.CreateSubscriptionEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ChangeSubscription.ChangeSubscriptionEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.EffectiveEntitlements.EffectiveEntitlementsEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ListUsage.ListUsageEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.RecordUsage.RecordUsageEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ListBillingAccounts.ListBillingAccountsEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreateBillingAccount.CreateBillingAccountEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ChangeBillingAccount.ChangeBillingAccountEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ListInvoices.ListInvoicesEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreateInvoice.CreateInvoiceEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ChangeInvoice.ChangeInvoiceEndpoint.Map(group);
    }
}
