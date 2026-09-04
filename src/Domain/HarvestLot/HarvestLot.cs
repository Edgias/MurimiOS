using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.HarvestLot;

public sealed class HarvestLot : TenantEntity
{
    private HarvestLot()
    {
    }

    public HarvestLot(
        Guid id,
        Guid cropZoneId,
        string lotCode,
        decimal quantity,
        string unit,
        DateTimeOffset harvestedAt,
        Guid sourceCommandId,
        string? notes) : base(id)
    {
        if (cropZoneId == Guid.Empty || sourceCommandId == Guid.Empty)
        {
            throw new ArgumentException("Crop zone and source command are required.");
        }

        if (quantity <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(quantity), "Harvest quantity must be positive.");
        }

        CropZoneId = cropZoneId;
        LotCode = RequireText(lotCode, nameof(lotCode), 120);
        MeasuredQuantity measured = MeasurementUnits.Capture(quantity, unit, unit);
        if (measured.Dimension is not (UnitDimension.Mass or UnitDimension.Volume or UnitDimension.Count))
            throw new ArgumentException("Harvest quantities must use a mass, volume, or count unit.", nameof(unit));
        Quantity = measured.CanonicalValue;
        Unit = measured.CanonicalUnit;
        EnteredQuantity = measured.EnteredValue;
        EnteredUnit = measured.EnteredUnit;
        DisplayUnit = measured.DisplayUnit;
        HarvestedAt = harvestedAt;
        SourceCommandId = sourceCommandId;
        Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
    }

    public Guid CropZoneId { get; private set; }

    public string LotCode { get; private set; } = string.Empty;

    public decimal Quantity { get; private set; }

    public string Unit { get; private set; } = string.Empty;

    public decimal EnteredQuantity { get; private set; }

    public string EnteredUnit { get; private set; } = string.Empty;

    public string DisplayUnit { get; private set; } = string.Empty;

    public DateTimeOffset HarvestedAt { get; private set; }

    public Guid SourceCommandId { get; private set; }

    public string? Notes { get; private set; }

    private static string RequireText(string value, string parameter, int maximumLength)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximumLength)
        {
            throw new ArgumentException($"{parameter} must contain between 1 and {maximumLength} characters.", parameter);
        }

        return result;
    }
}
