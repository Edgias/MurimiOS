using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.DerivedMediaAsset;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ConnectedAgricultureEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ConnectedAgriculture.ListDerivedMedia;
using DerivedMediaLifecycleStatus = global::EdgIQ.MurimiOS.Domain.DerivedMediaAsset.DerivedMediaLifecycleStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListDerivedMediaEndpoint
{
    internal static void Map(IEndpointRouteBuilder connected)
    {
        connected.MapGet("/farms/{farmId:guid}/derived-media", async Task<IResult> (Guid farmId, DerivedMediaLifecycleStatus? status, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ConnectedAgriculture.ListDerivedMedia.ListDerivedMediaQuery(farmId, status), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ConnectedAgricultureRead);

    }
}
