using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Location.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Location.LocationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Location.GetGroupId;
using LocationResponse = global::EdgIQ.MurimiOS.API.Endpoints.Location.Shared.LocationResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Location.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<LocationResponse>(200).Produces(404);

    }
}
