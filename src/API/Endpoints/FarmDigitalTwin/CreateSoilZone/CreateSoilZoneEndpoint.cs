using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CreateSoilZone;
using EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.FarmDigitalTwinEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CreateSoilZone;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using CoordinateRequest = global::EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.Shared.CoordinateRequest;
internal static class CreateSoilZoneEndpoint
{
    internal static void Map(IEndpointRouteBuilder twin)
    {
        twin.MapPost("/soil-zones", async Task<IResult> (Guid farmId, CreateSoilZoneRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FarmDigitalTwin.CreateSoilZone.CreateSoilZoneCommand(farmId, new global::EdgIQ.MurimiOS.Application.FarmDigitalTwin.CreateSoilZone.CreateSoilZoneRequest(request.Id, request.BoundaryId, request.FieldId, request.FieldBoundaryVersionId, request.Code, request.Name, request.Classification, request.ManagementGuidance, request.Exterior.Select(item => new global::EdgIQ.MurimiOS.Application.FarmDigitalTwin.CreateSoilZone.CoordinateRequest(item.Longitude, item.Latitude)).ToArray(), request.ValidFrom, request.Source)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FarmWrite);

    }
}
