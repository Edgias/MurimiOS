using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.ComplianceFinding;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Compliance.ComplianceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Compliance.ListFindings;
using ComplianceFindingStatus = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFindingStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListFindingsEndpoint
{
    internal static void Map(IEndpointRouteBuilder farms)
    {
        farms.MapGet("/findings", async Task<IResult> (Guid farmId, ComplianceFindingStatus? status, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Compliance.ListFindings.ListFindingsQuery(farmId, status), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.ComplianceRead);

    }
}
