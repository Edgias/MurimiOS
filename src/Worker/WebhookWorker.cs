using EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.Extensions.Options;

namespace EdgIQ.MurimiOS.Worker;
using WebhookDispatcherOptions = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging.WebhookDispatcherOptions;
using WebhookDispatcher = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging.WebhookDispatcher;

public sealed class WebhookWorker(WebhookDispatcher dispatcher, IOptions<WebhookDispatcherOptions> options,
    ILogger<WebhookWorker> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        Guid workerId = Guid.NewGuid(); logger.LogInformation("Webhook worker {WorkerId} started.", workerId);
        while (!stoppingToken.IsCancellationRequested)
        {
            try { int count = await dispatcher.DispatchBatchAsync(workerId, stoppingToken); if (count == 0) await Task.Delay(options.Value.PollMilliseconds, stoppingToken); }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested) { break; }
            catch (Exception e) { logger.LogError(e, "Webhook worker loop failed."); await Task.Delay(options.Value.PollMilliseconds, stoppingToken); }
        }
    }
}
