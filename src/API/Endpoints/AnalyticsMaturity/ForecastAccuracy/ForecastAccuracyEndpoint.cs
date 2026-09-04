using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.AnalyticsMaturityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.ForecastAccuracy;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ForecastAccuracyEndpoint
{
    internal static void Map(IEndpointRouteBuilder analytics)
    {
        analytics.MapGet("/farms/{farmId:guid}/forecast-accuracy", async Task<IResult> (Guid farmId, string? metricCode, DateTimeOffset? from, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.AnalyticsMaturity.ForecastAccuracy.ForecastAccuracyQuery(farmId, metricCode, from), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AnalyticsRead);

    }
}
