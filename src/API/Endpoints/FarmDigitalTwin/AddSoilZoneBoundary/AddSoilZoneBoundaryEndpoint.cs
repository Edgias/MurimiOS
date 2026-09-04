using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.AddSoilZoneBoundary;
using EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.FarmDigitalTwinEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.AddSoilZoneBoundary;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using CoordinateRequest = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.Shared.CoordinateRequest;
internal static class AddSoilZoneBoundaryEndpoint
{
    internal static void Map(IEndpointRouteBuilder twin)
    {
        twin.MapPost("/soil-zones/{zoneId:guid}/boundaries", async Task<IResult> (Guid farmId, Guid zoneId, AddSoilZoneBoundaryRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FarmDigitalTwin.AddSoilZoneBoundary.AddSoilZoneBoundaryCommand(farmId, zoneId, new global::EdgIQ.MurimiOS.Application.FarmDigitalTwin.AddSoilZoneBoundary.AddSoilZoneBoundaryRequest(request.Id, request.FieldBoundaryVersionId, request.Exterior.Select(item => new global::EdgIQ.MurimiOS.Application.FarmDigitalTwin.AddSoilZoneBoundary.CoordinateRequest(item.Longitude, item.Latitude)).ToArray(), request.ValidFrom, request.Source)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FarmWrite);

    }
}
