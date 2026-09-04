using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Domain.CropZone;

public sealed class CropZone : TenantEntity
{
    private CropZone()
    {
    }

    public CropZone(
        Guid id,
        Guid seasonId,
        Guid fieldId,
        Guid boundaryVersionId,
        string name,
        string crop,
        string? variety,
        MultiPolygon boundary) : base(id)
    {
        if (seasonId == Guid.Empty || fieldId == Guid.Empty || boundaryVersionId == Guid.Empty)
        {
            throw new ArgumentException("Season, field, and boundary version are required.");
        }

        if (boundary is null || boundary.IsEmpty || !boundary.IsValid || boundary.SRID != 4326)
        {
            throw new ArgumentException("A valid WGS84 crop-zone boundary is required.", nameof(boundary));
        }

        SeasonId = seasonId;
        FieldId = fieldId;
        BoundaryVersionId = boundaryVersionId;
        Name = RequireText(name, nameof(name), 160);
        Crop = RequireText(crop, nameof(crop), 120);
        Variety = OptionalText(variety, nameof(variety), 120);
        Boundary = boundary;
    }

    public Guid SeasonId { get; private set; }

    public Guid FieldId { get; private set; }

    public Guid BoundaryVersionId { get; private set; }

    public string Name { get; private set; } = string.Empty;

    public string Crop { get; private set; } = string.Empty;

    public string? Variety { get; private set; }

    public MultiPolygon Boundary { get; private set; } = null!;

    public decimal AreaHa { get; private set; }

    private static string RequireText(string value, string parameter, int maximumLength)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximumLength)
        {
            throw new ArgumentException($"{parameter} must contain between 1 and {maximumLength} characters.", parameter);
        }

        return result;
    }

    private static string? OptionalText(string? value, string parameter, int maximumLength)
    {
        string? result = value?.Trim();
        if (result?.Length > maximumLength)
        {
            throw new ArgumentException($"{parameter} cannot exceed {maximumLength} characters.", parameter);
        }

        return string.IsNullOrEmpty(result) ? null : result;
    }
}
