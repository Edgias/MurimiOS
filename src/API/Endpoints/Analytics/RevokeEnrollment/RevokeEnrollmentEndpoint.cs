using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Analytics.AnalyticsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Analytics.RevokeEnrollment;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class RevokeEnrollmentEndpoint
{
    internal static void Map(IEndpointRouteBuilder analytics)
    {
        analytics.MapPost("/benchmarks/enrollments/{enrollmentId:guid}/revoke", async Task<IResult> (Guid enrollmentId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Analytics.RevokeEnrollment.RevokeEnrollmentCommand(enrollmentId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AnalyticsBenchmark);

    }
}
