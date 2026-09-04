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
namespace EdgIQ.MurimiOS.API.Endpoints.MasterDataImport.Template;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class TemplateEndpoint
{
    internal static void Map(IEndpointRouteBuilder imports)
    {
        imports.MapGet("/templates/{type}", async Task<IResult> (string type, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.MasterDataImport.Template.TemplateQuery(type), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ImportsRead);

    }
}
