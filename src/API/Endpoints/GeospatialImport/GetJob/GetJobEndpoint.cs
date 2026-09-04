using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.GetJob;
using static EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.GeospatialImportEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.GetJob;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetJobEndpoint
{
    internal static void Map(IEndpointRouteBuilder imports)
    {
        imports.MapGet("/jobs/{jobId:guid}", async Task<IResult> (Guid jobId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.GeospatialImport.GetJob.GetJobQuery(jobId), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.ImportsRead);

    }
}
