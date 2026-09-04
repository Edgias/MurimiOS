using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.WorkItemStatusEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.GetGroupId;
using WorkItemStatusResponse = global::EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.Shared.WorkItemStatusResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkItemStatus.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<WorkItemStatusResponse>(200).Produces(404);

    }
}
