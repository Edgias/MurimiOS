using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.LivestockProductionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.Performance;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class PerformanceEndpoint
{
    internal static void Map(IEndpointRouteBuilder livestock)
    {
        livestock.MapGet("/subjects/{subjectId:guid}/performance", async Task<IResult> (Guid subjectId, DateTimeOffset? from, DateTimeOffset? to, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockProduction.Performance.PerformanceQuery(subjectId, from, to), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.LivestockRead);

    }
}
