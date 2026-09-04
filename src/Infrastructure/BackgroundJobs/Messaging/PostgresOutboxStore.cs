using Npgsql;
using NpgsqlTypes;

namespace EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging;

public sealed class PostgresOutboxStore : IOutboxStore, IAsyncDisposable
{
    private readonly NpgsqlDataSource dataSource;

    public PostgresOutboxStore(string connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
            throw new ArgumentException("A worker database connection is required.", nameof(connectionString));
        dataSource = NpgsqlDataSource.Create(connectionString);
    }

    public async Task<IReadOnlyList<OutboxEnvelope>> ClaimAsync(
        Guid workerId,
        int batchSize,
        TimeSpan lockDuration,
        DateTimeOffset now,
        CancellationToken cancellationToken)
    {
        await using NpgsqlCommand command = dataSource.CreateCommand(
            "select * from messaging.claim_outbox(@worker_id, @batch_size, @now, @lock_seconds)");
        command.Parameters.AddWithValue("worker_id", NpgsqlDbType.Uuid, workerId);
        command.Parameters.AddWithValue("batch_size", NpgsqlDbType.Integer, batchSize);
        command.Parameters.AddWithValue("now", NpgsqlDbType.TimestampTz, now.ToUniversalTime());
        command.Parameters.AddWithValue("lock_seconds", NpgsqlDbType.Integer, checked((int)lockDuration.TotalSeconds));

        var messages = new List<OutboxEnvelope>(batchSize);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        while (await reader.ReadAsync(cancellationToken))
        {
            messages.Add(new OutboxEnvelope(
                reader.GetGuid(reader.GetOrdinal("id")),
                reader.GetGuid(reader.GetOrdinal("tenant_id")),
                reader.GetString(reader.GetOrdinal("type")),
                reader.GetString(reader.GetOrdinal("payload")),
                reader.GetFieldValue<DateTimeOffset>(reader.GetOrdinal("occurred_at")),
                reader.GetInt32(reader.GetOrdinal("attempts")),
                reader.GetGuid(reader.GetOrdinal("lock_id"))));
        }
        return messages;
    }

    public Task<bool> MarkProcessedAsync(
        Guid messageId,
        Guid lockId,
        DateTimeOffset processedAt,
        CancellationToken cancellationToken) => ExecuteBooleanAsync(
            "select messaging.complete_outbox(@message_id, @lock_id, @at)",
            messageId,
            lockId,
            processedAt,
            null,
            null,
            null,
            cancellationToken);

    public Task<bool> MarkFailedAsync(
        Guid messageId,
        Guid lockId,
        string error,
        DateTimeOffset failedAt,
        DateTimeOffset retryAt,
        int maximumAttempts,
        CancellationToken cancellationToken) => ExecuteBooleanAsync(
            "select messaging.fail_outbox(@message_id, @lock_id, @failed_at, @retry_at, @error, @maximum_attempts)",
            messageId,
            lockId,
            failedAt,
            retryAt,
            error,
            maximumAttempts,
            cancellationToken);

    private async Task<bool> ExecuteBooleanAsync(
        string sql,
        Guid messageId,
        Guid lockId,
        DateTimeOffset at,
        DateTimeOffset? retryAt,
        string? error,
        int? maximumAttempts,
        CancellationToken cancellationToken)
    {
        await using NpgsqlCommand command = dataSource.CreateCommand(sql);
        command.Parameters.AddWithValue("message_id", NpgsqlDbType.Uuid, messageId);
        command.Parameters.AddWithValue("lock_id", NpgsqlDbType.Uuid, lockId);
        if (retryAt is null)
            command.Parameters.AddWithValue("at", NpgsqlDbType.TimestampTz, at.ToUniversalTime());
        else
        {
            command.Parameters.AddWithValue("failed_at", NpgsqlDbType.TimestampTz, at.ToUniversalTime());
            command.Parameters.AddWithValue("retry_at", NpgsqlDbType.TimestampTz, retryAt.Value.ToUniversalTime());
        }
        if (error is not null) command.Parameters.AddWithValue("error", NpgsqlDbType.Text, error);
        if (maximumAttempts is not null) command.Parameters.AddWithValue("maximum_attempts", NpgsqlDbType.Integer, maximumAttempts.Value);
        object? result = await command.ExecuteScalarAsync(cancellationToken);
        return result is true;
    }

    public ValueTask DisposeAsync() => dataSource.DisposeAsync();
}
