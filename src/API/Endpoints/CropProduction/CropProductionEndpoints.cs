using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropProduction;
using CropProductionRequest = global::EdgIQ.MurimiOS.API.Endpoints.CropProduction.Shared.CropProductionRequest;


internal static class CropProductionEndpoints
{
    public static RouteGroupBuilder MapCropProductions(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/crop-productions");
        group.WithTags("Crop Productions");
        group.WithParameterValidation(typeof(CropProductionRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.CropProduction.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.CropProduction.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.CropProduction.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.CropProduction.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.CropProduction.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

