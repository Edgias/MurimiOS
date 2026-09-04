using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using static EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.GeospatialImportEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.CreateJob;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using IncomingContentAdapter = global::EdgIQ.MurimiOS.API.Infrastructure.IncomingContentAdapter;
internal static class CreateJobEndpoint
{
    internal static void Map(IEndpointRouteBuilder imports)
    {
        imports.MapPost("/farms/{farmId:guid}/jobs", async Task<IResult> (Guid farmId, HttpRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.GeospatialImport.CreateJob.CreateJobCommand(farmId, EdgIQ.MurimiOS.API.Infrastructure.IncomingContentAdapter.Bind(request)), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.ImportsManage);

    }
}
