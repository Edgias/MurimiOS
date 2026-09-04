using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Equipment.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Equipment.EquipmentEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Equipment.GetGroup;
using EquipmentResponse = global::EdgIQ.MurimiOS.API.Endpoints.Equipment.Shared.EquipmentResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Equipment.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<EquipmentResponse>>(200);

    }
}
