using System.Text.Json;
using EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot;
using EdgIQ.MurimiOS.Domain.AnalyticsForecastEvaluation;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Analytics;
using Crop = global::EdgIQ.MurimiOS.Domain.Crop.Crop;
using Supplier = global::EdgIQ.MurimiOS.Domain.Supplier.Supplier;

using AnalyticsDimensionSnapshot = global::EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot.AnalyticsDimensionSnapshot;
using AnalyticsDimensionType = global::EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot.AnalyticsDimensionType;
using AnalyticsForecastEvaluation = global::EdgIQ.MurimiOS.Domain.AnalyticsForecastEvaluation.AnalyticsForecastEvaluation;
using AnalyticsFact = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFact;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;
using AnalyticsProjectionStatus = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionStatus;
using AnalyticsValueKind = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsValueKind;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;

public sealed class AnalyticsDimensionSnapshotBuilder
{
    public AnalyticsDimensionSnapshot[] Build(AnalyticsProjectionRun run, IReadOnlyCollection<AnalyticsFact> facts)
    {
        var snapshots = new List<AnalyticsDimensionSnapshot>
        {
            new(Guid.NewGuid(), run.Id, run.FarmId, AnalyticsDimensionType.Farm, run.FarmId,
                "farm", run.FarmId.ToString("N"), JsonSerializer.Serialize(new { run.FarmId }), run.PeriodEnd)
        };
        snapshots.AddRange(facts.Where(x => x.CropZoneId is not null).GroupBy(x => x.CropZoneId!.Value).Select(group =>
        {
            AnalyticsFact fact = group.First();
            return new AnalyticsDimensionSnapshot(Guid.NewGuid(), run.Id, run.FarmId, AnalyticsDimensionType.CropZone,
                group.Key, fact.GrainKey, $"crop-zone:{group.Key:N}", JsonSerializer.Serialize(new
                { fact.CropZoneId, fact.FieldId, fact.SeasonId, fact.Crop, fact.Variety }), run.PeriodEnd);
        }));
        snapshots.AddRange(facts.Where(x => x.LivestockSubjectId is not null).GroupBy(x => x.LivestockSubjectId!.Value).Select(group =>
        {
            AnalyticsFact fact = group.First();
            return new AnalyticsDimensionSnapshot(Guid.NewGuid(), run.Id, run.FarmId, AnalyticsDimensionType.LivestockSubject,
                group.Key, fact.GrainKey, $"livestock:{group.Key:N}", JsonSerializer.Serialize(new
                { fact.LivestockSubjectId, fact.Species }), run.PeriodEnd);
        }));
        snapshots.AddRange(facts.Where(x => x.SupplierId is not null).GroupBy(x => x.SupplierId!.Value).Select(group =>
        {
            AnalyticsFact fact = group.First();
            return new AnalyticsDimensionSnapshot(Guid.NewGuid(), run.Id, run.FarmId, AnalyticsDimensionType.Supplier,
                group.Key, fact.GrainKey, $"supplier:{group.Key:N}", fact.DimensionsJson ?? "{}", run.PeriodEnd);
        }));
        return snapshots.ToArray();
    }
}

public sealed class AnalyticsForecastEvaluator
{
    public async Task<AnalyticsForecastEvaluation[]> EvaluateAsync(AnalyticsProjectionRun run,
        IReadOnlyCollection<AnalyticsFact> newFacts, PlatformDbContext db, DateTimeOffset evaluatedAt,
        CancellationToken cancellationToken)
    {
        AnalyticsFact[] actuals = newFacts.Where(x => x.ValueKind == AnalyticsValueKind.Actual).ToArray();
        if (actuals.Length == 0) return [];
        string[] metrics = actuals.Select(x => x.MetricCode).Distinct().ToArray();
        string[] grains = actuals.Select(x => x.GrainKey).Distinct().ToArray();
        DateOnly maximumActualDate = actuals.Max(x => x.PeriodEnd);
        AnalyticsFact[] forecasts = await db.AnalyticsFacts.AsNoTracking()
            .Where(x => x.FarmId == run.FarmId && x.ValueKind == AnalyticsValueKind.Forecast
                && metrics.Contains(x.MetricCode) && grains.Contains(x.GrainKey)
                && x.PeriodEnd <= maximumActualDate && x.PredictiveModelDefinitionId != null
                && db.AnalyticsProjectionRuns.Any(r => r.Id == x.ProjectionRunId
                    && r.Status == AnalyticsProjectionStatus.Succeeded && r.SupersededAt == null))
            .ToArrayAsync(cancellationToken);
        Guid[] forecastIds = forecasts.Select(x => x.Id).ToArray();
        HashSet<Guid> evaluated = (await db.AnalyticsForecastEvaluations.AsNoTracking()
            .Where(x => forecastIds.Contains(x.ForecastFactId)).Select(x => x.ForecastFactId)
            .ToArrayAsync(cancellationToken)).ToHashSet();
        var results = new List<AnalyticsForecastEvaluation>();
        foreach (AnalyticsFact forecast in forecasts.Where(x => !evaluated.Contains(x.Id)))
        {
            AnalyticsFact? actual = actuals.Where(x => x.MetricCode == forecast.MetricCode
                    && x.GrainKey == forecast.GrainKey && x.Unit == forecast.Unit && x.PeriodEnd >= forecast.PeriodEnd)
                .OrderBy(x => x.PeriodEnd).FirstOrDefault();
            if (actual is null) continue;
            int horizon = Math.Max(0, forecast.PeriodEnd.DayNumber - DateOnly.FromDateTime(forecast.CreatedAtUtc.UtcDateTime).DayNumber);
            results.Add(new AnalyticsForecastEvaluation(Guid.NewGuid(), forecast.Id, actual.Id, run.FarmId,
                forecast.MetricCode, forecast.GrainKey, forecast.Value, actual.Value, forecast.Unit,
                horizon, forecast.FormulaVersion, evaluatedAt));
        }
        return results.ToArray();
    }
}
