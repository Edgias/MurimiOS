using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.EquipmentCategoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.PostGroup;
using EquipmentCategoryRequest = global::EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.Shared.EquipmentCategoryRequest;
using EquipmentCategoryResponse = global::EdgIQ.MurimiOS.API.Endpoints.EquipmentCategory.Shared.EquipmentCategoryResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, EquipmentCategoryRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.EquipmentCategory.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.EquipmentCategory.PostGroup.EquipmentCategoryRequest { Name = request.Name }), cancellationToken)).ToHttpResult()).Produces<EquipmentCategoryResponse>(201);

    }
}
