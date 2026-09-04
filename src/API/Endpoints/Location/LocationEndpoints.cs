using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Location;
using LocationRequest = global::EdgIQ.MurimiOS.API.Endpoints.Location.Shared.LocationRequest;


internal static class LocationEndpoints
{
    public static RouteGroupBuilder MapLocations(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/locations");
        group.WithTags("Locations");
        group.WithParameterValidation(typeof(LocationRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.Location.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Location.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Location.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Location.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Location.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

