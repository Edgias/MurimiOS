using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.BenchmarkCohortDefinition;
using EdgIQ.MurimiOS.Domain.AnalyticsRefreshRequest;
using EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot;
using EdgIQ.MurimiOS.Domain.PredictiveModelDefinition;
using EdgIQ.MurimiOS.Domain.AnalyticsForecastEvaluation;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using EdgIQ.MurimiOS.Domain.KpiDefinition;
using EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule;
using EdgIQ.MurimiOS.Domain.BenchmarkEnrollment;
using EdgIQ.MurimiOS.Domain.AnalyticsRetentionPolicy;
using EdgIQ.MurimiOS.Domain.Common;


using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;

using EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Analytics;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace EdgIQ.MurimiOS.API.FunctionalTests;
using AnalyticsFact = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFact;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;
using AnalyticsScheduleFrequency = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule.AnalyticsScheduleFrequency;
using AnalyticsRefreshRequest = global::EdgIQ.MurimiOS.Domain.AnalyticsRefreshRequest.AnalyticsRefreshRequest;
using AnalyticsRefreshRequestStatus = global::EdgIQ.MurimiOS.Domain.AnalyticsRefreshRequest.AnalyticsRefreshRequestStatus;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;
using PostgresAnalyticsMaintenanceStore = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Analytics.PostgresAnalyticsMaintenanceStore;
using AnalyticsProjectionEngine = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Analytics.AnalyticsProjectionEngine;
using TenantConnectionInterceptor = global::EdgIQ.MurimiOS.Infrastructure.Data.Interceptors.TenantConnectionInterceptor;

public sealed class AnalyticsEndToEndTests
{
    [IntegrationFact]
    public async Task Farm_domain_event_is_debounced_and_scheduled_as_a_revisioned_projection()
    {
        await using var factory = new MurimiApiFactory();
        Guid tenantId = Guid.NewGuid();
        Guid farmId = Guid.NewGuid();
        Guid outboxId = Guid.NewGuid();
        using HttpClient client = factory.CreateTenantClient(tenantId);
        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync("/api/v1/farms", new
        {
            id = farmId,
            name = "Event Refresh Integration Farm",
            code = $"ER{farmId:N}"[..12],
            countryCode = "ZW",
            timeZone = "Africa/Harare"
        })).StatusCode);
        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync($"/api/v1/analytics/farms/{farmId}/schedules", new
        {
            name = "Event-driven projection",
            frequency = (int)AnalyticsScheduleFrequency.Daily,
            lookbackDays = 45,
            currency = "USD",
            projectionVersion = "event-integration-v1",
            nextRunAt = DateTimeOffset.UtcNow.AddDays(1)
        })).StatusCode);

        await using (var connection = new NpgsqlConnection(MurimiApiFactory.ConnectionString))
        {
            await connection.OpenAsync();
            await using (var tenant = new NpgsqlCommand("select set_config('app.tenant_id',@tenant,false)", connection))
            {
                tenant.Parameters.AddWithValue("tenant", tenantId.ToString());
                await tenant.ExecuteNonQueryAsync();
            }
            await using var insert = new NpgsqlCommand("""
                insert into messaging.outbox_messages(
                    id,tenant_id,type,payload,occurred_at,processed_at,available_at,lock_id,locked_until,
                    dead_lettered_at,attempts,last_error)
                values(@id,@tenant,'CropPlan.FinancialRecorded',cast(@payload as jsonb),@occurred,null,@occurred,
                    null,null,null,0,null)
                """, connection);
            insert.Parameters.AddWithValue("id", outboxId);
            insert.Parameters.AddWithValue("tenant", tenantId);
            insert.Parameters.AddWithValue("payload", JsonSerializer.Serialize(new { farmId, amount = 125m }));
            insert.Parameters.AddWithValue("occurred", DateTimeOffset.UtcNow);
            Assert.Equal(1, await insert.ExecuteNonQueryAsync());
        }

        var store = new PostgresAnalyticsMaintenanceStore(MurimiApiFactory.ConnectionString);
        Assert.True(await store.ScheduleEventRefreshesAsync(DateTimeOffset.UtcNow.AddMinutes(1), 100, default) >= 1);
        await using PlatformDbContext db = CreateDb(tenantId);
        AnalyticsRefreshRequest request = await db.AnalyticsRefreshRequests.SingleAsync(x => x.SourceOutboxMessageId == outboxId);
        AnalyticsProjectionRun run = await db.AnalyticsProjectionRuns.SingleAsync(x => x.Id == request.ScheduledRunId);

        Assert.Equal(AnalyticsRefreshRequestStatus.Scheduled, request.Status);
        Assert.Equal("event", run.TriggerType);
        Assert.Equal(1, run.Revision);
        Assert.Equal(request.Id, run.SourceRefreshRequestId);
    }

    [IntegrationFact]
    public async Task Api_to_projection_to_warehouse_and_retention_round_trip_succeeds()
    {
        await using var factory = new MurimiApiFactory();
        Guid tenantId = Guid.NewGuid();
        Guid farmId = Guid.NewGuid();
        using HttpClient client = factory.CreateTenantClient(tenantId);

        HttpResponseMessage farmResponse = await client.PostAsJsonAsync("/api/v1/farms", new
        {
            id = farmId,
            name = "Analytics Integration Farm",
            code = $"AI{farmId:N}"[..12],
            countryCode = "ZW",
            timeZone = "Africa/Harare"
        });
        Assert.Equal(HttpStatusCode.Created, farmResponse.StatusCode);

        HttpResponseMessage projectionResponse = await client.PostAsJsonAsync($"/api/v1/analytics/farms/{farmId}/projections", new
        {
            periodStart = new DateOnly(2026, 1, 1),
            periodEnd = new DateOnly(2026, 8, 31),
            currency = "USD",
            projectionVersion = "integration-v1",
            scheduledFor = DateTimeOffset.Parse("2000-01-01T00:00:00Z")
        });
        Assert.Equal(HttpStatusCode.Created, projectionResponse.StatusCode);
        Guid runId = (await projectionResponse.Content.ReadFromJsonAsync<JsonElement>()).GetProperty("id").GetGuid();

        var store = new PostgresAnalyticsMaintenanceStore(MurimiApiFactory.ConnectionString);
        await using PlatformDbContext db = CreateDb(tenantId);
        AnalyticsProjectionRun run = await db.AnalyticsProjectionRuns.SingleAsync(x => x.Id == runId);
        run.Start(DateTimeOffset.UtcNow);
        AnalyticsFact[] facts = await new AnalyticsProjectionEngine().BuildFactsAsync(run, db, default);
        decimal quality = facts.Length == 0 ? 0 : decimal.Round(facts.Average(x => x.DataQualityPercent), 2);
        DateTimeOffset completedAt = DateTimeOffset.UtcNow;
        db.AnalyticsFacts.AddRange(facts);
        run.Succeed(completedAt, facts.Length, quality);
        await db.SaveChangesAsync();
        Assert.True(await store.RefreshWarehouseAsync(completedAt, default) >= 3);

        JsonElement projectedFacts = await client.GetFromJsonAsync<JsonElement>($"/api/v1/analytics/farms/{farmId}/facts");
        Assert.True(projectedFacts.GetArrayLength() >= 3);
        Assert.Contains(projectedFacts.EnumerateArray(), x => x.GetProperty("metricCode").GetString() == "farm.gross_margin");

        HttpResponseMessage scheduleResponse = await client.PostAsJsonAsync($"/api/v1/analytics/farms/{farmId}/schedules", new
        {
            name = "Nightly integration schedule",
            frequency = (int)AnalyticsScheduleFrequency.Daily,
            lookbackDays = 30,
            currency = "USD",
            projectionVersion = "scheduled-integration-v1",
            nextRunAt = DateTimeOffset.UtcNow.AddMinutes(-1)
        });
        Assert.Equal(HttpStatusCode.Created, scheduleResponse.StatusCode);
        Assert.True(await store.ScheduleDueAsync(DateTimeOffset.UtcNow, 10, default) >= 1);

        HttpResponseMessage retentionResponse = await client.PostAsJsonAsync("/api/v1/analytics/retention-policies", new
        {
            rawSensorDays = 90,
            rawTelemetryDays = 180,
            failedProjectionDays = 365,
            nextRunAt = DateTimeOffset.UtcNow.AddMinutes(-1)
        });
        Assert.Equal(HttpStatusCode.Created, retentionResponse.StatusCode);
        Assert.True(await store.RunDueRetentionAsync(DateTimeOffset.UtcNow, 10, default) >= 1);
        JsonElement policies = await client.GetFromJsonAsync<JsonElement>("/api/v1/analytics/retention-policies");
        Assert.NotEqual(JsonValueKind.Null, policies[0].GetProperty("lastRunAt").ValueKind);
    }

    private static PlatformDbContext CreateDb(Guid tenantId)
    {
        var tenant = new TestTenantContext(tenantId);
        var interceptor = new TenantConnectionInterceptor(tenant);
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseNpgsql(MurimiApiFactory.ConnectionString, npgsql => npgsql.UseNetTopologySuite())
            .UseSnakeCaseNamingConvention()
            .AddInterceptors(interceptor)
            .Options;
        return new PlatformDbContext(options, tenant);
    }

    private sealed class TestTenantContext(Guid tenantId) : ITenantContext
    {
        public Guid TenantId { get; } = tenantId;
        public bool IsResolved => true;
    }
}
