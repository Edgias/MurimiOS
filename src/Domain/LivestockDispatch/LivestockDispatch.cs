using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.LivestockDispatch;

public enum LivestockDispatchPurpose
{
    Movement = 1,
    Slaughter = 2,
    MilkCollection = 3,
    Sale = 4
}

public sealed class LivestockDispatch : TenantEntity
{
    private LivestockDispatch() { }

    public LivestockDispatch(
        Guid id,
        Guid subjectId,
        LivestockDispatchPurpose purpose,
        DateTimeOffset dispatchedAt,
        string destination,
        DateTimeOffset? activeHoldUntil,
        string? overrideReason,
        string? overrideAuthorizedBy) : base(id)
    {
        if (subjectId == Guid.Empty) throw new ArgumentException("A livestock subject is required.", nameof(subjectId));
        if (!Enum.IsDefined(purpose)) throw new ArgumentOutOfRangeException(nameof(purpose));
        if (activeHoldUntil > dispatchedAt && string.IsNullOrWhiteSpace(overrideReason))
            throw new InvalidOperationException("Dispatch is blocked by an active withdrawal hold.");
        if (!string.IsNullOrWhiteSpace(overrideReason) && string.IsNullOrWhiteSpace(overrideAuthorizedBy))
            throw new InvalidOperationException("A withdrawal override must record its authorizing identity.");

        SubjectId = subjectId;
        Purpose = purpose;
        DispatchedAt = dispatchedAt;
        Destination = RequireText(destination, nameof(destination), 240);
        WithdrawalHoldUntil = activeHoldUntil;
        OverrideReason = OptionalText(overrideReason, 1_000);
        OverrideAuthorizedBy = OptionalText(overrideAuthorizedBy, 200);
    }

    public Guid SubjectId { get; private set; }
    public LivestockDispatchPurpose Purpose { get; private set; }
    public DateTimeOffset DispatchedAt { get; private set; }
    public string Destination { get; private set; } = string.Empty;
    public DateTimeOffset? WithdrawalHoldUntil { get; private set; }
    public string? OverrideReason { get; private set; }
    public string? OverrideAuthorizedBy { get; private set; }

    private static string RequireText(string value, string parameter, int maximumLength)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximumLength)
            throw new ArgumentException($"{parameter} must contain between 1 and {maximumLength} characters.", parameter);
        return result;
    }

    private static string? OptionalText(string? value, int maximumLength)
    {
        string? result = value?.Trim();
        if (result?.Length > maximumLength) throw new ArgumentException($"Value cannot exceed {maximumLength} characters.", nameof(value));
        return string.IsNullOrEmpty(result) ? null : result;
    }
}
