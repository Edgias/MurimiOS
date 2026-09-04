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
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Analytics;
using EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EdgIQ.MurimiOS.Worker;
using AnalyticsDimensionSnapshot = global::EdgIQ.MurimiOS.Domain.AnalyticsDimensionSnapshot.AnalyticsDimensionSnapshot;
using AnalyticsForecastEvaluation = global::EdgIQ.MurimiOS.Domain.AnalyticsForecastEvaluation.AnalyticsForecastEvaluation;
using AnalyticsFact = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsFact;
using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;
using AnalyticsProjectionStatus = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionStatus;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ClaimedAnalyticsProjection = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Analytics.ClaimedAnalyticsProjection;
using IAnalyticsMaintenanceStore = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Analytics.IAnalyticsMaintenanceStore;
using AnalyticsDimensionSnapshotBuilder = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Analytics.AnalyticsDimensionSnapshotBuilder;
using AnalyticsForecastEvaluator = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Analytics.AnalyticsForecastEvaluator;
using AnalyticsProjectionEngine = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Analytics.AnalyticsProjectionEngine;

public sealed class AnalyticsMaintenanceOptions
{
    public bool Enabled { get; set; } = true;
    public int ScheduleBatchSize { get; set; } = 100;
    public int LockSeconds { get; set; } = 900;
    public int MaximumAttempts { get; set; } = 5;
    public int PollMilliseconds { get; set; } = 5_000;
    public int WarehouseRefreshMinutes { get; set; } = 15;
    public int RetentionBatchSize { get; set; } = 5;

    public void Validate()
    {
        if (ScheduleBatchSize is < 1 or > 1_000 || LockSeconds is < 30 or > 7_200
            || MaximumAttempts is < 1 or > 20 || PollMilliseconds is < 250 or > 300_000
            || WarehouseRefreshMinutes is < 1 or > 1_440 || RetentionBatchSize is < 1 or > 100)
            throw new InvalidOperationException("Analytics maintenance configuration is invalid.");
    }
}

public sealed class AnalyticsMaintenanceWorker(
    IAnalyticsMaintenanceStore store,
    IServiceScopeFactory scopeFactory,
    AnalyticsProjectionEngine projectionEngine,
    AnalyticsDimensionSnapshotBuilder dimensionBuilder,
    AnalyticsForecastEvaluator forecastEvaluator,
    TimeProvider timeProvider,
    IOptions<AnalyticsMaintenanceOptions> configuredOptions,
    ILogger<AnalyticsMaintenanceWorker> logger) : BackgroundService
{
    private readonly AnalyticsMaintenanceOptions options = configuredOptions.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Enabled)
        {
            logger.LogInformation("Analytics maintenance is disabled.");
            return;
        }

        DateTimeOffset nextWarehouseRefresh = DateTimeOffset.MinValue;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                DateTimeOffset now = timeProvider.GetUtcNow();
                int scheduled = await store.ScheduleDueAsync(now, options.ScheduleBatchSize, stoppingToken);
                if (scheduled > 0) logger.LogInformation("Scheduled {Count} analytics projection runs.", scheduled);
                int eventScheduled = await store.ScheduleEventRefreshesAsync(now, options.ScheduleBatchSize, stoppingToken);
                if (eventScheduled > 0) logger.LogInformation("Scheduled {Count} event-driven analytics refreshes.", eventScheduled);

                ClaimedAnalyticsProjection? claimed = await store.ClaimAsync(Guid.NewGuid(), now, options.LockSeconds,
                    options.MaximumAttempts, stoppingToken);
                if (claimed is not null)
                {
                    await ProcessProjectionAsync(claimed, stoppingToken);
                    continue;
                }

                if (now >= nextWarehouseRefresh)
                {
                    int aggregates = await store.RefreshWarehouseAsync(now, stoppingToken);
                    logger.LogInformation("Refreshed {Count} analytics warehouse aggregates.", aggregates);
                    nextWarehouseRefresh = now.AddMinutes(options.WarehouseRefreshMinutes);
                }

                int retentionRuns = await store.RunDueRetentionAsync(now, options.RetentionBatchSize, stoppingToken);
                if (retentionRuns > 0) logger.LogInformation("Completed {Count} analytics retention runs.", retentionRuns);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Analytics maintenance iteration failed.");
            }

            await Task.Delay(TimeSpan.FromMilliseconds(options.PollMilliseconds), timeProvider, stoppingToken);
        }
    }

    private async Task ProcessProjectionAsync(ClaimedAnalyticsProjection claimed, CancellationToken cancellationToken)
    {
        try
        {
            using IServiceScope scope = scopeFactory.CreateScope();
            scope.ServiceProvider.GetRequiredService<WorkerTenantContext>().Resolve(claimed.TenantId);
            PlatformDbContext db = scope.ServiceProvider.GetRequiredService<PlatformDbContext>();
            AnalyticsProjectionRun run = await db.AnalyticsProjectionRuns.SingleAsync(x => x.Id == claimed.RunId,
                cancellationToken);
            if (run.Status != AnalyticsProjectionStatus.Running)
                throw new InvalidOperationException("The claimed analytics projection is not running.");

            AnalyticsFact[] facts = await projectionEngine.BuildFactsAsync(run, db, cancellationToken);
            decimal quality = facts.Length == 0 ? 0 : decimal.Round(facts.Average(x => x.DataQualityPercent), 2);
            DateTimeOffset completedAt = timeProvider.GetUtcNow();
            AnalyticsDimensionSnapshot[] dimensions = dimensionBuilder.Build(run, facts);
            AnalyticsForecastEvaluation[] evaluations = await forecastEvaluator.EvaluateAsync(run, facts, db,
                completedAt, cancellationToken);
            AnalyticsProjectionRun[] priorRuns = await db.AnalyticsProjectionRuns
                .Where(x => x.FarmId == run.FarmId && x.Id != run.Id && x.Status == AnalyticsProjectionStatus.Succeeded
                    && x.SupersededAt == null && x.PeriodStart == run.PeriodStart && x.PeriodEnd == run.PeriodEnd
                    && x.ProjectionVersion == run.ProjectionVersion && x.Revision < run.Revision)
                .ToArrayAsync(cancellationToken);
            foreach (AnalyticsProjectionRun prior in priorRuns) prior.Supersede(run.Id, completedAt);
            run.Succeed(completedAt, facts.Length, quality);
            db.AnalyticsFacts.AddRange(facts);
            db.AnalyticsDimensionSnapshots.AddRange(dimensions);
            db.AnalyticsForecastEvaluations.AddRange(evaluations);
            string payload = JsonSerializer.Serialize(new
            {
                run.Id,
                run.FarmId,
                run.PeriodStart,
                run.PeriodEnd,
                facts = facts.Length,
                dimensions = dimensions.Length,
                forecastEvaluations = evaluations.Length,
                quality,
                attempt = claimed.Attempt
            });
            db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), claimed.TenantId,
                "AnalyticsProjectionCompleted", payload, completedAt));
            await db.SaveChangesAsync(cancellationToken);
            await store.CompleteAsync(run.Id, claimed.LeaseId, completedAt, cancellationToken);
            await store.RefreshWarehouseAsync(completedAt, cancellationToken);
            logger.LogInformation("Analytics projection {RunId} completed with {FactCount} facts.", run.Id, facts.Length);
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            DateTimeOffset failedAt = timeProvider.GetUtcNow();
            try
            {
                await store.FailAsync(claimed.RunId, claimed.LeaseId, failedAt, options.MaximumAttempts,
                    exception.Message, cancellationToken);
            }
            catch (Exception leaseException)
            {
                logger.LogError(leaseException, "Could not record analytics projection {RunId} failure.", claimed.RunId);
            }
            logger.LogWarning(exception, "Analytics projection {RunId} failed on attempt {Attempt}.",
                claimed.RunId, claimed.Attempt);
        }
    }
}
