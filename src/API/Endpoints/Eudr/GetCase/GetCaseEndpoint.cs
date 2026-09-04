using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using static EdgIQ.MurimiOS.API.Endpoints.Eudr.EudrEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Eudr.GetCase;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetCaseEndpoint
{
    internal static void Map(IEndpointRouteBuilder eudr)
    {
        eudr.MapGet("/cases/{caseId:guid}", async Task<IResult> (Guid caseId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Eudr.GetCase.GetCaseQuery(caseId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.EudrRead);

    }
}
