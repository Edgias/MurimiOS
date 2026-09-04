using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.WorkItem.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.WorkItem.WorkItemEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.WorkItem.PostGroup;
using WorkItemRequest = global::EdgIQ.MurimiOS.API.Endpoints.WorkItem.Shared.WorkItemRequest;
using WorkItemResponse = global::EdgIQ.MurimiOS.API.Endpoints.WorkItem.Shared.WorkItemResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, WorkItemRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.WorkItem.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.WorkItem.PostGroup.WorkItemRequest { Name = request.Name, Description = request.Description, StartDate = request.StartDate, EndDate = request.EndDate, WorkItemStatusId = request.WorkItemStatusId, WorkItemCategoryId = request.WorkItemCategoryId, WorkItemSubCategoryId = request.WorkItemSubCategoryId, FieldId = request.FieldId, SeasonId = request.SeasonId, CropProductionId = request.CropProductionId }), cancellationToken)).ToHttpResult()).Produces<WorkItemResponse>(201);

    }
}
