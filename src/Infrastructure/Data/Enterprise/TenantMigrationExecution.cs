using System.Buffers.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.Extensions.Options;
using Npgsql;

namespace EdgIQ.MurimiOS.Infrastructure.Data.Enterprise;

public sealed class TenantMigrationExecutionOptions
{
    public bool Enabled { get; set; } = true;
    public string ArtifactRoot { get; set; } = "App_Data/tenant-migrations";
    public int PollMilliseconds { get; set; } = 5_000;
    public int LeaseSeconds { get; set; } = 7_200;
    public int MaximumAttempts { get; set; } = 5;
    public int MaximumRecordBytes { get; set; } = 16_777_216;
    public int WriteDrainSeconds { get; set; } = 5;
    public void Validate()
    {
        if (string.IsNullOrWhiteSpace(ArtifactRoot)) throw new InvalidOperationException("Tenant migration artifact root is required.");
        if (PollMilliseconds is < 500 or > 60_000 || LeaseSeconds is < 60 or > 86_400 || MaximumAttempts is < 1 or > 20
            || MaximumRecordBytes is < 65_536 or > 67_108_864 || WriteDrainSeconds is < 0 or > 60)
            throw new InvalidOperationException("Tenant migration execution configuration is invalid.");
    }
}

public sealed record TenantMigrationWorkItem(Guid JobId, Guid TenantId, int Status, Guid SourcePlacementId,
    string SourceDatabaseKey, string SourceRegion, string ResidencyCountry, string DestinationDatabaseKey,
    string DestinationRegion, int DestinationMode, Guid DestinationPlacementId, string? ArtifactObjectKey,
    string? ArtifactSha256, string? ManifestJson, Guid LeaseId, int Attempt);

public sealed record TenantExportResult(string ObjectKey, string Sha256, long ByteSize, long RowCount, string ManifestJson);

public interface ITenantMigrationControlStore
{
    Task<TenantMigrationWorkItem?> ClaimAsync(Guid workerId, int leaseSeconds, int maximumAttempts, DateTimeOffset now, CancellationToken ct);
    Task MarkExportedAsync(TenantMigrationWorkItem item, TenantExportResult export, DateTimeOffset now, CancellationToken ct);
    Task MarkValidatedAsync(TenantMigrationWorkItem item, DateTimeOffset now, CancellationToken ct);
    Task MarkImportingAsync(TenantMigrationWorkItem item, DateTimeOffset now, CancellationToken ct);
    Task CompleteAsync(TenantMigrationWorkItem item, DateTimeOffset now, CancellationToken ct);
    Task FailAsync(TenantMigrationWorkItem item, string code, string message, DateTimeOffset now, int maximumAttempts, CancellationToken ct);
}

public sealed class PostgresTenantMigrationControlStore(TenantDatabaseRegistry registry) : ITenantMigrationControlStore
{
    public async Task<TenantMigrationWorkItem?> ClaimAsync(Guid workerId, int leaseSeconds, int maximumAttempts,
        DateTimeOffset now, CancellationToken ct)
    {
        await using var connection = new NpgsqlConnection(registry.ControlConnectionString); await connection.OpenAsync(ct);
        await using var command = new NpgsqlCommand("select * from enterprise.claim_tenant_migration(@worker,@lease,@maximum,@now)", connection);
        command.Parameters.AddWithValue("worker", workerId); command.Parameters.AddWithValue("lease", leaseSeconds);
        command.Parameters.AddWithValue("maximum", maximumAttempts); command.Parameters.AddWithValue("now", now);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct); if (!await reader.ReadAsync(ct)) return null;
        return new TenantMigrationWorkItem(reader.GetGuid(0), reader.GetGuid(1), reader.GetInt32(2), reader.GetGuid(3),
            reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetString(7), reader.GetString(8),
            reader.GetInt32(9), reader.GetGuid(10), NullableString(reader, 11), NullableString(reader, 12),
            NullableString(reader, 13), reader.GetGuid(14), reader.GetInt32(15));
    }

    public Task MarkExportedAsync(TenantMigrationWorkItem item, TenantExportResult export, DateTimeOffset now, CancellationToken ct)
        => ExecuteAsync("select enterprise.mark_tenant_migration_exported(@job,@lease,@key,@sha,@manifest,@bytes,@rows,@now)", item, now, ct,
            ("key", export.ObjectKey), ("sha", export.Sha256), ("manifest", export.ManifestJson), ("bytes", export.ByteSize), ("rows", export.RowCount));
    public Task MarkValidatedAsync(TenantMigrationWorkItem item, DateTimeOffset now, CancellationToken ct)
        => ExecuteAsync("select enterprise.advance_tenant_migration(@job,@lease,3,4,@now)", item, now, ct);
    public Task MarkImportingAsync(TenantMigrationWorkItem item, DateTimeOffset now, CancellationToken ct)
        => ExecuteAsync("select enterprise.advance_tenant_migration(@job,@lease,4,5,@now)", item, now, ct);
    public Task CompleteAsync(TenantMigrationWorkItem item, DateTimeOffset now, CancellationToken ct)
        => ExecuteAsync("select enterprise.complete_tenant_migration(@job,@lease,@now)", item, now, ct);

    public Task FailAsync(TenantMigrationWorkItem item, string code, string message, DateTimeOffset now,
        int maximumAttempts, CancellationToken ct)
        => ExecuteAsync("select enterprise.fail_tenant_migration_execution(@job,@lease,@code,@message,@maximum,@now)", item, now, ct,
            ("code", code), ("message", message), ("maximum", maximumAttempts));

    private async Task ExecuteAsync(string sql, TenantMigrationWorkItem item, DateTimeOffset now, CancellationToken ct,
        params (string Name, object Value)[] values)
    {
        await using var connection = new NpgsqlConnection(registry.ControlConnectionString); await connection.OpenAsync(ct);
        await using var command = new NpgsqlCommand(sql, connection); command.Parameters.AddWithValue("job", item.JobId);
        command.Parameters.AddWithValue("lease", item.LeaseId); command.Parameters.AddWithValue("now", now);
        foreach ((string name, object value) in values) command.Parameters.AddWithValue(name, value);
        if (await command.ExecuteScalarAsync(ct) is not bool changed || !changed)
            throw new InvalidOperationException("Tenant migration lease or state is no longer current.");
    }
    private static string? NullableString(NpgsqlDataReader reader, int ordinal) => reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
}

public sealed class TenantMigrationArtifactService(TenantDatabaseRegistry registry, IDataProtectionProvider protectionProvider,
    IOptions<TenantMigrationExecutionOptions> configured, TimeProvider timeProvider)
{
    private sealed record TableName(string Schema, string Table);
    private sealed record ArtifactRecord(string Schema, string Table, string RowJson);
    private sealed record TableCount(string Schema, string Table, long Rows);
    private sealed record ArtifactManifest(string Format, Guid TenantId, string SchemaVersion, DateTimeOffset CreatedAt,
        IReadOnlyList<TableCount> Tables);
    private static readonly byte[] Magic = "MROTM01\n"u8.ToArray();
    private readonly IDataProtector protector = protectionProvider.CreateProtector("MurimiOS.TenantMigrationArtifact.v1");
    private readonly TenantMigrationExecutionOptions options = configured.Value;
    private readonly string root = Path.GetFullPath(configured.Value.ArtifactRoot);

    public async Task<TenantExportResult> ExportAsync(TenantMigrationWorkItem item, CancellationToken ct)
    {
        string source = registry.GetConnectionString(item.SourceDatabaseKey);
        string directory = Path.Combine(root, item.TenantId.ToString("N")); Directory.CreateDirectory(directory);
        string path = Path.Combine(directory, item.JobId.ToString("N") + ".mrotenant");
        string temporary = path + ".tmp"; if (File.Exists(temporary)) File.Delete(temporary);
        var counts = new List<TableCount>(); long total = 0;
        await using var connection = new NpgsqlConnection(source); await connection.OpenAsync(ct); await SetTenantAsync(connection, item.TenantId, ct);
        string schemaVersion = await SchemaVersionAsync(connection, ct);
        IReadOnlyList<TableName> tables = await TenantTablesAsync(connection, ct);
        await using (var output = new FileStream(temporary, FileMode.CreateNew, FileAccess.Write, FileShare.None, 131_072, FileOptions.Asynchronous | FileOptions.WriteThrough))
        {
            await output.WriteAsync(Magic, ct);
            foreach (TableName table in tables)
            {
                long tableRows = 0; string qualified = Quote(table.Schema) + "." + Quote(table.Table);
                await using var command = new NpgsqlCommand($"select to_jsonb(value)::text from {qualified} value where tenant_id=@tenant", connection);
                command.Parameters.AddWithValue("tenant", item.TenantId);
                await using NpgsqlDataReader reader = await command.ExecuteReaderAsync(System.Data.CommandBehavior.SequentialAccess, ct);
                while (await reader.ReadAsync(ct))
                {
                    string row = reader.GetString(0); ValidateTenant(row, item.TenantId);
                    byte[] plain = JsonSerializer.SerializeToUtf8Bytes(new ArtifactRecord(table.Schema, table.Table, row));
                    if (plain.Length > options.MaximumRecordBytes) throw new InvalidDataException($"A row in {table.Schema}.{table.Table} exceeds the configured artifact record limit.");
                    byte[] protectedRecord = protector.Protect(plain); byte[] length = new byte[4]; BinaryPrimitives.WriteInt32BigEndian(length, protectedRecord.Length);
                    await output.WriteAsync(length, ct); await output.WriteAsync(protectedRecord, ct); tableRows++; total++;
                }
                counts.Add(new TableCount(table.Schema, table.Table, tableRows));
            }
            await output.FlushAsync(ct);
        }
        File.Move(temporary, path, true);
        string sha; await using (var input = File.OpenRead(path)) sha = Convert.ToHexStringLower(await SHA256.HashDataAsync(input, ct));
        string manifest = JsonSerializer.Serialize(new ArtifactManifest("murimios-tenant-export-v1", item.TenantId,
            schemaVersion, timeProvider.GetUtcNow(), counts));
        return new TenantExportResult(Relative(path), sha, new FileInfo(path).Length, total, manifest);
    }

    public async Task ValidateAsync(TenantMigrationWorkItem item, CancellationToken ct)
    {
        if (item.ArtifactObjectKey is null || item.ArtifactSha256 is null || item.ManifestJson is null)
            throw new InvalidDataException("Export artifact metadata is incomplete.");
        string path = Resolve(item.ArtifactObjectKey); await using (var input = File.OpenRead(path))
        {
            string actual = Convert.ToHexStringLower(await SHA256.HashDataAsync(input, ct));
            if (!CryptographicOperations.FixedTimeEquals(Convert.FromHexString(actual), Convert.FromHexString(item.ArtifactSha256)))
                throw new InvalidDataException("Tenant export artifact digest does not match its manifest.");
        }
        ArtifactManifest manifest = Manifest(item.ManifestJson, item.TenantId);
        await using var destination = new NpgsqlConnection(registry.GetConnectionString(item.DestinationDatabaseKey)); await destination.OpenAsync(ct);
        if (!string.Equals(await SchemaVersionAsync(destination, ct), manifest.SchemaVersion, StringComparison.Ordinal))
            throw new InvalidDataException("Source and destination database schema versions do not match.");
        long records = 0; await foreach (ArtifactRecord record in ReadAsync(path, ct)) { ValidateTenant(record.RowJson, item.TenantId); records++; }
        if (records != manifest.Tables.Sum(x => x.Rows)) throw new InvalidDataException("Tenant export artifact row count does not match its manifest.");
    }

    public async Task ImportAsync(TenantMigrationWorkItem item, CancellationToken ct)
    {
        ArtifactManifest manifest = Manifest(item.ManifestJson ?? throw new InvalidDataException("Manifest is missing."), item.TenantId);
        string path = Resolve(item.ArtifactObjectKey ?? throw new InvalidDataException("Artifact is missing."));
        await using var destination = new NpgsqlConnection(registry.GetConnectionString(item.DestinationDatabaseKey)); await destination.OpenAsync(ct);
        await using NpgsqlTransaction transaction = await destination.BeginTransactionAsync(ct); await SetTenantAsync(destination, item.TenantId, ct, transaction);
        var imported = new Dictionary<(string, string), long>();
        await foreach (ArtifactRecord record in ReadAsync(path, ct))
        {
            string qualified = Quote(record.Schema) + "." + Quote(record.Table);
            await using var command = new NpgsqlCommand($"insert into {qualified} select * from jsonb_populate_record(null::{qualified},@row::jsonb) on conflict do nothing", destination, transaction);
            command.Parameters.AddWithValue("row", record.RowJson); await command.ExecuteNonQueryAsync(ct);
            imported[(record.Schema, record.Table)] = imported.GetValueOrDefault((record.Schema, record.Table)) + 1;
        }
        foreach (TableCount expected in manifest.Tables)
        {
            string qualified = Quote(expected.Schema) + "." + Quote(expected.Table);
            await using var count = new NpgsqlCommand($"select count(*) from {qualified} where tenant_id=@tenant", destination, transaction);
            count.Parameters.AddWithValue("tenant", item.TenantId); long actual = (long)(await count.ExecuteScalarAsync(ct) ?? 0L);
            if (actual != expected.Rows) throw new InvalidDataException($"Imported row count for {expected.Schema}.{expected.Table} is {actual}, expected {expected.Rows}.");
        }
        await transaction.CommitAsync(ct);
    }

    private async IAsyncEnumerable<ArtifactRecord> ReadAsync(string path,
        [System.Runtime.CompilerServices.EnumeratorCancellation] CancellationToken ct)
    {
        await using var input = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read, 131_072, FileOptions.Asynchronous | FileOptions.SequentialScan);
        byte[] magic = new byte[Magic.Length]; await input.ReadExactlyAsync(magic, ct); if (!magic.SequenceEqual(Magic)) throw new InvalidDataException("Tenant artifact header is invalid.");
        byte[] lengthBytes = new byte[4];
        while (input.Position < input.Length)
        {
            await input.ReadExactlyAsync(lengthBytes, ct); int length = BinaryPrimitives.ReadInt32BigEndian(lengthBytes);
            if (length <= 0 || length > options.MaximumRecordBytes + 65_536) throw new InvalidDataException("Tenant artifact frame length is invalid.");
            byte[] protectedRecord = new byte[length]; await input.ReadExactlyAsync(protectedRecord, ct);
            ArtifactRecord? record;
            try { record = JsonSerializer.Deserialize<ArtifactRecord>(protector.Unprotect(protectedRecord)); }
            catch (Exception exception) when (exception is CryptographicException or JsonException) { throw new InvalidDataException("Tenant artifact authentication failed.", exception); }
            yield return record ?? throw new InvalidDataException("Tenant artifact record is empty.");
        }
    }

    private static async Task<IReadOnlyList<TableName>> TenantTablesAsync(NpgsqlConnection connection, CancellationToken ct)
    {
        const string sql = """
            select n.nspname,c.relname
            from pg_class c join pg_namespace n on n.oid=c.relnamespace
            where c.relkind in ('r','p') and n.nspname not in ('pg_catalog','information_schema')
              and exists(select 1 from pg_attribute a where a.attrelid=c.oid and a.attname='tenant_id' and not a.attisdropped)
              and not exists(select 1 from pg_inherits i where i.inhrelid=c.oid)
              and not (n.nspname='enterprise' and c.relname in ('tenant_placements','tenant_migration_jobs','tenant_migration_execution'))
              and n.nspname not in ('saas','security')
            order by case when n.nspname='access' and c.relname='tenant_memberships' then 0 else 1 end,n.nspname,c.relname
            """;
        var rows = new List<TableName>(); await using var command = new NpgsqlCommand(sql, connection);
        await using (NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct))
            while (await reader.ReadAsync(ct)) rows.Add(new(reader.GetString(0), reader.GetString(1)));
        return await OrderByDependenciesAsync(connection, rows, ct);
    }

    private static async Task<IReadOnlyList<TableName>> OrderByDependenciesAsync(NpgsqlConnection connection, List<TableName> tables, CancellationToken ct)
    {
        const string sql = """
            select ns.nspname,src.relname,np.nspname,parent.relname
            from pg_constraint fk join pg_class src on src.oid=fk.conrelid join pg_namespace ns on ns.oid=src.relnamespace
            join pg_class parent on parent.oid=fk.confrelid join pg_namespace np on np.oid=parent.relnamespace where fk.contype='f'
            """;
        var edges = new List<(TableName Child, TableName Parent)>(); await using var command = new NpgsqlCommand(sql, connection);
        await using (NpgsqlDataReader reader = await command.ExecuteReaderAsync(ct)) while (await reader.ReadAsync(ct))
            edges.Add((new(reader.GetString(0), reader.GetString(1)), new(reader.GetString(2), reader.GetString(3))));
        var remaining = new HashSet<TableName>(tables); var ordered = new List<TableName>();
        while (remaining.Count > 0)
        {
            TableName[] ready = remaining.Where(value => !edges.Any(edge => edge.Child == value && edge.Parent != value && remaining.Contains(edge.Parent)))
                .OrderBy(x => x.Schema, StringComparer.Ordinal).ThenBy(x => x.Table, StringComparer.Ordinal).ToArray();
            if (ready.Length == 0) ready = [remaining.OrderBy(x => x.Schema, StringComparer.Ordinal).ThenBy(x => x.Table, StringComparer.Ordinal).First()];
            foreach (TableName value in ready) { remaining.Remove(value); ordered.Add(value); }
        }
        return ordered;
    }

    private static async Task SetTenantAsync(NpgsqlConnection connection, Guid tenantId, CancellationToken ct, NpgsqlTransaction? transaction = null)
    { await using var command = new NpgsqlCommand("select set_config('app.tenant_id',@tenant,false)", connection, transaction); command.Parameters.AddWithValue("tenant", tenantId.ToString()); await command.ExecuteNonQueryAsync(ct); }
    private static async Task<string> SchemaVersionAsync(NpgsqlConnection connection, CancellationToken ct)
    { await using var command = new NpgsqlCommand("select migration_id from public.\"__EFMigrationsHistory\" order by migration_id desc limit 1", connection); return await command.ExecuteScalarAsync(ct) as string ?? throw new InvalidDataException("Database has no MurimiOS schema history."); }
    private static void ValidateTenant(string rowJson, Guid tenantId)
    { using JsonDocument document = JsonDocument.Parse(rowJson); if (!document.RootElement.TryGetProperty("tenant_id", out JsonElement property) || !Guid.TryParse(property.GetString(), out Guid actual) || actual != tenantId) throw new InvalidDataException("Artifact row tenant does not match the migration tenant."); }
    private static ArtifactManifest Manifest(string json, Guid tenantId)
    { ArtifactManifest? value = JsonSerializer.Deserialize<ArtifactManifest>(json); if (value is null || value.Format != "murimios-tenant-export-v1" || value.TenantId != tenantId) throw new InvalidDataException("Tenant export manifest is invalid."); return value; }
    private string Resolve(string key) { string path = Path.GetFullPath(Path.Combine(root, key.Replace('/', Path.DirectorySeparatorChar))); if (!path.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase)) throw new InvalidDataException("Artifact key escapes the configured root."); return path; }
    private string Relative(string path) => Path.GetRelativePath(root, path).Replace(Path.DirectorySeparatorChar, '/');
    private static string Quote(string identifier) => '"' + identifier.Replace("\"", "\"\"") + '"';
}
