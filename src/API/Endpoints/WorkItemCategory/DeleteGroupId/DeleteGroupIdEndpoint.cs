using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.WorkItemCategoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.DeleteGroupId;
internal static class DeleteGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapDelete("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkItemCategory.DeleteGroupId.DeleteGroupIdCommand(id), cancellationToken)).ToHttpResult()).Produces(404).Produces(200);

    }
}
