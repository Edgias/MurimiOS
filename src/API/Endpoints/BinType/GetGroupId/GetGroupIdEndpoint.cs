using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.BinType.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.BinType.BinTypeEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.BinType.GetGroupId;
using BinTypeResponse = global::EdgIQ.MurimiOS.API.Endpoints.BinType.Shared.BinTypeResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.BinType.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<BinTypeResponse>(200).Produces(404);

    }
}
