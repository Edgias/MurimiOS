using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Season.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Season.SeasonEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Season.GetGroup;
using SeasonResponse = global::EdgIQ.MurimiOS.API.Endpoints.Season.Shared.SeasonResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Season.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<SeasonResponse>>(200);

    }
}
