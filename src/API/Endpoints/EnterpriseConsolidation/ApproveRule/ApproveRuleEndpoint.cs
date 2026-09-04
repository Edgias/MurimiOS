using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.ApproveRule;
using static EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.EnterpriseConsolidationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.ApproveRule;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ApproveRuleEndpoint
{
    internal static void Map(IEndpointRouteBuilder enterprise)
    {
        enterprise.MapPost("/elimination-rules/{id:guid}/approve", async Task<IResult> (Guid id, ApproveEliminationRuleRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.EnterpriseConsolidation.ApproveRule.ApproveRuleCommand(id, new global::EdgIQ.MurimiOS.Application.EnterpriseConsolidation.ApproveRule.ApproveEliminationRuleRequest(request.ApprovalReference)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FinanceReconcile);

    }
}
