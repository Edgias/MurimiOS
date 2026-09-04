using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.EquipmentCategoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.GetGroup;
using EquipmentCategoryResponse = global::EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.Shared.EquipmentCategoryResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.EquipmentCategory.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<EquipmentCategoryResponse>>(200);

    }
}
