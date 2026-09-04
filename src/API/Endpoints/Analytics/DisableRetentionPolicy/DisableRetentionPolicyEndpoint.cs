using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Analytics.AnalyticsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Analytics.DisableRetentionPolicy;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class DisableRetentionPolicyEndpoint
{
    internal static void Map(IEndpointRouteBuilder analytics)
    {
        analytics.MapPost("/retention-policies/{policyId:guid}/disable", async Task<IResult> (Guid policyId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Analytics.DisableRetentionPolicy.DisableRetentionPolicyCommand(policyId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AnalyticsManage);

    }
}
