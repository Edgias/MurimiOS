using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.IngestionJob;

public sealed class IngestionJob : TenantEntity
{
    private IngestionJob() { }
    public IngestionJob(Guid id, Guid feedId, Guid farmId, IngestionJobKind kind, string idempotencyKey,
        DateTimeOffset requestedAt, string requestedBy, DateTimeOffset availableAt, int maximumAttempts,
        DateTimeOffset? windowStart = null, DateTimeOffset? windowEnd = null, Guid? previousJobId = null) : base(id)
    {
        if (feedId == Guid.Empty || farmId == Guid.Empty || previousJobId == Guid.Empty) throw new ArgumentException("Feed and farm are required and an optional previous job id must be non-empty.");
        if (!Enum.IsDefined(kind)) throw new ArgumentOutOfRangeException(nameof(kind));
        if (maximumAttempts is < 1 or > 20 || windowEnd <= windowStart) throw new ArgumentException("Maximum attempts must be 1-20 and a window end must follow its start.");
        FeedId = feedId; FarmId = farmId; Kind = kind; IdempotencyKey = Text(idempotencyKey, nameof(idempotencyKey), 160);
        RequestedAt = requestedAt.ToUniversalTime(); RequestedBy = Text(requestedBy, nameof(requestedBy), 240);
        AvailableAt = availableAt.ToUniversalTime(); MaximumAttempts = maximumAttempts;
        WindowStart = windowStart?.ToUniversalTime(); WindowEnd = windowEnd?.ToUniversalTime(); PreviousJobId = previousJobId;
        Status = IngestionJobStatus.Queued;
    }
    public Guid FeedId { get; private set; }
    public Guid FarmId { get; private set; }
    public IngestionJobKind Kind { get; private set; }
    public string IdempotencyKey { get; private set; } = string.Empty;
    public IngestionJobStatus Status { get; private set; }
    public DateTimeOffset RequestedAt { get; private set; }
    public string RequestedBy { get; private set; } = string.Empty;
    public DateTimeOffset AvailableAt { get; private set; }
    public int Attempts { get; private set; }
    public int MaximumAttempts { get; private set; }
    public Guid? LeaseId { get; private set; }
    public DateTimeOffset? LeasedUntil { get; private set; }
    public DateTimeOffset? LastClaimedAt { get; private set; }
    public DateTimeOffset? CompletedAt { get; private set; }
    public DateTimeOffset? WindowStart { get; private set; }
    public DateTimeOffset? WindowEnd { get; private set; }
    public Guid? PreviousJobId { get; private set; }
    public string? ResultCheckpointJson { get; private set; }
    public string? LastErrorCode { get; private set; }
    public string? LastErrorMessage { get; private set; }
    public bool IsClaimable(DateTimeOffset now) =>
        (Status is IngestionJobStatus.Queued or IngestionJobStatus.RetryScheduled) && AvailableAt <= now
        || Status == IngestionJobStatus.Running && LeasedUntil <= now;
    public void Claim(Guid leaseId, DateTimeOffset at, TimeSpan duration)
    {
        if (leaseId == Guid.Empty || duration <= TimeSpan.Zero) throw new ArgumentException("A lease id and positive duration are required.");
        if (!IsClaimable(at)) throw new InvalidOperationException("The ingestion job is not claimable.");
        Status = IngestionJobStatus.Running; LeaseId = leaseId; LastClaimedAt = at.ToUniversalTime(); LeasedUntil = LastClaimedAt.Value.Add(duration); Attempts++;
    }
    public void Succeed(Guid leaseId, DateTimeOffset at, string? checkpointJson)
    {
        EnsureLease(leaseId, at); Status = IngestionJobStatus.Succeeded; CompletedAt = at.ToUniversalTime();
        ResultCheckpointJson = Optional(checkpointJson, 32_000); LastErrorCode = null; LastErrorMessage = null; LeaseId = null; LeasedUntil = null;
    }
    public void Fail(Guid leaseId, DateTimeOffset at, DateTimeOffset retryAt, string code, string message)
    {
        EnsureLease(leaseId, at); LastErrorCode = Text(code, nameof(code), 80); LastErrorMessage = Text(message, nameof(message), 2_000);
        LeaseId = null; LeasedUntil = null;
        if (Attempts >= MaximumAttempts) { Status = IngestionJobStatus.DeadLettered; CompletedAt = at.ToUniversalTime(); }
        else { Status = IngestionJobStatus.RetryScheduled; AvailableAt = retryAt.ToUniversalTime(); }
    }
    public void FailPermanently(Guid leaseId, DateTimeOffset at, string code, string message)
    {
        EnsureLease(leaseId, at); LastErrorCode = Text(code, nameof(code), 80); LastErrorMessage = Text(message, nameof(message), 2_000);
        Status = IngestionJobStatus.DeadLettered; CompletedAt = at.ToUniversalTime(); LeaseId = null; LeasedUntil = null;
    }
    public void Cancel(DateTimeOffset at)
    {
        if (Status is IngestionJobStatus.Succeeded or IngestionJobStatus.DeadLettered or IngestionJobStatus.Cancelled) throw new InvalidOperationException("A terminal ingestion job cannot be cancelled.");
        Status = IngestionJobStatus.Cancelled; CompletedAt = at.ToUniversalTime(); LeaseId = null; LeasedUntil = null;
    }
    private void EnsureLease(Guid leaseId, DateTimeOffset at)
    {
        if (Status != IngestionJobStatus.Running || LeaseId != leaseId || LeasedUntil < at) throw new InvalidOperationException("The ingestion-job lease is missing, mismatched, or expired.");
    }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}
