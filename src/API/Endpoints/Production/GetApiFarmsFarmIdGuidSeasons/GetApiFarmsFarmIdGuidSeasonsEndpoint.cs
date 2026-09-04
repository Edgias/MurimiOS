using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Production.GetApiFarmsFarmIdGuidSeasons;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetApiFarmsFarmIdGuidSeasonsEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapGet("/farms/{farmId:guid}/seasons", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid farmId) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Production.GetApiFarmsFarmIdGuidSeasons.GetApiFarmsFarmIdGuidSeasonsQuery(farmId), cancellationToken)).ToHttpResult()).WithTags("Production").RequireAuthorization(PlatformPermissions.ProductionRead);

    }
}
