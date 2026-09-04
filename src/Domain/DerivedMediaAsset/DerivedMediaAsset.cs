using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.DerivedMediaAsset;

/// <summary>
/// Registers only reproducible, derived objects. Evidence, imports, audit packs and statutory records
/// deliberately use separate aggregates and can never enter this lifecycle queue.
/// </summary>
public sealed class DerivedMediaAsset : TenantEntity
{
    private DerivedMediaAsset() { }

    public DerivedMediaAsset(Guid id, Guid farmId, Guid? fieldId, DerivedMediaKind kind, string sourceType,
        Guid sourceId, string objectKey, string contentType, long byteSize, string sha256,
        DateTimeOffset generatedAt) : base(id)
    {
        if (farmId == Guid.Empty || sourceId == Guid.Empty || fieldId == Guid.Empty || !Enum.IsDefined(kind))
            throw new ArgumentException("Derived-media farm, field, source, or kind is invalid.");
        if (byteSize <= 0) throw new ArgumentOutOfRangeException(nameof(byteSize));
        string normalizedDigest = Text(sha256, nameof(sha256), 64).ToLowerInvariant();
        if (normalizedDigest.Length != 64 || normalizedDigest.Any(c => !Uri.IsHexDigit(c)))
            throw new ArgumentException("A 64-character SHA-256 digest is required.", nameof(sha256));
        FarmId = farmId; FieldId = fieldId; Kind = kind; SourceType = Text(sourceType, nameof(sourceType), 100);
        SourceId = sourceId; ObjectKey = Text(objectKey, nameof(objectKey), 1_024);
        ContentType = Text(contentType, nameof(contentType), 160); ByteSize = byteSize; Sha256 = normalizedDigest;
        GeneratedAt = generatedAt.ToUniversalTime(); Status = DerivedMediaLifecycleStatus.Active;
    }

    public Guid FarmId { get; private set; }
    public Guid? FieldId { get; private set; }
    public DerivedMediaKind Kind { get; private set; }
    public string SourceType { get; private set; } = string.Empty;
    public Guid SourceId { get; private set; }
    public string ObjectKey { get; private set; } = string.Empty;
    public string ContentType { get; private set; } = string.Empty;
    public long ByteSize { get; private set; }
    public string Sha256 { get; private set; } = string.Empty;
    public DateTimeOffset GeneratedAt { get; private set; }
    public DerivedMediaLifecycleStatus Status { get; private set; }
    public bool LegalHold { get; private set; }
    public string? LegalHoldReason { get; private set; }
    public string? LegalHoldChangedBy { get; private set; }
    public DateTimeOffset? LegalHoldChangedAt { get; private set; }
    public DateTimeOffset? DeletedAt { get; private set; }
    public int DeletionAttempts { get; private set; }
    public string? LastDeletionError { get; private set; }
    public Guid? DeletionLeaseId { get; private set; }
    public DateTimeOffset? DeletionLeasedUntil { get; private set; }

    public void PlaceLegalHold(string reason, string actor, DateTimeOffset at)
    {
        if (Status == DerivedMediaLifecycleStatus.Deleted)
            throw new InvalidOperationException("A deleted derived-media object cannot be placed on legal hold.");
        if (DeletionLeaseId.HasValue)
            throw new InvalidOperationException("A derived-media deletion already in progress cannot be placed on legal hold.");
        LegalHold = true; LegalHoldReason = Text(reason, nameof(reason), 1_000);
        LegalHoldChangedBy = Text(actor, nameof(actor), 240); LegalHoldChangedAt = at.ToUniversalTime();
        if (Status == DerivedMediaLifecycleStatus.PendingDeletion)
        {
            Status = DerivedMediaLifecycleStatus.Active;
            DeletionLeasedUntil = null;
        }
    }

    public void ReleaseLegalHold(string actor, DateTimeOffset at)
    {
        if (!LegalHold) throw new InvalidOperationException("The derived-media object is not on legal hold.");
        LegalHold = false; LegalHoldReason = null; LegalHoldChangedBy = Text(actor, nameof(actor), 240);
        LegalHoldChangedAt = at.ToUniversalTime();
    }

    private static string Text(string? value, string parameter, int maximum)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length == 0 || result.Length > maximum)
            throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter);
        return result;
    }
}
