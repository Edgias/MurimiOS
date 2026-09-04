using Npgsql;

namespace EdgIQ.MurimiOS.Infrastructure.Integrations;

public sealed record ClaimedIngestionJob(Guid TenantId, Guid JobId, Guid LeaseId);

public interface IScheduledIngestionStore
{
    Task<int> ScheduleDueAsync(DateTimeOffset now, int batchSize, int maximumAttempts, CancellationToken cancellationToken);
    Task<ClaimedIngestionJob?> ClaimAsync(Guid leaseId, DateTimeOffset now, int lockSeconds, CancellationToken cancellationToken);
}

public sealed class PostgresScheduledIngestionStore(string connectionString) : IScheduledIngestionStore
{
    public async Task<int> ScheduleDueAsync(DateTimeOffset now, int batchSize, int maximumAttempts, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand("select integrations.schedule_due_jobs(@now, @batch_size, @maximum_attempts)", connection);
        command.Parameters.AddWithValue("now", now);
        command.Parameters.AddWithValue("batch_size", batchSize);
        command.Parameters.AddWithValue("maximum_attempts", maximumAttempts);
        return Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
    }

    public async Task<ClaimedIngestionJob?> ClaimAsync(Guid leaseId, DateTimeOffset now, int lockSeconds, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand("select tenant_id, job_id, lease_id from integrations.claim_scheduled_job(@lease_id, @lock_seconds, @now)", connection);
        command.Parameters.AddWithValue("lease_id", leaseId);
        command.Parameters.AddWithValue("lock_seconds", lockSeconds);
        command.Parameters.AddWithValue("now", now);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken)
            ? new ClaimedIngestionJob(reader.GetGuid(0), reader.GetGuid(1), reader.GetGuid(2))
            : null;
    }
}
