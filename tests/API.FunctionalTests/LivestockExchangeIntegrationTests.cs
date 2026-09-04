using System.Net;
using System.Text.Json;
using Npgsql;

namespace EdgIQ.MurimiOS.API.FunctionalTests;

public sealed class LivestockExchangeIntegrationTests
{
    [IntegrationFact]
    public async Task Default_profiles_are_tenant_local_and_new_tables_are_forced_rls()
    {
        await using var factory = new MurimiApiFactory();
        Guid tenantA = Guid.NewGuid();
        Guid tenantB = Guid.NewGuid();
        using HttpClient clientA = factory.CreateTenantClient(tenantA);
        using HttpClient clientB = factory.CreateTenantClient(tenantB);

        HttpResponseMessage responseA = await clientA.PostAsync("/api/v1/livestock/regulatory-profiles/defaults", null);
        HttpResponseMessage responseB = await clientB.PostAsync("/api/v1/livestock/regulatory-profiles/defaults", null);
        Assert.Equal(HttpStatusCode.OK, responseA.StatusCode);
        Assert.Equal(HttpStatusCode.OK, responseB.StatusCode);
        Guid[] idsA = await IdsAsync(responseA);
        Guid[] idsB = await IdsAsync(responseB);
        Assert.Equal(3, idsA.Length);
        Assert.Equal(3, idsB.Length);
        Assert.Empty(idsA.Intersect(idsB));

        await using var connection = new NpgsqlConnection(MurimiApiFactory.ConnectionString);
        await connection.OpenAsync();
        await SetTenantAsync(connection, tenantA);
        await using (var count = new NpgsqlCommand(
            "select count(*) from livestock.regulatory_profiles where id=any(@ids)", connection))
        {
            count.Parameters.AddWithValue("ids", idsB);
            Assert.Equal(0L, Convert.ToInt64(await count.ExecuteScalarAsync()));
        }

        await using var security = new NpgsqlCommand("""
            select
              (select bool_and(c.relrowsecurity and c.relforcerowsecurity)
               from pg_class c join pg_namespace n on n.oid=c.relnamespace
               where n.nspname='livestock' and c.relname in
                 ('regulatory_profiles','exchange_submissions','exchange_attempts')),
              (select count(*)=3 from pg_trigger t join pg_class c on c.oid=t.tgrelid
               join pg_namespace n on n.oid=c.relnamespace
               where not t.tgisinternal and n.nspname='livestock' and t.tgname in
                 ('livestock_regulatory_profiles_immutable','livestock_exchange_submission_guard',
                  'livestock_exchange_attempts_immutable'))
            """, connection);
        await using NpgsqlDataReader reader = await security.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.True(reader.GetBoolean(0));
        Assert.True(reader.GetBoolean(1));
    }

    private static async Task<Guid[]> IdsAsync(HttpResponseMessage response)
    {
        using JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        return document.RootElement.EnumerateArray().Select(x => x.GetProperty("id").GetGuid()).ToArray();
    }

    private static async Task SetTenantAsync(NpgsqlConnection connection, Guid tenantId)
    {
        await using var command = new NpgsqlCommand("select set_config('app.tenant_id',@tenant,false)", connection);
        command.Parameters.AddWithValue("tenant", tenantId.ToString());
        await command.ExecuteNonQueryAsync();
    }
}
