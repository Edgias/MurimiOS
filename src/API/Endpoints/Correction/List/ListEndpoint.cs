using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.API.Endpoints.Audit.List;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Correction.CorrectionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Correction.List;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapGet("/corrections", async Task<IResult> (string? entityType, Guid? entityId, int? limit, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Correction.List.ListQuery(entityType, entityId, limit), cancellationToken)).ToHttpResult())
            .WithTags("Corrections")
            .RequireAuthorization(PlatformPermissions.AuditRead);

    }
}
