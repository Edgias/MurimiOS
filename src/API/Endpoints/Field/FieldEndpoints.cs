using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Field;
using FieldRequest = global::EdgIQ.MurimiOS.API.Endpoints.Field.Shared.FieldRequest;


internal static class FieldEndpoints
{
    public static RouteGroupBuilder MapFields(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/fields");
        group.WithTags("Fields");
        group.WithParameterValidation(typeof(FieldRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.Field.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Field.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Field.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Field.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Field.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

