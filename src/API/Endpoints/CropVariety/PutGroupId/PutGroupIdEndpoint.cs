using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropVariety.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.CropVariety.CropVarietyEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropVariety.PutGroupId;
using CropVarietyRequest = global::EdgIQ.MurimiOS.API.Endpoints.CropVariety.Shared.CropVarietyRequest;
internal static class PutGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPut("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id, CropVarietyRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropVariety.PutGroupId.PutGroupIdCommand(id, new global::EdgIQ.MurimiOS.Application.CropVariety.PutGroupId.CropVarietyRequest { Name = request.Name, CropId = request.CropId }), cancellationToken)).ToHttpResult()).Produces(200).Produces(404).Produces(400);

    }
}
