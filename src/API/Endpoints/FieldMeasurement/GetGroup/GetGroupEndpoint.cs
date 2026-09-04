using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.FieldMeasurementEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.GetGroup;
using FieldMeasurementResponse = global::EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.Shared.FieldMeasurementResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FieldMeasurement.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<FieldMeasurementResponse>>(200);

    }
}
