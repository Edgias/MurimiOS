using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.SeasonStatusEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.PostGroup;
using SeasonStatusRequest = global::EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.Shared.SeasonStatusRequest;
using SeasonStatusResponse = global::EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.Shared.SeasonStatusResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, SeasonStatusRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SeasonStatus.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.SeasonStatus.PostGroup.SeasonStatusRequest { Name = request.Name, IsDefault = request.IsDefault }), cancellationToken)).ToHttpResult()).Produces<SeasonStatusResponse>(201);

    }
}
