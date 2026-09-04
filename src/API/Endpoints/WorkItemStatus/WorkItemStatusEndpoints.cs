using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus;
using WorkItemStatusRequest = global::EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.Shared.WorkItemStatusRequest;


internal static class WorkItemStatusEndpoints
{
    public static RouteGroupBuilder MapWorkItemStatuses(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/work-item-statuses");
        group.WithTags("Work Item Statuses");
        group.WithParameterValidation(typeof(WorkItemStatusRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItemStatus.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

