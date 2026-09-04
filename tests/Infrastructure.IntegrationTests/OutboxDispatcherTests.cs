using EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.Extensions.Logging.Abstractions;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using OutboxEnvelope = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging.OutboxEnvelope;
using IOutboxStore = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging.IOutboxStore;
using IIntegrationEventPublisher = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging.IIntegrationEventPublisher;
using OutboxDispatcherOptions = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging.OutboxDispatcherOptions;
using OutboxDispatcher = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging.OutboxDispatcher;

public sealed class OutboxDispatcherTests
{
    [Fact]
    public void Message_leases_retry_and_dead_letters_without_losing_error_details()
    {
        DateTimeOffset now = DateTimeOffset.Parse("2026-08-29T06:00:00Z");
        var message = new OutboxMessage(Guid.NewGuid(), Guid.NewGuid(), "crop.harvested", "{}", now);
        Guid firstWorker = Guid.NewGuid();

        Assert.True(message.IsClaimable(now));
        message.Claim(firstWorker, now, TimeSpan.FromMinutes(1));
        Assert.Equal(1, message.Attempts);
        Assert.Throws<InvalidOperationException>(() =>
            message.MarkProcessed(Guid.NewGuid(), now));

        message.MarkFailed(firstWorker, "broker unavailable", now, now.AddSeconds(5), 2);
        Assert.False(message.IsClaimable(now.AddSeconds(4)));
        Assert.True(message.IsClaimable(now.AddSeconds(5)));

        Guid secondWorker = Guid.NewGuid();
        message.Claim(secondWorker, now.AddSeconds(5), TimeSpan.FromMinutes(1));
        message.MarkFailed(secondWorker, new string('x', 2_100), now.AddSeconds(6), now.AddSeconds(16), 2);

        Assert.Equal(2, message.Attempts);
        Assert.Equal(now.AddSeconds(6), message.DeadLetteredAt);
        Assert.Equal(2_000, message.LastError!.Length);
        Assert.False(message.IsClaimable(now.AddDays(1)));
    }

    [Fact]
    public async Task Dispatcher_publishes_and_completes_claimed_messages()
    {
        DateTimeOffset now = DateTimeOffset.Parse("2026-08-29T06:00:00Z");
        var clock = new TestTimeProvider(now);
        var envelope = NewEnvelope(attempt: 1);
        var store = new RecordingStore([envelope]);
        var publisher = new RecordingPublisher();
        var dispatcher = NewDispatcher(store, publisher, clock);

        int count = await dispatcher.DispatchBatchAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(1, count);
        Assert.Equal(envelope.Id, Assert.Single(publisher.Published).Id);
        Assert.Equal((envelope.Id, envelope.LockId), Assert.Single(store.Completed));
        Assert.Empty(store.Failed);
    }

    [Fact]
    public async Task Dispatcher_records_exponential_retry_after_publish_failure()
    {
        DateTimeOffset now = DateTimeOffset.Parse("2026-08-29T06:00:00Z");
        var envelope = NewEnvelope(attempt: 4);
        var store = new RecordingStore([envelope]);
        var dispatcher = NewDispatcher(store, new ThrowingPublisher(), new TestTimeProvider(now));

        int count = await dispatcher.DispatchBatchAsync(Guid.NewGuid(), CancellationToken.None);

        Assert.Equal(1, count);
        FailedCall failure = Assert.Single(store.Failed);
        Assert.Equal(now, failure.FailedAt);
        Assert.Equal(now.AddSeconds(40), failure.RetryAt);
        Assert.Equal(10, failure.MaximumAttempts);
        Assert.Contains("RabbitMQ unavailable", failure.Error);
        Assert.Empty(store.Completed);
    }

    private static OutboxDispatcher NewDispatcher(
        IOutboxStore store,
        IIntegrationEventPublisher publisher,
        TimeProvider clock) => new(
            store,
            publisher,
            new OutboxDispatcherOptions
            {
                InitialRetrySeconds = 5,
                MaximumRetrySeconds = 60,
                MaximumAttempts = 10
            },
            clock,
            NullLogger<OutboxDispatcher>.Instance);

    private static OutboxEnvelope NewEnvelope(int attempt) => new(
        Guid.NewGuid(),
        Guid.NewGuid(),
        "crop.harvested",
        "{\"harvestLotId\":\"abc\"}",
        DateTimeOffset.Parse("2026-08-29T05:59:00Z"),
        attempt,
        Guid.NewGuid());

    private sealed class TestTimeProvider(DateTimeOffset now) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => now;
    }

    private sealed class RecordingPublisher : IIntegrationEventPublisher
    {
        public List<OutboxEnvelope> Published { get; } = [];

        public Task PublishAsync(OutboxEnvelope message, CancellationToken cancellationToken)
        {
            Published.Add(message);
            return Task.CompletedTask;
        }
    }

    private sealed class ThrowingPublisher : IIntegrationEventPublisher
    {
        public Task PublishAsync(OutboxEnvelope message, CancellationToken cancellationToken) =>
            throw new InvalidOperationException("RabbitMQ unavailable");
    }

    private sealed record FailedCall(
        Guid MessageId,
        Guid LockId,
        string Error,
        DateTimeOffset FailedAt,
        DateTimeOffset RetryAt,
        int MaximumAttempts);

    private sealed class RecordingStore(IReadOnlyList<OutboxEnvelope> messages) : IOutboxStore
    {
        public List<(Guid MessageId, Guid LockId)> Completed { get; } = [];
        public List<FailedCall> Failed { get; } = [];

        public Task<IReadOnlyList<OutboxEnvelope>> ClaimAsync(
            Guid workerId,
            int batchSize,
            TimeSpan lockDuration,
            DateTimeOffset now,
            CancellationToken cancellationToken) => Task.FromResult(messages);

        public Task<bool> MarkProcessedAsync(
            Guid messageId,
            Guid lockId,
            DateTimeOffset processedAt,
            CancellationToken cancellationToken)
        {
            Completed.Add((messageId, lockId));
            return Task.FromResult(true);
        }

        public Task<bool> MarkFailedAsync(
            Guid messageId,
            Guid lockId,
            string error,
            DateTimeOffset failedAt,
            DateTimeOffset retryAt,
            int maximumAttempts,
            CancellationToken cancellationToken)
        {
            Failed.Add(new FailedCall(messageId, lockId, error, failedAt, retryAt, maximumAttempts));
            return Task.FromResult(true);
        }
    }
}
