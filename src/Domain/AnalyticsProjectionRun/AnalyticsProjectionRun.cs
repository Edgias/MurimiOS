using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using KpiDefinition = global::EdgIQ.MurimiOS.Domain.KpiDefinition.KpiDefinition;

public sealed class AnalyticsProjectionRun : TenantEntity
{
    private AnalyticsProjectionRun() { }
    public AnalyticsProjectionRun(Guid id, Guid farmId, DateOnly periodStart, DateOnly periodEnd, string currency,
        string projectionVersion, DateTimeOffset scheduledFor, string requestedBy, int revision = 1,
        string triggerType = "manual", Guid? sourceRefreshRequestId = null) : base(id)
    {
        if (farmId == Guid.Empty || periodEnd < periodStart || scheduledFor == default || revision <= 0
            || sourceRefreshRequestId == Guid.Empty) throw new ArgumentException("A farm, valid period, revision, and schedule are required.");
        string normalizedCurrency = currency?.Trim().ToUpperInvariant() ?? string.Empty;
        if (normalizedCurrency.Length != 3 || !normalizedCurrency.All(char.IsLetter)) throw new ArgumentException("A three-letter currency is required.");
        FarmId = farmId; PeriodStart = periodStart; PeriodEnd = periodEnd; Currency = normalizedCurrency;
        ProjectionVersion = KpiDefinition.Text(projectionVersion, 60); ScheduledFor = scheduledFor.ToUniversalTime(); RequestedBy = KpiDefinition.Text(requestedBy, 240);
        Revision = revision; TriggerType = KpiDefinition.Text(triggerType, 80).ToLowerInvariant(); SourceRefreshRequestId = sourceRefreshRequestId;
        Status = AnalyticsProjectionStatus.Queued;
    }
    public Guid FarmId { get; private set; }
    public DateOnly PeriodStart { get; private set; }
    public DateOnly PeriodEnd { get; private set; }
    public string Currency { get; private set; } = string.Empty;
    public string ProjectionVersion { get; private set; } = string.Empty;
    public DateTimeOffset ScheduledFor { get; private set; }
    public string RequestedBy { get; private set; } = string.Empty;
    public int Revision { get; private set; }
    public string TriggerType { get; private set; } = "manual";
    public Guid? SourceRefreshRequestId { get; private set; }
    public AnalyticsProjectionStatus Status { get; private set; }
    public DateTimeOffset? StartedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public int FactCount { get; private set; }
    public decimal? DataQualityPercent { get; private set; }
    public string? ErrorCode { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTimeOffset? SupersededAt { get; private set; }
    public Guid? SupersededByRunId { get; private set; }
    public void Start(DateTimeOffset at) { if (Status != AnalyticsProjectionStatus.Queued || at < ScheduledFor) throw new InvalidOperationException("Only a due queued projection can start."); Status = AnalyticsProjectionStatus.Running; StartedAt = at.ToUniversalTime(); }
    public void Succeed(DateTimeOffset at, int factCount, decimal dataQualityPercent) { if (Status != AnalyticsProjectionStatus.Running || factCount < 0 || dataQualityPercent is < 0 or > 100) throw new InvalidOperationException("Only a running projection can succeed with valid totals."); Status = AnalyticsProjectionStatus.Succeeded; CompletedAt = at.ToUniversalTime(); FactCount = factCount; DataQualityPercent = dataQualityPercent; }
    public void Fail(DateTimeOffset at, string code, string message) { if (Status != AnalyticsProjectionStatus.Running) throw new InvalidOperationException("Only a running projection can fail."); Status = AnalyticsProjectionStatus.Failed; CompletedAt = at.ToUniversalTime(); ErrorCode = KpiDefinition.Text(code, 80); ErrorMessage = KpiDefinition.Text(message, 2_000); }
    public void Cancel(DateTimeOffset at) { if (Status != AnalyticsProjectionStatus.Queued) throw new InvalidOperationException("Only a queued projection can be cancelled."); Status = AnalyticsProjectionStatus.Cancelled; CompletedAt = at.ToUniversalTime(); }
    public void Supersede(Guid replacementRunId, DateTimeOffset at) { if (Status != AnalyticsProjectionStatus.Succeeded || SupersededAt is not null || replacementRunId == Guid.Empty || replacementRunId == Id) throw new InvalidOperationException("Only a current succeeded projection can be superseded."); SupersededByRunId = replacementRunId; SupersededAt = at.ToUniversalTime(); }
}
