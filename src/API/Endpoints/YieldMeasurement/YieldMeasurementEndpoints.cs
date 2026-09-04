using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement;
using YieldMeasurementRequest = global::EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.Shared.YieldMeasurementRequest;


internal static class YieldMeasurementEndpoints
{
    public static RouteGroupBuilder MapYieldMeasurements(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/yield-measurements");
        group.WithTags("Yield Measurements");
        group.WithParameterValidation(typeof(YieldMeasurementRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

