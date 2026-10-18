using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class FixTenantMigrationClaim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION enterprise.claim_tenant_migration(
                    p_worker uuid,p_lease_seconds integer,p_maximum_attempts integer,p_now timestamptz)
                RETURNS TABLE(job_id uuid,tenant_id uuid,migration_status integer,source_placement_id uuid,
                    source_database_key text,source_region text,residency_country text,destination_database_key text,
                    destination_region text,destination_mode integer,destination_placement_id uuid,
                    artifact_object_key text,artifact_sha256 text,manifest_json text,lease_id uuid,attempt integer)
                LANGUAGE plpgsql VOLATILE SECURITY DEFINER SET search_path=pg_catalog,enterprise AS $fn$
                DECLARE v_job uuid; v_tenant uuid;
                BEGIN
                    IF p_worker IS NULL OR p_lease_seconds NOT BETWEEN 60 AND 86400 OR p_maximum_attempts NOT BETWEEN 1 AND 20
                    THEN RAISE EXCEPTION 'Invalid tenant migration claim parameters.' USING ERRCODE='22023'; END IF;
                    SELECT j.id,j.tenant_id INTO v_job,v_tenant
                    FROM enterprise.tenant_migration_jobs j
                    LEFT JOIN enterprise.tenant_migration_execution e ON e.job_id=j.id
                    WHERE j.status BETWEEN 1 AND 5 AND coalesce(e.attempts,0)<p_maximum_attempts
                      AND (e.job_id IS NULL OR (e.available_at<=p_now AND (e.lease_id IS NULL OR e.leased_until<=p_now)))
                    ORDER BY j.requested_at FOR UPDATE OF j SKIP LOCKED LIMIT 1;
                    IF v_job IS NULL THEN RETURN; END IF;
                    UPDATE enterprise.tenant_migration_jobs j SET status=CASE WHEN j.status=1 THEN 2 ELSE j.status END,
                        failure_code=NULL,failure_message=NULL,updated_at_utc=p_now,version=j.version+1 WHERE j.id=v_job;
                    INSERT INTO enterprise.tenant_migration_execution(job_id,tenant_id,destination_placement_id,attempts,
                        lease_id,leased_until,available_at,created_at,updated_at)
                    VALUES(v_job,v_tenant,gen_random_uuid(),1,p_worker,p_now+make_interval(secs=>p_lease_seconds),p_now,p_now,p_now)
                    ON CONFLICT ON CONSTRAINT tenant_migration_execution_pkey DO UPDATE
                    SET attempts=enterprise.tenant_migration_execution.attempts+1,
                        lease_id=p_worker,leased_until=p_now+make_interval(secs=>p_lease_seconds),updated_at=p_now,last_error=NULL;
                    RETURN QUERY SELECT j.id,j.tenant_id,j.status,j.source_placement_id,s.database_key::text,s.region::text,
                        s.residency_country::text,j.destination_database_key::text,j.destination_region::text,j.destination_mode,
                        e.destination_placement_id,j.artifact_object_key::text,j.artifact_sha256::text,j.manifest_json::text,
                        e.lease_id,e.attempts
                    FROM enterprise.tenant_migration_jobs j JOIN enterprise.tenant_placements s
                      ON s.tenant_id=j.tenant_id AND s.id=j.source_placement_id
                    JOIN enterprise.tenant_migration_execution e ON e.job_id=j.id WHERE j.id=v_job;
                END $fn$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
