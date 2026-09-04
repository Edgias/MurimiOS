using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.WorkItemCategoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.PostGroup;
using WorkItemCategoryRequest = global::EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.Shared.WorkItemCategoryRequest;
using WorkItemCategoryResponse = global::EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.Shared.WorkItemCategoryResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, WorkItemCategoryRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkItemCategory.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.WorkItemCategory.PostGroup.WorkItemCategoryRequest { Name = request.Name }), cancellationToken)).ToHttpResult()).Produces<WorkItemCategoryResponse>(201);

    }
}
