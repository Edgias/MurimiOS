using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.BinType;
using BinTypeRequest = global::EdgIQ.MurimiOS.API.Endpoints.BinType.Shared.BinTypeRequest;


internal static class BinTypeEndpoints
{
    public static RouteGroupBuilder MapBinTypes(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/bin-types");
        group.WithTags("Bin Types");
        group.WithParameterValidation(typeof(BinTypeRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.BinType.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.BinType.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.BinType.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.BinType.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.BinType.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

