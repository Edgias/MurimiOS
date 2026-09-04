using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropVariety.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.CropVariety.CropVarietyEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropVariety.GetGroup;
using CropVarietyResponse = global::EdgIQ.MurimiOS.API.Endpoints.CropVariety.Shared.CropVarietyResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropVariety.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<CropVarietyResponse>>(200);

    }
}
