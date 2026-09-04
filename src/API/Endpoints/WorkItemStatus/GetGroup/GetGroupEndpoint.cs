using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.WorkItemStatusEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.GetGroup;
using WorkItemStatusResponse = global::EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.Shared.WorkItemStatusResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkItemStatus.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<WorkItemStatusResponse>>(200);

    }
}
