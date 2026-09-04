using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Data.Common;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using EdgIQ.MurimiOS.API.Endpoints.GeospatialImport.Apply;
using static EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.MasterDataImportEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.Apply;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ApplyEndpoint
{
    internal static void Map(IEndpointRouteBuilder imports)
    {
        imports.MapPost("/jobs/{jobId:guid}/apply", async Task<IResult> (Guid jobId, int? batchSize, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.MasterDataImport.Apply.ApplyCommand(jobId, batchSize), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ImportsManage);

    }
}
