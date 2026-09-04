using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using static EdgIQ.MurimiOS.API.Endpoints.Eudr.EudrEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Eudr.ExportStatement;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ExportStatementEndpoint
{
    internal static void Map(IEndpointRouteBuilder eudr)
    {
        eudr.MapPost("/statements/{statementId:guid}/export", async Task<IResult> (Guid statementId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Eudr.ExportStatement.ExportStatementCommand(statementId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.EudrExport);

    }
}
