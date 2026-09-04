using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.NumberSequence.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.NumberSequence.NumberSequenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.NumberSequence.PostGroup;
using NumberSequenceRequest = global::EdgIQ.MurimiOS.API.Endpoints.NumberSequence.Shared.NumberSequenceRequest;
using NumberSequenceResponse = global::EdgIQ.MurimiOS.API.Endpoints.NumberSequence.Shared.NumberSequenceResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, NumberSequenceRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.NumberSequence.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.NumberSequence.PostGroup.NumberSequenceRequest { Entity = request.Entity, Prefix = request.Prefix, Seperator = request.Seperator, StartingNumber = request.StartingNumber, Suffix = request.Suffix }), cancellationToken)).ToHttpResult()).Produces<NumberSequenceResponse>(201);

    }
}
