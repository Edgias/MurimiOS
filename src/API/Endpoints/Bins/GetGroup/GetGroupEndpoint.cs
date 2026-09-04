using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Bins.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Bins.BinEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Bins.GetGroup;
using BinResponse = global::EdgIQ.MurimiOS.API.Endpoints.Bins.Shared.BinResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Bins.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<BinResponse>>(200);

    }
}
