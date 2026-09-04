using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WeatherLocation;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.IngestWeatherForecasts;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;
public sealed record WeatherForecastItem(Guid? Id, string ProviderForecastId, DateTimeOffset IssuedAt,
    DateTimeOffset ValidFrom, DateTimeOffset ValidTo, decimal? MinimumTemperatureC,
    decimal? MaximumTemperatureC, decimal? PrecipitationMm, decimal? PrecipitationProbabilityPercent,
    decimal? WindSpeedMps, decimal? RelativeHumidityPercent, InsightQuality Quality);
