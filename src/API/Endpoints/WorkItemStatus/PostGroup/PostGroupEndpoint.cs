using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.WorkItemStatusEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.PostGroup;
using WorkItemStatusRequest = global::EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.Shared.WorkItemStatusRequest;
using WorkItemStatusResponse = global::EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.Shared.WorkItemStatusResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, WorkItemStatusRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkItemStatus.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.WorkItemStatus.PostGroup.WorkItemStatusRequest { Name = request.Name, IsDefault = request.IsDefault }), cancellationToken)).ToHttpResult()).Produces<WorkItemStatusResponse>(201);

    }
}
