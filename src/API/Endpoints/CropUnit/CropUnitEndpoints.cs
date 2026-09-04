using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropUnit;
using CropUnitRequest = global::EdgIQ.MurimiOS.API.Endpoints.CropUnit.Shared.CropUnitRequest;


internal static class CropUnitEndpoints
{
    public static RouteGroupBuilder MapCropUnits(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/crop-units");
        group.WithTags("Crop Units");
        group.WithParameterValidation(typeof(CropUnitRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.CropUnit.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.CropUnit.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.CropUnit.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.CropUnit.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.CropUnit.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

