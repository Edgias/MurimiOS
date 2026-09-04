using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.WorkItemCategoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.PutGroupId;
using WorkItemCategoryRequest = global::EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.Shared.WorkItemCategoryRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, WorkItemCategoryRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkItemCategory.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.WorkItemCategory.PutGroupId.WorkItemCategoryRequest { Name = request.Name }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
