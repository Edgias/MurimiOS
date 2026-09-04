using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Bins;
using BinRequest = global::EdgIQ.MurimiOS.API.Endpoints.Bins.Shared.BinRequest;


internal static class BinEndpoints
{
    public static RouteGroupBuilder MapBins(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/bins");
        group.WithTags("Bins");
        group.WithParameterValidation(typeof(BinRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.Bins.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Bins.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Bins.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Bins.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.Bins.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

