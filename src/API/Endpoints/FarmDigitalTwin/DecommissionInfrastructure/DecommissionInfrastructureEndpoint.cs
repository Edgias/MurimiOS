using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.DecommissionInfrastructure;
using static EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.FarmDigitalTwinEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.DecommissionInfrastructure;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class DecommissionInfrastructureEndpoint
{
    internal static void Map(IEndpointRouteBuilder twin)
    {
        twin.MapPost("/infrastructure/{assetId:guid}/decommission", async Task<IResult> (Guid farmId, Guid assetId, DecommissionInfrastructureRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FarmDigitalTwin.DecommissionInfrastructure.DecommissionInfrastructureCommand(farmId, assetId, new global::EdgIQ.MurimiOS.Application.FarmDigitalTwin.DecommissionInfrastructure.DecommissionInfrastructureRequest(request.On, request.Reason)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FarmWrite);

    }
}
