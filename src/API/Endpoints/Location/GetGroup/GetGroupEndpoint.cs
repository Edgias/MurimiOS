using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Location.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Location.LocationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Location.GetGroup;
using LocationResponse = global::EdgIQ.MurimiOS.API.Endpoints.Location.Shared.LocationResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Location.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<LocationResponse>>(200);

    }
}
