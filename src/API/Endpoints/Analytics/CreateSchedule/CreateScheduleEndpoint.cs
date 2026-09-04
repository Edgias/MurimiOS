using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateSchedule;
using static EdgIQ.MurimiOS.API.Endpoints.Analytics.AnalyticsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateSchedule;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateScheduleEndpoint
{
    internal static void Map(IEndpointRouteBuilder analytics)
    {
        analytics.MapPost("/farms/{farmId:guid}/schedules", async Task<IResult> (Guid farmId, CreateAnalyticsScheduleRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Analytics.CreateSchedule.CreateScheduleCommand(farmId, new global::EdgIQ.MurimiOS.Application.Analytics.CreateSchedule.CreateAnalyticsScheduleRequest(request.Id, request.Name, request.Frequency, request.LookbackDays, request.Currency, request.ProjectionVersion, request.NextRunAt)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AnalyticsManage);

    }
}
