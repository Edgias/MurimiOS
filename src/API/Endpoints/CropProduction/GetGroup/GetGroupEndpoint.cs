using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropProduction.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.CropProduction.CropProductionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropProduction.GetGroup;
using CropProductionResponse = global::EdgIQ.MurimiOS.API.Endpoints.CropProduction.Shared.CropProductionResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropProduction.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<CropProductionResponse>>(200);

    }
}
