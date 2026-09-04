using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Asset.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Asset.AssetEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Asset.PostGroup;
using AssetRequest = global::EdgIQ.MurimiOS.API.Endpoints.Asset.Shared.AssetRequest;
using AssetResponse = global::EdgIQ.MurimiOS.API.Endpoints.Asset.Shared.AssetResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, AssetRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Asset.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.Asset.PostGroup.AssetRequest { Name = request.Name, SerialNumber = request.SerialNumber, Description = request.Description, PurchaseDate = request.PurchaseDate, PurchasePrice = request.PurchasePrice, SupplierId = request.SupplierId, PurchaseInvoiceId = request.PurchaseInvoiceId }), cancellationToken)).ToHttpResult()).Produces<AssetResponse>(201);

    }
}
