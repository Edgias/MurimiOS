using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;

namespace EdgIQ.MurimiOS.Domain.StorageLot;

public sealed class StorageLot : TenantEntity
{
    private StorageLot() { }

    public StorageLot(Guid id, Guid farmId, string lotCode, string crop, string? variety,
        decimal quantity, string unit, DateTimeOffset packedAt, string? storageLocation) : base(id)
    {
        if (farmId == Guid.Empty) throw new ArgumentException("A farm is required.", nameof(farmId));
        if (quantity <= 0) throw new ArgumentOutOfRangeException(nameof(quantity));
        FarmId = farmId;
        LotCode = Text(lotCode, nameof(lotCode), 120);
        Crop = Text(crop, nameof(crop), 120);
        Variety = Optional(variety, 120);
        MeasuredQuantity measured = MeasurementUnits.Capture(quantity, unit, unit);
        if (measured.Dimension is not (UnitDimension.Mass or UnitDimension.Volume or UnitDimension.Count))
            throw new ArgumentException("Storage quantities must use mass, volume, or count units.", nameof(unit));
        Quantity = measured.CanonicalValue;
        Unit = measured.CanonicalUnit;
        EnteredQuantity = measured.EnteredValue;
        EnteredUnit = measured.EnteredUnit;
        DisplayUnit = measured.DisplayUnit;
        PackedAt = packedAt;
        StorageLocation = Optional(storageLocation, 240);
    }

    public Guid FarmId { get; private set; }
    public string LotCode { get; private set; } = string.Empty;
    public string Crop { get; private set; } = string.Empty;
    public string? Variety { get; private set; }
    public decimal Quantity { get; private set; }
    public string Unit { get; private set; } = string.Empty;
    public decimal EnteredQuantity { get; private set; }
    public string EnteredUnit { get; private set; } = string.Empty;
    public string DisplayUnit { get; private set; } = string.Empty;
    public DateTimeOffset PackedAt { get; private set; }
    public string? StorageLocation { get; private set; }

    private static string Text(string? value, string parameter, int maximum)
    {
        string result = value?.Trim() ?? string.Empty;
        if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter);
        return result;
    }

    private static string? Optional(string? value, int maximum)
    {
        string? result = value?.Trim();
        if (result?.Length > maximum) throw new ArgumentException($"Value cannot exceed {maximum} characters.");
        return string.IsNullOrEmpty(result) ? null : result;
    }
}
