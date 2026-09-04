using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Season;
using SeasonRequest = global::EdgIQ.MurimiOS.API.Endpoints.Season.Shared.SeasonRequest;


internal static class SeasonEndpoints
{
    public static RouteGroupBuilder MapSeasons(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/seasons");
        group.WithTags("Seasons");
        group.WithParameterValidation(typeof(SeasonRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.Season.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Season.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Season.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Season.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Season.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

