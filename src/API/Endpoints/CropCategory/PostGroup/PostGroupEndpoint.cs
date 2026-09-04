using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropCategory.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.CropCategory.CropCategoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropCategory.PostGroup;
using CropCategoryRequest = global::EdgIQ.MurimiOS.API.Endpoints.CropCategory.Shared.CropCategoryRequest;
using CropCategoryResponse = global::EdgIQ.MurimiOS.API.Endpoints.CropCategory.Shared.CropCategoryResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, CropCategoryRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropCategory.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.CropCategory.PostGroup.CropCategoryRequest { Name = request.Name, Description = request.Description }), cancellationToken)).ToHttpResult()).Produces<CropCategoryResponse>(201);

    }
}
