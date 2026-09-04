using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Bins.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Bins.BinEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Bins.PutGroupId;
using BinRequest = global::EdgIQ.MurimiOS.API.Endpoints.Bins.Shared.BinRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (Guid id, BinRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Bins.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.Bins.PutGroupId.BinRequest { Name = request.Name, Capacity = request.Capacity, BinTypeId = request.BinTypeId, UnitGroupId = request.UnitGroupId, UnitMeasurementId = request.UnitMeasurementId, WarehouseId = request.WarehouseId }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
