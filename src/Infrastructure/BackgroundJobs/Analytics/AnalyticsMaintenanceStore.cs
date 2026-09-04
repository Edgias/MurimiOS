using Npgsql;

namespace EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Analytics;

public sealed record ClaimedAnalyticsProjection(Guid TenantId, Guid RunId, Guid LeaseId, int Attempt);

public interface IAnalyticsMaintenanceStore
{
    Task<int> ScheduleDueAsync(DateTimeOffset now, int batchSize, CancellationToken cancellationToken);
    Task<int> ScheduleEventRefreshesAsync(DateTimeOffset now, int batchSize, CancellationToken cancellationToken);
    Task<ClaimedAnalyticsProjection?> ClaimAsync(Guid leaseId, DateTimeOffset now, int lockSeconds,
        int maximumAttempts, CancellationToken cancellationToken);
    Task CompleteAsync(Guid runId, Guid leaseId, DateTimeOffset completedAt, CancellationToken cancellationToken);
    Task FailAsync(Guid runId, Guid leaseId, DateTimeOffset failedAt, int maximumAttempts, string error,
        CancellationToken cancellationToken);
    Task<int> RefreshWarehouseAsync(DateTimeOffset now, CancellationToken cancellationToken);
    Task<int> RunDueRetentionAsync(DateTimeOffset now, int batchSize, CancellationToken cancellationToken);
}

public sealed class PostgresAnalyticsMaintenanceStore(string connectionString) : IAnalyticsMaintenanceStore
{
    public async Task<int> ScheduleDueAsync(DateTimeOffset now, int batchSize, CancellationToken cancellationToken) =>
        await ScalarAsync("select analytics.schedule_due_projections(@now,@batch_size)", cancellationToken,
            new NpgsqlParameter("now", now), new NpgsqlParameter("batch_size", batchSize));

    public async Task<int> ScheduleEventRefreshesAsync(DateTimeOffset now, int batchSize,
        CancellationToken cancellationToken) =>
        await ScalarAsync("select analytics.schedule_event_refreshes(@now,@batch_size)", cancellationToken,
            new NpgsqlParameter("now", now), new NpgsqlParameter("batch_size", batchSize));

    public async Task<ClaimedAnalyticsProjection?> ClaimAsync(Guid leaseId, DateTimeOffset now, int lockSeconds,
        int maximumAttempts, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand(
            "select * from analytics.claim_projection(@lease_id,@lock_seconds,@maximum_attempts,@now)", connection);
        command.Parameters.AddWithValue("lease_id", leaseId);
        command.Parameters.AddWithValue("lock_seconds", lockSeconds);
        command.Parameters.AddWithValue("maximum_attempts", maximumAttempts);
        command.Parameters.AddWithValue("now", now);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken)
            ? new ClaimedAnalyticsProjection(reader.GetGuid(0), reader.GetGuid(1), reader.GetGuid(2), reader.GetInt32(3))
            : null;
    }

    public async Task CompleteAsync(Guid runId, Guid leaseId, DateTimeOffset completedAt,
        CancellationToken cancellationToken) =>
        _ = await ScalarAsync("select analytics.complete_projection(@run_id,@lease_id,@completed_at)", cancellationToken,
            new NpgsqlParameter("run_id", runId), new NpgsqlParameter("lease_id", leaseId),
            new NpgsqlParameter("completed_at", completedAt));

    public async Task FailAsync(Guid runId, Guid leaseId, DateTimeOffset failedAt, int maximumAttempts,
        string error, CancellationToken cancellationToken) =>
        _ = await ScalarAsync("select analytics.fail_projection(@run_id,@lease_id,@failed_at,@maximum_attempts,@error)", cancellationToken,
            new NpgsqlParameter("run_id", runId), new NpgsqlParameter("lease_id", leaseId),
            new NpgsqlParameter("failed_at", failedAt), new NpgsqlParameter("maximum_attempts", maximumAttempts),
            new NpgsqlParameter("error", error.Length <= 2_000 ? error : error[..2_000]));

    public async Task<int> RefreshWarehouseAsync(DateTimeOffset now, CancellationToken cancellationToken) =>
        await ScalarAsync("select analytics.refresh_warehouse(@now)", cancellationToken, new NpgsqlParameter("now", now));

    public async Task<int> RunDueRetentionAsync(DateTimeOffset now, int batchSize, CancellationToken cancellationToken) =>
        await ScalarAsync("select analytics.run_due_retention(@now,@batch_size)", cancellationToken,
            new NpgsqlParameter("now", now), new NpgsqlParameter("batch_size", batchSize));

    private async Task<int> ScalarAsync(string sql, CancellationToken cancellationToken, params NpgsqlParameter[] parameters)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddRange(parameters);
        return Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
    }
}
