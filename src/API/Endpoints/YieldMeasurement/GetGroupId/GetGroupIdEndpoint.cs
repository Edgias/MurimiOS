using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.YieldMeasurementEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.GetGroupId;
using YieldMeasurementResponse = global::EdgIQ.MurimiOS.API.Endpoints.YieldMeasurement.Shared.YieldMeasurementResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.YieldMeasurement.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<YieldMeasurementResponse>(200).Produces(404);

    }
}
