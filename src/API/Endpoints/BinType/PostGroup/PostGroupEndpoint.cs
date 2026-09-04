using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.BinType.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.BinType.BinTypeEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.BinType.PostGroup;
using BinTypeRequest = global::EdgIQ.MurimiOS.API.Endpoints.BinType.Shared.BinTypeRequest;
using BinTypeResponse = global::EdgIQ.MurimiOS.API.Endpoints.BinType.Shared.BinTypeResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, BinTypeRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.BinType.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.BinType.PostGroup.BinTypeRequest { Name = request.Name }), cancellationToken)).ToHttpResult()).Produces<BinTypeResponse>(201);

    }
}
