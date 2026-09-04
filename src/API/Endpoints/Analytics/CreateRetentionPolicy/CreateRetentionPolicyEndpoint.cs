using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateRetentionPolicy;
using static EdgIQ.MurimiOS.API.Endpoints.Analytics.AnalyticsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateRetentionPolicy;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateRetentionPolicyEndpoint
{
    internal static void Map(IEndpointRouteBuilder analytics)
    {
        analytics.MapPost("/retention-policies", async Task<IResult> (CreateAnalyticsRetentionPolicyRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Analytics.CreateRetentionPolicy.CreateRetentionPolicyCommand(new global::EdgIQ.MurimiOS.Application.Analytics.CreateRetentionPolicy.CreateAnalyticsRetentionPolicyRequest(request.Id, request.RawSensorDays, request.RawTelemetryDays, request.FailedProjectionDays, request.NextRunAt, request.DerivedMediaDays, request.ExpiredMapLayerDays)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AnalyticsManage);

    }
}
