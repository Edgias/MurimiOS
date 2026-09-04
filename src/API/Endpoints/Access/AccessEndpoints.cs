using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Access;
using GetAccessMeEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Access.GetAccessMe.GetAccessMeEndpoint;
using UpdateEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Access.Update.UpdateEndpoint;


public static class AccessEndpoints
{
    public static void MapAccessEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder access = api.MapGroup("/access").WithTags("Access");
        global::EdgIQ.MurimiOS.API.Endpoints.Access.GetAccessMe.GetAccessMeEndpoint.Map(access);
        global::EdgIQ.MurimiOS.API.Endpoints.Access.List.ListEndpoint.Map(access);
        global::EdgIQ.MurimiOS.API.Endpoints.Access.Create.CreateEndpoint.Map(access);
        global::EdgIQ.MurimiOS.API.Endpoints.Access.Update.UpdateEndpoint.Map(access);
    }
}
