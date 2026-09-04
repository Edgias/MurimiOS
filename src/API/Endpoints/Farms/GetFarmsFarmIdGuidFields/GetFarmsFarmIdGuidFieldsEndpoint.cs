using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System.Text.Json;
using static EdgIQ.MurimiOS.API.Endpoints.Platform.PlatformEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Farms.GetFarmsFarmIdGuidFields;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class GetFarmsFarmIdGuidFieldsEndpoint
{
    internal static void Map(IEndpointRouteBuilder farms)
    {

        farms.MapGet("/{farmId:guid}/fields", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken, Guid farmId) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Farms.GetFarmsFarmIdGuidFields.GetFarmsFarmIdGuidFieldsQuery(farmId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FarmRead);

    }
}
