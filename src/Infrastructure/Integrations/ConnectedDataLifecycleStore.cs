using Npgsql;

namespace EdgIQ.MurimiOS.Infrastructure.Integrations;

public sealed record ClaimedDerivedMedia(Guid TenantId, Guid AssetId, Guid LeaseId, int Attempt, string ObjectKey);

public interface IConnectedDataLifecycleStore
{
    Task<int> EnsureFuturePartitionsAsync(DateTimeOffset now, int yearsAhead, CancellationToken cancellationToken);
    Task<ClaimedDerivedMedia?> ClaimDerivedMediaAsync(Guid leaseId, DateTimeOffset now, int lockSeconds,
        int maximumAttempts, CancellationToken cancellationToken);
    Task<int> CompleteDerivedMediaAsync(Guid assetId, Guid leaseId, DateTimeOffset completedAt,
        CancellationToken cancellationToken);
    Task<int> FailDerivedMediaAsync(Guid assetId, Guid leaseId, DateTimeOffset failedAt, string error,
        CancellationToken cancellationToken);
}

public sealed class PostgresConnectedDataLifecycleStore(string connectionString) : IConnectedDataLifecycleStore
{
    public async Task<int> EnsureFuturePartitionsAsync(DateTimeOffset now, int yearsAhead,
        CancellationToken cancellationToken) => await ScalarAsync(
        "select connected.ensure_future_partitions(@now,@years_ahead)", cancellationToken,
        new NpgsqlParameter("now", now), new NpgsqlParameter("years_ahead", yearsAhead));

    public async Task<ClaimedDerivedMedia?> ClaimDerivedMediaAsync(Guid leaseId, DateTimeOffset now, int lockSeconds,
        int maximumAttempts, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand(
            "select * from connected.claim_derived_media(@lease_id,@lock_seconds,@maximum_attempts,@now)", connection);
        command.Parameters.AddWithValue("lease_id", leaseId);
        command.Parameters.AddWithValue("lock_seconds", lockSeconds);
        command.Parameters.AddWithValue("maximum_attempts", maximumAttempts);
        command.Parameters.AddWithValue("now", now);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(cancellationToken);
        return await reader.ReadAsync(cancellationToken)
            ? new ClaimedDerivedMedia(reader.GetGuid(0), reader.GetGuid(1), reader.GetGuid(2), reader.GetInt32(3),
                reader.GetString(4))
            : null;
    }

    public Task<int> CompleteDerivedMediaAsync(Guid assetId, Guid leaseId, DateTimeOffset completedAt,
        CancellationToken cancellationToken) => ScalarAsync(
        "select connected.complete_derived_media_deletion(@asset_id,@lease_id,@completed_at)", cancellationToken,
        new NpgsqlParameter("asset_id", assetId), new NpgsqlParameter("lease_id", leaseId),
        new NpgsqlParameter("completed_at", completedAt));

    public Task<int> FailDerivedMediaAsync(Guid assetId, Guid leaseId, DateTimeOffset failedAt, string error,
        CancellationToken cancellationToken) => ScalarAsync(
        "select connected.fail_derived_media_deletion(@asset_id,@lease_id,@failed_at,@error)", cancellationToken,
        new NpgsqlParameter("asset_id", assetId), new NpgsqlParameter("lease_id", leaseId),
        new NpgsqlParameter("failed_at", failedAt),
        new NpgsqlParameter("error", error.Length <= 2_000 ? error : error[..2_000]));

    private async Task<int> ScalarAsync(string sql, CancellationToken cancellationToken,
        params NpgsqlParameter[] parameters)
    {
        await using var connection = new NpgsqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand(sql, connection);
        command.Parameters.AddRange(parameters);
        return Convert.ToInt32(await command.ExecuteScalarAsync(cancellationToken));
    }
}
