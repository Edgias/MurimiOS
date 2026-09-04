using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.FieldMeasurementEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.PostGroup;
using FieldMeasurementRequest = global::EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.Shared.FieldMeasurementRequest;
using FieldMeasurementResponse = global::EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.Shared.FieldMeasurementResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, FieldMeasurementRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FieldMeasurement.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.FieldMeasurement.PostGroup.FieldMeasurementRequest { Name = request.Name }), cancellationToken)).ToHttpResult()).Produces<FieldMeasurementResponse>(201);

    }
}
