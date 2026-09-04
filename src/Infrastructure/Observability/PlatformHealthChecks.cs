using System.Net.Sockets;
using EdgIQ.MurimiOS.Infrastructure.Storage;
using EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Npgsql;
using RabbitMQ.Client;

namespace EdgIQ.MurimiOS.Infrastructure.Observability;
using IMediaStorageHealthProbe = global::EdgIQ.MurimiOS.Infrastructure.Storage.IMediaStorageHealthProbe;
using ClamAvScannerOptions = global::EdgIQ.MurimiOS.Infrastructure.Storage.ClamAvScannerOptions;
using RabbitMqPublisherOptions = global::EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging.RabbitMqPublisherOptions;

public sealed class PostgresHealthCheck(string connectionString) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dataSource = NpgsqlDataSource.Create(connectionString);
            await using NpgsqlCommand command = dataSource.CreateCommand("select 1");
            await command.ExecuteScalarAsync(cancellationToken);
            return HealthCheckResult.Healthy("PostgreSQL is accepting commands.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Unhealthy("PostgreSQL is unavailable.", exception);
        }
    }
}

public sealed class OutboxBacklogHealthCheck(
    string connectionString,
    TimeProvider timeProvider,
    TimeSpan maximumLag) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await using var dataSource = NpgsqlDataSource.Create(connectionString);
            await using NpgsqlCommand command = dataSource.CreateCommand("select * from messaging.outbox_health()");
            await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
            await reader.ReadAsync(cancellationToken);
            long pending = reader.GetInt64(reader.GetOrdinal("pending_count"));
            long deadLetters = reader.GetInt64(reader.GetOrdinal("dead_letter_count"));
            int oldestOrdinal = reader.GetOrdinal("oldest_pending_at");
            DateTimeOffset? oldest = reader.IsDBNull(oldestOrdinal)
                ? null
                : reader.GetFieldValue<DateTimeOffset>(oldestOrdinal);
            TimeSpan lag = oldest is null ? TimeSpan.Zero : timeProvider.GetUtcNow() - oldest.Value;
            var data = new Dictionary<string, object>
            {
                ["pending"] = pending,
                ["deadLetters"] = deadLetters,
                ["oldestLagSeconds"] = Math.Max(0, lag.TotalSeconds)
            };
            return deadLetters > 0 || lag > maximumLag
                ? HealthCheckResult.Degraded("Outbox delivery requires operator attention.", data: data)
                : HealthCheckResult.Healthy("Outbox delivery is within its operational budget.", data);
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Degraded("Outbox health could not be evaluated.", exception);
        }
    }
}

public sealed class RabbitMqHealthCheck(RabbitMqPublisherOptions options) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            options.Validate();
            var factory = new ConnectionFactory
            {
                HostName = options.HostName,
                Port = options.Port,
                UserName = options.UserName,
                Password = options.Password,
                VirtualHost = options.VirtualHost,
                AutomaticRecoveryEnabled = false
            };
            await using IConnection connection = await factory.CreateConnectionAsync(
                "murimios-health", cancellationToken);
            return connection.IsOpen
                ? HealthCheckResult.Healthy("RabbitMQ is accepting connections.")
                : HealthCheckResult.Degraded("RabbitMQ connection did not open.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Degraded("RabbitMQ is unavailable; transactional records remain queued.", exception);
        }
    }
}

public sealed class MediaStorageHealthCheck(IMediaStorageHealthProbe storage) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            await storage.CheckHealthAsync(cancellationToken);
            return HealthCheckResult.Healthy("Media storage is available.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Degraded("Media storage is unavailable; record keeping remains available.", exception);
        }
    }
}

public sealed class ClamAvHealthCheck(ClamAvScannerOptions options) : IHealthCheck
{
    public async Task<HealthCheckResult> CheckHealthAsync(
        HealthCheckContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            options.Validate();
            using var client = new TcpClient();
            await client.ConnectAsync(options.Host, options.Port, cancellationToken);
            return HealthCheckResult.Healthy("ClamAV is accepting connections.");
        }
        catch (Exception exception)
        {
            return HealthCheckResult.Degraded("ClamAV is unavailable; new media completion will fail closed.", exception);
        }
    }
}
