using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using static EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.FarmDigitalTwinEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.ListSoilZones;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListSoilZonesEndpoint
{
    internal static void Map(IEndpointRouteBuilder twin)
    {
        twin.MapGet("/soil-zones", async Task<IResult> (Guid farmId, Guid? fieldId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FarmDigitalTwin.ListSoilZones.ListSoilZonesQuery(farmId, fieldId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FarmRead);

    }
}
