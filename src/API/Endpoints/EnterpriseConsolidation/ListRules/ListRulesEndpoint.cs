using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.EnterpriseConsolidationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.ListRules;
using EliminationRuleStatus = global::EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule.EliminationRuleStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListRulesEndpoint
{
    internal static void Map(IEndpointRouteBuilder enterprise)
    {
        enterprise.MapGet("/elimination-rules", async Task<IResult> (Guid? organisationNodeId, EliminationRuleStatus? status, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.EnterpriseConsolidation.ListRules.ListRulesQuery(organisationNodeId, status), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FinanceRead);

    }
}
