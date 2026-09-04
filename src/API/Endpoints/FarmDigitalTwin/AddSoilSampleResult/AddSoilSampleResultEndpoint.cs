using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.AddSoilSampleResult;
using static EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.FarmDigitalTwinEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FarmDigitalTwin.AddSoilSampleResult;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class AddSoilSampleResultEndpoint
{
    internal static void Map(IEndpointRouteBuilder twin)
    {
        twin.MapPost("/soil-samples/{sampleId:guid}/results", async Task<IResult> (Guid farmId, Guid sampleId, CreateSoilSampleResultRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FarmDigitalTwin.AddSoilSampleResult.AddSoilSampleResultCommand(farmId, sampleId, new global::EdgIQ.MurimiOS.Application.FarmDigitalTwin.AddSoilSampleResult.CreateSoilSampleResultRequest(request.Id, request.AnalyteCode, request.AnalyteName, request.Value, request.Unit, request.Method, request.DetectionLimit, request.ResultFlag, request.RecordedAt)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FarmWrite);

    }
}
