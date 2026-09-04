using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropCategory.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.CropCategory.CropCategoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropCategory.PutGroupId;
using CropCategoryRequest = global::EdgIQ.MurimiOS.API.Endpoints.CropCategory.Shared.CropCategoryRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, CropCategoryRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropCategory.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.CropCategory.PutGroupId.CropCategoryRequest { Name = request.Name, Description = request.Description }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
