using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ItemCategory.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.ItemCategory.ItemCategoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ItemCategory.GetGroupId;
using ItemCategoryResponse = global::EdgIQ.MurimiOS.API.Endpoints.ItemCategory.Shared.ItemCategoryResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ItemCategory.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<ItemCategoryResponse>(200).Produces(404);

    }
}
