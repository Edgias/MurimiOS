using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Sync.PostSyncPush;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class PostSyncPushEndpoint
{
    internal static void Map(IEndpointRouteBuilder sync)
    {

        sync.MapPost("/push", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, SyncPushRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Sync.PostSyncPush.PostSyncPushCommand(new global::EdgIQ.MurimiOS.Application.Sync.PostSyncPush.SyncPushRequest(request.DeviceId, request.Commands.Select(item => new global::EdgIQ.MurimiOS.Application.Sync.PostSyncPush.SyncCommandEnvelope(item.CommandId, item.Sequence, item.Type, item.SchemaVersion, item.CapturedAt, item.Payload)).ToArray())), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SyncUse);

    }
}
