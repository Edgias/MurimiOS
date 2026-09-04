using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Analytics.ResumeSchedule;
using static EdgIQ.MurimiOS.API.Endpoints.Analytics.AnalyticsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Analytics.ResumeSchedule;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ResumeScheduleEndpoint
{
    internal static void Map(IEndpointRouteBuilder analytics)
    {
        analytics.MapPost("/schedules/{scheduleId:guid}/resume", async Task<IResult> (Guid scheduleId, ResumeAnalyticsScheduleRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Analytics.ResumeSchedule.ResumeScheduleCommand(scheduleId, new global::EdgIQ.MurimiOS.Application.Analytics.ResumeSchedule.ResumeAnalyticsScheduleRequest(request.NextRunAt)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AnalyticsManage);

    }
}
