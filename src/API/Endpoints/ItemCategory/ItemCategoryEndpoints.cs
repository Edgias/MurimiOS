using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ItemCategory;
using ItemCategoryRequest = global::EdgIQ.MurimiOS.API.Endpoints.ItemCategory.Shared.ItemCategoryRequest;


internal static class ItemCategoryEndpoints
{
    public static RouteGroupBuilder MapItemCategories(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/item-categories");
        group.WithTags("Item Categories");
        group.WithParameterValidation(typeof(ItemCategoryRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.ItemCategory.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.ItemCategory.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.ItemCategory.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.ItemCategory.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.ItemCategory.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

