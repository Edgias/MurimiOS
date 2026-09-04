using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CreateTenure;
using static EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.FarmDigitalTwinEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CreateTenure;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateTenureEndpoint
{
    internal static void Map(IEndpointRouteBuilder twin)
    {
        twin.MapPost("/land-tenures", async Task<IResult> (Guid farmId, CreateLandTenureRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FarmDigitalTwin.CreateTenure.CreateTenureCommand(farmId, new global::EdgIQ.MurimiOS.Application.FarmDigitalTwin.CreateTenure.CreateLandTenureRequest(request.Id, request.FieldId, request.Type, request.HolderName, request.CounterpartyName, request.Reference, request.RegisteredAreaHa, request.ValidFrom, request.ValidTo, request.DocumentReference, request.Notes)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FarmWrite);

    }
}
