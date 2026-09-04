using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropCategory.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.CropCategory.CropCategoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropCategory.GetGroup;
using CropCategoryResponse = global::EdgIQ.MurimiOS.API.Endpoints.CropCategory.Shared.CropCategoryResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropCategory.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<CropCategoryResponse>>(200);

    }
}
