using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SoilType.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.SoilType.SoilTypeEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SoilType.GetGroup;
using SoilTypeResponse = global::EdgIQ.MurimiOS.API.Endpoints.SoilType.Shared.SoilTypeResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SoilType.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<SoilTypeResponse>>(200);

    }
}
