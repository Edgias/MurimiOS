using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.BinType.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.BinType.BinTypeEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.BinType.GetGroup;
using BinTypeResponse = global::EdgIQ.MurimiOS.API.Endpoints.BinType.Shared.BinTypeResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.BinType.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<BinTypeResponse>>(200);

    }
}
