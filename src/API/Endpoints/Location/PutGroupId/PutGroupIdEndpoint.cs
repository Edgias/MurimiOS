using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Location.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Location.LocationEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Location.PutGroupId;
using LocationRequest = global::EdgIQ.MurimiOS.API.Endpoints.Location.Shared.LocationRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, LocationRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Location.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.Location.PutGroupId.LocationRequest { Name = request.Name, Latitude = request.Latitude, Longitude = request.Longitude, LocationAddress = request.LocationAddress }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
