using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement;
using FieldMeasurementRequest = global::EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.Shared.FieldMeasurementRequest;


internal static class FieldMeasurementEndpoints
{
    public static RouteGroupBuilder MapFieldMeasurements(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/field-measurements");
        group.WithTags("Field Measurements");
        group.WithParameterValidation(typeof(FieldMeasurementRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

