using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropCategory;
using CropCategoryRequest = global::EdgIQ.MurimiOS.API.Endpoints.CropCategory.Shared.CropCategoryRequest;


internal static class CropCategoryEndpoints
{
    public static RouteGroupBuilder MapCropCategories(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/crop-categories");
        group.WithTags("Crop Categories");
        group.WithParameterValidation(typeof(CropCategoryRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.CropCategory.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.CropCategory.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.CropCategory.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.CropCategory.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.CropCategory.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

