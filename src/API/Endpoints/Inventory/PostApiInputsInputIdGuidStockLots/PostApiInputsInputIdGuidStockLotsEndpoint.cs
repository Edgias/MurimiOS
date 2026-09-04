using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Inventory.PostApiInputsInputIdGuidStockLots;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class PostApiInputsInputIdGuidStockLotsEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {

        api.MapPost("/inputs/{inputId:guid}/stock-lots", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid inputId, CreateInputStockLotRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Inventory.PostApiInputsInputIdGuidStockLots.PostApiInputsInputIdGuidStockLotsCommand(inputId, new global::EdgIQ.MurimiOS.Application.Inventory.PostApiInputsInputIdGuidStockLots.CreateInputStockLotRequest(request.LotCode, request.ReceivedOn, request.ExpiresOn, request.InitialQuantity)), cancellationToken)).ToHttpResult()).WithTags("Inventory").RequireAuthorization(PlatformPermissions.InventoryWrite);

    }
}
