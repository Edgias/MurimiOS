using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Production.GetApiSeasonsSeasonIdGuidCropZones;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetApiSeasonsSeasonIdGuidCropZonesEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {

        api.MapGet("/seasons/{seasonId:guid}/crop-zones", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid seasonId) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Production.GetApiSeasonsSeasonIdGuidCropZones.GetApiSeasonsSeasonIdGuidCropZonesQuery(seasonId), cancellationToken)).ToHttpResult()).WithTags("Production").RequireAuthorization(PlatformPermissions.ProductionRead);

    }
}
