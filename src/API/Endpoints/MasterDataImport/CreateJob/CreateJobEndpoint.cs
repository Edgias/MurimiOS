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
namespace EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.CreateJob;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using IncomingContentAdapter = global::EdgIQ.MurimiOS.API.Infrastructure.IncomingContentAdapter;
internal static class CreateJobEndpoint
{
    internal static void Map(IEndpointRouteBuilder imports)
    {
        imports.MapPost("/jobs", async Task<IResult> (string type, HttpRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.MasterDataImport.CreateJob.CreateJobCommand(type, EdgIQ.MurimiOS.API.Infrastructure.IncomingContentAdapter.Bind(request)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ImportsManage);

    }
}
