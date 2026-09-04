using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.YieldMeasurementEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.PutGroupId;
using YieldMeasurementRequest = global::EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.Shared.YieldMeasurementRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, YieldMeasurementRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.YieldMeasurement.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.YieldMeasurement.PutGroupId.YieldMeasurementRequest { Name = request.Name }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
