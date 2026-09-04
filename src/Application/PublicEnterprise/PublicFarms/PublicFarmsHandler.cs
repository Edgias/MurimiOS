using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.PublicApiClient;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.PublicEnterprise.PublicFarms;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;

using PublicApiAccessLog = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiAccessLog;
using PublicApiScopes = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiScopes;

public sealed class PublicFarmsHandler(UseCaseContext context, IPlatformDbContext db, IMutableTenantContext tenant,IRequestQuotaStore quotaStore) : IRequestHandler<PublicFarmsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PublicFarmsQuery message, CancellationToken cancellationToken) => PublicFarmsAsync(context, db, tenant, cancellationToken);

    private async Task<UseCaseResult> PublicFarmsAsync(UseCaseContext context, IPlatformDbContext db, IMutableTenantContext tenant, CancellationToken ct)
    {
        PublicApiIdentity? identity = await AuthenticateApiAsync(context, db, tenant, PublicApiScopes.FarmsRead, ct); if (identity is null) return Unauthorized(context);
        var rows = await db.Farms.AsNoTracking().Where(x => identity.FarmIds.Length == 0 || identity.FarmIds.Contains(x.Id)).OrderBy(x => x.Name).Select(x => new { x.Id, x.Name, x.Code, x.CountryCode, x.TimeZone }).ToArrayAsync(ct);
        await AuditPublicApiAsync(identity, PublicApiScopes.FarmsRead, 200, context, db, ct); return UseCaseResults.Ok(rows);
    }


    private async Task<PublicApiIdentity?> AuthenticateApiAsync(UseCaseContext context, IPlatformDbContext db, IMutableTenantContext tenant, string requiredScope, CancellationToken ct)
    {
        string raw = context.User.HeaderValues("X-API-Key").FirstOrDefault() ?? context.User.Header("Authorization").Replace("ApiKey ", "", StringComparison.OrdinalIgnoreCase);
        if (string.IsNullOrWhiteSpace(raw)) return null; string hash = Sha(raw); PublicApiIdentity? identity = null;
        await db.Database.OpenConnectionAsync(ct); try { await using var command = db.Database.GetDbConnection().CreateCommand(); command.CommandText = "select tenant_id,client_id,scopes,farm_ids from external_api.authenticate_api_key(@hash)"; var parameter = command.CreateParameter(); parameter.ParameterName = "hash"; parameter.Value = hash; command.Parameters.Add(parameter); await using var reader = await command.ExecuteReaderAsync(ct); if (await reader.ReadAsync(ct)) identity = new(reader.GetGuid(0), reader.GetGuid(1), reader.GetFieldValue<string[]>(2), reader.GetFieldValue<Guid[]>(3)); } finally { await db.Database.CloseConnectionAsync(); }
        if (identity is null || !identity.Scopes.Contains(requiredScope, StringComparer.Ordinal)) return null;
        tenant.Resolve(identity.TenantId);
        RequestQuotaDecision quota = await quotaStore
            .ConsumeClientAsync(identity.TenantId, identity.ClientId, DateTimeOffset.UtcNow, ct);
        context.Response["X-RateLimit-Remaining"] = quota.RequestRemaining.ToString();
        context.Response["X-DailyQuota-Remaining"] = quota.DailyRemaining.ToString();
        if (!quota.Allowed) { context.Response["Retry-After"] = quota.RetryAfterSeconds.ToString(); context.State["public_api_quota"] = quota; return null; }
        return identity;
    }


    private async Task AuditPublicApiAsync(PublicApiIdentity identity, string scope, int status, UseCaseContext context, IPlatformDbContext db, CancellationToken ct)
    { db.PublicApiAccessLogs.Add(new PublicApiAccessLog(Guid.NewGuid(), identity.ClientId, scope, context.User.Method, context.User.Path, status, context.User.CorrelationId, DateTimeOffset.UtcNow, RemoteHash(context))); await db.SaveChangesAsync(ct); }

    private void Add(Dictionary<string, object?> target, string[] attributes, string key, object? value) { if (attributes.Contains(key, StringComparer.Ordinal)) target[key] = value; }

    private string Sha(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    private string? RemoteHash(UseCaseContext context) => context.User.RemoteAddress is { } address ? Sha(address.ToString()) : null;

    private Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private UseCaseResult Unauthorized(UseCaseContext context) => context.State.TryGetValue("public_api_quota", out object? value) && value is RequestQuotaDecision quota
        ? UseCaseResults.Json(new { code = "request_quota_exceeded", message = "The API client request rate or daily quota has been exceeded.", retryAfterSeconds = quota.RetryAfterSeconds }, statusCode: 429)
        : UseCaseResults.Json(new { code = "invalid_api_key", message = "A valid API key with the required scope is required." }, statusCode: 401);

    private sealed record PublicApiIdentity(Guid TenantId, Guid ClientId, string[] Scopes, Guid[] FarmIds) { public bool AllowsFarm(Guid id) => FarmIds.Length == 0 || FarmIds.Contains(id); }

}
