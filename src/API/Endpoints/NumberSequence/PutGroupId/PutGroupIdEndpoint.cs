using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.NumberSequence.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.NumberSequence.NumberSequenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.NumberSequence.PutGroupId;
using NumberSequenceRequest = global::EdgIQ.MurimiOS.API.Endpoints.NumberSequence.Shared.NumberSequenceRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, NumberSequenceRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.NumberSequence.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.NumberSequence.PutGroupId.NumberSequenceRequest { Entity = request.Entity, Prefix = request.Prefix, Seperator = request.Seperator, StartingNumber = request.StartingNumber, Suffix = request.Suffix }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
