using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SoilType;
using SoilTypeRequest = global::EdgIQ.MurimiOS.API.Endpoints.SoilType.Shared.SoilTypeRequest;


internal static class SoilTypeEndpoints
{
    public static RouteGroupBuilder MapSoilTypes(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/soil-types");
        group.WithTags("Soil Types");
        group.WithParameterValidation(typeof(SoilTypeRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.SoilType.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SoilType.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SoilType.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SoilType.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SoilType.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

