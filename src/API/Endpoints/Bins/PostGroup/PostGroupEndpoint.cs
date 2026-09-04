using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Bins.Shared;

using static EdgIQ.MurimiOS.API.Endpoints.Bins.BinEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Bins.PostGroup;
using BinResponse = global::EdgIQ.MurimiOS.API.Endpoints.Bins.Shared.BinResponse;
using BinRequest = global::EdgIQ.MurimiOS.API.Endpoints.Bins.Shared.BinRequest;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (BinRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Bins.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.Bins.PostGroup.BinRequest { Name = request.Name, Capacity = request.Capacity, BinTypeId = request.BinTypeId, UnitGroupId = request.UnitGroupId, UnitMeasurementId = request.UnitMeasurementId, WarehouseId = request.WarehouseId }), cancellationToken)).ToHttpResult()).Produces<BinResponse>(201);

    }
}
