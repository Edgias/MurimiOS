using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Inventory.GetApiInputsInputIdGuidStockLots;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetApiInputsInputIdGuidStockLotsEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {

        api.MapGet("/inputs/{inputId:guid}/stock-lots", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid inputId) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Inventory.GetApiInputsInputIdGuidStockLots.GetApiInputsInputIdGuidStockLotsQuery(inputId), cancellationToken)).ToHttpResult()).WithTags("Inventory").RequireAuthorization(PlatformPermissions.InventoryRead);

    }
}
