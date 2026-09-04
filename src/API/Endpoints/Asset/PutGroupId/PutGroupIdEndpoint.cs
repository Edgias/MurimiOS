using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Asset.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Asset.AssetEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Asset.PutGroupId;
using AssetRequest = global::EdgIQ.MurimiOS.API.Endpoints.Asset.Shared.AssetRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, AssetRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Asset.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.Asset.PutGroupId.AssetRequest { Name = request.Name, SerialNumber = request.SerialNumber, Description = request.Description, PurchaseDate = request.PurchaseDate, PurchasePrice = request.PurchasePrice, SupplierId = request.SupplierId, PurchaseInvoiceId = request.PurchaseInvoiceId }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
