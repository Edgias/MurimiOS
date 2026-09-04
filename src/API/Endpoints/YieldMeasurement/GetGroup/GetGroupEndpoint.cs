using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.YieldMeasurementEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.GetGroup;
using YieldMeasurementResponse = global::EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.Shared.YieldMeasurementResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.YieldMeasurement.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<YieldMeasurementResponse>>(200);

    }
}
