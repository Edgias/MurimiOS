using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition;
using EdgIQ.MurimiOS.Domain.AnalyticsRefreshRequest;
using EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot;
using EdgIQ.MurimiOS.Domain.PredictiveModelDefinition;
using EdgIQ.MurimiOS.Domain.AnalyticsForecastEvaluation;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using EdgIQ.MurimiOS.Domain.KpiDefinition;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule;
using EdgIQ.MurimiOS.Domain.BenchmarkEnrollment;
using EdgIQ.MurimiOS.Domain.AnalyticsRetentionPolicy;
using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;

using EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Analytics;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using AnalyticsDimensionSnapshot = global::EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot.AnalyticsDimensionSnapshot;
using AnalyticsDimensionType = global::EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot.AnalyticsDimensionType;
using AnalyticsForecastEvaluation = global::EdgIQ.MurimiOS.Domain.AnalyticsForecastEvaluation.AnalyticsForecastEvaluation;
using AnalyticsFact = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFact;
using AnalyticsFactKind = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFactKind;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;
using AnalyticsProjectionStatus = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionStatus;
using AnalyticsValueKind = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsValueKind;
using AnalyticsProjectionSchedule = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule.AnalyticsProjectionSchedule;
using AnalyticsScheduleFrequency = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule.AnalyticsScheduleFrequency;
using AnalyticsRetentionPolicy = global::EdgIQ.MurimiOS.Domain.AnalyticsRetentionPolicy.AnalyticsRetentionPolicy;
using AnalyticsGovernanceStatus = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.AnalyticsGovernanceStatus;
using BenchmarkCohortDefinition = global::EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition.BenchmarkCohortDefinition;
using BenchmarkEnrollment = global::EdgIQ.MurimiOS.Domain.BenchmarkEnrollment.BenchmarkEnrollment;
using KpiDefinition = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinition;
using KpiDefinitionStatus = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinitionStatus;
using PredictiveModelDefinition = global::EdgIQ.MurimiOS.Domain.PredictiveModelDefinition.PredictiveModelDefinition;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;
using AnalyticsDimensionSnapshotBuilder = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Analytics.AnalyticsDimensionSnapshotBuilder;

public sealed class AnalyticsTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-08-31T08:00:00Z");

    [Fact]
    public void Kpi_contract_is_versioned_and_lifecycle_controlled()
    {
        var definition = new KpiDefinition(Guid.NewGuid(), Guid.NewGuid(), "crop.yield.kg_per_ha", "Crop yield",
            "Harvested mass per mapped crop-zone hectare", "sum(harvest_kg)/area_ha", "kg/ha", "season",
            "Agronomy", "Non-mass harvest lots", "25 points each for area, harvest, finance and operations", 1);
        definition.Activate(Now); definition.Retire(Now.AddDays(1));
        Assert.Equal(KpiDefinitionStatus.Retired, definition.Status);
        Assert.Throws<InvalidOperationException>(() => definition.Activate(Now));
    }

    [Fact]
    public void Projection_run_requires_due_schedule_and_records_quality()
    {
        var run = new AnalyticsProjectionRun(Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 1, 1),
            new DateOnly(2026, 12, 31), "usd", "v1", Now, "analyst");
        Assert.Throws<InvalidOperationException>(() => run.Start(Now.AddSeconds(-1)));
        run.Start(Now); run.Succeed(Now.AddMinutes(1), 12, 82.5m);
        Assert.Equal(AnalyticsProjectionStatus.Succeeded, run.Status);
        Assert.Equal(82.5m, run.DataQualityPercent);
    }

    [Fact]
    public void Projection_revisions_preserve_history_and_mark_the_previous_run_superseded()
    {
        Guid farmId = Guid.NewGuid();
        var previous = new AnalyticsProjectionRun(Guid.NewGuid(), farmId, new DateOnly(2026, 1, 1),
            new DateOnly(2026, 8, 31), "USD", "v2", Now, "scheduler", 1, "scheduled");
        previous.Start(Now); previous.Succeed(Now.AddMinutes(1), 3, 90);
        var replacement = new AnalyticsProjectionRun(Guid.NewGuid(), farmId, previous.PeriodStart,
            previous.PeriodEnd, "USD", "v2", Now.AddMinutes(2), "event", 2, "event", Guid.NewGuid());

        previous.Supersede(replacement.Id, Now.AddMinutes(3));

        Assert.Equal(2, replacement.Revision);
        Assert.Equal(replacement.Id, previous.SupersededByRunId);
        Assert.NotNull(previous.SupersededAt);
    }

    [Fact]
    public void Fact_preserves_formula_dimensions_quality_and_plan_actual_kind()
    {
        var fact = new AnalyticsFact(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            AnalyticsFactKind.Crop, AnalyticsValueKind.Plan, "crop.yield.kg_per_ha", 8_500, "kg/ha",
            new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), 75, 4, "v1", "crop-zone:1",
            seasonId: Guid.NewGuid(), cropZoneId: Guid.NewGuid(), crop: "Maize", variety: "SC 727");
        Assert.Equal(AnalyticsValueKind.Plan, fact.ValueKind);
        Assert.Equal(75, fact.DataQualityPercent);
        Assert.Equal("Maize", fact.Crop);
    }

    [Fact]
    public void Benchmarking_requires_privacy_threshold_and_supports_revocation()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new BenchmarkEnrollment(Guid.NewGuid(), "crop.yield",
            "maize-zimbabwe", 4, Now, "owner"));
        var enrollment = new BenchmarkEnrollment(Guid.NewGuid(), "crop.yield", "maize-zimbabwe", 5, Now, "owner");
        enrollment.Revoke(Now.AddDays(1), "owner");
        Assert.False(enrollment.IsActive);
    }

    [Fact]
    public void Benchmark_cohort_requires_specialist_approval_before_use()
    {
        var cohort = new BenchmarkCohortDefinition(Guid.NewGuid(), "maize-zimbabwe", 1, "Zimbabwe maize",
            "Comparable rain-fed maize farms", "{\"country\":\"ZW\",\"crop\":\"maize\"}", 8,
            new DateOnly(2026, 1, 1), null, "Agronomy benchmarks board", "analyst");

        Assert.False(cohort.AppliesOn(new DateOnly(2026, 6, 1)));
        cohort.Approve(Now, "benchmark-specialist", "AGR-2026-41");

        Assert.True(cohort.AppliesOn(new DateOnly(2026, 6, 1)));
        Assert.Equal(AnalyticsGovernanceStatus.Active, cohort.Status);
        Assert.Throws<InvalidOperationException>(() => cohort.Approve(Now, "other", "duplicate"));
    }

    [Fact]
    public void Predictive_model_definition_records_validation_and_controlled_lifecycle()
    {
        var model = new PredictiveModelDefinition(Guid.NewGuid(), "crop-yield-baseline", 3,
            "Risk adjusted yield baseline", "crop.yield.kg_per_ha", "risk-adjusted-weighted-baseline-v1",
            "{\"features\":[\"historicalYield\",\"plannedYield\",\"fieldRisk\"]}", 730, 20,
            310m, 7.5m, "validation/2026-q2", "data-science");

        model.Approve(Now, "model-risk-specialist", "MRM-2026-12");
        model.Retire(Now.AddDays(30), "model-owner");

        Assert.Equal(AnalyticsGovernanceStatus.Retired, model.Status);
        Assert.Equal(7.5m, model.ValidationMapePercent);
    }

    [Fact]
    public void Forecast_facts_retain_model_confidence_and_prediction_interval()
    {
        Guid modelId = Guid.NewGuid();
        var fact = new AnalyticsFact(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            AnalyticsFactKind.Crop, AnalyticsValueKind.Forecast, "crop.yield.kg_per_ha", 8_000m, "kg/ha",
            new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), 85, 12, "yield-model:3", "crop-zone:1",
            cropZoneId: Guid.NewGuid(), predictiveModelDefinitionId: modelId, confidencePercent: 92.5m,
            lowerBound: 7_200m, upperBound: 8_800m);

        Assert.Equal(modelId, fact.PredictiveModelDefinitionId);
        Assert.Equal(92.5m, fact.ConfidencePercent);
        Assert.Throws<ArgumentException>(() => new AnalyticsFact(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            AnalyticsFactKind.Crop, AnalyticsValueKind.Actual, "crop.yield.kg_per_ha", 8_000m, "kg/ha",
            new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), 85, 12, "v1", "crop-zone:1",
            confidencePercent: 90));
    }

    [Fact]
    public void Forecast_evaluation_calculates_absolute_percentage_error_and_bias()
    {
        var evaluation = new AnalyticsForecastEvaluation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "crop.yield.kg_per_ha", "crop-zone:1", 8_000m, 10_000m, "kg/ha", 90, "yield-model:3", Now);

        Assert.Equal(2_000m, evaluation.AbsoluteError);
        Assert.Equal(20m, evaluation.AbsolutePercentageError);
        Assert.Equal(-2_000m, evaluation.Bias);
    }

    [Fact]
    public void Dimension_snapshot_is_content_addressed_for_historical_reproducibility()
    {
        var run = new AnalyticsProjectionRun(Guid.NewGuid(), Guid.NewGuid(), new DateOnly(2026, 8, 1),
            new DateOnly(2026, 8, 31), "USD", "v2", Now, "analyst");
        AnalyticsDimensionSnapshot snapshot = new AnalyticsDimensionSnapshotBuilder().Build(run, [])[0];

        Assert.Equal(AnalyticsDimensionType.Farm, snapshot.DimensionType);
        Assert.Equal(64, snapshot.AttributesSha256.Length);
        Assert.Equal(run.PeriodEnd, snapshot.ValidOn);
    }

    [Fact]
    public void Projection_schedule_validates_frequency_and_supports_pause_resume()
    {
        var schedule = new AnalyticsProjectionSchedule(Guid.NewGuid(), Guid.NewGuid(), "Daily farm projection",
            AnalyticsScheduleFrequency.Daily, 30, "usd", "v2", Now, "analyst");

        schedule.Pause();
        Assert.False(schedule.IsActive);
        schedule.Resume(Now.AddDays(1));

        Assert.True(schedule.IsActive);
        Assert.Equal("USD", schedule.Currency);
        Assert.Equal(Now.AddDays(1), schedule.NextRunAt);
        Assert.Throws<ArgumentException>(() => new AnalyticsProjectionSchedule(Guid.NewGuid(), Guid.NewGuid(), "Invalid",
            AnalyticsScheduleFrequency.Daily, 0, "USD", "v1", Now, "analyst"));
    }

    [Fact]
    public void Retention_policy_never_accepts_short_raw_data_windows()
    {
        Assert.Throws<ArgumentException>(() => new AnalyticsRetentionPolicy(Guid.NewGuid(), 29, 90, 90, Now, "owner"));
        var policy = new AnalyticsRetentionPolicy(Guid.NewGuid(), 90, 180, 365, Now, "owner");

        policy.Disable();

        Assert.False(policy.IsActive);
        Assert.Equal(90, policy.DerivedMediaDays);
        Assert.Equal(90, policy.ExpiredMapLayerDays);
        Assert.Throws<InvalidOperationException>(policy.Disable);
    }

    [Fact]
    public async Task Analytical_facts_are_append_only_in_application_persistence()
    {
        Guid tenantId = Guid.NewGuid();
        var options = new DbContextOptionsBuilder<PlatformDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        await using var db = new PlatformDbContext(options, new TestTenantContext(tenantId));
        var fact = new AnalyticsFact(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            AnalyticsFactKind.Farm, AnalyticsValueKind.Actual, "farm.margin", 100, "USD",
            new DateOnly(2026, 1, 1), new DateOnly(2026, 12, 31), 80, 3, "v1", "farm");
        db.AnalyticsFacts.Add(fact); await db.SaveChangesAsync(); db.AnalyticsFacts.Remove(fact);
        await Assert.ThrowsAsync<InvalidOperationException>(() => db.SaveChangesAsync());
    }

    [Fact]
    public void Analytics_permissions_keep_projection_management_out_of_viewer_roles()
    {
        Assert.Contains(PlatformPermissions.AnalyticsManage, PlatformPermissions.ForRole(TenantRole.FarmManager));
        Assert.Contains(PlatformPermissions.AnalyticsBenchmark, PlatformPermissions.ForRole(TenantRole.Auditor));
        Assert.DoesNotContain(PlatformPermissions.AnalyticsManage, PlatformPermissions.ForRole(TenantRole.Viewer));
    }

    private sealed class TestTenantContext(Guid tenantId) : ITenantContext { public Guid TenantId { get; } = tenantId; public bool IsResolved => true; }
}
