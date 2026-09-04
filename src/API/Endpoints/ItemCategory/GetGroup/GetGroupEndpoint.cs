using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ItemCategory.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.ItemCategory.ItemCategoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ItemCategory.GetGroup;
using ItemCategoryResponse = global::EdgIQ.MurimiOS.API.Endpoints.ItemCategory.Shared.ItemCategoryResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ItemCategory.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<ItemCategoryResponse>>(200);

    }
}
