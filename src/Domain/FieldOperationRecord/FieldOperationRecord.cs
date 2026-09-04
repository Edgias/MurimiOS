using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.FieldOperationRecord;

public sealed class FieldOperationRecord : TenantEntity
{
    private FieldOperationRecord()
    {
    }

    public FieldOperationRecord(
        Guid id,
        Guid fieldId,
        Guid boundaryVersionId,
        string operationType,
        DateTimeOffset occurredAt,
        DateTimeOffset recordedAt,
        Guid sourceCommandId,
        string? notes,
        Guid? cropZoneId = null) : base(id)
    {
        if (fieldId == Guid.Empty || boundaryVersionId == Guid.Empty || sourceCommandId == Guid.Empty)
        {
            throw new ArgumentException("Field, boundary version, and source command are required.");
        }

        if (cropZoneId == Guid.Empty)
        {
            throw new ArgumentException("A crop-zone id must be non-empty when supplied.", nameof(cropZoneId));
        }

        string normalizedType = operationType?.Trim() ?? string.Empty;
        if (normalizedType.Length is 0 or > 80)
        {
            throw new ArgumentException("An operation type between 1 and 80 characters is required.", nameof(operationType));
        }

        FieldId = fieldId;
        BoundaryVersionId = boundaryVersionId;
        OperationType = normalizedType;
        OccurredAt = occurredAt.ToUniversalTime();
        RecordedAt = recordedAt.ToUniversalTime();
        SourceCommandId = sourceCommandId;
        Notes = notes?.Trim();
        CropZoneId = cropZoneId;
    }

    public Guid FieldId { get; private set; }

    public Guid BoundaryVersionId { get; private set; }

    public Guid? CropZoneId { get; private set; }

    public string OperationType { get; private set; } = string.Empty;

    public DateTimeOffset OccurredAt { get; private set; }

    public DateTimeOffset RecordedAt { get; private set; }

    public Guid SourceCommandId { get; private set; }

    public string? Notes { get; private set; }

    public Guid? SupersededById { get; private set; }

    public string? CorrectionReason { get; private set; }

    public void Supersede(Guid replacementId, string reason)
    {
        if (replacementId == Guid.Empty || replacementId == Id)
            throw new ArgumentException("A distinct replacement operation is required.", nameof(replacementId));
        if (SupersededById is not null)
            throw new InvalidOperationException("This field operation has already been superseded.");
        string normalizedReason = reason?.Trim() ?? string.Empty;
        if (normalizedReason.Length is 0 or > 2_000)
            throw new ArgumentException("A correction reason between 1 and 2000 characters is required.", nameof(reason));
        SupersededById = replacementId;
        CorrectionReason = normalizedReason;
    }
}
