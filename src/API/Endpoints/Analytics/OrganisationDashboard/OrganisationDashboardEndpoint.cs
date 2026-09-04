using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Analytics.AnalyticsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Analytics.OrganisationDashboard;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class OrganisationDashboardEndpoint
{
    internal static void Map(IEndpointRouteBuilder analytics)
    {
        analytics.MapGet("/organisation/{organisationNodeId:guid}/dashboard", async Task<IResult> (Guid organisationNodeId, DateOnly? periodEnd, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Analytics.OrganisationDashboard.OrganisationDashboardQuery(organisationNodeId, periodEnd), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AnalyticsRead);

    }
}
