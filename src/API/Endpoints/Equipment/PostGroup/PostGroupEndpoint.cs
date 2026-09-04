using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Equipment.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Equipment.EquipmentEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Equipment.PostGroup;
using EquipmentRequest = global::EdgIQ.MurimiOS.API.Endpoints.Equipment.Shared.EquipmentRequest;
using EquipmentResponse = global::EdgIQ.MurimiOS.API.Endpoints.Equipment.Shared.EquipmentResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, EquipmentRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Equipment.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.Equipment.PostGroup.EquipmentRequest { Name = request.Name, Manufacturer = request.Manufacturer, Model = request.Model, RegistrationNumber = request.RegistrationNumber, Year = request.Year, Description = request.Description, EquipmentCategoryId = request.EquipmentCategoryId }), cancellationToken)).ToHttpResult()).Produces<EquipmentResponse>(201);

    }
}
