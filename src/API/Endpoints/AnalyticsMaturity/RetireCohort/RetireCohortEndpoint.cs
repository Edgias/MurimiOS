using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.AnalyticsMaturityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.RetireCohort;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class RetireCohortEndpoint
{
    internal static void Map(IEndpointRouteBuilder analytics)
    {
        analytics.MapPost("/benchmark-cohorts/{id:guid}/retire", async Task<IResult> (Guid id, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.AnalyticsMaturity.RetireCohort.RetireCohortCommand(id), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AnalyticsManage);

    }
}
