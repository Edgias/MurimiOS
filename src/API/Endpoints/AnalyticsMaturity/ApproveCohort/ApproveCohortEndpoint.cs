using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.AnalyticsMaturityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.ApproveCohort;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using AnalyticsApprovalRequest = global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.Shared.AnalyticsApprovalRequest;
internal static class ApproveCohortEndpoint
{
    internal static void Map(IEndpointRouteBuilder analytics)
    {
        analytics.MapPost("/benchmark-cohorts/{id:guid}/approve", async Task<IResult> (Guid id, AnalyticsApprovalRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.AnalyticsMaturity.ApproveCohort.ApproveCohortCommand(id, new global::EdgIQ.MurimiOS.Application.AnalyticsMaturity.ApproveCohort.AnalyticsApprovalRequest(request.ApprovalReference)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AnalyticsManage);

    }
}
