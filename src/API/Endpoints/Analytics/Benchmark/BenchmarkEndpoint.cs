using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Analytics.AnalyticsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Analytics.Benchmark;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class BenchmarkEndpoint
{
    internal static void Map(IEndpointRouteBuilder analytics)
    {
        analytics.MapGet("/benchmarks/{metricCode}", async Task<IResult> (string metricCode, string cohortKey, DateOnly from, DateOnly to, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Analytics.Benchmark.BenchmarkQuery(metricCode, cohortKey, from, to), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AnalyticsBenchmark);

    }
}
