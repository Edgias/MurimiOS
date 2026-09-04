using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Traceability.GetRecallImpact;
using static EdgIQ.MurimiOS.API.Endpoints.Traceability.TraceabilityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Traceability.GetRecallImpact;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetRecallImpactEndpoint
{
    internal static void Map(IEndpointRouteBuilder recalls)
    {
        recalls.MapGet("/{recallId:guid}/impact", async Task<IResult> (Guid recallId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Traceability.GetRecallImpact.GetRecallImpactQuery(recallId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.TraceabilityRead);

    }
}
