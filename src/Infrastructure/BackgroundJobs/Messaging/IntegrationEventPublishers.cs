using System.Text;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging;

public sealed class LoggingIntegrationEventPublisher(
    ILogger<LoggingIntegrationEventPublisher> logger) : IIntegrationEventPublisher
{
    public Task PublishAsync(OutboxEnvelope message, CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Published integration event {MessageId} ({MessageType}) for tenant {TenantId} on attempt {Attempt}.",
            message.Id, message.Type, message.TenantId, message.Attempt);
        return Task.CompletedTask;
    }
}

public sealed class RabbitMqPublisherOptions
{
    public string HostName { get; set; } = "localhost";
    public int Port { get; set; } = 5672;
    public string UserName { get; set; } = "guest";
    public string Password { get; set; } = "guest";
    public string VirtualHost { get; set; } = "/";
    public string Exchange { get; set; } = "murimios.events";
    public string AppId { get; set; } = "murimios-worker";

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(HostName))
            throw new InvalidOperationException("A RabbitMQ host name is required.");
        if (Port is < 1 or > 65_535)
            throw new InvalidOperationException("The RabbitMQ port is invalid.");
        if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
            throw new InvalidOperationException("RabbitMQ credentials are required.");
        if (string.IsNullOrWhiteSpace(VirtualHost))
            throw new InvalidOperationException("A RabbitMQ virtual host is required.");
        if (string.IsNullOrWhiteSpace(Exchange) || Exchange.Length > 255)
            throw new InvalidOperationException("A RabbitMQ exchange name is required.");
        if (string.IsNullOrWhiteSpace(AppId) || AppId.Length > 255)
            throw new InvalidOperationException("A RabbitMQ application id is required.");
    }
}

public sealed class RabbitMqIntegrationEventPublisher : IIntegrationEventPublisher, IAsyncDisposable
{
    private readonly RabbitMqPublisherOptions options;
    private readonly SemaphoreSlim initializationLock = new(1, 1);
    private IConnection? connection;
    private IChannel? channel;

    public RabbitMqIntegrationEventPublisher(RabbitMqPublisherOptions options)
    {
        options.Validate();
        this.options = options;
    }

    public async Task PublishAsync(OutboxEnvelope message, CancellationToken cancellationToken)
    {
        IChannel activeChannel = await GetChannelAsync(cancellationToken);
        var properties = new BasicProperties
        {
            Persistent = true,
            ContentType = "application/json",
            MessageId = message.Id.ToString(),
            Type = message.Type,
            AppId = options.AppId,
            Timestamp = new AmqpTimestamp(message.OccurredAt.ToUnixTimeSeconds()),
            Headers = new Dictionary<string, object?>
            {
                ["tenant_id"] = message.TenantId.ToString(),
                ["occurred_at"] = message.OccurredAt.ToString("O"),
                ["attempt"] = message.Attempt
            }!
        };
        ReadOnlyMemory<byte> body = Encoding.UTF8.GetBytes(message.Payload);
        try
        {
            await activeChannel.BasicPublishAsync(
                options.Exchange,
                RoutingKey(message.Type),
                mandatory: true,
                properties,
                body,
                cancellationToken);
        }
        catch
        {
            await ResetAsync();
            throw;
        }
    }

    private async Task<IChannel> GetChannelAsync(CancellationToken cancellationToken)
    {
        if (channel?.IsOpen == true) return channel;
        await initializationLock.WaitAsync(cancellationToken);
        try
        {
            if (channel?.IsOpen == true) return channel;
            await ResetAsync();
            var factory = new ConnectionFactory
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password,
                VirtualHost = options.VirtualHost,
                AutomaticRecoveryEnabled = true,
                TopologyRecoveryEnabled = true
            };
            connection = await factory.CreateConnectionAsync(options.AppId, cancellationToken);
            channel = await connection.CreateChannelAsync(
                new CreateChannelOptions(
                    publisherConfirmationsEnabled: true,
                    publisherConfirmationTrackingEnabled: true),
                cancellationToken);
            await channel.ExchangeDeclareAsync(
                options.Exchange,
                ExchangeType.Topic,
                durable: true,
                autoDelete: false,
                arguments: null,
                passive: false,
                noWait: false,
                cancellationToken);
            return channel;
        }
        finally
        {
            initializationLock.Release();
        }
    }

    private static string RoutingKey(string type)
    {
        string key = new(type.Trim().ToLowerInvariant()
            .Select(character => char.IsAsciiLetterOrDigit(character) || character is '.' or '-'
                ? character
                : '-')
            .ToArray());
        return string.Join('.', key.Split('.', StringSplitOptions.RemoveEmptyEntries));
    }

    private async Task ResetAsync()
    {
        if (channel is not null)
        {
            try { await channel.DisposeAsync(); } catch { }
            channel = null;
        }
        if (connection is not null)
        {
            try { await connection.DisposeAsync(); } catch { }
            connection = null;
        }
    }

    public async ValueTask DisposeAsync()
    {
        await initializationLock.WaitAsync();
        try { await ResetAsync(); }
        finally
        {
            initializationLock.Release();
            initializationLock.Dispose();
        }
    }
}
