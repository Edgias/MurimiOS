using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Analytics.AnalyticsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Analytics.ListSchedules;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListSchedulesEndpoint
{
    internal static void Map(IEndpointRouteBuilder analytics)
    {
        analytics.MapGet("/schedules", async Task<IResult> (EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Analytics.ListSchedules.ListSchedulesQuery(), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AnalyticsRead);

    }
}
