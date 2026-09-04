using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.AnalyticsMaturityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.ListDimensions;
using AnalyticsDimensionType = global::EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot.AnalyticsDimensionType;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListDimensionsEndpoint
{
    internal static void Map(IEndpointRouteBuilder analytics)
    {
        analytics.MapGet("/farms/{farmId:guid}/dimensions", async Task<IResult> (Guid farmId, Guid? projectionRunId, AnalyticsDimensionType? type, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.AnalyticsMaturity.ListDimensions.ListDimensionsQuery(farmId, projectionRunId, type), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AnalyticsRead);

    }
}
