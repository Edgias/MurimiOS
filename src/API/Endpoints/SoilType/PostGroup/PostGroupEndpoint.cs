using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SoilType.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.SoilType.SoilTypeEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SoilType.PostGroup;
using SoilTypeRequest = global::EdgIQ.MurimiOS.API.Endpoints.SoilType.Shared.SoilTypeRequest;
using SoilTypeResponse = global::EdgIQ.MurimiOS.API.Endpoints.SoilType.Shared.SoilTypeResponse;
internal static class PostGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapPost("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, SoilTypeRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SoilType.PostGroup.PostGroupCommand(new global::EdgIQ.MurimiOS.Application.SoilType.PostGroup.SoilTypeRequest { Name = request.Name }), cancellationToken)).ToHttpResult()).Produces<SoilTypeResponse>(201);

    }
}
