using EdgIQ.MurimiOS.API.Infrastructure;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using EdgIQ.MurimiOS.API.Endpoints.Platform.GetApiPlatform;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Platform.GetApiPlatform;
internal static class GetApiPlatformEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {

        api.MapGet("/platform", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Platform.GetApiPlatform.GetApiPlatformQuery(), cancellationToken)).ToHttpResult()).Produces<PlatformInfoResponse>(200).WithTags("Platform");

    }
}
