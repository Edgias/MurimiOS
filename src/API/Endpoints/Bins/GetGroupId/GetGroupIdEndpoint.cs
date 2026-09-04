using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Bins.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Bins.BinEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Bins.GetGroupId;
using BinResponse = global::EdgIQ.MurimiOS.API.Endpoints.Bins.Shared.BinResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (Guid id, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Bins.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<BinResponse>(200).Produces(404);

    }
}
