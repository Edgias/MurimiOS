using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using static EdgIQ.MurimiOS.API.Endpoints.Eudr.EudrEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Eudr.GetStatement;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetStatementEndpoint
{
    internal static void Map(IEndpointRouteBuilder eudr)
    {
        eudr.MapGet("/statements/{statementId:guid}", async Task<IResult> (Guid statementId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Eudr.GetStatement.GetStatementQuery(statementId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.EudrRead);

    }
}
