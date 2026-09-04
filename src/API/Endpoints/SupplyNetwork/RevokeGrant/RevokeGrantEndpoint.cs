using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.SupplyNetworkEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.RevokeGrant;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using RevokeSupplyAccessRequest = global::EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.Shared.RevokeSupplyAccessRequest;
internal static class RevokeGrantEndpoint
{
    internal static void Map(IEndpointRouteBuilder supply)
    {
        supply.MapPost("/grants/{grantId:guid}/revoke", async Task<IResult> (Guid grantId, RevokeSupplyAccessRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplyNetwork.RevokeGrant.RevokeGrantCommand(grantId, new global::EdgIQ.MurimiOS.Application.SupplyNetwork.RevokeGrant.RevokeSupplyAccessRequest(request.Reason)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.DataGrantManage);

    }
}
