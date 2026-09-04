using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.Eudr.SubmitStatement;
using static EdgIQ.MurimiOS.API.Endpoints.Eudr.EudrEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Eudr.SubmitStatement;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class SubmitStatementEndpoint
{
    internal static void Map(IEndpointRouteBuilder eudr)
    {
        eudr.MapPost("/statements/{statementId:guid}/submit", async Task<IResult> (Guid statementId, SubmitEudrStatementRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Eudr.SubmitStatement.SubmitStatementCommand(statementId, new global::EdgIQ.MurimiOS.Application.Eudr.SubmitStatement.SubmitEudrStatementRequest(request.SubmissionReference)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.EudrExport);

    }
}
