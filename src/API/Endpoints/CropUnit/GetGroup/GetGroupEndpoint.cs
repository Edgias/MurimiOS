using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropUnit.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.CropUnit.CropUnitEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropUnit.GetGroup;
using CropUnitResponse = global::EdgIQ.MurimiOS.API.Endpoints.CropUnit.Shared.CropUnitResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropUnit.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<CropUnitResponse>>(200);

    }
}
