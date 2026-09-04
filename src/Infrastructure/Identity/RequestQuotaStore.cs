using EdgIQ.MurimiOS.Infrastructure.Data.Enterprise;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.Extensions.Options;
using Npgsql;
using NpgsqlTypes;

namespace EdgIQ.MurimiOS.Infrastructure.Identity;
using IRequestQuotaStore = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IRequestQuotaStore;
using RequestQuotaDecision = global::EdgIQ.MurimiOS.Application.Common.Abstractions.RequestQuotaDecision;
using TenantDatabaseRegistry = global::EdgIQ.MurimiOS.Infrastructure.Data.Enterprise.TenantDatabaseRegistry;

public sealed class RequestQuotaOptions
{
    public bool Enabled { get; set; } = true;
    public int TenantRequestLimit { get; set; } = 600;
    public int TenantWindowSeconds { get; set; } = 60;
    public int TenantDailyQuota { get; set; } = 100_000;
    public int ClientRequestLimit { get; set; } = 120;
    public int ClientWindowSeconds { get; set; } = 60;
    public int ClientDailyQuota { get; set; } = 20_000;
    public void Validate()
    {
        ValidateSet(TenantRequestLimit, TenantWindowSeconds, TenantDailyQuota);
        ValidateSet(ClientRequestLimit, ClientWindowSeconds, ClientDailyQuota);
    }
    private static void ValidateSet(int limit, int seconds, int daily)
    { if (limit is < 1 or > 1_000_000 || seconds is < 1 or > 86_400 || daily < limit || daily > 100_000_000) throw new InvalidOperationException("Request quota defaults are invalid."); }
}

public sealed class RequestQuotaStore(TenantDatabaseRegistry registry, IOptions<RequestQuotaOptions> configured)
:EdgIQ.MurimiOS.Application.Common.Abstractions.IRequestQuotaStore{
    private readonly RequestQuotaOptions options = configured.Value;

    public async Task<RequestQuotaDecision> ConsumeTenantAsync(Guid tenantId, DateTimeOffset now, CancellationToken ct)
        => await ConsumeAsync(tenantId, null, now, options.TenantRequestLimit, options.TenantWindowSeconds, options.TenantDailyQuota, ct);

    public async Task<RequestQuotaDecision> ConsumeClientAsync(Guid tenantId, Guid clientId, DateTimeOffset now, CancellationToken ct)
        => await ConsumeAsync(tenantId, clientId, now, options.ClientRequestLimit, options.ClientWindowSeconds, options.ClientDailyQuota, ct);

    private async Task<RequestQuotaDecision> ConsumeAsync(Guid tenantId, Guid? clientId, DateTimeOffset now,
        int requestLimit, int windowSeconds, int dailyQuota, CancellationToken ct)
    {
        if (!options.Enabled) return new(true, int.MaxValue, int.MaxValue, 0);
        await using var connection = new NpgsqlConnection(registry.ControlConnectionString); await connection.OpenAsync(ct);
        await using var command = new NpgsqlCommand("select * from security.consume_request_quota(@tenant,@client,@now,@request_limit,@window_seconds,@daily_quota)", connection);
        command.Parameters.AddWithValue("tenant", tenantId);
        command.Parameters.Add(new NpgsqlParameter("client", NpgsqlDbType.Uuid)
            { Value = clientId.HasValue ? clientId.Value : DBNull.Value });
        command.Parameters.AddWithValue("now", now);
        command.Parameters.AddWithValue("request_limit", requestLimit);
        command.Parameters.AddWithValue("window_seconds", windowSeconds);
        command.Parameters.AddWithValue("daily_quota", dailyQuota);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct);
        if (!await reader.ReadAsync(ct)) throw new InvalidOperationException("Quota function returned no decision.");
        return new(reader.GetBoolean(0), reader.GetInt32(1), reader.GetInt32(2), reader.GetInt32(3));
    }
}
