using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Season.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Season.SeasonEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Season.PostGroup;
using SeasonRequest = global::EdgIQ.MurimiOS.API.Endpoints.Season.Shared.SeasonRequest;
using SeasonResponse = global::EdgIQ.MurimiOS.API.Endpoints.Season.Shared.SeasonResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, SeasonRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Season.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.Season.PostGroup.SeasonRequest { Name = request.Name, StartDate = request.StartDate, EndDate = request.EndDate, SeasonStatusId = request.SeasonStatusId }), cancellationToken)).ToHttpResult()).Produces<SeasonResponse>(201);

    }
}
