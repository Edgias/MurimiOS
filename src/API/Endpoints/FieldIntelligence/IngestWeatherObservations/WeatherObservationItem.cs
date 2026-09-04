using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WeatherLocation;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.IngestWeatherObservations;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;
public sealed record WeatherObservationItem(Guid? Id, string ProviderObservationId, DateTimeOffset ObservedAt,
    decimal? TemperatureC, decimal? RelativeHumidityPercent, decimal? PrecipitationMm,
    decimal? WindSpeedMps, decimal? WindDirectionDegrees, decimal? PressureHpa,
    decimal? SoilTemperatureC, decimal? SoilMoisturePercent, InsightQuality Quality);
