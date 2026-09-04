namespace EdgIQ.MurimiOS.Domain.OutboxMessage;

public sealed class OutboxMessage
{
    private OutboxMessage()
    {
    }

    public OutboxMessage(
        Guid id,
        Guid tenantId,
        string type,
        string payload,
        DateTimeOffset occurredAt)
    {
        if (id == Guid.Empty || tenantId == Guid.Empty) throw new ArgumentException("Message and tenant identifiers are required.");
        if (string.IsNullOrWhiteSpace(type) || type.Length > 200) throw new ArgumentException("A message type is required.", nameof(type));
        if (string.IsNullOrWhiteSpace(payload)) throw new ArgumentException("A message payload is required.", nameof(payload));
        Id = id;
        TenantId = tenantId;
        Type = type.Trim();
        Payload = payload;
        OccurredAt = occurredAt.ToUniversalTime();
        AvailableAt = OccurredAt;
    }

    public Guid Id { get; private set; }

    public Guid TenantId { get; private set; }

    public string Type { get; private set; } = string.Empty;

    public string Payload { get; private set; } = string.Empty;

    public DateTimeOffset OccurredAt { get; private set; }

    public DateTimeOffset? ProcessedAt { get; private set; }

    public DateTimeOffset AvailableAt { get; private set; }

    public Guid? LockId { get; private set; }

    public DateTimeOffset? LockedUntil { get; private set; }

    public DateTimeOffset? DeadLetteredAt { get; private set; }

    public int Attempts { get; private set; }

    public string? LastError { get; private set; }

    public bool IsClaimable(DateTimeOffset now) =>
        ProcessedAt is null
        && DeadLetteredAt is null
        && AvailableAt <= now
        && (LockedUntil is null || LockedUntil <= now);

    public void Claim(Guid lockId, DateTimeOffset now, TimeSpan lockDuration)
    {
        if (lockId == Guid.Empty) throw new ArgumentException("A lock id is required.", nameof(lockId));
        if (lockDuration <= TimeSpan.Zero) throw new ArgumentOutOfRangeException(nameof(lockDuration));
        if (!IsClaimable(now)) throw new InvalidOperationException("The outbox message is not claimable.");
        LockId = lockId;
        LockedUntil = now.ToUniversalTime().Add(lockDuration);
        Attempts++;
    }

    public void MarkProcessed(Guid lockId, DateTimeOffset processedAt)
    {
        EnsureLock(lockId);
        ProcessedAt = processedAt.ToUniversalTime();
        LastError = null;
        LockId = null;
        LockedUntil = null;
    }

    public void MarkFailed(
        Guid lockId,
        string error,
        DateTimeOffset failedAt,
        DateTimeOffset retryAt,
        int maximumAttempts)
    {
        EnsureLock(lockId);
        if (maximumAttempts <= 0) throw new ArgumentOutOfRangeException(nameof(maximumAttempts));
        string normalized = string.IsNullOrWhiteSpace(error) ? "Unknown publishing failure." : error.Trim();
        LastError = normalized.Length <= 2_000 ? normalized : normalized[..2_000];
        LockId = null;
        LockedUntil = null;
        if (Attempts >= maximumAttempts)
        {
            DeadLetteredAt = failedAt.ToUniversalTime();
        }
        else
        {
            AvailableAt = retryAt.ToUniversalTime();
        }
    }

    private void EnsureLock(Guid lockId)
    {
        if (lockId == Guid.Empty || LockId != lockId)
            throw new InvalidOperationException("The caller does not own this outbox message lock.");
    }
}
