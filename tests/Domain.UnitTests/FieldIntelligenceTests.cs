using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.WeatherLocation;
using EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;
using EdgIQ.MurimiOS.Domain.FieldRiskRule;
using EdgIQ.MurimiOS.Domain.FieldInsightAlert;
using EdgIQ.MurimiOS.Domain.FieldMapLayer;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Domain.UnitTests;
using FieldInsightAlert = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.FieldInsightAlert;
using InsightAlertStatus = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.InsightAlertStatus;
using FieldRiskAssessment = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.FieldRiskAssessment;
using FieldRiskRule = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.FieldRiskRule;
using RiskAggregation = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskAggregation;
using RiskComparison = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskComparison;
using RiskMetric = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskMetric;
using RiskRuleStatus = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskRuleStatus;
using SatelliteFieldStatistic = global::EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic.SatelliteFieldStatistic;
using VegetationIndex = global::EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic.VegetationIndex;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;
using ScoutingCategory = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingCategory;
using ScoutingObservation = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingObservation;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using TenantRole = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantRole;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;
using WeatherLocation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherLocation;
using WeatherObservation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherObservation;

public sealed class FieldIntelligenceTests
{
    private static readonly DateTimeOffset Now = DateTimeOffset.Parse("2026-08-30T10:00:00Z");

    [Fact]
    public void Weather_location_normalizes_grid_and_validates_coordinates()
    {
        var location = new WeatherLocation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            " Grid:-17.8:31.0 ", -17.81234567m, 31.01234567m, 1_490m, Now);

        Assert.Equal("grid:-17.8:31.0", location.GridKey);
        Assert.Equal(-17.812346m, location.Latitude);
        Assert.Throws<ArgumentOutOfRangeException>(() => new WeatherLocation(Guid.NewGuid(),
            Guid.NewGuid(), null, "invalid", -91, 31, null, Now));
    }

    [Fact]
    public void Weather_observation_requires_a_measurement_and_valid_ranges()
    {
        Assert.Throws<ArgumentException>(() => CreateWeather(null, null, null));
        Assert.Throws<ArgumentOutOfRangeException>(() => CreateWeather(25, 101, 0));

        WeatherObservation row = CreateWeather(25.4m, 64m, 1.2m);

        Assert.Equal(25.4m, row.TemperatureC);
        Assert.Equal(InsightQuality.Valid, row.Quality);
    }

    [Fact]
    public void Satellite_statistics_preserve_boundary_and_quality_metadata()
    {
        var row = new SatelliteFieldStatistic(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), "scene-42", VegetationIndex.Ndvi, null, Now, Now.AddMinutes(5),
            0.62m, 0.2m, 0.88m, 0.11m, 92m, 8m, InsightQuality.Valid);

        Assert.Equal(VegetationIndex.Ndvi, row.Index);
        Assert.Equal(92m, row.ValidPixelPercent);
        Assert.Throws<ArgumentException>(() => new SatelliteFieldStatistic(Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), "bad", VegetationIndex.Ndvi,
            null, Now, Now, 0.1m, 0.2m, 0.9m, 0, 100, 0, InsightQuality.Valid));
    }

    [Fact]
    public void Scouting_observation_validates_wgs84_point_and_captures_boundary_snapshot()
    {
        var point = new Point(31.02, -17.81) { SRID = 4326 };
        var row = new ScoutingObservation(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            null, ScoutingCategory.Pest, InsightSeverity.Warning, "fall-armyworm", "Armyworm signs",
            "Inspect whorls in the north block.", Now, Now.AddMinutes(2), "agronomist", point);

        Assert.Equal(4326, row.Location!.SRID);
        Assert.Equal("fall-armyworm", row.IssueCode);
        Assert.Throws<ArgumentException>(() => new ScoutingObservation(Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), null, ScoutingCategory.Pest, InsightSeverity.Warning, null,
            "Invalid", "Invalid location", Now, Now, "user", new Point(200, -17)));
    }

    [Fact]
    public void Risk_rule_is_versioned_and_lifecycle_is_one_way()
    {
        var rule = CreateRule();

        rule.Activate(Now);
        rule.Retire(Now.AddDays(1));

        Assert.Equal(RiskRuleStatus.Retired, rule.Status);
        Assert.Throws<InvalidOperationException>(() => rule.Activate(Now.AddDays(2)));
        Assert.Throws<ArgumentException>(() => new FieldRiskRule(Guid.NewGuid(), "bad", 0, "Bad", null,
            RiskMetric.NdviMean, RiskAggregation.Latest, RiskComparison.LessThan, null, null, 0.3m, 24, 1,
            InsightSeverity.Warning, "Bad", Now, null, "owner"));
    }

    [Fact]
    public void Risk_assessment_requires_observed_value_when_triggered()
    {
        Assert.Throws<ArgumentException>(() => new FieldRiskAssessment(Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), 1, Now, Now.AddHours(-24), Now,
            null, 0, true, InsightSeverity.Warning, "v1", "{}"));

        var result = new FieldRiskAssessment(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            Guid.NewGuid(), 1, Now, Now.AddHours(-24), Now, 0.22m, 2, true,
            InsightSeverity.Warning, "v1", "{\"samples\":2}");

        Assert.True(result.Triggered);
        Assert.Equal(2, result.SampleCount);
    }

    [Fact]
    public void Alert_supports_acknowledgement_resolution_and_single_follow_up()
    {
        var alert = new FieldInsightAlert(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "risk:ndvi:field", nameof(FieldRiskAssessment), Guid.NewGuid(), InsightSeverity.Warning,
            "Low NDVI", "NDVI is below the configured threshold.", Now, Now.AddHours(-1), null);
        Guid workOrderId = Guid.NewGuid();

        alert.Acknowledge(Now.AddMinutes(1), "manager");
        alert.LinkWorkOrder(workOrderId);
        alert.Resolve(Now.AddHours(1), "manager", "Scouting confirmed crop recovery.");

        Assert.Equal(InsightAlertStatus.Resolved, alert.Status);
        Assert.Equal(workOrderId, alert.FollowUpWorkOrderId);
        Assert.Throws<InvalidOperationException>(() => alert.LinkWorkOrder(Guid.NewGuid()));
        Assert.Throws<InvalidOperationException>(() => alert.Dismiss(Now, "manager", "Too late"));
    }

    [Fact]
    public void Field_intelligence_permissions_preserve_read_write_and_evaluate_boundaries()
    {
        Assert.Contains(PlatformPermissions.FieldIntelligenceEvaluate, PlatformPermissions.ForRole(TenantRole.FarmManager));
        Assert.Contains(PlatformPermissions.FieldIntelligenceWrite, PlatformPermissions.ForRole(TenantRole.Agronomist));
        Assert.Contains(PlatformPermissions.FieldIntelligenceRead, PlatformPermissions.ForRole(TenantRole.Auditor));
        Assert.Contains(PlatformPermissions.FieldIntelligenceWrite, PlatformPermissions.ForRole(TenantRole.Operator));
        Assert.DoesNotContain(PlatformPermissions.FieldIntelligenceEvaluate, PlatformPermissions.ForRole(TenantRole.Operator));
        Assert.DoesNotContain(PlatformPermissions.FieldIntelligenceWrite, PlatformPermissions.ForRole(TenantRole.Viewer));
    }

    private static WeatherObservation CreateWeather(decimal? temperature, decimal? humidity, decimal? rain) =>
        new(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid(),
            "weather-42", Now, Now.AddMinutes(1), temperature, humidity, rain, null, null, null, null, null,
            InsightQuality.Valid);

    private static FieldRiskRule CreateRule() => new(Guid.NewGuid(), "low-ndvi", 1, "Low NDVI", "Maize",
        RiskMetric.NdviMean, RiskAggregation.Latest, RiskComparison.LessThan, null, null, 0.35m, 168, 1,
        InsightSeverity.Warning, "Inspect crop vigour.", Now, null, "agronomist");
}
