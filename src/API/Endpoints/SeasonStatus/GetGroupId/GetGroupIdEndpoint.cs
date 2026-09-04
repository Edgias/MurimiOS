using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.SeasonStatusEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.GetGroupId;
using SeasonStatusResponse = global::EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.Shared.SeasonStatusResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SeasonStatus.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<SeasonStatusResponse>(200).Produces(404);

    }
}
