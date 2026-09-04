using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkItem;
using WorkItemRequest = global::EdgIQ.MurimiOS.API.Endpoints.WorkItem.Shared.WorkItemRequest;


internal static class WorkItemEndpoints
{
    public static RouteGroupBuilder MapWorkItems(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/work-items");
        group.WithTags("Work Items");
        group.WithParameterValidation(typeof(WorkItemRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItem.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItem.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItem.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItem.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItem.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

