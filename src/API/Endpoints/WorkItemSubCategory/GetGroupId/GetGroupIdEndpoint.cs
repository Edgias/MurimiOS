using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkItemSubCategory.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.WorkItemSubCategory.WorkItemSubCategoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemSubCategory.GetGroupId;
using WorkItemSubCategoryResponse = global::EdgIQ.MurimiOS.API.Endpoints.WorkItemSubCategory.Shared.WorkItemSubCategoryResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkItemSubCategory.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<WorkItemSubCategoryResponse>(200).Produces(404);

    }
}
