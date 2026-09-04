using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Inventory.GetApiFarmsFarmIdGuidInputs;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetApiFarmsFarmIdGuidInputsEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapGet("/farms/{farmId:guid}/inputs", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid farmId) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Inventory.GetApiFarmsFarmIdGuidInputs.GetApiFarmsFarmIdGuidInputsQuery(farmId), cancellationToken)).ToHttpResult()).WithTags("Inventory").RequireAuthorization(PlatformPermissions.InventoryRead);

    }
}
