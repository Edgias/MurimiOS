using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Cryptography;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.Apply;
using static EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.GeospatialImportEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.Apply;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ApplyEndpoint
{
    internal static void Map(IEndpointRouteBuilder imports)
    {
        imports.MapPost("/jobs/{jobId:guid}/apply", async Task<IResult> (Guid jobId, ApplyGeospatialImportRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.GeospatialImport.Apply.ApplyCommand(jobId, new global::EdgIQ.MurimiOS.Application.GeospatialImport.Apply.ApplyGeospatialImportRequest(request.Features.Select(item => new global::EdgIQ.MurimiOS.Application.GeospatialImport.Apply.ApplyGeospatialImportFeature(item.FeatureId, item.ValidFrom, item.FieldId, item.FieldName)).ToArray())), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.ImportsManage);

    }
}
