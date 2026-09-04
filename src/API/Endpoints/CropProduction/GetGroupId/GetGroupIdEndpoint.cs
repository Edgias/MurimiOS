using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropProduction.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.CropProduction.CropProductionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropProduction.GetGroupId;
using CropProductionResponse = global::EdgIQ.MurimiOS.API.Endpoints.CropProduction.Shared.CropProductionResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropProduction.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<CropProductionResponse>(200).Produces(404);

    }
}
