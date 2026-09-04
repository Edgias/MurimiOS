using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.ItemCategory.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.ItemCategory.ItemCategoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.ItemCategory.PutGroupId;
using ItemCategoryRequest = global::EdgIQ.MurimiOS.API.Endpoints.ItemCategory.Shared.ItemCategoryRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, ItemCategoryRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.ItemCategory.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.ItemCategory.PutGroupId.ItemCategoryRequest { Name = request.Name }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
