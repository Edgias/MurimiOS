using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.YieldMeasurementEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.PostGroup;
using YieldMeasurementRequest = global::EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.Shared.YieldMeasurementRequest;
using YieldMeasurementResponse = global::EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.Shared.YieldMeasurementResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, YieldMeasurementRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.YieldMeasurement.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.YieldMeasurement.PostGroup.YieldMeasurementRequest { Name = request.Name }), cancellationToken)).ToHttpResult()).Produces<YieldMeasurementResponse>(201);

    }
}
