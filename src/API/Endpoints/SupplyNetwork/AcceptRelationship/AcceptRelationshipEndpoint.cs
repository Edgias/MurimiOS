using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.SupplyNetworkEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.AcceptRelationship;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class AcceptRelationshipEndpoint
{
    internal static void Map(IEndpointRouteBuilder supply)
    {
        supply.MapPost("/relationships/{relationshipId:guid}/accept", async Task<IResult> (Guid relationshipId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplyNetwork.AcceptRelationship.AcceptRelationshipCommand(relationshipId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SupplyNetworkManage);

    }
}
