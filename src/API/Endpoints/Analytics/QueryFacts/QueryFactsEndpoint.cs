using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Analytics.AnalyticsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Analytics.QueryFacts;
using AnalyticsValueKind = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsValueKind;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class QueryFactsEndpoint
{
    internal static void Map(IEndpointRouteBuilder analytics)
    {
        analytics.MapGet("/farms/{farmId:guid}/facts", async Task<IResult> (Guid farmId, string? metricCode, AnalyticsValueKind? valueKind, DateOnly? from, DateOnly? to, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Analytics.QueryFacts.QueryFactsQuery(farmId, metricCode, valueKind, from, to), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AnalyticsRead);

    }
}
