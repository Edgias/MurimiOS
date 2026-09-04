using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Asset.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.Asset.AssetEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Asset.GetGroupId;
using AssetResponse = global::EdgIQ.MurimiOS.API.Endpoints.Asset.Shared.AssetResponse;
internal static class GetGroupIdEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/{id}", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid id) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Asset.GetGroupId.GetGroupIdQuery(id), cancellationToken)).ToHttpResult()).Produces<AssetResponse>(200).Produces(404);

    }
}
