using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.SupplyNetworkEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.ReadSharedHarvestLots;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ReadSharedHarvestLotsEndpoint
{
    internal static void Map(IEndpointRouteBuilder supply)
    {
        supply.MapGet("/shared/harvest-lots", async Task<IResult> (Guid grantId, string purpose, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplyNetwork.ReadSharedHarvestLots.ReadSharedHarvestLotsQuery(grantId, purpose), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SupplyNetworkRead);

    }
}
