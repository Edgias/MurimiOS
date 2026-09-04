using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Npgsql;

namespace EdgIQ.MurimiOS.API.FunctionalTests;

public sealed class SaasControlPlaneIntegrationTests
{
    [IntegrationFact]
    public async Task Provision_plan_trial_usage_limit_and_suspension_form_one_enforced_lifecycle()
    {
        await using var factory = new MurimiApiFactory();
        Guid tenantId = Guid.NewGuid();
        Guid marker = Guid.NewGuid();
        using HttpClient client = factory.CreateTenantClient(tenantId);
        DateTimeOffset now = DateTimeOffset.UtcNow;

        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync("/api/v1/saas/account", new
        {
            legalName = "Control Plane Integration Farm",
            displayName = "Control Plane Farm",
            slug = $"control-{marker:N}",
            countryCode = "ZW",
            billingCurrency = "USD",
            timeZone = "Africa/Harare"
        })).StatusCode);

        HttpResponseMessage planCreated = await client.PostAsJsonAsync("/api/v1/saas/plans", new
        {
            code = $"producer-{marker:N}",
            name = "Producer Integration",
            planVersion = 1,
            billingInterval = 1,
            basePrice = 25,
            currency = "USD",
            effectiveFrom = now.AddMinutes(-1)
        });
        Assert.Equal(HttpStatusCode.Created, planCreated.StatusCode);
        Guid planId = (await JsonDocument.ParseAsync(await planCreated.Content.ReadAsStreamAsync())).RootElement.GetProperty("id").GetGuid();

        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync($"/api/v1/saas/plans/{planId}/entitlements", new
        {
            key = "api-requests",
            kind = 2,
            includedAmount = 10,
            unit = "requests",
            enforcement = 3
        })).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync($"/api/v1/saas/plans/{planId}/publish", new { })).StatusCode);

        HttpResponseMessage subscriptionCreated = await client.PostAsJsonAsync("/api/v1/saas/subscriptions", new
        {
            planVersionId = planId,
            startsAt = now.AddMinutes(-1),
            currentPeriodEndsAt = now.AddDays(30),
            trialEndsAt = now.AddDays(14),
            providerSubscriptionReference = $"sub_{marker:N}"
        });
        Assert.Equal(HttpStatusCode.Created, subscriptionCreated.StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync("/api/v1/saas/account/activate", new { reason = "" })).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/v1/platform")).StatusCode);

        var usage = new
        {
            idempotencyKey = $"usage-{marker:N}",
            metricCode = "api-requests",
            quantity = 8,
            unit = "requests",
            source = "integration-test",
            dimensions = new { operation = "list-farms" }
        };
        Assert.Equal(HttpStatusCode.Created, (await client.PostAsJsonAsync("/api/v1/saas/usage", usage)).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync("/api/v1/saas/usage", usage)).StatusCode);
        HttpResponseMessage exceeded = await client.PostAsJsonAsync("/api/v1/saas/usage", new
        {
            idempotencyKey = $"usage-over-{marker:N}", metricCode = "api-requests", quantity = 3,
            unit = "requests", source = "integration-test", dimensions = new { }
        });
        Assert.Equal(HttpStatusCode.TooManyRequests, exceeded.StatusCode);

        Assert.Equal(HttpStatusCode.OK, (await client.PostAsJsonAsync("/api/v1/saas/account/suspend", new { reason = "Integration suspension" })).StatusCode);
        Assert.Equal((HttpStatusCode)423, (await client.GetAsync("/api/v1/platform")).StatusCode);
        Assert.Equal(HttpStatusCode.OK, (await client.GetAsync("/api/v1/saas/account")).StatusCode);
    }

    [IntegrationFact]
    public async Task Saas_tables_are_forced_rls_and_cross_tenant_account_reads_are_hidden()
    {
        await using var connection = new NpgsqlConnection(MurimiApiFactory.ConnectionString);
        await connection.OpenAsync();
        await using var policy = new NpgsqlCommand("""
            select bool_and(c.relrowsecurity and c.relforcerowsecurity)
            from pg_class c join pg_namespace n on n.oid=c.relnamespace
            where n.nspname='saas' and c.relname in (
              'tenant_accounts','plan_versions','plan_entitlements','subscriptions',
              'usage_events','usage_periods','billing_accounts','invoices')
            """, connection);
        Assert.True((bool)(await policy.ExecuteScalarAsync() ?? false));

        Guid tenantA = Guid.NewGuid(); Guid tenantB = Guid.NewGuid();
        await SetTenantAsync(connection, tenantA);
        await using (var insert = new NpgsqlCommand("""
            insert into saas.tenant_accounts(
              id,legal_name,display_name,slug,country_code,billing_currency,time_zone,status,
              provisioned_at,provisioned_by,tenant_id,created_at_utc,updated_at_utc,version)
            values(@id,'RLS A','RLS A',@slug,'ZW','USD','Africa/Harare',1,now(),'test',@tenant,now(),now(),1)
            """, connection))
        {
            insert.Parameters.AddWithValue("id", Guid.NewGuid()); insert.Parameters.AddWithValue("slug", $"rls-{Guid.NewGuid():N}");
            insert.Parameters.AddWithValue("tenant", tenantA); await insert.ExecuteNonQueryAsync();
        }
        await SetTenantAsync(connection, tenantB);
        await using var count = new NpgsqlCommand("select count(*) from saas.tenant_accounts", connection);
        Assert.Equal(0L, Convert.ToInt64(await count.ExecuteScalarAsync()));
    }

    private static async Task SetTenantAsync(NpgsqlConnection connection, Guid tenantId)
    {
        await using var command = new NpgsqlCommand("select set_config('app.tenant_id',@tenant,false)", connection);
        command.Parameters.AddWithValue("tenant", tenantId.ToString()); await command.ExecuteNonQueryAsync();
    }
}
