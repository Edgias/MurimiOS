using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Sync.GetSyncPull;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetSyncPullEndpoint
{
    internal static void Map(IEndpointRouteBuilder sync)
    {

        sync.MapGet("/pull", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, long checkpoint, int? limit) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Sync.GetSyncPull.GetSyncPullQuery(checkpoint, limit), cancellationToken)).ToHttpResult()).Produces<SyncPullResponse>(200).RequireAuthorization(PlatformPermissions.SyncUse);

    }
}
