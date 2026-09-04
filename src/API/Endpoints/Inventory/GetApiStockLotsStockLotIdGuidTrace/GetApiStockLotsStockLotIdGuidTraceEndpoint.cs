using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Inventory.GetApiStockLotsStockLotIdGuidTrace;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetApiStockLotsStockLotIdGuidTraceEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {

        api.MapGet("/stock-lots/{stockLotId:guid}/trace", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid stockLotId) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Inventory.GetApiStockLotsStockLotIdGuidTrace.GetApiStockLotsStockLotIdGuidTraceQuery(stockLotId), cancellationToken)).ToHttpResult()).WithTags("Traceability").RequireAuthorization(PlatformPermissions.InventoryRead);

    }
}
