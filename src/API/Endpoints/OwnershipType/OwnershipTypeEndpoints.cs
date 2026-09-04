using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.OwnershipType;
using OwnershipTypeRequest = global::EdgIQ.MurimiOS.API.Endpoints.OwnershipType.Shared.OwnershipTypeRequest;


internal static class OwnershipTypeEndpoints
{
    public static RouteGroupBuilder MapOwnershipTypes(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/ownership-types");
        group.WithTags("Ownership Types");
        group.WithParameterValidation(typeof(OwnershipTypeRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.OwnershipType.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.OwnershipType.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.OwnershipType.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.OwnershipType.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.OwnershipType.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

