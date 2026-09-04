using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.WeatherLocation;
using EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;
using EdgIQ.MurimiOS.Domain.FieldRiskRule;
using EdgIQ.MurimiOS.Domain.FieldInsightAlert;
using EdgIQ.MurimiOS.Domain.FieldMapLayer;
using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.EvaluateRisks;
using WeatherLocation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherLocation;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using FieldInsightAlert = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.FieldInsightAlert;
using InsightAlertStatus = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.InsightAlertStatus;
using FieldMapLayer = global::EdgIQ.MurimiOS.Domain.FieldMapLayer.FieldMapLayer;
using MapLayerKind = global::EdgIQ.MurimiOS.Domain.FieldMapLayer.MapLayerKind;
using FieldRiskAssessment = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.FieldRiskAssessment;
using FieldRiskRule = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.FieldRiskRule;
using RiskAggregation = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskAggregation;
using RiskComparison = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskComparison;
using RiskMetric = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskMetric;
using RiskRuleStatus = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskRuleStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SatelliteFieldStatistic = global::EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic.SatelliteFieldStatistic;
using VegetationIndex = global::EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic.VegetationIndex;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;
using ScoutingCategory = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingCategory;
using ScoutingObservation = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingObservation;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;
using WeatherObservation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherObservation;

public sealed class EvaluateRisksHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<EvaluateRisksCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(EvaluateRisksCommand message, CancellationToken cancellationToken) => EvaluateRisksAsync(message.fieldId, db, scope, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> EvaluateRisksAsync(Guid fieldId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock,
        UseCaseContext context, CancellationToken cancellationToken)
    {
        var target = await LoadFieldAsync(fieldId, db, scope, cancellationToken);
        if (target.Field is null) return UseCaseResults.NotFound();
        DateTimeOffset now = clock.GetUtcNow();
        string[] crops = await db.CropZones.AsNoTracking().Where(x => x.FieldId == fieldId)
            .Select(x => x.Crop).Distinct().ToArrayAsync(cancellationToken);
        FieldRiskRule[] rules = await db.FieldRiskRules.Where(x => x.Status == RiskRuleStatus.Active
            && x.EffectiveFrom <= now && (x.EffectiveTo == null || x.EffectiveTo > now)
            && (x.Crop == null || crops.Contains(x.Crop))).OrderBy(x => x.RuleKey).ToArrayAsync(cancellationToken);
        var results = new List<FieldRiskAssessment>(); int createdAlerts = 0; int resolvedAlerts = 0;
        foreach (FieldRiskRule rule in rules)
        {
            DateTimeOffset windowStart = now.AddHours(-rule.LookbackHours);
            MetricSampleSet samples = await LoadMetricSamplesAsync(fieldId, rule, windowStart, now, db, cancellationToken);
            decimal? observed = Aggregate(samples.Values, rule.Aggregation);
            bool triggered = samples.Values.Count >= rule.MinimumSamples && observed is { } value && Compare(value, rule.Comparison, rule.Threshold);
            string snapshot = JsonSerializer.Serialize(new { rule.Metric, rule.Aggregation, rule.Comparison, rule.Threshold,
                Samples = samples.Values.Count, ObservedValue = observed, samples.LatestSourceAt });
            var assessment = new FieldRiskAssessment(Guid.NewGuid(), target.Field.FarmId, fieldId,
                target.Boundary!.Id, rule.Id, rule.RuleVersion, now, windowStart, now, observed,
                samples.Values.Count, triggered, rule.Severity, EvaluatorVersion, snapshot);
            db.FieldRiskAssessments.Add(assessment); results.Add(assessment);
            string alertKey = $"risk:{rule.RuleKey}:{fieldId:N}";
            FieldInsightAlert? existing = await db.FieldInsightAlerts.SingleOrDefaultAsync(x => x.AlertKey == alertKey
                && (x.Status == InsightAlertStatus.Open || x.Status == InsightAlertStatus.Acknowledged), cancellationToken);
            if (triggered && existing is null)
            {
                var alert = new FieldInsightAlert(Guid.NewGuid(), target.Field.FarmId, fieldId, target.Boundary.Id,
                    alertKey, nameof(FieldRiskAssessment), assessment.Id, rule.Severity, rule.Name,
                    RenderRiskMessage(rule, observed!.Value), now, samples.LatestSourceAt ?? now, null);
                db.FieldInsightAlerts.Add(alert); createdAlerts++;
                db.FieldMapLayers.Add(BuildRiskLayer(alert, target.Field.Name, target.Boundary, assessment, now));
                AddEvent(db, tenant, actor, nameof(FieldInsightAlert), alert.Id, target.Field.FarmId,
                    "InsightAlertOpened", "FieldIntelligence.AlertOpened", new { alert.Id, alert.AlertKey, alert.FieldId, alert.Severity, RuleId = rule.Id, assessment.ObservedValue }, now, context);
            }
            else if (!triggered && existing is not null)
            {
                existing.Resolve(now, actor.Subject, "The deterministic risk rule no longer meets its configured threshold.");
                resolvedAlerts++;
                AddEvent(db, tenant, actor, nameof(FieldInsightAlert), existing.Id, target.Field.FarmId,
                    "InsightAlertResolved", "FieldIntelligence.AlertResolved", new { existing.Id, existing.AlertKey, RuleId = rule.Id, Automatic = true }, now, context);
            }
        }
        AddEvent(db, tenant, actor, nameof(FieldRiskAssessment), fieldId, target.Field.FarmId,
            "FieldRisksEvaluated", "FieldIntelligence.RisksEvaluated", new { FieldId = fieldId, Rules = rules.Length, Triggered = results.Count(x => x.Triggered), CreatedAlerts = createdAlerts, ResolvedAlerts = resolvedAlerts, EvaluatorVersion }, now, context);
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Ok(new EvaluateFieldRisksResponse(fieldId, now, EvaluatorVersion,
            results.Select(ToAssessment).ToArray(), createdAlerts, resolvedAlerts));
    }

    private const string EvaluatorVersion = "field-risk-v1";


    private static async Task<MetricSampleSet> LoadMetricSamplesAsync(Guid fieldId, FieldRiskRule rule,
        DateTimeOffset from, DateTimeOffset to, IPlatformDbContext db, CancellationToken cancellationToken)
    {
        RiskMetric metric = rule.Metric;
        if (metric is RiskMetric.TemperatureC or RiskMetric.PrecipitationMm or RiskMetric.RelativeHumidityPercent or RiskMetric.WindSpeedMps)
        {
            WeatherObservation[] rows = await db.WeatherObservations.AsNoTracking()
                .Where(x => x.FieldId == fieldId && x.Quality != InsightQuality.Invalid && x.ObservedAt >= from && x.ObservedAt <= to)
                .OrderBy(x => x.ObservedAt).ToArrayAsync(cancellationToken);
            IEnumerable<decimal?> selected = metric switch
            {
                RiskMetric.TemperatureC => rows.Select(x => x.TemperatureC),
                RiskMetric.PrecipitationMm => rows.Select(x => x.PrecipitationMm),
                RiskMetric.RelativeHumidityPercent => rows.Select(x => x.RelativeHumidityPercent),
                _ => rows.Select(x => x.WindSpeedMps)
            };
            return new MetricSampleSet(selected.Where(x => x.HasValue).Select(x => x!.Value).ToArray(), rows.LastOrDefault()?.ObservedAt);
        }
        if (metric is RiskMetric.NdviMean or RiskMetric.NdreMean or RiskMetric.NdmiMean)
        {
            VegetationIndex index = metric == RiskMetric.NdviMean ? VegetationIndex.Ndvi : metric == RiskMetric.NdreMean ? VegetationIndex.Ndre : VegetationIndex.Ndmi;
            SatelliteFieldStatistic[] rows = await db.SatelliteFieldStatistics.AsNoTracking()
                .Where(x => x.FieldId == fieldId && x.Index == index && x.Quality != InsightQuality.Invalid && x.AcquiredAt >= from && x.AcquiredAt <= to)
                .OrderBy(x => x.AcquiredAt).ToArrayAsync(cancellationToken);
            return new MetricSampleSet(rows.Select(x => x.Mean).ToArray(), rows.LastOrDefault()?.AcquiredAt);
        }
        IQueryable<ScoutingObservation> scoutingQuery = db.ScoutingObservations.AsNoTracking()
            .Where(x => x.FieldId == fieldId && x.ObservedAt >= from && x.ObservedAt <= to);
        if (rule.ScoutingCategory is { } category) scoutingQuery = scoutingQuery.Where(x => x.Category == category);
        if (rule.IssueCode is { } issueCode) scoutingQuery = scoutingQuery.Where(x => x.IssueCode == issueCode);
        ScoutingObservation[] scouting = await scoutingQuery
            .OrderBy(x => x.ObservedAt).ToArrayAsync(cancellationToken);
        return new MetricSampleSet(scouting.Select(x => (decimal)x.Severity).ToArray(), scouting.LastOrDefault()?.ObservedAt);
    }


    private static decimal? Aggregate(IReadOnlyList<decimal> values, RiskAggregation aggregation)
    {
        if (values.Count == 0) return null;
        return aggregation switch
        {
            RiskAggregation.Latest => values[^1],
            RiskAggregation.Minimum => values.Min(),
            RiskAggregation.Maximum => values.Max(),
            RiskAggregation.Average => values.Average(),
            RiskAggregation.Sum => values.Sum(),
            _ => throw new ArgumentOutOfRangeException(nameof(aggregation))
        };
    }


    private static bool Compare(decimal value, RiskComparison comparison, decimal threshold) => comparison switch
    {
        RiskComparison.GreaterThan => value > threshold,
        RiskComparison.GreaterThanOrEqual => value >= threshold,
        RiskComparison.LessThan => value < threshold,
        RiskComparison.LessThanOrEqual => value <= threshold,
        RiskComparison.Equal => value == threshold,
        _ => false
    };


    private static string RenderRiskMessage(FieldRiskRule rule, decimal observed) =>
        $"{rule.Message} Observed {observed:0.###}; configured threshold {rule.Comparison} {rule.Threshold:0.###} using {rule.Aggregation} over {rule.LookbackHours} hour(s).";


    private static async Task<FieldTarget> LoadFieldAsync(Guid fieldId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, CancellationToken cancellationToken)
    {
        FarmField? field = await db.FarmFields.AsNoTracking().SingleOrDefaultAsync(x => x.Id == fieldId, cancellationToken);
        if (field is null || !await scope.CanAccessFarmAsync(field.FarmId, cancellationToken)) return new(null, null);
        FieldBoundaryVersion? boundary = await db.FieldBoundaryVersions.AsNoTracking().Where(x => x.FieldId == fieldId && x.ValidTo == null)
            .OrderByDescending(x => x.ValidFrom).FirstOrDefaultAsync(cancellationToken);
        return boundary is null ? new(null, null) : new(field, boundary);
    }


    private static FieldMapLayer BuildRiskLayer(FieldInsightAlert alert, string fieldName,
        FieldBoundaryVersion boundary, FieldRiskAssessment assessment, DateTimeOffset now)
    {
        string geoJson = BoundaryGeoJson(boundary.Boundary, new { alert.Id, alert.FieldId, alert.Severity, alert.Title, alert.Message, assessment.ObservedValue, assessment.AssessedAt });
        string color = alert.Severity >= InsightSeverity.Critical ? "#7f1d1d" : alert.Severity >= InsightSeverity.Warning ? "#dc2626" : "#f59e0b";
        string style = JsonSerializer.Serialize(new { fillColor = color, fillOpacity = 0.35, strokeColor = color, strokeWidth = 2 });
        return new FieldMapLayer(Guid.NewGuid(), alert.FarmId, alert.FieldId, alert.BoundaryVersionId,
            MapLayerKind.Risk, $"{fieldName} risk - {alert.Title}", nameof(FieldInsightAlert), alert.Id,
            alert.ObservedAt, now, alert.DetectedAt, alert.ExpiresAt, geoJson, style, InsightQuality.Valid);
    }


    private static string BoundaryGeoJson(MultiPolygon boundary, object properties) =>
        JsonSerializer.Serialize(new { type = "Feature", geometry = MultiPolygonCoordinates(boundary), properties });


    private static object MultiPolygonCoordinates(MultiPolygon boundary) => new
    {
        type = "MultiPolygon",
        coordinates = Enumerable.Range(0, boundary.NumGeometries).Select(polygonIndex =>
        {
            var polygon = (Polygon)boundary.GetGeometryN(polygonIndex);
            var rings = new List<double[][]> { polygon.ExteriorRing.Coordinates.Select(c => new[] { c.X, c.Y }).ToArray() };
            rings.AddRange(Enumerable.Range(0, polygon.NumInteriorRings).Select(index => polygon.GetInteriorRingN(index).Coordinates.Select(c => new[] { c.X, c.Y }).ToArray()));
            return rings.ToArray();
        }).ToArray()
    };

    private static FieldRiskAssessmentResponse ToAssessment(FieldRiskAssessment x) => new(x.Id, x.FarmId, x.FieldId,
        x.BoundaryVersionId, x.RuleId, x.RuleVersion, x.AssessedAt, x.WindowStart, x.WindowEnd, x.ObservedValue,
        x.SampleCount, x.Triggered, x.Severity, x.EvaluatorVersion, JsonSerializer.Deserialize<JsonElement>(x.InputSnapshotJson));


    private static void AddEvent(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string entityType, Guid entityId, Guid? farmId, string action, string eventType, object payload,
        DateTimeOffset now, UseCaseContext context)
    {
        string json = JsonSerializer.Serialize(payload);
        string syncOperation = action switch
        {
            "InsightAlertAcknowledged" => "AlertAcknowledged",
            "InsightAlertDismissed" => "AlertDismissed",
            "AlertWorkOrderCreated" => "AlertWorkCreated",
            _ => action
        };
        if (farmId is { } farm) db.SyncChanges.Add(new SyncChange(tenant.TenantId, farm, entityType, entityId, syncOperation, json, now));
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, entityType, entityId, farmId, actor.ScopeOrganisationNodeId, json,
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now));
    }

    private sealed record FieldTarget(FarmField? Field, FieldBoundaryVersion? Boundary);

    private sealed record MetricSampleSet(IReadOnlyList<decimal> Values, DateTimeOffset? LatestSourceAt);

}
