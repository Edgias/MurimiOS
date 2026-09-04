using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Asset.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Asset.AssetEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Asset.GetGroup;
using AssetResponse = global::EdgIQ.MurimiOS.API.Endpoints.Asset.Shared.AssetResponse;
internal static class GetGroupEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Asset.GetGroup.GetGroupQuery(), cancellationToken)).ToHttpResult()).Produces<IEnumerable<AssetResponse>>(200);

    }
}
