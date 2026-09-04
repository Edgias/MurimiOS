using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Crop.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Crop.CropEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Crop.GetGroup;
using CropResponse = global::EdgIQ.MurimiOS.API.Endpoints.Crop.Shared.CropResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Crop.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<CropResponse>>(200);

    }
}
