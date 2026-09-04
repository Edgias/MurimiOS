using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using static EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.GeospatialImportEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.ListJobs;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListJobsEndpoint
{
    internal static void Map(IEndpointRouteBuilder imports)
    {
        imports.MapGet("/farms/{farmId:guid}/jobs", async Task<IResult> (Guid farmId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.GeospatialImport.ListJobs.ListJobsQuery(farmId), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.ImportsRead);

    }
}
