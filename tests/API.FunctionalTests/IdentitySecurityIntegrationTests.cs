using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Npgsql;

namespace EdgIQ.MurimiOS.API.FunctionalTests;

public sealed class IdentitySecurityIntegrationTests
{
    [IntegrationFact]
    public async Task Security_tables_are_forced_rls_and_quota_consumption_is_atomic()
    {
        await using var connection = new NpgsqlConnection(MurimiApiFactory.ConnectionString);
        await connection.OpenAsync();
        await using (var securityContract = new NpgsqlCommand("""
            select
              (select bool_and(c.relrowsecurity and c.relforcerowsecurity)
               from pg_class c join pg_namespace n on n.oid=c.relnamespace
               where n.nspname='security' and c.relname in (
                 'rate_limit_policies','credential_rotation_policies','devices','sessions',
                 'events','recovery_requests','request_quota_buckets')),
              (select bool_and(not exists(
                   select 1 from aclexplode(coalesce(p.proacl,acldefault('f',p.proowner))) a
                   where a.grantee=0 and a.privilege_type='EXECUTE'))
               from pg_proc p join pg_namespace n on n.oid=p.pronamespace
               where n.nspname='security' and p.proname='consume_request_quota')
            """, connection))
        await using (NpgsqlDataReader reader = await securityContract.ExecuteReaderAsync())
        {
            Assert.True(await reader.ReadAsync());
            Assert.True(reader.GetBoolean(0));
            Assert.True(reader.GetBoolean(1));
        }

        Guid tenantId = Guid.NewGuid();
        (bool allowed, int requestRemaining, int dailyRemaining) first = await ConsumeAsync(connection, tenantId);
        (bool allowed, int requestRemaining, int dailyRemaining) second = await ConsumeAsync(connection, tenantId);
        (bool allowed, int requestRemaining, int dailyRemaining) third = await ConsumeAsync(connection, tenantId);

        Assert.Equal((true, 1, 1), first);
        Assert.Equal((true, 0, 0), second);
        Assert.Equal((false, 0, 0), third);
    }

    [IntegrationFact]
    public async Task Tenant_rate_policy_is_enforced_by_the_api_middleware()
    {
        await using var factory = new MurimiApiFactory();
        Guid tenantId = Guid.NewGuid();
        using HttpClient client = factory.CreateTenantClient(tenantId);

        HttpResponseMessage created = await client.PostAsJsonAsync("/api/v1/security/rate-limit-policies", new
        {
            requestLimit = 2,
            windowSeconds = 60,
            dailyQuota = 100
        });
        Assert.Equal(HttpStatusCode.Created, created.StatusCode);

        HttpResponseMessage finalAllowed = await client.GetAsync("/api/v1/security/rate-limit-policies");
        Assert.Equal(HttpStatusCode.OK, finalAllowed.StatusCode);
        Assert.Equal("0", finalAllowed.Headers.GetValues("X-RateLimit-Remaining").Single());

        HttpResponseMessage rejected = await client.GetAsync("/api/v1/security/rate-limit-policies");
        Assert.Equal(HttpStatusCode.TooManyRequests, rejected.StatusCode);
        Assert.True(rejected.Headers.RetryAfter is not null);
    }

    [IntegrationFact]
    public async Task Recovery_tokens_are_returned_once_verified_anonymously_and_audited()
    {
        await using var factory = new MurimiApiFactory();
        Guid tenantId = Guid.NewGuid();
        string subject = $"recovery-{Guid.NewGuid():N}";
        using HttpClient client = factory.CreateTenantClient(tenantId);

        HttpResponseMessage memberCreated = await client.PostAsJsonAsync("/api/v1/access/memberships", new
        {
            externalSubject = subject,
            displayName = "Recovery Integration User",
            email = "recovery@example.test",
            role = 9,
            status = 2,
            scopeOrganisationNodeId = (Guid?)null,
            permissionGrants = Array.Empty<string>()
        });
        Assert.Equal(HttpStatusCode.Created, memberCreated.StatusCode);
        Guid membershipId = (await JsonDocument.ParseAsync(await memberCreated.Content.ReadAsStreamAsync()))
            .RootElement.GetProperty("id").GetGuid();

        HttpResponseMessage issued = await client.PostAsJsonAsync("/api/v1/security/recovery-requests", new
        {
            membershipId,
            maximumAttempts = 3
        });
        Assert.Equal(HttpStatusCode.Created, issued.StatusCode);
        JsonElement issue = (await JsonDocument.ParseAsync(await issued.Content.ReadAsStreamAsync())).RootElement;
        Guid recoveryId = issue.GetProperty("recovery").GetProperty("id").GetGuid();
        string secret = issue.GetProperty("secret").GetString()!;

        using HttpClient anonymous = factory.CreateClient();
        HttpResponseMessage verified = await anonymous.PostAsJsonAsync("/api/public/v1/security/recovery/verify", new
        {
            tenantId,
            requestId = recoveryId,
            token = secret
        });
        Assert.Equal(HttpStatusCode.Accepted, verified.StatusCode);

        HttpResponseMessage reuse = await anonymous.PostAsJsonAsync("/api/public/v1/security/recovery/verify", new
        {
            tenantId,
            requestId = recoveryId,
            token = secret
        });
        Assert.Equal(HttpStatusCode.Conflict, reuse.StatusCode);

        HttpResponseMessage events = await client.GetAsync("/api/v1/security/events?eventType=RecoveryVerified");
        Assert.Equal(HttpStatusCode.OK, events.StatusCode);
        JsonElement eventRows = (await JsonDocument.ParseAsync(await events.Content.ReadAsStreamAsync())).RootElement;
        Assert.Single(eventRows.EnumerateArray());
    }

    [IntegrationFact]
    public async Task Credential_rotation_policy_blocks_nonexpiring_and_overlong_api_keys()
    {
        await using var factory = new MurimiApiFactory();
        Guid tenantId = Guid.NewGuid();
        using HttpClient client = factory.CreateTenantClient(tenantId);

        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync(
            "/api/v1/security/credential-rotation-policies", new
            {
                maximumAgeDays = 30,
                warningDays = 7,
                requireExpiry = true
            })).StatusCode);

        HttpResponseMessage missingExpiry = await client.PostAsJsonAsync("/api/v1/public-api/clients", new
        {
            name = "No expiry client",
            scopes = new[] { "farms.read" },
            farmIds = Array.Empty<Guid>(),
            expiresAt = (DateTimeOffset?)null
        });
        Assert.Equal(HttpStatusCode.BadRequest, missingExpiry.StatusCode);

        HttpResponseMessage overlong = await client.PostAsJsonAsync("/api/v1/public-api/clients", new
        {
            name = "Overlong client",
            scopes = new[] { "farms.read" },
            farmIds = Array.Empty<Guid>(),
            expiresAt = DateTimeOffset.UtcNow.AddDays(31)
        });
        Assert.Equal(HttpStatusCode.BadRequest, overlong.StatusCode);

        HttpResponseMessage valid = await client.PostAsJsonAsync("/api/v1/public-api/clients", new
        {
            name = "Compliant client",
            scopes = new[] { "farms.read" },
            farmIds = Array.Empty<Guid>(),
            expiresAt = DateTimeOffset.UtcNow.AddDays(29)
        });
        Assert.Equal(HttpStatusCode.Created, valid.StatusCode);
    }

    private static async Task<(bool Allowed, int RequestRemaining, int DailyRemaining)> ConsumeAsync(
        NpgsqlConnection connection, Guid tenantId)
    {
        await using var command = new NpgsqlCommand("""
            select allowed, request_remaining, daily_remaining
            from security.consume_request_quota(@tenant,null,now(),2,60,2)
            """, connection);
        command.Parameters.AddWithValue("tenant", tenantId);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        return (reader.GetBoolean(0), reader.GetInt32(1), reader.GetInt32(2));
    }
}
