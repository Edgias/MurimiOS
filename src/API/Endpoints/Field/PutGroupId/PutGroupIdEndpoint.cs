using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Field.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Field.FieldEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Field.PutGroupId;
using FieldRequest = global::EdgIQ.MurimiOS.API.Endpoints.Field.Shared.FieldRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, FieldRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Field.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.Field.PutGroupId.FieldRequest { Name = request.Name, LocationId = request.LocationId, FieldMeasurementId = request.FieldMeasurementId, UsableArea = request.UsableArea, SoilTypeId = request.SoilTypeId, OwnershipTypeId = request.OwnershipTypeId }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
