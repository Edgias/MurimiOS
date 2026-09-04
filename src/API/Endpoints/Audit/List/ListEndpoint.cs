using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Audit.List;
using static EdgIQ.MurimiOS.API.Endpoints.Audit.AuditEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Audit.List;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapGet("/audit/events", async Task<IResult> (DateTimeOffset? before, string? entityType, Guid? entityId, int? limit, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Audit.List.ListQuery(before, entityType, entityId, limit), cancellationToken)).ToHttpResult())
            .WithTags("Audit")
            .RequireAuthorization(PlatformPermissions.AuditRead);

    }
}
