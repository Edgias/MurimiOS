using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Data.Common;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.MasterDataImportJob;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using NetTopologySuite.IO;
using static EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.MasterDataImportEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.ListJobs;
using MasterDataImportType = global::EdgIQ.MurimiOS.Domain.MasterDataImportJob.MasterDataImportType;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListJobsEndpoint
{
    internal static void Map(IEndpointRouteBuilder imports)
    {
        imports.MapGet("/jobs", async Task<IResult> (MasterDataImportType? type, int? limit, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.MasterDataImport.ListJobs.ListJobsQuery(type, limit), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ImportsRead);

    }
}
