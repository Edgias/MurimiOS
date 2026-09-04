using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WeatherLocation;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.Weather;
using WeatherLocation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherLocation;

using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;
using WeatherForecast = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherForecast;
using WeatherObservation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherObservation;

public sealed record WeatherObservationResponse(Guid Id, Guid LocationId, Guid FarmId, Guid? FieldId,
    Guid FeedId, Guid IngestionRecordId, string ProviderObservationId, DateTimeOffset ObservedAt,
    DateTimeOffset ReceivedAt, decimal? TemperatureC, decimal? RelativeHumidityPercent,
    decimal? PrecipitationMm, decimal? WindSpeedMps, decimal? WindDirectionDegrees,
    decimal? PressureHpa, decimal? SoilTemperatureC, decimal? SoilMoisturePercent, InsightQuality Quality);
