using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Analytics.EnrollBenchmark;
using static EdgIQ.MurimiOS.API.Endpoints.Analytics.AnalyticsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Analytics.EnrollBenchmark;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class EnrollBenchmarkEndpoint
{
    internal static void Map(IEndpointRouteBuilder analytics)
    {
        analytics.MapPost("/benchmarks/enrollments", async Task<IResult> (CreateBenchmarkEnrollmentRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Analytics.EnrollBenchmark.EnrollBenchmarkCommand(new global::EdgIQ.MurimiOS.Application.Analytics.EnrollBenchmark.CreateBenchmarkEnrollmentRequest(request.Id, request.MetricCode, request.CohortDefinitionId)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AnalyticsBenchmark);

    }
}
