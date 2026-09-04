using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Crop;
using CropRequest = global::EdgIQ.MurimiOS.API.Endpoints.Crop.Shared.CropRequest;


internal static class CropEndpoints
{
    public static RouteGroupBuilder MapCrops(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/crops");
        group.WithTags("Crops");
        group.WithParameterValidation(typeof(CropRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.Crop.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Crop.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Crop.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Crop.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Crop.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

