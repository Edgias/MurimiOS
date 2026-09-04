using EdgIQ.MurimiOS.Infrastructure.Data.Enterprise;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.Extensions.Options;

namespace EdgIQ.MurimiOS.Worker;
using TenantDatabaseRouteResolver = global::EdgIQ.MurimiOS.Infrastructure.Data.Enterprise.TenantDatabaseRouteResolver;
using TenantMigrationExecutionOptions = global::EdgIQ.MurimiOS.Infrastructure.Data.Enterprise.TenantMigrationExecutionOptions;
using TenantMigrationWorkItem = global::EdgIQ.MurimiOS.Infrastructure.Data.Enterprise.TenantMigrationWorkItem;
using TenantExportResult = global::EdgIQ.MurimiOS.Infrastructure.Data.Enterprise.TenantExportResult;
using ITenantMigrationControlStore = global::EdgIQ.MurimiOS.Infrastructure.Data.Enterprise.ITenantMigrationControlStore;
using TenantMigrationArtifactService = global::EdgIQ.MurimiOS.Infrastructure.Data.Enterprise.TenantMigrationArtifactService;

public sealed class TenantMigrationWorker(ITenantMigrationControlStore store, TenantMigrationArtifactService artifacts,
    TenantDatabaseRouteResolver routes, IOptions<TenantMigrationExecutionOptions> configured, TimeProvider timeProvider,
    ILogger<TenantMigrationWorker> logger) : BackgroundService
{
    private readonly TenantMigrationExecutionOptions options = configured.Value;
    private readonly Guid workerId = Guid.NewGuid();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!options.Enabled) { logger.LogInformation("Tenant migration execution is disabled."); return; }
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                TenantMigrationWorkItem? item = await store.ClaimAsync(workerId, options.LeaseSeconds,
                    options.MaximumAttempts, timeProvider.GetUtcNow(), stoppingToken);
                if (item is null) { await Task.Delay(options.PollMilliseconds, stoppingToken); continue; }
                await ExecuteAsync(item, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception exception)
            {
                logger.LogError(exception, "Tenant migration discovery failed.");
                await Task.Delay(options.PollMilliseconds, stoppingToken);
            }
        }
    }

    private async Task ExecuteAsync(TenantMigrationWorkItem item, CancellationToken ct)
    {
        try
        {
            if (item.Status == 2)
            {
                logger.LogInformation("Exporting tenant {TenantId} for migration {JobId}.", item.TenantId, item.JobId);
                if (options.WriteDrainSeconds > 0 && item.Attempt == 1)
                    await Task.Delay(TimeSpan.FromSeconds(options.WriteDrainSeconds), ct);
                TenantExportResult export = await artifacts.ExportAsync(item, ct);
                await store.MarkExportedAsync(item, export, timeProvider.GetUtcNow(), ct);
                item = item with { Status = 3, ArtifactObjectKey = export.ObjectKey, ArtifactSha256 = export.Sha256, ManifestJson = export.ManifestJson };
            }
            if (item.Status == 3)
            {
                await artifacts.ValidateAsync(item, ct);
                await store.MarkValidatedAsync(item, timeProvider.GetUtcNow(), ct);
                item = item with { Status = 4 };
            }
            if (item.Status == 4)
            {
                await store.MarkImportingAsync(item, timeProvider.GetUtcNow(), ct);
                item = item with { Status = 5 };
            }
            if (item.Status == 5)
            {
                logger.LogInformation("Importing tenant {TenantId} for migration {JobId}.", item.TenantId, item.JobId);
                await artifacts.ImportAsync(item, ct);
                await store.CompleteAsync(item, timeProvider.GetUtcNow(), ct);
                routes.Invalidate(item.TenantId);
                logger.LogInformation("Tenant migration {JobId} completed and routing was cut over to {DatabaseKey}.",
                    item.JobId, item.DestinationDatabaseKey);
            }
        }
        catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
        catch (Exception exception)
        {
            string code = exception is InvalidDataException ? "migration_validation_failed" : "migration_execution_failed";
            logger.LogError(exception, "Tenant migration {JobId} failed at status {Status}.", item.JobId, item.Status);
            await store.FailAsync(item, code, exception.Message, timeProvider.GetUtcNow(), options.MaximumAttempts, CancellationToken.None);
        }
    }
}
