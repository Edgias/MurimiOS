using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Production.GetApiCropZonesCropZoneIdGuidOperations;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetApiCropZonesCropZoneIdGuidOperationsEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {

        api.MapGet("/crop-zones/{cropZoneId:guid}/operations", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid cropZoneId) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Production.GetApiCropZonesCropZoneIdGuidOperations.GetApiCropZonesCropZoneIdGuidOperationsQuery(cropZoneId), cancellationToken)).ToHttpResult()).WithTags("Production").RequireAuthorization(PlatformPermissions.ProductionRead);

    }
}
