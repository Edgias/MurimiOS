using EdgIQ.MurimiOS.Infrastructure.Integrations;
using EdgIQ.MurimiOS.Application.Common.Abstractions;

using EdgIQ.MurimiOS.Infrastructure.Storage;
using Microsoft.Extensions.Options;

namespace EdgIQ.MurimiOS.Worker;
using ClaimedDerivedMedia = global::EdgIQ.MurimiOS.Infrastructure.Integrations.ClaimedDerivedMedia;
using IConnectedDataLifecycleStore = global::EdgIQ.MurimiOS.Infrastructure.Integrations.IConnectedDataLifecycleStore;
using IMediaStorage = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IMediaStorage;

public sealed class ConnectedDataLifecycleOptions
{
    public bool Enabled { get; set; } = true;
    public int PartitionYearsAhead { get; set; } = 3;
    public int PartitionCheckMinutes { get; set; } = 360;
    public int DeletionBatchSize { get; set; } = 25;
    public int DeletionLockSeconds { get; set; } = 300;
    public int MaximumDeletionAttempts { get; set; } = 8;
    public int PollMilliseconds { get; set; } = 5_000;

    public void Validate()
    {
        if (PartitionYearsAhead is < 1 or > 10 || PartitionCheckMinutes is < 5 or > 10_080
            || DeletionBatchSize is < 1 or > 500 || DeletionLockSeconds is < 30 or > 7_200
            || MaximumDeletionAttempts is < 1 or > 20 || PollMilliseconds is < 250 or > 300_000)
            throw new InvalidOperationException("Connected-data lifecycle configuration is invalid.");
    }
}

public sealed class ConnectedDataLifecycleWorker(
    IConnectedDataLifecycleStore store,
    IMediaStorage mediaStorage,
    TimeProvider timeProvider,
    IOptions<ConnectedDataLifecycleOptions> configuredOptions,
    ILogger<ConnectedDataLifecycleWorker> logger) : BackgroundService
{
    private readonly ConnectedDataLifecycleOptions options = configuredOptions.Value;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Enabled)
        {
            logger.LogInformation("Connected-data lifecycle maintenance is disabled.");
            return;
        }

        DateTimeOffset nextPartitionCheck = DateTimeOffset.MinValue;
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                DateTimeOffset now = timeProvider.GetUtcNow();
                if (now >= nextPartitionCheck)
                {
                    int partitionResult = await store.EnsureFuturePartitionsAsync(now, options.PartitionYearsAhead,
                        stoppingToken);
                    if (partitionResult < 0)
                        logger.LogError("Connected-data partition maintenance recorded {FailureCount} failed partition creations. Inspect connected.partition_maintenance_runs.", -partitionResult);
                    else if (partitionResult > 0)
                        logger.LogInformation("Created {Count} connected-data partitions.", partitionResult);
                    nextPartitionCheck = now.AddMinutes(options.PartitionCheckMinutes);
                }

                int processed = 0;
                while (processed < options.DeletionBatchSize)
                {
                    ClaimedDerivedMedia? claimed = await store.ClaimDerivedMediaAsync(Guid.NewGuid(), now,
                        options.DeletionLockSeconds, options.MaximumDeletionAttempts, stoppingToken);
                    if (claimed is null) break;
                    await DeleteAsync(claimed, stoppingToken);
                    processed++;
                }
                if (processed > 0) logger.LogInformation("Processed {Count} derived-media deletions.", processed);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Connected-data lifecycle iteration failed.");
            }

            await Task.Delay(TimeSpan.FromMilliseconds(options.PollMilliseconds), timeProvider, stoppingToken);
        }
    }

    private async Task DeleteAsync(ClaimedDerivedMedia claimed, CancellationToken cancellationToken)
    {
        try
        {
            await mediaStorage.DeleteDerivedAsync(claimed.TenantId, claimed.ObjectKey, cancellationToken);
            int completed = await store.CompleteDerivedMediaAsync(claimed.AssetId, claimed.LeaseId,
                timeProvider.GetUtcNow(), cancellationToken);
            if (completed != 1) throw new InvalidOperationException("The derived-media deletion lease was lost.");
        }
        catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
        {
            throw;
        }
        catch (Exception exception)
        {
            await store.FailDerivedMediaAsync(claimed.AssetId, claimed.LeaseId, timeProvider.GetUtcNow(),
                exception.Message, cancellationToken);
            logger.LogWarning(exception, "Derived-media deletion {AssetId} failed on attempt {Attempt}.",
                claimed.AssetId, claimed.Attempt);
        }
    }
}
