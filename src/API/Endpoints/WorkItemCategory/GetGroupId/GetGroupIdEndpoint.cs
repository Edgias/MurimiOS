using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.WorkItemCategoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.GetGroupId;
using WorkItemCategoryResponse = global::EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.Shared.WorkItemCategoryResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkItemCategory.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<WorkItemCategoryResponse>(200).Produces(404);

    }
}
