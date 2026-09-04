using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.EquipmentCategoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.GetGroupId;
using EquipmentCategoryResponse = global::EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.Shared.EquipmentCategoryResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.EquipmentCategory.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<EquipmentCategoryResponse>(200).Produces(404);

    }
}
