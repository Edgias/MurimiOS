using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.NumberSequence.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.NumberSequence.NumberSequenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.NumberSequence.GetGroupId;
using NumberSequenceResponse = global::EdgIQ.MurimiOS.API.Endpoints.NumberSequence.Shared.NumberSequenceResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.NumberSequence.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<NumberSequenceResponse>(200).Produces(404);

    }
}
