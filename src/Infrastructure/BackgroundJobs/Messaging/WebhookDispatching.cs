using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace EdgIQ.MurimiOS.Infrastructure.BackgroundJobs.Messaging;

public sealed record WebhookEnvelope(Guid Id, Guid TenantId, Guid SubscriptionId, string Endpoint,
    string SigningKeyHash, string EventType, string Payload, DateTimeOffset OccurredAt, int Attempt, Guid LeaseId);

public sealed class WebhookDispatcherOptions
{
    public int BatchSize { get; set; } = 25; public int LockSeconds { get; set; } = 60; public int PollMilliseconds { get; set; } = 1000;
    public int InitialRetrySeconds { get; set; } = 10; public int MaximumRetrySeconds { get; set; } = 3600; public int RequestTimeoutSeconds { get; set; } = 15;
    public void Validate() { if (BatchSize is < 1 or > 200 || LockSeconds is < 5 or > 3600 || PollMilliseconds is < 100 or > 60000 || InitialRetrySeconds is < 1 or > 3600 || MaximumRetrySeconds < InitialRetrySeconds || RequestTimeoutSeconds is < 1 or > 120) throw new InvalidOperationException("Webhook dispatcher configuration is invalid."); }
}

public interface IWebhookStore
{
    Task<IReadOnlyList<WebhookEnvelope>> ClaimAsync(Guid workerId, int batchSize, TimeSpan lockDuration, DateTimeOffset now, CancellationToken ct);
    Task<bool> MarkDeliveredAsync(Guid id, Guid leaseId, DateTimeOffset at, int status, string? excerpt, CancellationToken ct);
    Task<bool> MarkFailedAsync(Guid id, Guid leaseId, DateTimeOffset at, DateTimeOffset retryAt, int? status, string error, string? excerpt, CancellationToken ct);
}

public sealed class PostgresWebhookStore(string connectionString) : IWebhookStore
{
    public async Task<IReadOnlyList<WebhookEnvelope>> ClaimAsync(Guid workerId, int batchSize, TimeSpan lockDuration, DateTimeOffset now, CancellationToken ct)
    {
        var rows = new List<WebhookEnvelope>(); await using var connection = new NpgsqlConnection(connectionString); await connection.OpenAsync(ct);
        await using var command = new NpgsqlCommand("select * from external_api.claim_webhook_deliveries(@worker,@batch,@seconds,@now)", connection);
        command.Parameters.AddWithValue("worker", workerId); command.Parameters.AddWithValue("batch", batchSize); command.Parameters.AddWithValue("seconds", (int)lockDuration.TotalSeconds); command.Parameters.AddWithValue("now", now);
        await using var reader = await command.ExecuteReaderAsync(ct); while (await reader.ReadAsync(ct)) rows.Add(new(reader.GetGuid(0), reader.GetGuid(1), reader.GetGuid(2), reader.GetString(3), reader.GetString(4), reader.GetString(5), reader.GetString(6), reader.GetFieldValue<DateTimeOffset>(7), reader.GetInt32(8), reader.GetGuid(9))); return rows;
    }
    public Task<bool> MarkDeliveredAsync(Guid id, Guid leaseId, DateTimeOffset at, int status, string? excerpt, CancellationToken ct) => ScalarAsync("select external_api.complete_webhook_delivery(@id,@lease,@at,@status,@excerpt)", id, leaseId, at, at, status, "", excerpt, ct);
    public Task<bool> MarkFailedAsync(Guid id, Guid leaseId, DateTimeOffset at, DateTimeOffset retryAt, int? status, string error, string? excerpt, CancellationToken ct) => ScalarAsync("select external_api.fail_webhook_delivery(@id,@lease,@at,@retry,@status,@error,@excerpt)", id, leaseId, at, retryAt, status, error, excerpt, ct);
    private async Task<bool> ScalarAsync(string sql, Guid id, Guid leaseId, DateTimeOffset at, DateTimeOffset retryAt, int? status, string error, string? excerpt, CancellationToken ct)
    { await using var connection = new NpgsqlConnection(connectionString); await connection.OpenAsync(ct); await using var command = new NpgsqlCommand(sql, connection); command.Parameters.AddWithValue("id", id); command.Parameters.AddWithValue("lease", leaseId); command.Parameters.AddWithValue("at", at); if (sql.Contains("@retry")) command.Parameters.AddWithValue("retry", retryAt); command.Parameters.AddWithValue("status", (object?)status ?? DBNull.Value); if (sql.Contains("@error")) command.Parameters.AddWithValue("error", error); command.Parameters.AddWithValue("excerpt", (object?)excerpt ?? DBNull.Value); return (bool)(await command.ExecuteScalarAsync(ct) ?? false); }
}

public sealed class WebhookDispatcher(IWebhookStore store, HttpClient http, WebhookDispatcherOptions options,
    TimeProvider clock, ILogger<WebhookDispatcher> logger)
{
    public async Task<int> DispatchBatchAsync(Guid workerId, CancellationToken ct)
    {
        options.Validate(); IReadOnlyList<WebhookEnvelope> rows = await store.ClaimAsync(workerId, options.BatchSize, TimeSpan.FromSeconds(options.LockSeconds), clock.GetUtcNow(), ct);
        foreach (WebhookEnvelope row in rows)
        {
            DateTimeOffset now = clock.GetUtcNow(); string timestamp = now.ToUnixTimeSeconds().ToString(); string signature = Sign(row.SigningKeyHash, timestamp, row.Payload);
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Post, row.Endpoint) { Content = new StringContent(row.Payload, Encoding.UTF8, "application/json") };
                request.Headers.Add("X-MurimiOS-Delivery", row.Id.ToString()); request.Headers.Add("X-MurimiOS-Event", row.EventType); request.Headers.Add("X-MurimiOS-Timestamp", timestamp); request.Headers.Add("X-MurimiOS-Signature", $"v1={signature}"); request.Headers.UserAgent.Add(new ProductInfoHeaderValue("MurimiOS-Webhooks", "1.0"));
                using HttpResponseMessage response = await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, ct); string? excerpt = await ExcerptAsync(response, ct);
                if (response.IsSuccessStatusCode) await store.MarkDeliveredAsync(row.Id, row.LeaseId, clock.GetUtcNow(), (int)response.StatusCode, excerpt, ct);
                else await store.MarkFailedAsync(row.Id, row.LeaseId, clock.GetUtcNow(), clock.GetUtcNow().Add(Retry(row.Attempt)), (int)response.StatusCode, $"Webhook returned HTTP {(int)response.StatusCode}.", excerpt, ct);
            }
            catch (OperationCanceledException) when (ct.IsCancellationRequested) { throw; }
            catch (Exception e) { DateTimeOffset failed = clock.GetUtcNow(); await store.MarkFailedAsync(row.Id, row.LeaseId, failed, failed.Add(Retry(row.Attempt)), null, e.Message[..Math.Min(e.Message.Length, 2000)], null, ct); logger.LogWarning(e, "Webhook delivery {DeliveryId} failed.", row.Id); }
        }
        return rows.Count;
    }
    public TimeSpan Retry(int attempt) => TimeSpan.FromSeconds(Math.Min(options.InitialRetrySeconds * Math.Pow(2, Math.Clamp(attempt - 1, 0, 20)), options.MaximumRetrySeconds));
    public static string Sign(string keyHash, string timestamp, string payload) { byte[] key = Convert.FromHexString(keyHash); byte[] bytes = Encoding.UTF8.GetBytes($"{timestamp}.{payload}"); return Convert.ToHexString(HMACSHA256.HashData(key, bytes)).ToLowerInvariant(); }
    private static async Task<string?> ExcerptAsync(HttpResponseMessage response, CancellationToken ct) { if (response.Content.Headers.ContentLength == 0) return null; string value = await response.Content.ReadAsStringAsync(ct); return value[..Math.Min(value.Length, 2000)]; }
}
