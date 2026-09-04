using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Domain.FarmField;

public sealed class FieldBoundaryVersion : TenantEntity
{
    private FieldBoundaryVersion()
    {
    }

    public FieldBoundaryVersion(
        Guid id,
        Guid fieldId,
        MultiPolygon boundary,
        DateOnly validFrom,
        string source) : base(id)
    {
        if (fieldId == Guid.Empty)
        {
            throw new ArgumentException("A field is required.", nameof(fieldId));
        }

        if (boundary is null || boundary.IsEmpty || !boundary.IsValid)
        {
            throw new ArgumentException("A valid, non-empty field boundary is required.", nameof(boundary));
        }

        FieldId = fieldId;
        Boundary = boundary;
        Boundary.SRID = 4326;
        ValidFrom = validFrom;
        Source = string.IsNullOrWhiteSpace(source) ? "drawn" : source.Trim().ToLowerInvariant();
    }

    public Guid FieldId { get; private set; }

    public MultiPolygon Boundary { get; private set; } = default!;

    public decimal AreaHa { get; private set; }

    public DateOnly ValidFrom { get; private set; }

    public DateOnly? ValidTo { get; private set; }

    public string Source { get; private set; } = string.Empty;

    public void Close(DateOnly validTo)
    {
        if (validTo < ValidFrom)
        {
            throw new ArgumentOutOfRangeException(nameof(validTo));
        }

        ValidTo = validTo;
    }
}
