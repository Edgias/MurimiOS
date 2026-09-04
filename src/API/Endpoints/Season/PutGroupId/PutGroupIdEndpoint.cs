using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Season.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Season.SeasonEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Season.PutGroupId;
using SeasonRequest = global::EdgIQ.MurimiOS.API.Endpoints.Season.Shared.SeasonRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, SeasonRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Season.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.Season.PutGroupId.SeasonRequest { Name = request.Name, StartDate = request.StartDate, EndDate = request.EndDate, SeasonStatusId = request.SeasonStatusId }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
