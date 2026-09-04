using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.NumberSequence.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.NumberSequence.NumberSequenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.NumberSequence.GetGroup;
using NumberSequenceResponse = global::EdgIQ.MurimiOS.API.Endpoints.NumberSequence.Shared.NumberSequenceResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.NumberSequence.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<NumberSequenceResponse>>(200);

    }
}
