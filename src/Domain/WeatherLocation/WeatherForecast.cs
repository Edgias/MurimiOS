using EdgIQ.MurimiOS.Domain.Common;


using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Domain.WeatherLocation;

public sealed class WeatherForecast : TenantEntity
{
    private WeatherForecast() { }
    public WeatherForecast(Guid id, Guid locationId, Guid farmId, Guid? fieldId, Guid feedId,
        Guid ingestionRecordId, string providerForecastId, DateTimeOffset issuedAt, DateTimeOffset validFrom,
        DateTimeOffset validTo, decimal? minimumTemperatureC, decimal? maximumTemperatureC,
        decimal? precipitationMm, decimal? precipitationProbabilityPercent, decimal? windSpeedMps,
        decimal? relativeHumidityPercent, InsightQuality quality) : base(id)
    {
        if (new[] { locationId, farmId, feedId, ingestionRecordId }.Any(x => x == Guid.Empty) || fieldId == Guid.Empty) throw new ArgumentException("Required identifiers must be non-empty.");
        if (validTo <= validFrom || !Enum.IsDefined(quality)) throw new ArgumentException("A valid forecast period and quality are required.");
        Range(minimumTemperatureC, -100, 70, nameof(minimumTemperatureC)); Range(maximumTemperatureC, -100, 70, nameof(maximumTemperatureC));
        if (minimumTemperatureC > maximumTemperatureC) throw new ArgumentException("Minimum temperature cannot exceed maximum temperature.");
        Range(precipitationMm, 0, 10_000, nameof(precipitationMm)); Range(precipitationProbabilityPercent, 0, 100, nameof(precipitationProbabilityPercent));
        Range(windSpeedMps, 0, 150, nameof(windSpeedMps)); Range(relativeHumidityPercent, 0, 100, nameof(relativeHumidityPercent));
        LocationId = locationId; FarmId = farmId; FieldId = fieldId; FeedId = feedId; IngestionRecordId = ingestionRecordId;
        ProviderForecastId = Text(providerForecastId, nameof(providerForecastId), 240); IssuedAt = issuedAt.ToUniversalTime();
        ValidFrom = validFrom.ToUniversalTime(); ValidTo = validTo.ToUniversalTime(); MinimumTemperatureC = minimumTemperatureC;
        MaximumTemperatureC = maximumTemperatureC; PrecipitationMm = precipitationMm;
        PrecipitationProbabilityPercent = precipitationProbabilityPercent; WindSpeedMps = windSpeedMps;
        RelativeHumidityPercent = relativeHumidityPercent; Quality = quality;
    }
    public Guid LocationId { get; private set; }
    public Guid FarmId { get; private set; }
    public Guid? FieldId { get; private set; }
    public Guid FeedId { get; private set; }
    public Guid IngestionRecordId { get; private set; }
    public string ProviderForecastId { get; private set; } = string.Empty;
    public DateTimeOffset IssuedAt { get; private set; }
    public DateTimeOffset ValidFrom { get; private set; }
    public DateTimeOffset ValidTo { get; private set; }
    public decimal? MinimumTemperatureC { get; private set; }
    public decimal? MaximumTemperatureC { get; private set; }
    public decimal? PrecipitationMm { get; private set; }
    public decimal? PrecipitationProbabilityPercent { get; private set; }
    public decimal? WindSpeedMps { get; private set; }
    public decimal? RelativeHumidityPercent { get; private set; }
    public InsightQuality Quality { get; private set; }
    private static void Range(decimal? value, decimal minimum, decimal maximum, string parameter) { if (value.HasValue && (value < minimum || value > maximum)) throw new ArgumentOutOfRangeException(parameter); }
    private static string Text(string? value, string parameter, int maximum) { string result = value?.Trim() ?? string.Empty; if (result.Length is 0 || result.Length > maximum) throw new ArgumentException($"{parameter} is required and cannot exceed {maximum} characters.", parameter); return result; }
}
