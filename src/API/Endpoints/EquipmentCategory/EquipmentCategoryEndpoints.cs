using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory;
using EquipmentCategoryRequest = global::EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.Shared.EquipmentCategoryRequest;


internal static class EquipmentCategoryEndpoints
{
    public static RouteGroupBuilder MapMachineryCategories(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/equipment-categories");
        group.WithTags("Equipment Categories");
        group.WithParameterValidation(typeof(EquipmentCategoryRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

