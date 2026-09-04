using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropProduction.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.CropProduction.CropProductionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropProduction.PutGroupId;
using CropProductionRequest = global::EdgIQ.MurimiOS.API.Endpoints.CropProduction.Shared.CropProductionRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, CropProductionRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropProduction.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.CropProduction.PutGroupId.CropProductionRequest { Name = request.Name, CropId = request.CropId, SeasonId = request.SeasonId, ExpectedYield = request.ExpectedYield, ActualYield = request.ActualYield }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
