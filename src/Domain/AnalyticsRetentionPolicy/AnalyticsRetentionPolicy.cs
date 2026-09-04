using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.AnalyticsRetentionPolicy;
using KpiDefinition = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinition;

public sealed class AnalyticsRetentionPolicy : TenantEntity
{
    private AnalyticsRetentionPolicy() { }
    public AnalyticsRetentionPolicy(Guid id, int rawSensorDays, int rawTelemetryDays, int failedProjectionDays,
        DateTimeOffset nextRunAt, string configuredBy, int derivedMediaDays = 90, int expiredMapLayerDays = 90) : base(id)
    {
        if (rawSensorDays is < 30 or > 3650 || rawTelemetryDays is < 30 or > 3650
            || failedProjectionDays is < 30 or > 3650 || derivedMediaDays is < 30 or > 3650
            || expiredMapLayerDays is < 30 or > 3650 || nextRunAt == default)
            throw new ArgumentException("Retention periods must be between 30 and 3650 days and require a next run.");
        RawSensorDays = rawSensorDays; RawTelemetryDays = rawTelemetryDays; FailedProjectionDays = failedProjectionDays;
        DerivedMediaDays = derivedMediaDays; ExpiredMapLayerDays = expiredMapLayerDays;
        NextRunAt = nextRunAt.ToUniversalTime(); ConfiguredBy = KpiDefinition.Text(configuredBy, 240); IsActive = true;
    }
    public int RawSensorDays { get; private set; }
    public int RawTelemetryDays { get; private set; }
    public int FailedProjectionDays { get; private set; }
    public int DerivedMediaDays { get; private set; }
    public int ExpiredMapLayerDays { get; private set; }
    public DateTimeOffset NextRunAt { get; private set; }
    public DateTimeOffset? LastRunAt { get; private set; }
    public string ConfiguredBy { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public void Disable() { if (!IsActive) throw new InvalidOperationException("Retention policy is already disabled."); IsActive = false; }
}
