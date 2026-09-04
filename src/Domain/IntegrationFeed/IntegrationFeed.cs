using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.IntegrationFeed;

public sealed class IntegrationFeed : TenantEntity
{
    private IntegrationFeed() { }
    public IntegrationFeed(Guid id, Guid connectionId, Guid farmId, IntegrationFeedType feedType,
        IntegrationTargetType targetType, Guid? targetId, string name, string externalSourceId,
        int pollingIntervalMinutes, int freshnessThresholdMinutes, string configurationJson,
        DateTimeOffset nextPollAt) : base(id)
    {
        if (connectionId == Guid.Empty || farmId == Guid.Empty || targetId == Guid.Empty) throw new ArgumentException("Connection and farm are required and an optional target id must be non-empty.");
        if (!Enum.IsDefined(feedType) || !Enum.IsDefined(targetType)) throw new ArgumentOutOfRangeException(nameof(feedType));
        if (targetType != IntegrationTargetType.Farm && targetId is null) throw new ArgumentException("A non-farm feed requires a target id.", nameof(targetId));
        if (pollingIntervalMinutes is < 1 or > 43_200 || freshnessThresholdMinutes is < 1 or > 129_600) throw new ArgumentOutOfRangeException(nameof(pollingIntervalMinutes));
        ConnectionId = connectionId; FarmId = farmId; FeedType = feedType; TargetType = targetType; TargetId = targetId;
        Name = Text(name, nameof(name), 200); ExternalSourceId = Text(externalSourceId, nameof(externalSourceId), 240);
        PollingIntervalMinutes = pollingIntervalMinutes; FreshnessThresholdMinutes = freshnessThresholdMinutes;
        ConfigurationJson = Json(configurationJson, nameof(configurationJson), 32_000); NextPollAt = nextPollAt.ToUniversalTime();
        Status = IntegrationFeedStatus.Active;
    }
    public Guid ConnectionId { get; private set; }
    public Guid FarmId { get; private set; }
    public IntegrationFeedType FeedType { get; private set; }
    public IntegrationTargetType TargetType { get; private set; }
    public Guid? TargetId { get; private set; }
    public string Name { get; private set; } = string.Empty;
    public string ExternalSourceId { get; private set; } = string.Empty;
    public int PollingIntervalMinutes { get; private set; }
    public int FreshnessThresholdMinutes { get; private set; }
    public string ConfigurationJson { get; private set; } = "{}";
    public IntegrationFeedStatus Status { get; private set; }
    public string? CheckpointJson { get; private set; }
    public DateTimeOffset NextPollAt { get; private set; }
    public DateTimeOffset? LastObservedAt { get; private set; }
    public DateTimeOffset? LastReceivedAt { get; private set; }
    public DateTimeOffset? LastSucceededAt { get; private set; }
    public DateTimeOffset? LastFailedAt { get; private set; }
    public string? LastErrorCode { get; private set; }
    public string? LastErrorMessage { get; private set; }
    public void Pause() { if (Status != IntegrationFeedStatus.Active) throw new InvalidOperationException("Only an active feed can be paused."); Status = IntegrationFeedStatus.Paused; }
    public void Resume(DateTimeOffset nextPollAt) { if (Status != IntegrationFeedStatus.Paused) throw new InvalidOperationException("Only a paused feed can be resumed."); Status = IntegrationFeedStatus.Active; NextPollAt = nextPollAt.ToUniversalTime(); }
    public void Disable() { if (Status == IntegrationFeedStatus.Disabled) throw new InvalidOperationException("The feed is already disabled."); Status = IntegrationFeedStatus.Disabled; }
    public void RecordSuccess(DateTimeOffset at, DateTimeOffset? latestObservedAt, string? checkpointJson)
    {
        LastSucceededAt = at.ToUniversalTime(); LastReceivedAt = at.ToUniversalTime();
        if (latestObservedAt is { } observed && (LastObservedAt is null || observed > LastObservedAt)) LastObservedAt = observed.ToUniversalTime();
        CheckpointJson = OptionalJson(checkpointJson, 32_000); LastErrorCode = null; LastErrorMessage = null;
        NextPollAt = at.ToUniversalTime().AddMinutes(PollingIntervalMinutes);
    }
    public void RecordFailure(DateTimeOffset at, string code, string message, DateTimeOffset retryAt)
    {
        LastFailedAt = at.ToUniversalTime(); LastErrorCode = Text(code, nameof(code), 80); LastErrorMessage = Text(message, nameof(message), 2_000); NextPollAt = retryAt.ToUniversalTime();
    }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string Json(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string? OptionalJson(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"JSON cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}
