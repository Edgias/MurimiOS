using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.WorkItemStatusEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.DeleteGroupId;
internal static class DeleteGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapDelete("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkItemStatus.DeleteGroupId.DeleteGroupIdCommand(id), cancellationToken)).ToHttpResult()).Produces(404).Produces(200);

    }
}
