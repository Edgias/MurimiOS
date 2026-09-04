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
public sealed record WeatherForecastResponse(Guid Id, Guid LocationId, Guid FarmId, Guid? FieldId,
    Guid FeedId, Guid IngestionRecordId, string ProviderForecastId, DateTimeOffset IssuedAt,
    DateTimeOffset ValidFrom, DateTimeOffset ValidTo, decimal? MinimumTemperatureC,
    decimal? MaximumTemperatureC, decimal? PrecipitationMm, decimal? PrecipitationProbabilityPercent,
    decimal? WindSpeedMps, decimal? RelativeHumidityPercent, InsightQuality Quality);
