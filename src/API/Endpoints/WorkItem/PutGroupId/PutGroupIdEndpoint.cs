using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkItem.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.WorkItem.WorkItemEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkItem.PutGroupId;
using WorkItemRequest = global::EdgIQ.MurimiOS.API.Endpoints.WorkItem.Shared.WorkItemRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, WorkItemRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkItem.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.WorkItem.PutGroupId.WorkItemRequest { Name = request.Name, Description = request.Description, StartDate = request.StartDate, EndDate = request.EndDate, WorkItemStatusId = request.WorkItemStatusId, WorkItemCategoryId = request.WorkItemCategoryId, WorkItemSubCategoryId = request.WorkItemSubCategoryId, FieldId = request.FieldId, SeasonId = request.SeasonId, CropProductionId = request.CropProductionId }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
