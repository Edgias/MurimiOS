using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Domain.TenantRegionalSettings;
using EdgIQ.MurimiOS.Domain.ReferenceCatalogue;
using EdgIQ.MurimiOS.Domain.WeatherLocation;
using EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;
using EdgIQ.MurimiOS.Domain.FieldRiskRule;
using EdgIQ.MurimiOS.Domain.FieldInsightAlert;
using EdgIQ.MurimiOS.Domain.FieldMapLayer;
using EdgIQ.MurimiOS.Domain.FinanceOpeningBalance;
using EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule;
using EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun;
using EdgIQ.MurimiOS.Domain.LegalCompany;
using EdgIQ.MurimiOS.Domain.CostCentre;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using EdgIQ.MurimiOS.Domain.AccountingImportBatch;
using EdgIQ.MurimiOS.Domain.AccountingReconciliation;
using EdgIQ.MurimiOS.Domain.LabourCostEntry;
using EdgIQ.MurimiOS.Domain.MachineryCostEntry;
using EdgIQ.MurimiOS.Domain.EquipmentAsset;
using EdgIQ.MurimiOS.Domain.ConnectedDevice;
using EdgIQ.MurimiOS.Domain.SensorReading;
using EdgIQ.MurimiOS.Domain.MachineryTaskFile;
using EdgIQ.MurimiOS.Domain.BusinessDocumentExchange;
using EdgIQ.MurimiOS.Domain.MachineryTelemetryRecord;
using EdgIQ.MurimiOS.Domain.DerivedMediaAsset;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Integrations;

using EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.Worker;
using AccountingImportBatch = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingImportBatch;
using AccountingStatementLine = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingStatementLine;
using BusinessDocumentExchange = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.BusinessDocumentExchange;
using ExchangeDirection = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.ExchangeDirection;
using ConnectedDevice = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDevice;
using ConnectedDeviceStatus = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDeviceStatus;
using SensorChannel = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.SensorChannel;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;
using EquipmentStatus = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentStatus;
using IngestionAttempt = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionAttempt;
using IngestionAttemptOutcome = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionAttemptOutcome;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;
using IngestionQuality = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionQuality;
using IngestionRecord = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecord;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;
using IntegrationCredential = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredential;
using IntegrationCredentialStatus = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredentialStatus;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;
using IntegrationTargetType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationTargetType;
using MachineryTelemetryRecord = global::EdgIQ.MurimiOS.Domain.MachineryTelemetryRecord.MachineryTelemetryRecord;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using ReferenceCatalogue = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogue;
using ReferenceCatalogueItem = global::EdgIQ.MurimiOS.Domain.ReferenceCatalogue.ReferenceCatalogueItem;
using SatelliteFieldStatistic = global::EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic.SatelliteFieldStatistic;
using SensorReading = global::EdgIQ.MurimiOS.Domain.SensorReading.SensorReading;
using SensorValueQuality = global::EdgIQ.MurimiOS.Domain.SensorReading.SensorValueQuality;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;
using WeatherForecast = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherForecast;
using WeatherLocation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherLocation;
using WeatherObservation = global::EdgIQ.MurimiOS.Domain.WeatherLocation.WeatherObservation;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using CanonicalRestAdapter = global::EdgIQ.MurimiOS.Infrastructure.Integrations.CanonicalRestAdapter;
using ClaimedIngestionJob = global::EdgIQ.MurimiOS.Infrastructure.Integrations.ClaimedIngestionJob;
using IScheduledIngestionStore = global::EdgIQ.MurimiOS.Infrastructure.Integrations.IScheduledIngestionStore;
using AccountingBatchValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.AccountingBatchValue;
using BusinessDocumentValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.BusinessDocumentValue;
using IProviderAdapter = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IProviderAdapter;
using MachineryTelemetryValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.MachineryTelemetryValue;
using ProviderAdapterException = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderAdapterException;
using ProviderIngestionContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderIngestionContext;
using ProviderIngestionResult = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderIngestionResult;
using ProviderRecord = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderRecord;
using ProviderSpatialTarget = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderSpatialTarget;
using RegulatoryCatalogueItemValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.RegulatoryCatalogueItemValue;
using SatelliteStatisticValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.SatelliteStatisticValue;
using SensorChannelValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.SensorChannelValue;
using SensorReadingValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.SensorReadingValue;
using WeatherForecastValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.WeatherForecastValue;
using WeatherObservationValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.WeatherObservationValue;

public sealed class ProviderIngestionWorker(
    IScheduledIngestionStore store,
    IServiceScopeFactory scopeFactory,
    IOptions<ProviderIngestionOptions> options,
    TimeProvider timeProvider,
    ILogger<ProviderIngestionWorker> logger) : BackgroundService
{
    private readonly Guid workerId = Guid.NewGuid();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        ProviderIngestionOptions settings = options.Value;
        if (!settings.Enabled) { logger.LogInformation("Provider ingestion worker is disabled."); return; }
        TimeSpan idleDelay = TimeSpan.FromMilliseconds(settings.PollMilliseconds);
        logger.LogInformation("Provider ingestion worker {WorkerId} started.", workerId);
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                DateTimeOffset now = timeProvider.GetUtcNow();
                await store.ScheduleDueAsync(now, settings.ScheduleBatchSize, settings.MaximumAttempts, stoppingToken);
                ClaimedIngestionJob? claimed = await store.ClaimAsync(workerId, now, settings.LockSeconds, stoppingToken);
                if (claimed is null) await Task.Delay(idleDelay, stoppingToken);
                else await ProcessAsync(claimed, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception exception)
            {
                logger.LogError(exception, "Provider ingestion worker {WorkerId} loop failed.", workerId);
                await Task.Delay(idleDelay, stoppingToken);
            }
        }
    }

    private async Task ProcessAsync(ClaimedIngestionJob claimed, CancellationToken cancellationToken)
    {
        await using AsyncServiceScope scope = scopeFactory.CreateAsyncScope();
        scope.ServiceProvider.GetRequiredService<WorkerTenantContext>().Resolve(claimed.TenantId);
        PlatformDbContext db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
        DateTimeOffset startedAt = timeProvider.GetUtcNow();
        IngestionJob job = await db.IngestionJobs.SingleAsync(x => x.Id == claimed.JobId, cancellationToken);
        IntegrationFeed feed = await db.IntegrationFeeds.SingleAsync(x => x.Id == job.FeedId, cancellationToken);
        IntegrationConnection connection = await db.IntegrationConnections.SingleAsync(x => x.Id == feed.ConnectionId, cancellationToken);
        try
        {
            IProviderAdapter adapter = scope.ServiceProvider.GetServices<IProviderAdapter>()
                .SingleOrDefault(x => x.ProviderCode.Equals(connection.ProviderCode, StringComparison.OrdinalIgnoreCase) && x.FeedType == feed.FeedType)
                ?? throw new ProviderAdapterException("provider_adapter_missing", $"No adapter is registered for {connection.ProviderCode}/{feed.FeedType}.", false);
            string? credential = await ReadCredentialAsync(scope.ServiceProvider, db, connection.Id, startedAt, cancellationToken);
            ProviderSpatialTarget? target = await ResolveTargetAsync(db, feed, cancellationToken);
            var context = new ProviderIngestionContext(claimed.TenantId, job.Id, feed.Id, feed.FarmId, feed.FeedType,
                feed.TargetType, feed.TargetId, feed.ExternalSourceId, connection.ConfigurationJson, feed.ConfigurationJson,
                feed.CheckpointJson, credential, job.WindowStart, job.WindowEnd, startedAt, target);
            ProviderIngestionResult result = await adapter.IngestAsync(context, cancellationToken);
            DateTimeOffset completedAt = timeProvider.GetUtcNow();
            int accepted = await PersistRecordsAsync(db, job, feed, result.Records, target, completedAt, cancellationToken);
            DateTimeOffset? latest = result.Records.Count == 0 ? null : result.Records.Max(x => x.ObservedAt);
            job.Succeed(claimed.LeaseId, completedAt, result.CheckpointJson);
            feed.RecordSuccess(completedAt, latest, result.CheckpointJson);
            connection.RecordSuccess(completedAt, result.RateLimitRemaining, result.RateLimitResetAt);
            db.IngestionAttempts.Add(new IngestionAttempt(Guid.NewGuid(), job.Id, job.Attempts, startedAt, completedAt,
                IngestionAttemptOutcome.Succeeded, result.HttpStatusCode, result.RequestUnits, 0m, null, null, null));
            db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), claimed.TenantId, "integration.ingestion.completed",
                JsonSerializer.Serialize(new { jobId = job.Id, feedId = feed.Id, provider = connection.ProviderCode, acceptedRecords = accepted }), completedAt));
            await db.SaveChangesAsync(cancellationToken);
            logger.LogInformation("Ingestion job {JobId} accepted {RecordCount} provider records.", job.Id, accepted);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            await RecordFailureAsync(db, job.Id, feed.Id, connection.Id, claimed.LeaseId, startedAt, exception, cancellationToken);
        }
    }

    private static async Task<string?> ReadCredentialAsync(IServiceProvider services, PlatformDbContext db, Guid connectionId, DateTimeOffset now, CancellationToken cancellationToken)
    {
        IntegrationCredential? value = await db.IntegrationCredentials.AsNoTracking()
            .Where(x => x.ConnectionId == connectionId && x.Status == IntegrationCredentialStatus.Active && x.ValidFrom <= now && (x.ExpiresAt == null || x.ExpiresAt > now))
            .OrderByDescending(x => x.CredentialVersion).FirstOrDefaultAsync(cancellationToken);
        return value is null ? null : services.GetRequiredService<ProviderCredentialReader>().Unprotect(value.ProtectedReference);
    }

    private static async Task<ProviderSpatialTarget?> ResolveTargetAsync(PlatformDbContext db, IntegrationFeed feed, CancellationToken cancellationToken)
    {
        using JsonDocument configuration = JsonDocument.Parse(feed.ConfigurationJson);
        decimal? latitude = Number(configuration.RootElement, "latitude");
        decimal? longitude = Number(configuration.RootElement, "longitude");
        if (feed.TargetType == IntegrationTargetType.Field && feed.TargetId is { } fieldId)
        {
            var boundary = await db.FieldBoundaryVersions.AsNoTracking().Where(x => x.FieldId == fieldId && x.ValidTo == null)
                .OrderByDescending(x => x.ValidFrom).FirstOrDefaultAsync(cancellationToken);
            if (boundary is not null)
            {
                Point centroid = boundary.Boundary.Centroid;
                return new ProviderSpatialTarget(fieldId, boundary.Id, latitude ?? (decimal)centroid.Y, longitude ?? (decimal)centroid.X, ToGeoJson(boundary.Boundary));
            }
            if (latitude is not null && longitude is not null) return new ProviderSpatialTarget(fieldId, null, latitude.Value, longitude.Value, null);
        }
        return latitude is not null && longitude is not null ? new ProviderSpatialTarget(null, null, latitude.Value, longitude.Value, null) : null;
    }

    private static async Task<int> PersistRecordsAsync(PlatformDbContext db, IngestionJob job, IntegrationFeed feed,
        IReadOnlyList<ProviderRecord> records, ProviderSpatialTarget? target, DateTimeOffset receivedAt, CancellationToken cancellationToken)
    {
        int accepted = 0;
        WeatherLocation? location = null;
        ReferenceCatalogue? catalogue = null;
        var regulatoryCodes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var seenRecords = new HashSet<string>(StringComparer.Ordinal);
        foreach (ProviderRecord source in records)
        {
            string digest = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(source.PayloadJson))).ToLowerInvariant();
            if (!seenRecords.Add($"{source.ExternalRecordId}\n{digest}")) continue;
            if (await db.IngestionRecords.AnyAsync(x => x.FeedId == feed.Id && x.ExternalRecordId == source.ExternalRecordId && x.ContentSha256 == digest, cancellationToken)) continue;
            var record = new IngestionRecord(Guid.NewGuid(), job.Id, feed.Id, feed.FarmId, source.ExternalRecordId, 1,
                source.ObservedAt, receivedAt, source.SourceModifiedAt, digest, source.PayloadJson, IngestionQuality.Valid);
            db.IngestionRecords.Add(record);
            switch (source.Normalized)
            {
                case WeatherObservationValue value:
                    location ??= await GetWeatherLocationAsync(db, feed, target, receivedAt, cancellationToken);
                    db.WeatherObservations.Add(new WeatherObservation(Guid.NewGuid(), location.Id, feed.FarmId, target?.FieldId, feed.Id,
                        record.Id, source.ExternalRecordId, source.ObservedAt, receivedAt, value.TemperatureC, value.RelativeHumidityPercent,
                        value.PrecipitationMm, value.WindSpeedMps, value.WindDirectionDegrees, value.PressureHpa, value.SoilTemperatureC,
                        value.SoilMoisturePercent, InsightQuality.Valid));
                    break;
                case WeatherForecastValue value:
                    location ??= await GetWeatherLocationAsync(db, feed, target, receivedAt, cancellationToken);
                    db.WeatherForecasts.Add(new WeatherForecast(Guid.NewGuid(), location.Id, feed.FarmId, target?.FieldId, feed.Id,
                        record.Id, source.ExternalRecordId, value.IssuedAt, source.ObservedAt, value.ValidTo, value.MinimumTemperatureC,
                        value.MaximumTemperatureC, value.PrecipitationMm, value.PrecipitationProbabilityPercent, value.WindSpeedMps,
                        value.RelativeHumidityPercent, InsightQuality.Valid));
                    break;
                case SatelliteStatisticValue value when target?.FieldId is { } fieldId && target.BoundaryVersionId is { } boundaryId:
                    db.SatelliteFieldStatistics.Add(new SatelliteFieldStatistic(Guid.NewGuid(), feed.FarmId, fieldId, boundaryId, feed.Id,
                        record.Id, source.ExternalRecordId, value.Index, value.CustomIndex, source.ObservedAt, receivedAt, value.Mean,
                        value.Minimum, value.Maximum, value.StandardDeviation, value.ValidPixelPercent, value.CloudPercent, InsightQuality.Valid));
                    break;
                case RegulatoryCatalogueItemValue value:
                    catalogue ??= await GetCatalogueAsync(db, value.CatalogueVersion, value.EffectiveFrom, cancellationToken);
                    string code = value.Code.ToUpperInvariant();
                    if (regulatoryCodes.Add(code) && !await db.ReferenceCatalogueItems.AnyAsync(x => x.CatalogueId == catalogue.Id && x.Code == code, cancellationToken))
                        db.ReferenceCatalogueItems.Add(new ReferenceCatalogueItem(Guid.NewGuid(), catalogue.Id, code, value.Name,
                            value.Description, value.AttributesJson, value.EffectiveFrom, value.EffectiveTo));
                    break;
                case SensorReadingValue value:
                {
                    ConnectedDevice? device = await db.ConnectedDevices.SingleOrDefaultAsync(x => x.FarmId == feed.FarmId
                        && x.DeviceKey == value.DeviceKey && x.Status == ConnectedDeviceStatus.Active, cancellationToken);
                    if (device is null)
                    {
                        record.Reject(receivedAt, "connected_device_missing", $"Active device '{value.DeviceKey}' was not found.");
                        accepted++; continue;
                    }
                    string[] channelKeys = value.Channels.Select(x => x.ChannelKey).Distinct(StringComparer.OrdinalIgnoreCase).ToArray();
                    SensorChannel[] channels = await db.SensorChannels.Where(x => x.DeviceId == device.Id && x.IsActive
                        && channelKeys.Contains(x.ChannelKey)).ToArrayAsync(cancellationToken);
                    var channelMap = channels.ToDictionary(x => x.ChannelKey, StringComparer.OrdinalIgnoreCase);
                    if (channelMap.Count != channelKeys.Length || value.Channels.Any(x => !channelMap[x.ChannelKey].Unit.Equals(x.Unit, StringComparison.OrdinalIgnoreCase)))
                    {
                        record.Reject(receivedAt, "sensor_channel_mapping_invalid", "Every provider channel must map to an active device channel with the same unit.");
                        accepted++; continue;
                    }
                    foreach (SensorChannelValue item in value.Channels)
                    {
                        SensorChannel channel = channelMap[item.ChannelKey];
                        db.SensorReadings.Add(new SensorReading(Guid.NewGuid(), feed.FarmId, device.Id, channel.Id,
                            channel.MetricCode, item.Value, item.Unit, source.ObservedAt, receivedAt,
                            channel.IsWithinRange(item.Value) ? SensorValueQuality.Valid : SensorValueQuality.Suspect,
                            item.ExternalReadingId, digest, record.Id, item.Latitude, item.Longitude));
                    }
                    device.RecordSeen(source.ObservedAt, value.FirmwareVersion);
                    break;
                }
                case MachineryTelemetryValue value:
                {
                    EquipmentAsset? equipment = await db.EquipmentAssets.SingleOrDefaultAsync(x => x.FarmId == feed.FarmId
                        && x.Code == value.EquipmentCode.ToUpper() && x.Status != EquipmentStatus.Retired, cancellationToken);
                    if (equipment is null)
                    {
                        record.Reject(receivedAt, "equipment_missing", $"Equipment '{value.EquipmentCode}' was not found.");
                        accepted++; continue;
                    }
                    ConnectedDevice? device = null;
                    if (!string.IsNullOrWhiteSpace(value.DeviceKey))
                        device = await db.ConnectedDevices.SingleOrDefaultAsync(x => x.FarmId == feed.FarmId
                            && x.EquipmentId == equipment.Id && x.DeviceKey == value.DeviceKey && x.Status == ConnectedDeviceStatus.Active, cancellationToken);
                    if (!string.IsNullOrWhiteSpace(value.DeviceKey) && device is null)
                    {
                        record.Reject(receivedAt, "machinery_device_missing", $"Active machinery device '{value.DeviceKey}' was not found.");
                        accepted++; continue;
                    }
                    db.MachineryTelemetry.Add(new MachineryTelemetryRecord(Guid.NewGuid(), feed.FarmId, equipment.Id,
                        device?.Id, source.ObservedAt, receivedAt, value.EngineHours, value.SpeedKph, value.FuelLevelPercent,
                        value.State, value.Latitude, value.Longitude, source.ExternalRecordId, digest, record.Id));
                    device?.RecordSeen(source.ObservedAt, null);
                    break;
                }
                case BusinessDocumentValue value:
                {
                    string documentHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value.DocumentJson))).ToLowerInvariant();
                    db.BusinessDocumentExchanges.Add(new BusinessDocumentExchange(Guid.NewGuid(), feed.ConnectionId,
                        feed.FarmId, value.DocumentType, ExchangeDirection.Inbound, value.ExternalDocumentId,
                        value.SchemaVersion, value.MappingVersion, value.DocumentJson, documentHash, source.ObservedAt, receivedAt));
                    break;
                }
                case AccountingBatchValue value:
                {
                    if (await db.AccountingImportBatches.AnyAsync(x => x.FarmId == feed.FarmId
                        && x.Provider == CanonicalRestAdapter.Code && x.ExternalBatchId == value.ExternalBatchId, cancellationToken))
                    {
                        record.Reject(receivedAt, "accounting_batch_exists", "The accounting batch identifier has already been imported.");
                        accepted++; continue;
                    }
                    Guid batchId = Guid.NewGuid();
                    db.AccountingImportBatches.Add(new AccountingImportBatch(batchId, feed.FarmId, CanonicalRestAdapter.Code,
                        value.ExternalBatchId, value.StatementStart, value.StatementEnd, value.Currency, receivedAt, "provider-ingestion-worker"));
                    db.AccountingStatementLines.AddRange(value.Lines.Select(line => new AccountingStatementLine(Guid.NewGuid(),
                        batchId, line.ExternalLineId, line.OccurredAt, line.EntryType, line.Amount, value.Currency,
                        line.AccountCode, line.Reference, line.Description)));
                    break;
                }
                default:
                    record.Reject(receivedAt, "normalized_target_invalid", "The provider record cannot be mapped to the configured feed target.");
                    accepted++;
                    continue;
            }
            record.MarkProcessed(receivedAt);
            accepted++;
        }
        return accepted;
    }

    private static async Task<WeatherLocation> GetWeatherLocationAsync(PlatformDbContext db, IntegrationFeed feed, ProviderSpatialTarget? target,
        DateTimeOffset now, CancellationToken cancellationToken)
    {
        if (target is null) throw new ProviderAdapterException("weather_location_missing", "Weather coordinates are required.", false);
        string gridKey = $"{target.Latitude:F6}:{target.Longitude:F6}";
        WeatherLocation? location = await db.WeatherLocations.FirstOrDefaultAsync(x => x.FarmId == feed.FarmId && x.FieldId == target.FieldId && x.GridKey == gridKey, cancellationToken);
        if (location is not null) return location;
        location = new WeatherLocation(Guid.NewGuid(), feed.FarmId, target.FieldId, gridKey, target.Latitude, target.Longitude, null, now);
        db.WeatherLocations.Add(location);
        return location;
    }

    private static async Task<ReferenceCatalogue> GetCatalogueAsync(PlatformDbContext db, string version, DateOnly effectiveFrom, CancellationToken cancellationToken)
    {
        ReferenceCatalogue? catalogue = await db.ReferenceCatalogues.FirstOrDefaultAsync(x => x.Key == "eu-pesticides-active-substances" && x.CatalogueVersion == version, cancellationToken);
        if (catalogue is not null) return catalogue;
        catalogue = new ReferenceCatalogue(Guid.NewGuid(), "eu-pesticides-active-substances", "EU pesticide active substances", version,
            null, "European Commission", effectiveFrom);
        db.ReferenceCatalogues.Add(catalogue);
        return catalogue;
    }

    private async Task RecordFailureAsync(PlatformDbContext db, Guid jobId, Guid feedId, Guid connectionId, Guid leaseId,
        DateTimeOffset startedAt, Exception exception, CancellationToken cancellationToken)
    {
        db.ChangeTracker.Clear();
        IngestionJob job = await db.IngestionJobs.SingleAsync(x => x.Id == jobId, cancellationToken);
        IntegrationFeed feed = await db.IntegrationFeeds.SingleAsync(x => x.Id == feedId, cancellationToken);
        IntegrationConnection connection = await db.IntegrationConnections.SingleAsync(x => x.Id == connectionId, cancellationToken);
        DateTimeOffset failedAt = timeProvider.GetUtcNow();
        ProviderAdapterException? providerFailure = exception as ProviderAdapterException;
        string code = providerFailure?.Code ?? "provider_ingestion_failed";
        string message = exception.Message.Length > 2_000 ? exception.Message[..2_000] : exception.Message;
        bool retriable = providerFailure?.Retriable ?? true;
        DateTimeOffset retryAt = failedAt.AddSeconds(Math.Min(3_600, 10 * Math.Pow(2, Math.Max(0, job.Attempts - 1))));
        if (retriable) job.Fail(leaseId, failedAt, retryAt, code, message); else job.FailPermanently(leaseId, failedAt, code, message);
        feed.RecordFailure(failedAt, code, message, retryAt);
        connection.RecordFailure(failedAt, code, message, providerFailure?.HttpStatusCode is >= 500);
        db.IngestionAttempts.Add(new IngestionAttempt(Guid.NewGuid(), job.Id, job.Attempts, startedAt, failedAt,
            retriable ? IngestionAttemptOutcome.RetriableFailure : IngestionAttemptOutcome.PermanentFailure,
            providerFailure?.HttpStatusCode, 0, 0m, null, code, message));
        await db.SaveChangesAsync(cancellationToken);
        logger.LogWarning(exception, "Ingestion job {JobId} failed with {ErrorCode}; retriable={Retriable}.", job.Id, code, retriable);
    }

    private static decimal? Number(JsonElement value, string name) =>
        value.TryGetProperty(name, out JsonElement property) && property.TryGetDecimal(out decimal result) ? result : null;

    private static string ToGeoJson(MultiPolygon geometry) => JsonSerializer.Serialize(new
    {
        type = "MultiPolygon",
        coordinates = geometry.Geometries.Cast<Polygon>().Select(polygon =>
            new[] { polygon.ExteriorRing }.Concat(Enumerable.Range(0, polygon.NumInteriorRings).Select(polygon.GetInteriorRingN))
                .Select(ring => ring.Coordinates.Select(coordinate => new[] { coordinate.X, coordinate.Y }).ToArray()).ToArray()).ToArray()
    });
}
