using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.WorkItemCategoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.GetGroup;
using WorkItemCategoryResponse = global::EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.Shared.WorkItemCategoryResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkItemCategory.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<WorkItemCategoryResponse>>(200);

    }
}
