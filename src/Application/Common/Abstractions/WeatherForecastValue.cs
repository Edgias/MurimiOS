using System.Text.Json;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record WeatherForecastValue(
    DateTimeOffset IssuedAt,
    DateTimeOffset ValidTo,
    decimal? MinimumTemperatureC,
    decimal? MaximumTemperatureC,
    decimal? PrecipitationMm,
    decimal? PrecipitationProbabilityPercent,
    decimal? WindSpeedMps,
    decimal? RelativeHumidityPercent) : ProviderNormalizedRecord;
