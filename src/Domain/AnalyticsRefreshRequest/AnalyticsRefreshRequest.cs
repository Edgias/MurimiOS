using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.AnalyticsRefreshRequest;

/// <summary>Database-triggered queue row. Mutations are performed only by analytics maintenance functions.</summary>
public sealed class AnalyticsRefreshRequest : TenantEntity
{
    private AnalyticsRefreshRequest() { }
    public Guid FarmId { get; private set; }
    public Guid SourceOutboxMessageId { get; private set; }
    public string TriggerType { get; private set; } = string.Empty;
    public DateTimeOffset FirstTriggeredAt { get; private set; }
    public DateTimeOffset LastTriggeredAt { get; private set; }
    public DateTimeOffset AvailableAt { get; private set; }
    public AnalyticsRefreshRequestStatus Status { get; private set; }
    public Guid? ScheduledRunId { get; private set; }
    public DateTimeOffset? ProcessedAt { get; private set; }
    public int EventCount { get; private set; }
    public string? LastError { get; private set; }
}
