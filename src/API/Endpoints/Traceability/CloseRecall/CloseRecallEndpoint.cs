using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Traceability.CloseRecall;
using static EdgIQ.MurimiOS.API.Endpoints.Traceability.TraceabilityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.CloseRecall;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CloseRecallEndpoint
{
    internal static void Map(IEndpointRouteBuilder recalls)
    {
        recalls.MapPost("/{recallId:guid}/close", async Task<IResult> (Guid recallId, CloseRecallRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Traceability.CloseRecall.CloseRecallCommand(recallId, new global::EdgIQ.MurimiOS.Application.Traceability.CloseRecall.CloseRecallRequest(request.ClosedAt)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.RecallManage);

    }
}
