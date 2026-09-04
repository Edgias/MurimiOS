using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using static EdgIQ.MurimiOS.API.Endpoints.Eudr.EudrEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Eudr.ListCases;
using EudrCaseStatus = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrCaseStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListCasesEndpoint
{
    internal static void Map(IEndpointRouteBuilder eudr)
    {
        eudr.MapGet("/cases", async Task<IResult> (Guid? farmId, EudrCaseStatus? status, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Eudr.ListCases.ListCasesQuery(farmId, status), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.EudrRead);

    }
}
