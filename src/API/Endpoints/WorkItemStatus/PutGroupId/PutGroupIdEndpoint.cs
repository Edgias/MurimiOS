using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.WorkItemStatusEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.PutGroupId;
using WorkItemStatusRequest = global::EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.Shared.WorkItemStatusRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, WorkItemStatusRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkItemStatus.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.WorkItemStatus.PutGroupId.WorkItemStatusRequest { Name = request.Name, IsDefault = request.IsDefault }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
