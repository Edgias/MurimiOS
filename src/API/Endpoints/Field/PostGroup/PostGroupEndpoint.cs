using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Field.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Field.FieldEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Field.PostGroup;
using FieldRequest = global::EdgIQ.MurimiOS.API.Endpoints.Field.Shared.FieldRequest;
using FieldResponse = global::EdgIQ.MurimiOS.API.Endpoints.Field.Shared.FieldResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, FieldRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Field.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.Field.PostGroup.FieldRequest { Name = request.Name, LocationId = request.LocationId, FieldMeasurementId = request.FieldMeasurementId, UsableArea = request.UsableArea, SoilTypeId = request.SoilTypeId, OwnershipTypeId = request.OwnershipTypeId }), cancellationToken)).ToHttpResult()).Produces<FieldResponse>(201);

    }
}
