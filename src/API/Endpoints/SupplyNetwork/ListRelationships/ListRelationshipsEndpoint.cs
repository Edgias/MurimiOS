using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.SupplyNetworkEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.ListRelationships;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListRelationshipsEndpoint
{
    internal static void Map(IEndpointRouteBuilder supply)
    {
        supply.MapGet("/relationships", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplyNetwork.ListRelationships.ListRelationshipsQuery(), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SupplyNetworkRead);

    }
}
