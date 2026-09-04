using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkItem.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.WorkItem.WorkItemEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkItem.GetGroup;
using WorkItemResponse = global::EdgIQ.MurimiOS.API.Endpoints.WorkItem.Shared.WorkItemResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkItem.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<WorkItemResponse>>(200);

    }
}
