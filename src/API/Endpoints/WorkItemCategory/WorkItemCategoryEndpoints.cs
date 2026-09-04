using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory;
using WorkItemCategoryRequest = global::EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.Shared.WorkItemCategoryRequest;


internal static class WorkItemCategoryEndpoints
{
    public static RouteGroupBuilder MapWorkItemCategories(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/work-item-categories");
        group.WithTags("Work Item Categories");
        group.WithParameterValidation(typeof(WorkItemCategoryRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItemCategory.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

