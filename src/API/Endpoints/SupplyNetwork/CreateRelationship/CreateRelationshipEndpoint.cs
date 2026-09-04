using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.CreateRelationship;
using static EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.SupplyNetworkEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.CreateRelationship;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateRelationshipEndpoint
{
    internal static void Map(IEndpointRouteBuilder supply)
    {
        supply.MapPost("/relationships", async Task<IResult> (CreateSupplyRelationshipRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplyNetwork.CreateRelationship.CreateRelationshipCommand(new global::EdgIQ.MurimiOS.Application.SupplyNetwork.CreateRelationship.CreateSupplyRelationshipRequest(request.Id, request.CounterpartyTenantId, request.OurRole, request.Name)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SupplyNetworkManage);

    }
}
