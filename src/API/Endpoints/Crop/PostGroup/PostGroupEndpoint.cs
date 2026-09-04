using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Crop.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Crop.CropEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Crop.PostGroup;
using CropRequest = global::EdgIQ.MurimiOS.API.Endpoints.Crop.Shared.CropRequest;
using CropResponse = global::EdgIQ.MurimiOS.API.Endpoints.Crop.Shared.CropResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, CropRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Crop.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.Crop.PostGroup.CropRequest { Name = request.Name, CropCategoryId = request.CropCategoryId, CropUnitId = request.CropUnitId }), cancellationToken)).ToHttpResult()).Produces<CropResponse>(201);

    }
}
