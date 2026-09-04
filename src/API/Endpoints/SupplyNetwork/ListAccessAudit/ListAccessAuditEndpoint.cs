using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.ListAccessAudit;
using static EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.SupplyNetworkEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.ListAccessAudit;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListAccessAuditEndpoint
{
    internal static void Map(IEndpointRouteBuilder supply)
    {
        supply.MapGet("/access-audit", async Task<IResult> (Guid? grantId, int? limit, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplyNetwork.ListAccessAudit.ListAccessAuditQuery(grantId, limit), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SupplyNetworkRead);

    }
}
