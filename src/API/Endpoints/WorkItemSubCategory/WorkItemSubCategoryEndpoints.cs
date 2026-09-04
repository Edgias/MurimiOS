using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.WorkItemSubCategory;
using WorkItemSubCategoryRequest = global::EdgIQ.MurimiOS.API.Endpoints.WorkItemSubCategory.Shared.WorkItemSubCategoryRequest;


internal static class WorkItemSubCategoryEndpoints
{
    public static RouteGroupBuilder MapWorkItemSubCategories(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/work-item-sub-categories");
        group.WithTags("Work Item Sub Categories");
        group.WithParameterValidation(typeof(WorkItemSubCategoryRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItemSubCategory.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItemSubCategory.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItemSubCategory.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItemSubCategory.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.WorkItemSubCategory.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

