using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropVariety.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.CropVariety.CropVarietyEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropVariety.PostGroup;
using CropVarietyRequest = global::EdgIQ.MurimiOS.API.Endpoints.CropVariety.Shared.CropVarietyRequest;
using CropVarietyResponse = global::EdgIQ.MurimiOS.API.Endpoints.CropVariety.Shared.CropVarietyResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, CropVarietyRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropVariety.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.CropVariety.PostGroup.CropVarietyRequest { Name = request.Name, CropId = request.CropId }), cancellationToken)).ToHttpResult()).Produces<CropVarietyResponse>(201);

    }
}
