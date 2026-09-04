using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.FieldMeasurementEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.PutGroupId;
using FieldMeasurementRequest = global::EdgIQ.MurimiOS.API.Endpoints.FieldMeasurement.Shared.FieldMeasurementRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, FieldMeasurementRequest rquest) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FieldMeasurement.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.FieldMeasurement.PutGroupId.FieldMeasurementRequest { Name = rquest.Name }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
