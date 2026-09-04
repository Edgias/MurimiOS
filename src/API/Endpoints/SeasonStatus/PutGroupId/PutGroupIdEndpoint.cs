using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.SeasonStatusEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.PutGroupId;
using SeasonStatusRequest = global::EdgIQ.MurimiOS.API.Endpoints.SeasonStatus.Shared.SeasonStatusRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, SeasonStatusRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SeasonStatus.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.SeasonStatus.PutGroupId.SeasonStatusRequest { Name = request.Name, IsDefault = request.IsDefault }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
