using Microsoft.Extensions.Logging;
using System.Diagnostics;
using EdgIQ.MurimiOS.Application.Common.Observability;

namespace EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging;
using PlatformDiagnostics = global::EdgIQ.MurimiOS.Application.Common.Observability.PlatformDiagnostics;

public sealed record OutboxEnvelope(
    Guid Id,
    Guid TenantId,
    string Type,
    string Payload,
    DateTimeOffset OccurredAt,
    int Attempt,
    Guid LockId);

public interface IOutboxStore
{
    Task<IReadOnlyList<OutboxEnvelope>> ClaimAsync(
        Guid workerId,
        int batchSize,
        TimeSpan lockDuration,
        DateTimeOffset now,
        CancellationToken cancellationToken);

    Task<bool> MarkProcessedAsync(
        Guid messageId,
        Guid lockId,
        DateTimeOffset processedAt,
        CancellationToken cancellationToken);

    Task<bool> MarkFailedAsync(
        Guid messageId,
        Guid lockId,
        string error,
        DateTimeOffset failedAt,
        DateTimeOffset retryAt,
        int maximumAttempts,
        CancellationToken cancellationToken);
}

public interface IIntegrationEventPublisher
{
    Task PublishAsync(OutboxEnvelope message, CancellationToken cancellationToken);
}

public sealed class OutboxDispatcherOptions
{
    public int BatchSize { get; set; } = 50;
    public int LockSeconds { get; set; } = 60;
    public int MaximumAttempts { get; set; } = 10;
    public int InitialRetrySeconds { get; set; } = 5;
    public int MaximumRetrySeconds { get; set; } = 3_600;
    public int PollMilliseconds { get; set; } = 1_000;

    public void Validate()
    {
        if (BatchSize is < 1 or > 500) throw new InvalidOperationException("Outbox batch size must be between 1 and 500.");
        if (LockSeconds is < 5 or > 3_600) throw new InvalidOperationException("Outbox lock duration must be between 5 and 3600 seconds.");
        if (MaximumAttempts is < 1 or > 100) throw new InvalidOperationException("Outbox maximum attempts must be between 1 and 100.");
        if (InitialRetrySeconds is < 1 or > 3_600 || MaximumRetrySeconds < InitialRetrySeconds)
            throw new InvalidOperationException("Outbox retry delays are invalid.");
        if (PollMilliseconds is < 100 or > 60_000) throw new InvalidOperationException("Outbox poll interval must be between 100 and 60000 milliseconds.");
    }
}

public sealed class OutboxDispatcher(
    IOutboxStore store,
    IIntegrationEventPublisher publisher,
    OutboxDispatcherOptions options,
    TimeProvider timeProvider,
    ILogger<OutboxDispatcher> logger)
{
    public async Task<int> DispatchBatchAsync(Guid workerId, CancellationToken cancellationToken)
    {
        options.Validate();
        DateTimeOffset now = timeProvider.GetUtcNow();
        IReadOnlyList<OutboxEnvelope> messages = await store.ClaimAsync(
            workerId,
            options.BatchSize,
            TimeSpan.FromSeconds(options.LockSeconds),
            now,
            cancellationToken);

        foreach (OutboxEnvelope message in messages)
        {
            using Activity? activity = PlatformDiagnostics.Activities.StartActivity("outbox.publish", ActivityKind.Producer);
            activity?.SetTag("messaging.message.id", message.Id);
            activity?.SetTag("messaging.message.type", message.Type);
            activity?.SetTag("tenant.id", message.TenantId);
            activity?.SetTag("murimios.module", "messaging");
            activity?.SetTag("outbox.attempt", message.Attempt);
            try
            {
                await publisher.PublishAsync(message, cancellationToken);
                bool completed = await store.MarkProcessedAsync(
                    message.Id, message.LockId, timeProvider.GetUtcNow(), cancellationToken);
                if (!completed)
                    logger.LogWarning("Outbox lock was lost before completion for message {MessageId}.", message.Id);
                else
                {
                    PlatformDiagnostics.OutboxPublished.Add(1,
                        new KeyValuePair<string, object?>("message.type", message.Type));
                    PlatformDiagnostics.OutboxLagSeconds.Record(
                        Math.Max(0, (timeProvider.GetUtcNow() - message.OccurredAt).TotalSeconds),
                        new KeyValuePair<string, object?>("message.type", message.Type));
                    activity?.SetStatus(ActivityStatusCode.Ok);
                }
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception)
            {
                activity?.SetStatus(ActivityStatusCode.Error, exception.Message);
                PlatformDiagnostics.OutboxFailures.Add(1,
                    new KeyValuePair<string, object?>("message.type", message.Type));
                TimeSpan delay = RetryDelay(message.Attempt);
                DateTimeOffset failedAt = timeProvider.GetUtcNow();
                await store.MarkFailedAsync(
                    message.Id,
                    message.LockId,
                    exception.ToString(),
                    failedAt,
                    failedAt.Add(delay),
                    options.MaximumAttempts,
                    cancellationToken);
                logger.LogError(exception,
                    "Publishing outbox message {MessageId} failed on attempt {Attempt}; retry delay is {RetryDelay}.",
                    message.Id, message.Attempt, delay);
            }
        }

        return messages.Count;
    }

    public TimeSpan RetryDelay(int attempt)
    {
        int exponent = Math.Clamp(attempt - 1, 0, 30);
        double seconds = options.InitialRetrySeconds * Math.Pow(2, exponent);
        return TimeSpan.FromSeconds(Math.Min(seconds, options.MaximumRetrySeconds));
    }
}
