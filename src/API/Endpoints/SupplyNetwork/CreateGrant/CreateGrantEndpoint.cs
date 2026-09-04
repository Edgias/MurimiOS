using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.CreateGrant;
using static EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.SupplyNetworkEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplyNetwork.CreateGrant;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateGrantEndpoint
{
    internal static void Map(IEndpointRouteBuilder supply)
    {
        supply.MapPost("/grants", async Task<IResult> (CreateDataGrantRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplyNetwork.CreateGrant.CreateGrantCommand(new global::EdgIQ.MurimiOS.Application.SupplyNetwork.CreateGrant.CreateDataGrantRequest(request.Id, request.RelationshipId, request.Name, request.Purpose, request.EntityCategories, request.Attributes, request.FarmIds, request.SeasonIds, request.Commodities, request.ValidFrom, request.ValidTo, request.DownstreamDisclosureAllowed, request.PreviousGrantId)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.DataGrantManage);

    }
}
