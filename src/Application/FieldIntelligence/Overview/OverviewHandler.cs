using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.FieldInsightAlert;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.Overview;
using FieldInsightAlert = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.FieldInsightAlert;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;

using InsightAlertStatus = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.InsightAlertStatus;
using IntegrationFeedStatus = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedStatus;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using IntegrationTargetType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationTargetType;

public sealed class OverviewHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, TimeProvider clock) : IRequestHandler<OverviewQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(OverviewQuery message, CancellationToken cancellationToken) => OverviewAsync(message.farmId, db, scope, clock, cancellationToken);

    private static async Task<UseCaseResult> OverviewAsync(Guid farmId, IPlatformDbContext db, IOrganisationScopeAccess scope,
        TimeProvider clock, CancellationToken cancellationToken)
    {
        if (!await scope.CanAccessFarmAsync(farmId, cancellationToken) || !await db.Farms.AnyAsync(x => x.Id == farmId, cancellationToken)) return UseCaseResults.NotFound();
        DateTimeOffset now = clock.GetUtcNow();
        var feeds = await db.IntegrationFeeds.AsNoTracking().Where(x => x.FarmId == farmId && (x.FeedType == IntegrationFeedType.Weather || x.FeedType == IntegrationFeedType.Satellite || x.FeedType == IntegrationFeedType.Sensor))
            .Select(x => new InsightFeedFreshnessResponse(x.Id, x.FeedType, x.Name, x.TargetType, x.TargetId, x.Status,
                x.LastObservedAt, x.LastReceivedAt, x.LastSucceededAt, x.LastFailedAt, x.LastErrorCode,
                x.LastReceivedAt == null || now - x.LastReceivedAt > TimeSpan.FromMinutes(x.FreshnessThresholdMinutes), x.FreshnessThresholdMinutes)).ToArrayAsync(cancellationToken);
        int openAlerts = await db.FieldInsightAlerts.CountAsync(x => x.FarmId == farmId && (x.Status == InsightAlertStatus.Open || x.Status == InsightAlertStatus.Acknowledged), cancellationToken);
        DateTimeOffset? latestWeather = await db.WeatherObservations.Where(x => x.FarmId == farmId).MaxAsync(x => (DateTimeOffset?)x.ObservedAt, cancellationToken);
        DateTimeOffset? latestSatellite = await db.SatelliteFieldStatistics.Where(x => x.FarmId == farmId).MaxAsync(x => (DateTimeOffset?)x.AcquiredAt, cancellationToken);
        DateTimeOffset? latestScouting = await db.ScoutingObservations.Where(x => x.FarmId == farmId).MaxAsync(x => (DateTimeOffset?)x.ObservedAt, cancellationToken);
        return UseCaseResults.Ok(new FarmIntelligenceOverviewResponse(farmId, now, latestWeather, latestSatellite, latestScouting, openAlerts, feeds));
    }


}
