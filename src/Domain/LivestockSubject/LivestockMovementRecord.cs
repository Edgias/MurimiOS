using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.LivestockSubject;

public sealed class LivestockMovementRecord : TenantEntity
{
    private LivestockMovementRecord() { }
    public LivestockMovementRecord(Guid id, Guid subjectId, LivestockMovementType type,
        Guid? fromHoldingId, Guid? toHoldingId, DateTimeOffset occurredAt, string? reference,
        Guid? sourceCommandId, string? notes) : base(id)
    {
        if (subjectId == Guid.Empty) throw new ArgumentException("A livestock subject is required.", nameof(subjectId));
        if (!Enum.IsDefined(type)) throw new ArgumentOutOfRangeException(nameof(type));
        if (fromHoldingId == Guid.Empty || toHoldingId == Guid.Empty || sourceCommandId == Guid.Empty)
            throw new ArgumentException("Optional identifiers must be non-empty when supplied.");
        if (type is LivestockMovementType.Birth or LivestockMovementType.Purchase && toHoldingId is null)
            throw new ArgumentException("Birth and purchase records require a destination holding.");
        if (type == LivestockMovementType.Transfer && (fromHoldingId is null || toHoldingId is null || fromHoldingId == toHoldingId))
            throw new ArgumentException("A transfer requires distinct origin and destination holdings.");
        if (type is LivestockMovementType.Sale or LivestockMovementType.Death && fromHoldingId is null)
            throw new ArgumentException("Sale and death records require an origin holding.");
        SubjectId = subjectId; Type = type; FromHoldingId = fromHoldingId; ToHoldingId = toHoldingId;
        OccurredAt = occurredAt; Reference = Optional(reference, 160); SourceCommandId = sourceCommandId; Notes = Optional(notes, 2_000);
    }
    public Guid SubjectId { get; private set; }
    public LivestockMovementType Type { get; private set; }
    public Guid? FromHoldingId { get; private set; }
    public Guid? ToHoldingId { get; private set; }
    public DateTimeOffset OccurredAt { get; private set; }
    public string? Reference { get; private set; }
    public Guid? SourceCommandId { get; private set; }
    public string? Notes { get; private set; }
    private static string? Optional(string? value, int maximum) { string? result = value?.Trim(); if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters."); return string.IsNullOrEmpty(result) ? null : result; }
}
