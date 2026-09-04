using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WeatherLocation;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Shared;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;
public sealed record WeatherForecastResponse(Guid Id, Guid LocationId, Guid FarmId, Guid? FieldId,
    Guid FeedId, Guid IngestionRecordId, string ProviderForecastId, DateTimeOffset IssuedAt,
    DateTimeOffset ValidFrom, DateTimeOffset ValidTo, decimal? MinimumTemperatureC,
    decimal? MaximumTemperatureC, decimal? PrecipitationMm, decimal? PrecipitationProbabilityPercent,
    decimal? WindSpeedMps, decimal? RelativeHumidityPercent, InsightQuality Quality);
