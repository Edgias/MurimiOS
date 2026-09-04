using EdgIQ.MurimiOS.Application.Common.Abstractions;
using System.Collections.Concurrent;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Npgsql;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Enterprise;
using ITenantDatabaseRegistry = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantDatabaseRegistry;
using ITenantDatabaseRouteResolver = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantDatabaseRouteResolver;

public sealed class TenantDatabaseOptions
{
    public string ControlDatabaseKey { get; set; } = "primary";
    public Dictionary<string, string> Connections { get; set; } = new(StringComparer.OrdinalIgnoreCase);
    public int RouteCacheSeconds { get; set; } = 15;

    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ControlDatabaseKey)) throw new InvalidOperationException("TenantDatabases:ControlDatabaseKey is required.");
        if (RouteCacheSeconds is < 1 or > 300) throw new InvalidOperationException("TenantDatabases:RouteCacheSeconds must be between 1 and 300.");
    }
}

public sealed class TenantDatabaseRegistry
:EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantDatabaseRegistry{
    private readonly Dictionary<string, string> connections = new(StringComparer.OrdinalIgnoreCase);

    public TenantDatabaseRegistry(IOptions<TenantDatabaseOptions> configured, IConfiguration configuration,
        string controlConnectionString)
    {
        TenantDatabaseOptions options = configured.Value;
        options.Validate();
        ControlDatabaseKey = options.ControlDatabaseKey.Trim();
        connections[ControlDatabaseKey] = ValidateConnectionString(controlConnectionString, ControlDatabaseKey);
        foreach ((string key, string reference) in options.Connections)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(reference))
                throw new InvalidOperationException("Tenant database keys and connection references cannot be blank.");
            string? value = configuration.GetConnectionString(reference.Trim());
            connections[key.Trim()] = ValidateConnectionString(value ?? reference, key);
        }
    }

    public string ControlDatabaseKey { get; }
    public IReadOnlyCollection<string> DatabaseKeys => connections.Keys;
    public bool Contains(string databaseKey) => !string.IsNullOrWhiteSpace(databaseKey) && connections.ContainsKey(databaseKey);
    public string GetConnectionString(string databaseKey) => connections.TryGetValue(databaseKey, out string? value)
        ? value : throw new InvalidOperationException($"Tenant database key '{databaseKey}' is not configured on this host.");
    public string ControlConnectionString => GetConnectionString(ControlDatabaseKey);

    private static string ValidateConnectionString(string connectionString, string key)
    {
        try
        {
            var parsed = new NpgsqlConnectionStringBuilder(connectionString);
            if (string.IsNullOrWhiteSpace(parsed.Host) || string.IsNullOrWhiteSpace(parsed.Database)) throw new ArgumentException();
            return parsed.ConnectionString;
        }
        catch (Exception exception) when (exception is ArgumentException or FormatException)
        {
            throw new InvalidOperationException($"Tenant database key '{key}' does not resolve to a valid PostgreSQL connection string.", exception);
        }
    }
}

public sealed class TenantDatabaseRouteResolver(TenantDatabaseRegistry registry, IOptions<TenantDatabaseOptions> options,
    TimeProvider timeProvider)
:EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantDatabaseRouteResolver{
    private sealed record CacheEntry(string DatabaseKey, DateTimeOffset ExpiresAt);
    private readonly ConcurrentDictionary<Guid, CacheEntry> cache = new();
    private readonly TimeSpan cacheDuration = TimeSpan.FromSeconds(options.Value.RouteCacheSeconds);

    public string ResolveConnectionString(Guid tenantId)
        => registry.GetConnectionString(ResolveDatabaseKeyAsync(tenantId, CancellationToken.None).GetAwaiter().GetResult());

    public async Task<string> ResolveConnectionStringAsync(Guid tenantId, CancellationToken cancellationToken)
        => registry.GetConnectionString(await ResolveDatabaseKeyAsync(tenantId, cancellationToken));

    public void Invalidate(Guid tenantId) => cache.TryRemove(tenantId, out _);

    public async Task<bool> IsWriteBlockedAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        await using var connection = new NpgsqlConnection(registry.ControlConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand("select enterprise.tenant_migration_write_blocked(@tenant_id)", connection);
        command.Parameters.AddWithValue("tenant_id", tenantId);
        return await command.ExecuteScalarAsync(cancellationToken) is true;
    }

    private async Task<string> ResolveDatabaseKeyAsync(Guid tenantId, CancellationToken cancellationToken)
    {
        if (tenantId == Guid.Empty) return registry.ControlDatabaseKey;
        DateTimeOffset now = timeProvider.GetUtcNow();
        if (cache.TryGetValue(tenantId, out CacheEntry? existing) && existing.ExpiresAt > now) return existing.DatabaseKey;
        await using var connection = new NpgsqlConnection(registry.ControlConnectionString);
        await connection.OpenAsync(cancellationToken);
        await using var command = new NpgsqlCommand("select enterprise.resolve_tenant_database_key(@tenant_id)", connection);
        command.Parameters.AddWithValue("tenant_id", tenantId);
        string key = await command.ExecuteScalarAsync(cancellationToken) as string ?? registry.ControlDatabaseKey;
        if (!registry.Contains(key)) throw new InvalidOperationException($"Active tenant placement references unconfigured database key '{key}'.");
        cache[tenantId] = new CacheEntry(key, now.Add(cacheDuration));
        return key;
    }
}
