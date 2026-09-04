namespace EdgIQ.MurimiOS.Domain.Common;

public enum UnitDimension
{
    Area = 1,
    Mass = 2,
    Volume = 3,
    Length = 4,
    Temperature = 5,
    Count = 6,
    Time = 7
}

public sealed record MeasurementUnitDefinition(
    string Code,
    string Name,
    string Symbol,
    UnitDimension Dimension,
    string CanonicalCode,
    decimal ScaleToCanonical,
    decimal OffsetToCanonical,
    int DefaultDecimalPlaces);

public static class MeasurementUnits
{
    private static readonly MeasurementUnitDefinition[] definitions =
    [
        new("ha", "hectare", "ha", UnitDimension.Area, "ha", 1m, 0m, 4),
        new("m2", "square metre", "m²", UnitDimension.Area, "ha", 0.0001m, 0m, 2),
        new("acre", "acre", "ac", UnitDimension.Area, "ha", 0.40468564224m, 0m, 4),

        new("kg", "kilogram", "kg", UnitDimension.Mass, "kg", 1m, 0m, 4),
        new("g", "gram", "g", UnitDimension.Mass, "kg", 0.001m, 0m, 2),
        new("t", "metric tonne", "t", UnitDimension.Mass, "kg", 1000m, 0m, 4),
        new("lb", "pound", "lb", UnitDimension.Mass, "kg", 0.45359237m, 0m, 4),

        new("L", "litre", "L", UnitDimension.Volume, "L", 1m, 0m, 4),
        new("mL", "millilitre", "mL", UnitDimension.Volume, "L", 0.001m, 0m, 2),
        new("m3", "cubic metre", "m³", UnitDimension.Volume, "L", 1000m, 0m, 4),
        new("gal_us", "US gallon", "US gal", UnitDimension.Volume, "L", 3.785411784m, 0m, 4),

        new("m", "metre", "m", UnitDimension.Length, "m", 1m, 0m, 3),
        new("mm", "millimetre", "mm", UnitDimension.Length, "m", 0.001m, 0m, 1),
        new("cm", "centimetre", "cm", UnitDimension.Length, "m", 0.01m, 0m, 2),
        new("km", "kilometre", "km", UnitDimension.Length, "m", 1000m, 0m, 3),
        new("ft", "foot", "ft", UnitDimension.Length, "m", 0.3048m, 0m, 3),

        new("C", "degree Celsius", "°C", UnitDimension.Temperature, "C", 1m, 0m, 1),
        new("F", "degree Fahrenheit", "°F", UnitDimension.Temperature, "C", 0.5555555555555555555555555556m, -17.777777777777777777777777778m, 1),
        new("K", "kelvin", "K", UnitDimension.Temperature, "C", 1m, -273.15m, 1),

        new("each", "each", "each", UnitDimension.Count, "each", 1m, 0m, 0),
        new("head", "head", "head", UnitDimension.Count, "head", 1m, 0m, 0),

        new("min", "minute", "min", UnitDimension.Time, "min", 1m, 0m, 0),
        new("h", "hour", "h", UnitDimension.Time, "min", 60m, 0m, 2),
        new("day", "day", "d", UnitDimension.Time, "min", 1440m, 0m, 2)
    ];

    private static readonly IReadOnlyDictionary<string, MeasurementUnitDefinition> byCode =
        definitions.ToDictionary(x => x.Code, StringComparer.OrdinalIgnoreCase);

    public static IReadOnlyList<MeasurementUnitDefinition> All => definitions;

    public static MeasurementUnitDefinition Get(string code)
    {
        string normalized = code?.Trim() ?? string.Empty;
        return byCode.TryGetValue(normalized, out MeasurementUnitDefinition? definition)
            ? definition
            : throw new ArgumentException($"Unsupported measurement unit '{normalized}'.", nameof(code));
    }

    public static string Normalize(string code, UnitDimension? expectedDimension = null)
    {
        MeasurementUnitDefinition definition = Get(code);
        if (expectedDimension is not null && definition.Dimension != expectedDimension)
            throw new ArgumentException($"Unit '{code}' is not a {expectedDimension} unit.", nameof(code));
        return definition.Code;
    }

    public static decimal Convert(decimal value, string fromUnit, string toUnit)
    {
        MeasurementUnitDefinition source = Get(fromUnit);
        MeasurementUnitDefinition target = Get(toUnit);
        if (source.Dimension != target.Dimension)
            throw new ArgumentException($"Cannot convert {source.Dimension} to {target.Dimension}.", nameof(toUnit));
        if (!source.CanonicalCode.Equals(target.CanonicalCode, StringComparison.OrdinalIgnoreCase))
            throw new ArgumentException($"Units '{fromUnit}' and '{toUnit}' do not share a canonical unit.", nameof(toUnit));

        decimal canonical = value * source.ScaleToCanonical + source.OffsetToCanonical;
        return (canonical - target.OffsetToCanonical) / target.ScaleToCanonical;
    }

    public static MeasuredQuantity Capture(decimal enteredValue, string enteredUnit, string? displayUnit = null)
    {
        MeasurementUnitDefinition source = Get(enteredUnit);
        string normalizedDisplay = Normalize(displayUnit ?? source.Code, source.Dimension);
        decimal canonicalValue = Convert(enteredValue, source.Code, source.CanonicalCode);
        return new MeasuredQuantity(
            canonicalValue,
            source.CanonicalCode,
            enteredValue,
            source.Code,
            normalizedDisplay,
            source.Dimension);
    }
}

public readonly record struct Quantity<TUnit>
    where TUnit : struct, Enum
{
    public Quantity(decimal value, TUnit unit)
    {
        if (!Enum.IsDefined(unit)) throw new ArgumentOutOfRangeException(nameof(unit));
        Value = value;
        Unit = unit;
    }

    public decimal Value { get; }
    public TUnit Unit { get; }
}

public readonly record struct MeasuredQuantity(
    decimal CanonicalValue,
    string CanonicalUnit,
    decimal EnteredValue,
    string EnteredUnit,
    string DisplayUnit,
    UnitDimension Dimension)
{
    public decimal DisplayValue => MeasurementUnits.Convert(CanonicalValue, CanonicalUnit, DisplayUnit);
}
