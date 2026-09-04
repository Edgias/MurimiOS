using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.SeasonStatusEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.GetGroup;
using SeasonStatusResponse = global::EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.Shared.SeasonStatusResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SeasonStatus.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<SeasonStatusResponse>>(200);

    }
}
