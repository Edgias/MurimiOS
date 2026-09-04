using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Crop.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Crop.CropEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Crop.PutGroupId;
using CropRequest = global::EdgIQ.MurimiOS.API.Endpoints.Crop.Shared.CropRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, CropRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Crop.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.Crop.PutGroupId.CropRequest { Name = request.Name, CropCategoryId = request.CropCategoryId, CropUnitId = request.CropUnitId }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
