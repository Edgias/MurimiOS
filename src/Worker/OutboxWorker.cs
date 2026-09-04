using EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.Extensions.Options;

namespace EdgIQ.MurimiOS.Worker;
using OutboxDispatcherOptions = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging.OutboxDispatcherOptions;
using OutboxDispatcher = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging.OutboxDispatcher;

public sealed class OutboxWorker(
    OutboxDispatcher dispatcher,
    IOptions<OutboxDispatcherOptions> options,
    ILogger<OutboxWorker> logger) : BackgroundService
{
    private readonly Guid workerId = Guid.NewGuid();

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        TimeSpan idleDelay = TimeSpan.FromMilliseconds(options.Value.PollMilliseconds);
        logger.LogInformation("Outbox worker {WorkerId} started.", workerId);

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                int claimed = await dispatcher.DispatchBatchAsync(workerId, stoppingToken);
                if (claimed == 0)
                    await Task.Delay(idleDelay, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception exception)
            {
                logger.LogError(exception, "Outbox worker {WorkerId} loop failed.", workerId);
                try
                {
                    await Task.Delay(idleDelay, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
            }
        }

        logger.LogInformation("Outbox worker {WorkerId} stopped.", workerId);
    }
}
