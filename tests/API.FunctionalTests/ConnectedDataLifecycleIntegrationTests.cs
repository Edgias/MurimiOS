using System.Net;
using System.Net.Http.Json;
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
using EdgIQ.MurimiOS.Domain.WeatherLocation;
using EdgIQ.MurimiOS.Domain.SatelliteFieldStatistic;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;
using EdgIQ.MurimiOS.Domain.FieldRiskRule;
using EdgIQ.MurimiOS.Domain.FieldInsightAlert;
using EdgIQ.MurimiOS.Domain.FieldMapLayer;
using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;

using EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Analytics;
using EdgIQ.MurimiOS.Infrastructure.Integrations;

using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EdgIQ.MurimiOS.API.FunctionalTests;
using DerivedMediaAsset = global::EdgIQ.MurimiOS.Domain.DerivedMediaAsset.DerivedMediaAsset;
using DerivedMediaKind = global::EdgIQ.MurimiOS.Domain.DerivedMediaAsset.DerivedMediaKind;
using DerivedMediaLifecycleStatus = global::EdgIQ.MurimiOS.Domain.DerivedMediaAsset.DerivedMediaLifecycleStatus;
using FieldMapLayer = global::EdgIQ.MurimiOS.Domain.FieldMapLayer.FieldMapLayer;
using MapLayerKind = global::EdgIQ.MurimiOS.Domain.FieldMapLayer.MapLayerKind;
using InsightQuality = global::EdgIQ.MurimiOS.Domain.WeatherLocation.InsightQuality;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ClaimedDerivedMedia = global::EdgIQ.MurimiOS.Infrastructure.Integrations.ClaimedDerivedMedia;
using PostgresConnectedDataLifecycleStore = global::EdgIQ.MurimiOS.Infrastructure.Integrations.PostgresConnectedDataLifecycleStore;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;
using PostgresAnalyticsMaintenanceStore = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Analytics.PostgresAnalyticsMaintenanceStore;
using TenantConnectionInterceptor = global::EdgIQ.MurimiOS.Infrastructure.Data.Interceptors.TenantConnectionInterceptor;

public sealed class ConnectedDataLifecycleIntegrationTests
{
    [IntegrationFact]
    public async Task Retention_queues_only_registered_derived_media_and_future_partitions_are_present()
    {
        await using var factory = new MurimiApiFactory();
        Guid tenantId = Guid.NewGuid();
        Guid farmId = Guid.NewGuid();
        using HttpClient client = factory.CreateTenantClient(tenantId);
        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync("/api/v1/farms", new
        {
            id = farmId,
            name = "Lifecycle Integration Farm",
            code = $"LC{farmId:N}"[..12],
            countryCode = "ZW",
            timeZone = "Africa/Harare"
        })).StatusCode);
        Guid fieldId = Guid.NewGuid();
        Guid boundaryId = Guid.NewGuid();
        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync($"/api/v1/farms/{farmId}/fields", new
        {
            id = fieldId,
            boundaryId,
            name = "Lifecycle field",
            validFrom = new DateOnly(2025, 1, 1),
            exterior = new[]
            {
                new { longitude = 31.00, latitude = -17.80 },
                new { longitude = 31.01, latitude = -17.80 },
                new { longitude = 31.01, latitude = -17.79 },
                new { longitude = 31.00, latitude = -17.80 }
            },
            source = "integration-test"
        })).StatusCode);

        Guid assetId = Guid.NewGuid();
        Guid layerId = Guid.NewGuid();
        await using (PlatformDbContext db = CreateDb(tenantId))
        {
            db.DerivedMediaAssets.Add(new DerivedMediaAsset(assetId, farmId, null,
                DerivedMediaKind.SatelliteRaster, "integration-test", Guid.NewGuid(),
                $"{tenantId:N}/derived/{assetId:N}.tif", "image/tiff", 128, new string('a', 64),
                DateTimeOffset.Parse("2000-01-01T00:00:00Z")));
            db.FieldMapLayers.Add(new FieldMapLayer(layerId, farmId, fieldId, boundaryId,
                MapLayerKind.SatelliteIndex, "Expired NDVI", "integration-test", Guid.NewGuid(),
                DateTimeOffset.UtcNow.AddDays(-70), DateTimeOffset.UtcNow.AddDays(-69),
                DateTimeOffset.UtcNow.AddDays(-70), DateTimeOffset.UtcNow.AddDays(-60),
                "{\"type\":\"FeatureCollection\",\"features\":[]}", "{}", InsightQuality.Valid));
            await db.SaveChangesAsync();
        }

        HttpResponseMessage policy = await client.PostAsJsonAsync("/api/v1/analytics/retention-policies", new
        {
            rawSensorDays = 90,
            rawTelemetryDays = 90,
            failedProjectionDays = 90,
            derivedMediaDays = 30,
            expiredMapLayerDays = 30,
            nextRunAt = DateTimeOffset.UtcNow.AddMinutes(-1)
        });
        Assert.Equal(HttpStatusCode.Created, policy.StatusCode);

        var analytics = new PostgresAnalyticsMaintenanceStore(MurimiApiFactory.ConnectionString);
        Assert.True(await analytics.RunDueRetentionAsync(DateTimeOffset.UtcNow, 10, default) >= 1);
        await using (PlatformDbContext db = CreateDb(tenantId))
        {
            DerivedMediaAsset asset = await db.DerivedMediaAssets.SingleAsync(x => x.Id == assetId);
            Assert.Equal(DerivedMediaLifecycleStatus.PendingDeletion, asset.Status);
            Assert.False(await db.FieldMapLayers.AnyAsync(x => x.Id == layerId));
        }

        var lifecycle = new PostgresConnectedDataLifecycleStore(MurimiApiFactory.ConnectionString);
        ClaimedDerivedMedia? claimed = null;
        for (int index = 0; index < 50; index++)
        {
            ClaimedDerivedMedia? candidate = await lifecycle.ClaimDerivedMediaAsync(Guid.NewGuid(),
                DateTimeOffset.UtcNow, 300, 8, default);
            Assert.NotNull(candidate);
            Assert.Equal(1, await lifecycle.CompleteDerivedMediaAsync(candidate.AssetId, candidate.LeaseId,
                DateTimeOffset.UtcNow, default));
            if (candidate.AssetId == assetId) { claimed = candidate; break; }
        }
        Assert.NotNull(claimed);
        Assert.Equal(tenantId, claimed.TenantId);
        Assert.Equal(assetId, claimed.AssetId);
        await using (PlatformDbContext db = CreateDb(tenantId))
        {
            Assert.Equal(DerivedMediaLifecycleStatus.Deleted,
                (await db.DerivedMediaAssets.SingleAsync(x => x.Id == assetId)).Status);
        }
        Assert.True(await lifecycle.EnsureFuturePartitionsAsync(DateTimeOffset.UtcNow, 3, default) >= 0);
        int finalYear = DateTimeOffset.UtcNow.Year + 3;
        await using var connection = new NpgsqlConnection(MurimiApiFactory.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(
            "select to_regclass(@sensor) is not null and to_regclass(@telemetry) is not null", connection);
        command.Parameters.AddWithValue("sensor", $"connected.sensor_readings_{finalYear}");
        command.Parameters.AddWithValue("telemetry", $"connected.machinery_telemetry_{finalYear}");
        Assert.True(Convert.ToBoolean(await command.ExecuteScalarAsync()));
    }

    private static PlatformDbContext CreateDb(Guid tenantId)
    {
        var tenant = new TestTenantContext(tenantId);
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseNpgsql(MurimiApiFactory.ConnectionString, npgsql => npgsql.UseNetTopologySuite())
            .UseSnakeCaseNamingConvention()
            .AddInterceptors(new TenantConnectionInterceptor(tenant)).Options;
        return new PlatformDbContext(options, tenant);
    }

    private sealed class TestTenantContext(Guid tenantId) : ITenantContext
    {
        public Guid TenantId { get; } = tenantId;
        public bool IsResolved => true;
    }
}
