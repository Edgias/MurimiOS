using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.BinType.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.BinType.BinTypeEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.BinType.PutGroupId;
using BinTypeRequest = global::EdgIQ.MurimiOS.API.Endpoints.BinType.Shared.BinTypeRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, BinTypeRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.BinType.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.BinType.PutGroupId.BinTypeRequest { Name = request.Name }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
