using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CreateConsolidation;
using static EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.EnterpriseConsolidationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CreateConsolidation;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateConsolidationEndpoint
{
    internal static void Map(IEndpointRouteBuilder enterprise)
    {
        enterprise.MapPost("/organisations/{organisationNodeId:guid}/consolidations", async Task<IResult> (Guid organisationNodeId, CreateEnterpriseConsolidationRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.EnterpriseConsolidation.CreateConsolidation.CreateConsolidationCommand(organisationNodeId, new global::EdgIQ.MurimiOS.Application.EnterpriseConsolidation.CreateConsolidation.CreateEnterpriseConsolidationRequest(request.Id, request.PeriodStart, request.PeriodEnd, request.Currency)), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.FinanceReconcile);

    }
}
