using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Season.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Season.SeasonEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Season.GetGroupId;
using SeasonResponse = global::EdgIQ.MurimiOS.API.Endpoints.Season.Shared.SeasonResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Season.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<SeasonResponse>(200).Produces(404);

    }
}
