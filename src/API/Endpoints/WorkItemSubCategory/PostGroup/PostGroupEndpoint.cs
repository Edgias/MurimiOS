using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkItemSubCategory.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.WorkItemSubCategory.WorkItemSubCategoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemSubCategory.PostGroup;
using WorkItemSubCategoryRequest = global::EdgIQ.MurimiOS.API.Endpoints.WorkItemSubCategory.Shared.WorkItemSubCategoryRequest;
using WorkItemSubCategoryResponse = global::EdgIQ.MurimiOS.API.Endpoints.WorkItemSubCategory.Shared.WorkItemSubCategoryResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, WorkItemSubCategoryRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkItemSubCategory.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.WorkItemSubCategory.PostGroup.WorkItemSubCategoryRequest { Name = request.Name, WorkItemCategoryId = request.WorkItemCategoryId }), cancellationToken)).ToHttpResult()).Produces<WorkItemSubCategoryResponse>(201);

    }
}
