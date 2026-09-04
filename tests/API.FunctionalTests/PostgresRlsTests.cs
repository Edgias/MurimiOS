using System.Net;
using System.Net.Http.Json;
using Npgsql;

namespace EdgIQ.MurimiOS.API.FunctionalTests;

public sealed class PostgresRlsTests
{
    [IntegrationFact]
    public async Task Forced_rls_filters_reads_and_rejects_cross_tenant_writes()
    {
        await using var factory = new MurimiApiFactory();
        Guid tenantA = Guid.NewGuid();
        Guid tenantB = Guid.NewGuid();
        Guid farmA = Guid.NewGuid();
        Guid farmB = Guid.NewGuid();
        using HttpClient clientA = factory.CreateTenantClient(tenantA);
        using HttpClient clientB = factory.CreateTenantClient(tenantB);
        Assert.Equal(HttpStatusCode.Created, (await clientA.PostAsJsonAsync("/api/v1/farms", Farm(farmA, "A"))).StatusCode);
        Assert.Equal(HttpStatusCode.Created, (await clientB.PostAsJsonAsync("/api/v1/farms", Farm(farmB, "B"))).StatusCode);

        await using var connection = new NpgsqlConnection(MurimiApiFactory.ConnectionString);
        await connection.OpenAsync();
        await SetTenantAsync(connection, tenantA);
        Assert.Equal(1L, await CountFarmAsync(connection, farmA));
        Assert.Equal(0L, await CountFarmAsync(connection, farmB));
        await SetTenantAsync(connection, tenantB);
        Assert.Equal(0L, await CountFarmAsync(connection, farmA));
        Assert.Equal(1L, await CountFarmAsync(connection, farmB));

        await SetTenantAsync(connection, tenantA);
        await using NpgsqlTransaction transaction = await connection.BeginTransactionAsync();
        await using var insert = new NpgsqlCommand("""
            insert into analytics.retention_policies(
                id,raw_sensor_days,raw_telemetry_days,failed_projection_days,next_run_at,last_run_at,
                configured_by,is_active,tenant_id,created_at_utc,updated_at_utc,version)
            values(@id,90,90,90,now(),null,'rls-test',false,@tenant,now(),now(),1)
            """, connection, transaction);
        insert.Parameters.AddWithValue("id", Guid.NewGuid());
        insert.Parameters.AddWithValue("tenant", tenantB);
        Npgsql.PostgresException failure = await Assert.ThrowsAsync<Npgsql.PostgresException>(() => insert.ExecuteNonQueryAsync());
        Assert.Equal("42501", failure.SqlState);
        await transaction.RollbackAsync();
    }

    [IntegrationFact]
    public async Task Sensitive_maintenance_tables_are_forced_rls_and_functions_are_not_public()
    {
        await using var connection = new NpgsqlConnection(MurimiApiFactory.ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand("""
            select
              (select bool_and(c.relrowsecurity and c.relforcerowsecurity)
               from pg_class c join pg_namespace n on n.oid=c.relnamespace
               where n.nspname='analytics' and c.relname in (
                 'projection_schedules','retention_policies','projection_execution','farm_metric_rollups',
                 'sensor_daily_rollups','machinery_daily_rollups','retention_runs','benchmark_cohort_definitions',
                 'predictive_model_definitions','dimension_snapshots','forecast_evaluations','refresh_requests')),
              (select bool_and(c.relrowsecurity and c.relforcerowsecurity)
               from pg_class c join pg_namespace n on n.oid=c.relnamespace
               where n.nspname='finance' and c.relname in (
                 'legal_companies','intercompany_elimination_rules','consolidation_runs',
                 'consolidation_elimination_entries')),
              (select bool_and(c.relrowsecurity and c.relforcerowsecurity)
               from pg_class c join pg_namespace n on n.oid=c.relnamespace
               where n.nspname='connected' and c.relname in (
                 'derived_media_assets','lifecycle_retention_runs')),
              (select bool_and(not exists(
                   select 1 from aclexplode(coalesce(p.proacl,acldefault('f',p.proowner))) a
                   where a.grantee=0 and a.privilege_type='EXECUTE'))
               from pg_proc p join pg_namespace n on n.oid=p.pronamespace
               where n.nspname='analytics' and p.proname in (
                 'run_due_retention','claim_projection','schedule_event_refreshes','enqueue_refresh_from_outbox'))
              ,(select bool_and(not exists(
                   select 1 from aclexplode(coalesce(p.proacl,acldefault('f',p.proowner))) a
                   where a.grantee=0 and a.privilege_type='EXECUTE'))
               from pg_proc p join pg_namespace n on n.oid=p.pronamespace
               where n.nspname='connected' and p.proname in (
                 'ensure_future_partitions','claim_derived_media','complete_derived_media_deletion',
                 'fail_derived_media_deletion'))
            """, connection);
        await using NpgsqlDataReader reader = await command.ExecuteReaderAsync();
        Assert.True(await reader.ReadAsync());
        Assert.True(reader.GetBoolean(0));
        Assert.True(reader.GetBoolean(1));
        Assert.True(reader.GetBoolean(2));
        Assert.True(reader.GetBoolean(3));
        Assert.True(reader.GetBoolean(4));
    }

    private static object Farm(Guid id, string suffix) => new
    {
        id,
        name = $"PostgreSQL RLS Farm {suffix}",
        code = $"RLS{suffix}{id:N}"[..12],
        countryCode = "ZW",
        timeZone = "Africa/Harare"
    };

    private static async Task SetTenantAsync(NpgsqlConnection connection, Guid tenantId)
    {
        await using var command = new NpgsqlCommand("select set_config('app.tenant_id',@tenant,false)", connection);
        command.Parameters.AddWithValue("tenant", tenantId.ToString());
        await command.ExecuteNonQueryAsync();
    }

    private static async Task<long> CountFarmAsync(NpgsqlConnection connection, Guid farmId)
    {
        await using var command = new NpgsqlCommand("select count(*) from farm.farms where id=@id", connection);
        command.Parameters.AddWithValue("id", farmId);
        return Convert.ToInt64(await command.ExecuteScalarAsync());
    }
}
