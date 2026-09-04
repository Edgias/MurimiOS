using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Equipment.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Equipment.EquipmentEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Equipment.PutGroupId;
using EquipmentRequest = global::EdgIQ.MurimiOS.API.Endpoints.Equipment.Shared.EquipmentRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, EquipmentRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Equipment.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.Equipment.PutGroupId.EquipmentRequest { Name = request.Name, Manufacturer = request.Manufacturer, Model = request.Model, RegistrationNumber = request.RegistrationNumber, Year = request.Year, Description = request.Description, EquipmentCategoryId = request.EquipmentCategoryId }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
