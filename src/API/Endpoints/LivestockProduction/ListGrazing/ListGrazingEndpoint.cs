using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.LivestockProductionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.ListGrazing;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListGrazingEndpoint
{
    internal static void Map(IEndpointRouteBuilder farms)
    {
        farms.MapGet("/grazing-plans", async Task<IResult> (Guid farmId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockProduction.ListGrazing.ListGrazingQuery(farmId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.LivestockRead);

    }
}
