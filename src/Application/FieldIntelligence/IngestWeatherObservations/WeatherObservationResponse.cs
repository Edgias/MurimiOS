using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.WeatherLocation;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.FieldIntelligence.IngestWeatherObservations;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;

using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using IngestionRecord = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecord;
using IngestionRecordStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecordStatus;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using IntegrationTargetType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationTargetType;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;
using WeatherLocation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherLocation;
using WeatherObservation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherObservation;

public sealed record WeatherObservationResponse(Guid Id, Guid LocationId, Guid FarmId, Guid? FieldId,
    Guid FeedId, Guid IngestionRecordId, string ProviderObservationId, DateTimeOffset ObservedAt,
    DateTimeOffset ReceivedAt, decimal? TemperatureC, decimal? RelativeHumidityPercent,
    decimal? PrecipitationMm, decimal? WindSpeedMps, decimal? WindDirectionDegrees,
    decimal? PressureHpa, decimal? SoilTemperatureC, decimal? SoilMoisturePercent, InsightQuality Quality);
