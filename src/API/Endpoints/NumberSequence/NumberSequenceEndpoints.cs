using EdgIQ.MurimiOS.API.Infrastructure.Filters;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.NumberSequence;
using NumberSequenceRequest = global::EdgIQ.MurimiOS.API.Endpoints.NumberSequence.Shared.NumberSequenceRequest;


internal static class NumberSequenceEndpoints
{
    public static RouteGroupBuilder MapNumberSequences(this IEndpointRouteBuilder builder)
    {
        RouteGroupBuilder group = builder.MapGroup("/number-sequences");
        group.WithTags("Number Sequences");
        group.WithParameterValidation(typeof(NumberSequenceRequest));
        global::EdgIQ.MurimiOS.API.Endpoints.NumberSequence.GetGroup.GetGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.NumberSequence.GetGroupId.GetGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.NumberSequence.PostGroup.PostGroupEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.NumberSequence.PutGroupId.PutGroupIdEndpoint.Map(group);
        global::EdgIQ.MurimiOS.API.Endpoints.NumberSequence.DeleteGroupId.DeleteGroupIdEndpoint.Map(group);

        return group;
    }

}

