using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropCategory.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.CropCategory.CropCategoryEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropCategory.GetGroupId;
using CropCategoryResponse = global::EdgIQ.MurimiOS.API.Endpoints.CropCategory.Shared.CropCategoryResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropCategory.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<CropCategoryResponse>(200).Produces(404);

    }
}
