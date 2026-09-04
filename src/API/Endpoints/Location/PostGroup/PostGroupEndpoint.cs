using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Location.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Location.LocationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Location.PostGroup;
using LocationRequest = global::EdgIQ.MurimiOS.API.Endpoints.Location.Shared.LocationRequest;
using LocationResponse = global::EdgIQ.MurimiOS.API.Endpoints.Location.Shared.LocationResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, LocationRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Location.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.Location.PostGroup.LocationRequest { Name = request.Name, Latitude = request.Latitude, Longitude = request.Longitude, LocationAddress = request.LocationAddress }), cancellationToken)).ToHttpResult()).Produces<LocationResponse>(201);

    }
}
