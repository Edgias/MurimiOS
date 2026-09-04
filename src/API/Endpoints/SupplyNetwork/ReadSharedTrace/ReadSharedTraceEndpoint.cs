using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.SupplyNetworkEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.ReadSharedTrace;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ReadSharedTraceEndpoint
{
    internal static void Map(IEndpointRouteBuilder supply)
    {
        supply.MapGet("/shared/trace/{grantId:guid}/{harvestLotId:guid}", async Task<IResult> (Guid grantId, Guid harvestLotId, string purpose, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplyNetwork.ReadSharedTrace.ReadSharedTraceQuery(grantId, harvestLotId, purpose), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SupplyNetworkRead);

    }
}
