using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropUnit.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.CropUnit.CropUnitEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropUnit.PostGroup;
using CropUnitRequest = global::EdgIQ.MurimiOS.API.Endpoints.CropUnit.Shared.CropUnitRequest;
using CropUnitResponse = global::EdgIQ.MurimiOS.API.Endpoints.CropUnit.Shared.CropUnitResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, CropUnitRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropUnit.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.CropUnit.PostGroup.CropUnitRequest { Name = request.Name }), cancellationToken)).ToHttpResult()).Produces<CropUnitResponse>(201);

    }
}
