using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SoilType.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.SoilType.SoilTypeEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SoilType.GetGroupId;
using SoilTypeResponse = global::EdgIQ.MurimiOS.API.Endpoints.SoilType.Shared.SoilTypeResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SoilType.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<SoilTypeResponse>(200).Produces(404);

    }
}
