using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ItemCategory.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.ItemCategory.ItemCategoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ItemCategory.PostGroup;
using ItemCategoryRequest = global::EdgIQ.MurimiOS.API.Endpoints.ItemCategory.Shared.ItemCategoryRequest;
using ItemCategoryResponse = global::EdgIQ.MurimiOS.API.Endpoints.ItemCategory.Shared.ItemCategoryResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, ItemCategoryRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ItemCategory.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.ItemCategory.PostGroup.ItemCategoryRequest { Name = request.Name }), cancellationToken)).ToHttpResult()).Produces<ItemCategoryResponse>(201);

    }
}
