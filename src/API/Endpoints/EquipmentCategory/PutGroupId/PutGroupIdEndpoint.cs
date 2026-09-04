using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.EquipmentCategoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.PutGroupId;
using EquipmentCategoryRequest = global::EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.Shared.EquipmentCategoryRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, EquipmentCategoryRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.EquipmentCategory.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.EquipmentCategory.PutGroupId.EquipmentCategoryRequest { Name = request.Name }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
