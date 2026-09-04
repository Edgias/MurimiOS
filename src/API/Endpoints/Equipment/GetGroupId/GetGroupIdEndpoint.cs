using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Equipment.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Equipment.EquipmentEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Equipment.GetGroupId;
using EquipmentResponse = global::EdgIQ.MurimiOS.API.Endpoints.Equipment.Shared.EquipmentResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Equipment.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<EquipmentResponse>(200).Produces(404);

    }
}
