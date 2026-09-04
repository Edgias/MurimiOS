using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CreateRule;
using static EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.EnterpriseConsolidationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CreateRule;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateRuleEndpoint
{
    internal static void Map(IEndpointRouteBuilder enterprise)
    {
        enterprise.MapPost("/elimination-rules", async Task<IResult> (CreateEliminationRuleRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.EnterpriseConsolidation.CreateRule.CreateRuleCommand(new global::EdgIQ.MurimiOS.Application.EnterpriseConsolidation.CreateRule.CreateEliminationRuleRequest(request.Id, request.ConsolidationOrganisationNodeId, request.SellerCompanyId, request.BuyerCompanyId, request.RuleKey, request.RuleVersion, request.Name, request.Currency, request.BalancingTolerance, request.EffectiveFrom, request.EffectiveTo)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FinanceReconcile);

    }
}
