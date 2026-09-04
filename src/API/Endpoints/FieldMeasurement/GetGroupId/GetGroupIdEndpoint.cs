using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.FieldMeasurementEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.GetGroupId;
using FieldMeasurementResponse = global::EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.Shared.FieldMeasurementResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FieldMeasurement.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<FieldMeasurementResponse>(200).Produces(404);

    }
}
