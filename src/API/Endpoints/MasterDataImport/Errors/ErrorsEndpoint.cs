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
using static EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.MasterDataImportEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.Errors;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ErrorsEndpoint
{
    internal static void Map(IEndpointRouteBuilder imports)
    {
        imports.MapGet("/jobs/{jobId:guid}/errors.csv", async Task<IResult> (Guid jobId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.MasterDataImport.Errors.ErrorsQuery(jobId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ImportsRead);

    }
}
