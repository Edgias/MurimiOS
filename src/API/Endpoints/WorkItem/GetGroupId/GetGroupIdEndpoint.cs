using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkItem.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.WorkItem.WorkItemEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkItem.GetGroupId;
using WorkItemResponse = global::EdgIQ.MurimiOS.API.Endpoints.WorkItem.Shared.WorkItemResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkItem.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<WorkItemResponse>(200).Produces(404);

    }
}
