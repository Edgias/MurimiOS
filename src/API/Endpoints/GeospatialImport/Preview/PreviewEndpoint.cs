using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using static EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.GeospatialImportEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.Preview;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class PreviewEndpoint
{
    internal static void Map(IEndpointRouteBuilder imports)
    {
        imports.MapGet("/jobs/{jobId:guid}/preview", async Task<IResult> (Guid jobId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.GeospatialImport.Preview.PreviewQuery(jobId), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.ImportsRead);

    }
}
