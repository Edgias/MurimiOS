using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.ChangeSoilSample;
using static EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.FarmDigitalTwinEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.ChangeSoilSample;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ChangeSoilSampleEndpoint
{
    internal static void Map(IEndpointRouteBuilder twin)
    {
        twin.MapPost("/soil-samples/{sampleId:guid}/{command:regex(^submit|analyse|reject$)}", async Task<IResult> (Guid farmId, Guid sampleId, string command, SoilSampleCommandRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FarmDigitalTwin.ChangeSoilSample.ChangeSoilSampleCommand(farmId, sampleId, command, new global::EdgIQ.MurimiOS.Application.FarmDigitalTwin.ChangeSoilSample.SoilSampleCommandRequest(request.Reason)), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.FarmWrite);

    }
}
