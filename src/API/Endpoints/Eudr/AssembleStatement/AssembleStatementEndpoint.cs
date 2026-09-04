using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.Eudr.AssembleStatement;
using static EdgIQ.MurimiOS.API.Endpoints.Eudr.EudrEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Eudr.AssembleStatement;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class AssembleStatementEndpoint
{
    internal static void Map(IEndpointRouteBuilder eudr)
    {
        eudr.MapPost("/cases/{caseId:guid}/statements", async Task<IResult> (Guid caseId, AssembleEudrStatementRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Eudr.AssembleStatement.AssembleStatementCommand(caseId, new global::EdgIQ.MurimiOS.Application.Eudr.AssembleStatement.AssembleEudrStatementRequest(request.Id, request.DeclarationText)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.EudrExport);

    }
}
