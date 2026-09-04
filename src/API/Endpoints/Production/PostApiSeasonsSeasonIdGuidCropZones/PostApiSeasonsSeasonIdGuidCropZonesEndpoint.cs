using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Production.PostApiSeasonsSeasonIdGuidCropZones;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using CoordinateRequest = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.Shared.CoordinateRequest;
internal static class PostApiSeasonsSeasonIdGuidCropZonesEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {

        api.MapPost("/seasons/{seasonId:guid}/crop-zones", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid seasonId, CreateCropZoneRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Production.PostApiSeasonsSeasonIdGuidCropZones.PostApiSeasonsSeasonIdGuidCropZonesCommand(seasonId, new global::EdgIQ.MurimiOS.Application.Production.PostApiSeasonsSeasonIdGuidCropZones.CreateCropZoneRequest(request.Id, request.FieldId, request.BoundaryVersionId, request.Name, request.Crop, request.Variety, request.Exterior is null ? null : request.Exterior.Select(item => new global::EdgIQ.MurimiOS.Application.Production.PostApiSeasonsSeasonIdGuidCropZones.CoordinateRequest(item.Longitude, item.Latitude)).ToArray())), cancellationToken)).ToHttpResult()).WithTags("Production").RequireAuthorization(PlatformPermissions.ProductionWrite);

    }
}
