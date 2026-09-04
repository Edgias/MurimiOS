using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Crop.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Crop.CropEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Crop.GetGroupId;
using CropResponse = global::EdgIQ.MurimiOS.API.Endpoints.Crop.Shared.CropResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Crop.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<CropResponse>(200).Produces(404);

    }
}
