using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Production.PostApiFarmsFarmIdGuidSeasons;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class PostApiFarmsFarmIdGuidSeasonsEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {

        api.MapPost("/farms/{farmId:guid}/seasons", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid farmId, CreateSeasonRequest request) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Production.PostApiFarmsFarmIdGuidSeasons.PostApiFarmsFarmIdGuidSeasonsCommand(farmId, new global::EdgIQ.MurimiOS.Application.Production.PostApiFarmsFarmIdGuidSeasons.CreateSeasonRequest(request.Id, request.Name, request.StartsOn, request.EndsOn, request.Status)), cancellationToken)).ToHttpResult()).WithTags("Production").RequireAuthorization(PlatformPermissions.ProductionWrite);

    }
}
