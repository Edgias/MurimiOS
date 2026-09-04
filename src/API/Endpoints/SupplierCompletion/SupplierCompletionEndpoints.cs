using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion;
using AssessRiskEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.AssessRisk.AssessRiskEndpoint;
using ConsumeSelfServiceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.ConsumeSelfService.ConsumeSelfServiceEndpoint;
using CreatePriceRuleEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.CreatePriceRule.CreatePriceRuleEndpoint;
using IssueSelfServiceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.IssueSelfService.IssueSelfServiceEndpoint;
using LinkHistoryEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.LinkHistory.LinkHistoryEndpoint;
using ListPriceRulesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.ListPriceRules.ListPriceRulesEndpoint;
using ListReconciliationsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.ListReconciliations.ListReconciliationsEndpoint;
using ListRiskAssessmentsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.ListRiskAssessments.ListRiskAssessmentsEndpoint;
using PreviewSelfServiceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.PreviewSelfService.PreviewSelfServiceEndpoint;
using QuoteEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.Quote.QuoteEndpoint;
using ReconcileEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.Reconcile.ReconcileEndpoint;
using RevokeSelfServiceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.RevokeSelfService.RevokeSelfServiceEndpoint;


public static class SupplierCompletionEndpoints
{

    public static void Map(IEndpointRouteBuilder endpoints, RouteGroupBuilder api)
    {
        RouteGroupBuilder portal = api.MapGroup("/supplier-portal").WithTags("Supplier portal");
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.ListRiskAssessments.ListRiskAssessmentsEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.AssessRisk.AssessRiskEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.IssueSelfService.IssueSelfServiceEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.RevokeSelfService.RevokeSelfServiceEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.LinkHistory.LinkHistoryEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.ListPriceRules.ListPriceRulesEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.CreatePriceRule.CreatePriceRuleEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.Quote.QuoteEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.ListReconciliations.ListReconciliationsEndpoint.Map(portal);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.Reconcile.ReconcileEndpoint.Map(portal);

        RouteGroupBuilder publicApi = endpoints.MapGroup("/api/public/v1/supplier-accounts")
            .WithTags("Supplier self-service").AllowAnonymous();
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.PreviewSelfService.PreviewSelfServiceEndpoint.Map(publicApi);
        global::EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.ConsumeSelfService.ConsumeSelfServiceEndpoint.Map(publicApi);
    }
}
