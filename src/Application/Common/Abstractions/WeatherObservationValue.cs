using System.Text.Json;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record WeatherObservationValue(
    decimal? TemperatureC,
    decimal? RelativeHumidityPercent,
    decimal? PrecipitationMm,
    decimal? WindSpeedMps,
    decimal? WindDirectionDegrees,
    decimal? PressureHpa,
    decimal? SoilTemperatureC,
    decimal? SoilMoisturePercent) : ProviderNormalizedRecord;
