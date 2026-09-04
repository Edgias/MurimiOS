using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule;
using KpiDefinition = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinition;

public sealed class AnalyticsProjectionSchedule : TenantEntity
{
    private AnalyticsProjectionSchedule() { }
    public AnalyticsProjectionSchedule(Guid id, Guid farmId, string name, AnalyticsScheduleFrequency frequency,
        int lookbackDays, string currency, string projectionVersion, DateTimeOffset nextRunAt, string createdBy) : base(id)
    {
        if (farmId == Guid.Empty || !Enum.IsDefined(frequency) || lookbackDays is < 1 or > 366 || nextRunAt == default)
            throw new ArgumentException("A farm, valid frequency, lookback, and next run are required.");
        string normalizedCurrency = currency?.Trim().ToUpperInvariant() ?? string.Empty;
        if (normalizedCurrency.Length != 3 || !normalizedCurrency.All(char.IsLetter)) throw new ArgumentException("A three-letter currency is required.");
        FarmId = farmId; Name = KpiDefinition.Text(name, 200); Frequency = frequency; LookbackDays = lookbackDays;
        Currency = normalizedCurrency; ProjectionVersion = KpiDefinition.Text(projectionVersion, 60);
        NextRunAt = nextRunAt.ToUniversalTime(); CreatedBy = KpiDefinition.Text(createdBy, 240); IsActive = true;
    }
    public Guid FarmId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public AnalyticsScheduleFrequency Frequency { get; private set; }
    public int LookbackDays { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public string ProjectionVersion { get; private set; } = string.Empty;
    public DateTimeOffset NextRunAt { get; private set; }
    public DateTimeOffset? LastScheduledAt { get; private set; }
    public string CreatedBy { get; private set; } = string.Empty;
    public bool IsActive { get; private set; }
    public void Pause() { if (!IsActive) throw new InvalidOperationException("Schedule is already paused."); IsActive = false; }
    public void Resume(DateTimeOffset nextRunAt) { if (IsActive || nextRunAt == default) throw new InvalidOperationException("Only a paused schedule can resume with a next run."); IsActive = true; NextRunAt = nextRunAt.ToUniversalTime(); }
}
