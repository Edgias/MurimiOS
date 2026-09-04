using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CreateSoilSample;
using static EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.FarmDigitalTwinEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.CreateSoilSample;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateSoilSampleEndpoint
{
    internal static void Map(IEndpointRouteBuilder twin)
    {
        twin.MapPost("/soil-samples", async Task<IResult> (Guid farmId, CreateSoilSampleRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FarmDigitalTwin.CreateSoilSample.CreateSoilSampleCommand(farmId, new global::EdgIQ.MurimiOS.Application.FarmDigitalTwin.CreateSoilSample.CreateSoilSampleRequest(request.Id, request.FieldId, request.FieldBoundaryVersionId, request.SoilZoneBoundaryVersionId, request.SampleCode, request.Longitude, request.Latitude, request.SampledAt, request.SampledBy, request.DepthFromCm, request.DepthToCm, request.LaboratoryName, request.LaboratoryReference, request.Notes)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FarmWrite);

    }
}
