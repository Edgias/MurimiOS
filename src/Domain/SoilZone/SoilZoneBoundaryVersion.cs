using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;

namespace EdgIQ.MurimiOS.Domain.SoilZone;

public sealed class SoilZoneBoundaryVersion : TenantEntity
{
    private SoilZoneBoundaryVersion() { }
    public SoilZoneBoundaryVersion(Guid id, Guid soilZoneId, Guid fieldBoundaryVersionId, MultiPolygon boundary,
        DateOnly validFrom, string source) : base(id)
    {
        if (soilZoneId == Guid.Empty || fieldBoundaryVersionId == Guid.Empty || boundary is null || boundary.IsEmpty || !boundary.IsValid)
            throw new ArgumentException("Soil zone, field boundary, and valid geometry are required.");
        SoilZoneId = soilZoneId; FieldBoundaryVersionId = fieldBoundaryVersionId; Boundary = boundary; Boundary.SRID = 4326;
        ValidFrom = validFrom; Source = DigitalTwinText.Required(source, 80).ToLowerInvariant();
    }
    public Guid SoilZoneId { get; private set; }
    public Guid FieldBoundaryVersionId { get; private set; }
    public MultiPolygon Boundary { get; private set; } = default!;
    public decimal AreaHa { get; private set; }
    public DateOnly ValidFrom { get; private set; }
    public DateOnly? ValidTo { get; private set; }
    public string Source { get; private set; } = "";
    public void Close(DateOnly validTo)
    {
        if (ValidTo.HasValue || validTo < ValidFrom) throw new InvalidOperationException("The soil-zone boundary cannot be closed on that date.");
        ValidTo = validTo;
    }
}
