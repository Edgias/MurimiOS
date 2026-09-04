using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Analytics.AnalyticsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Analytics.FarmDashboard;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class FarmDashboardEndpoint
{
    internal static void Map(IEndpointRouteBuilder analytics)
    {
        analytics.MapGet("/farms/{farmId:guid}/dashboard", async Task<IResult> (Guid farmId, DateOnly? periodEnd, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Analytics.FarmDashboard.FarmDashboardQuery(farmId, periodEnd), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AnalyticsRead);

    }
}
