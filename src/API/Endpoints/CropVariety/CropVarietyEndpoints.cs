using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropVariety;
using CropVarietyRequest = global::EdgIQ.MurimiOS.API.Endpoints.CropVariety.Shared.CropVarietyRequest;


internal static class CropVarietyEndpoints
{
    public static RouteGroupBuilder MapCropVarieties(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/crop-varieties");
        group.WithTags("Crop Varieties");
        group.WithParameterValidation(typeof(CropVarietyRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.CropVariety.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.CropVariety.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.CropVariety.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.CropVariety.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.CropVariety.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

