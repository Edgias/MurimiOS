using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Inventory.GetApiCropZonesCropZoneIdGuidHarvestLots;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetApiCropZonesCropZoneIdGuidHarvestLotsEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {

        api.MapGet("/crop-zones/{cropZoneId:guid}/harvest-lots", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid cropZoneId) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Inventory.GetApiCropZonesCropZoneIdGuidHarvestLots.GetApiCropZonesCropZoneIdGuidHarvestLotsQuery(cropZoneId), cancellationToken)).ToHttpResult()).WithTags("Traceability").RequireAuthorization(PlatformPermissions.ProductionRead);

    }
}
