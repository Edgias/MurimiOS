using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Domain.WeatherLocation;

public sealed class WeatherObservation : TenantEntity
{
    private WeatherObservation() { }
    public WeatherObservation(Guid id, Guid locationId, Guid farmId, Guid? fieldId, Guid feedId,
        Guid ingestionRecordId, string providerObservationId, DateTimeOffset observedAt, DateTimeOffset receivedAt,
        decimal? temperatureC, decimal? relativeHumidityPercent, decimal? precipitationMm,
        decimal? windSpeedMps, decimal? windDirectionDegrees, decimal? pressureHpa,
        decimal? soilTemperatureC, decimal? soilMoisturePercent, InsightQuality quality) : base(id)
    {
        Required(locationId, farmId, feedId, ingestionRecordId); OptionalId(fieldId);
        if (!Enum.IsDefined(quality)) throw new ArgumentOutOfRangeException(nameof(quality));
        Range(temperatureC, -100, 70, nameof(temperatureC)); Range(relativeHumidityPercent, 0, 100, nameof(relativeHumidityPercent));
        Range(precipitationMm, 0, 10_000, nameof(precipitationMm)); Range(windSpeedMps, 0, 150, nameof(windSpeedMps));
        Range(windDirectionDegrees, 0, 360, nameof(windDirectionDegrees)); Range(pressureHpa, 500, 1_200, nameof(pressureHpa));
        Range(soilTemperatureC, -100, 100, nameof(soilTemperatureC)); Range(soilMoisturePercent, 0, 100, nameof(soilMoisturePercent));
        if (new decimal?[] { temperatureC, relativeHumidityPercent, precipitationMm, windSpeedMps, pressureHpa, soilTemperatureC, soilMoisturePercent }.All(x => x is null))
            throw new ArgumentException("At least one weather measurement is required.");
        LocationId = locationId; FarmId = farmId; FieldId = fieldId; FeedId = feedId; IngestionRecordId = ingestionRecordId;
        ProviderObservationId = Text(providerObservationId, nameof(providerObservationId), 240);
        ObservedAt = observedAt.ToUniversalTime(); ReceivedAt = receivedAt.ToUniversalTime();
        TemperatureC = temperatureC; RelativeHumidityPercent = relativeHumidityPercent; PrecipitationMm = precipitationMm;
        WindSpeedMps = windSpeedMps; WindDirectionDegrees = windDirectionDegrees; PressureHpa = pressureHpa;
        SoilTemperatureC = soilTemperatureC; SoilMoisturePercent = soilMoisturePercent; Quality = quality;
    }
    public Guid LocationId { get; private set; }
    public Guid FarmId { get; private set; }
    public Guid? FieldId { get; private set; }
    public Guid FeedId { get; private set; }
    public Guid IngestionRecordId { get; private set; }
    public string ProviderObservationId { get; private set; } = string.Empty;
    public DateTimeOffset ObservedAt { get; private set; }
    public DateTimeOffset ReceivedAt { get; private set; }
    public decimal? TemperatureC { get; private set; }
    public decimal? RelativeHumidityPercent { get; private set; }
    public decimal? PrecipitationMm { get; private set; }
    public decimal? WindSpeedMps { get; private set; }
    public decimal? WindDirectionDegrees { get; private set; }
    public decimal? PressureHpa { get; private set; }
    public decimal? SoilTemperatureC { get; private set; }
    public decimal? SoilMoisturePercent { get; private set; }
    public InsightQuality Quality { get; private set; }
    private static void Required(params Guid[] ids) { if (ids.Any(x => x == Guid.Empty)) throw new ArgumentException("Required identifiers must be non-empty."); }
    private static void OptionalId(Guid? id) { if (id == Guid.Empty) throw new ArgumentException("An optional identifier must be non-empty."); }
    private static void Range(decimal? value, decimal minimum, decimal maximum, string parameter) { if (value.HasValue && (value < minimum || value > maximum)) throw new ArgumentOutOfRangeException(parameter); }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
}
