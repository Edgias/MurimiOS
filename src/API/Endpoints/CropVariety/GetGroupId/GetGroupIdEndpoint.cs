using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropVariety.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.CropVariety.CropVarietyEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropVariety.GetGroupId;
using CropVarietyResponse = global::EdgIQ.MurimiOS.API.Endpoints.CropVariety.Shared.CropVarietyResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropVariety.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<CropVarietyResponse>(200).Produces(404);

    }
}
