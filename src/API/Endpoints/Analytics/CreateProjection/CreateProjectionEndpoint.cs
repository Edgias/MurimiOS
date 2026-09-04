using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateProjection;
using static EdgIQ.MurimiOS.API.Endpoints.Analytics.AnalyticsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateProjection;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateProjectionEndpoint
{
    internal static void Map(IEndpointRouteBuilder analytics)
    {
        analytics.MapPost("/farms/{farmId:guid}/projections", async Task<IResult> (Guid farmId, CreateAnalyticsProjectionRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Analytics.CreateProjection.CreateProjectionCommand(farmId, new global::EdgIQ.MurimiOS.Application.Analytics.CreateProjection.CreateAnalyticsProjectionRequest(request.Id, request.PeriodStart, request.PeriodEnd, request.Currency, request.ProjectionVersion, request.ScheduledFor)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AnalyticsManage);

    }
}
