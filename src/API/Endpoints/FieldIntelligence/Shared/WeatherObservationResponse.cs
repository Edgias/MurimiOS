using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WeatherLocation;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Shared;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;

public sealed record WeatherObservationResponse(Guid Id, Guid LocationId, Guid FarmId, Guid? FieldId,
    Guid FeedId, Guid IngestionRecordId, string ProviderObservationId, DateTimeOffset ObservedAt,
    DateTimeOffset ReceivedAt, decimal? TemperatureC, decimal? RelativeHumidityPercent,
    decimal? PrecipitationMm, decimal? WindSpeedMps, decimal? WindDirectionDegrees,
    decimal? PressureHpa, decimal? SoilTemperatureC, decimal? SoilMoisturePercent, InsightQuality Quality);
