using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SeasonStatus;
using SeasonStatusRequest = global::EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.Shared.SeasonStatusRequest;


internal static class SeasonStatusEndpoints
{
    public static RouteGroupBuilder MapSeasonStatuses(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/season-statuses");
        group.WithTags("Season Statuses");
        group.WithParameterValidation(typeof(SeasonStatusRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

