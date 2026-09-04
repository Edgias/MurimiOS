using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropProduction.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.CropProduction.CropProductionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropProduction.PostGroup;
using CropProductionRequest = global::EdgIQ.MurimiOS.API.Endpoints.CropProduction.Shared.CropProductionRequest;
using CropProductionResponse = global::EdgIQ.MurimiOS.API.Endpoints.CropProduction.Shared.CropProductionResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, CropProductionRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropProduction.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.CropProduction.PostGroup.CropProductionRequest { Name = request.Name, CropId = request.CropId, SeasonId = request.SeasonId, ExpectedYield = request.ExpectedYield, ActualYield = request.ActualYield }), cancellationToken)).ToHttpResult()).Produces<CropProductionResponse>(201);

    }
}
