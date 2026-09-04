using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Domain.WeatherLocation;

public sealed class WeatherLocation : TenantEntity
{
    private WeatherLocation() { }
    public WeatherLocation(Guid id, Guid farmId, Guid? fieldId, string gridKey, decimal latitude,
        decimal longitude, decimal? elevationMetres, DateTimeOffset createdAt) : base(id)
    {
        Required(farmId, nameof(farmId)); OptionalId(fieldId, nameof(fieldId));
        if (latitude is < -90 or > 90 || longitude is < -180 or > 180) throw new ArgumentOutOfRangeException(nameof(latitude));
        if (elevationMetres is < -500 or > 9_000) throw new ArgumentOutOfRangeException(nameof(elevationMetres));
        FarmId = farmId; FieldId = fieldId; GridKey = Text(gridKey, nameof(gridKey), 120).ToLowerInvariant();
        Latitude = decimal.Round(latitude, 6); Longitude = decimal.Round(longitude, 6);
        ElevationMetres = elevationMetres; CreatedAt = createdAt.ToUniversalTime();
    }
    public Guid FarmId { get; private set; }
    public Guid? FieldId { get; private set; }
    public string GridKey { get; private set; } = string.Empty;
    public decimal Latitude { get; private set; }
    public decimal Longitude { get; private set; }
    public decimal? ElevationMetres { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    private static void Required(Guid id, string parameter) { if (id == Guid.Empty) throw new ArgumentException("A non-empty identifier is required.", parameter); }
    private static void OptionalId(Guid? id, string parameter) { if (id == Guid.Empty) throw new ArgumentException("An optional identifier must be non-empty.", parameter); }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
}
