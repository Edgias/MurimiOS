using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantDatabaseRoutingAndExecution : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE TABLE enterprise.tenant_migration_execution(
                    job_id uuid PRIMARY KEY REFERENCES enterprise.tenant_migration_jobs(id) ON DELETE RESTRICT,
                    tenant_id uuid NOT NULL,
                    destination_placement_id uuid NOT NULL UNIQUE,
                    attempts integer NOT NULL DEFAULT 0,
                    lease_id uuid NULL,
                    leased_until timestamptz NULL,
                    available_at timestamptz NOT NULL DEFAULT statement_timestamp(),
                    artifact_byte_size bigint NULL,
                    row_count bigint NULL,
                    last_error text NULL,
                    created_at timestamptz NOT NULL DEFAULT statement_timestamp(),
                    updated_at timestamptz NOT NULL DEFAULT statement_timestamp(),
                    CONSTRAINT ck_tenant_migration_execution CHECK (
                        attempts >= 0 AND (artifact_byte_size IS NULL OR artifact_byte_size >= 0)
                        AND (row_count IS NULL OR row_count >= 0))
                );
                CREATE INDEX ix_tenant_migration_execution_claim
                    ON enterprise.tenant_migration_execution(available_at,leased_until);
                CREATE INDEX ix_tenant_migration_execution_tenant
                    ON enterprise.tenant_migration_execution(tenant_id,job_id);

                ALTER TABLE enterprise.tenant_migration_execution ENABLE ROW LEVEL SECURITY;
                ALTER TABLE enterprise.tenant_migration_execution FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON enterprise.tenant_migration_execution
                    USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid)
                    WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);

                CREATE OR REPLACE FUNCTION enterprise.resolve_tenant_database_key(p_tenant_id uuid)
                RETURNS text LANGUAGE sql STABLE SECURITY DEFINER SET search_path=pg_catalog,enterprise AS $fn$
                    SELECT p.database_key::text
                    FROM enterprise.tenant_placements p
                    WHERE p.tenant_id=p_tenant_id AND p.is_primary AND p.effective_to IS NULL
                      AND p.effective_from<=statement_timestamp()
                    ORDER BY p.effective_from DESC LIMIT 1
                $fn$;

                CREATE OR REPLACE FUNCTION enterprise.tenant_migration_write_blocked(p_tenant_id uuid)
                RETURNS boolean LANGUAGE sql STABLE SECURITY DEFINER SET search_path=pg_catalog,enterprise AS $fn$
                    SELECT EXISTS(SELECT 1 FROM enterprise.tenant_migration_jobs j
                        WHERE j.tenant_id=p_tenant_id AND j.status BETWEEN 2 AND 5)
                $fn$;

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

                CREATE OR REPLACE FUNCTION enterprise.mark_tenant_migration_exported(p_job uuid,p_lease uuid,p_key text,
                    p_sha text,p_manifest text,p_bytes bigint,p_rows bigint,p_now timestamptz)
                RETURNS boolean LANGUAGE sql VOLATILE SECURITY DEFINER SET search_path=pg_catalog,enterprise AS $fn$
                    WITH eligible AS (
                        SELECT 1 FROM enterprise.tenant_migration_execution e JOIN enterprise.tenant_migration_jobs j ON j.id=e.job_id
                        WHERE e.job_id=p_job AND e.lease_id=p_lease AND e.leased_until>p_now AND j.status=2
                    ), changed AS (
                        UPDATE enterprise.tenant_migration_jobs j SET status=3,artifact_object_key=left(p_key,1000),
                            artifact_sha256=lower(p_sha),manifest_json=p_manifest::jsonb,updated_at_utc=p_now,version=j.version+1
                        WHERE j.id=p_job AND EXISTS(SELECT 1 FROM eligible) RETURNING 1
                    ), tracked AS (
                        UPDATE enterprise.tenant_migration_execution e SET artifact_byte_size=p_bytes,row_count=p_rows,updated_at=p_now
                        WHERE e.job_id=p_job AND EXISTS(SELECT 1 FROM changed) RETURNING 1
                    ) SELECT EXISTS(SELECT 1 FROM tracked)
                $fn$;

                CREATE OR REPLACE FUNCTION enterprise.advance_tenant_migration(p_job uuid,p_lease uuid,p_expected integer,
                    p_next integer,p_now timestamptz)
                RETURNS boolean LANGUAGE sql VOLATILE SECURITY DEFINER SET search_path=pg_catalog,enterprise AS $fn$
                    WITH changed AS (
                        UPDATE enterprise.tenant_migration_jobs j SET status=p_next,updated_at_utc=p_now,version=j.version+1
                        FROM enterprise.tenant_migration_execution e WHERE j.id=p_job AND e.job_id=j.id AND e.lease_id=p_lease
                          AND e.leased_until>p_now AND j.status=p_expected AND (p_expected,p_next) IN ((3,4),(4,5)) RETURNING 1
                    ) SELECT EXISTS(SELECT 1 FROM changed)
                $fn$;

                CREATE OR REPLACE FUNCTION enterprise.complete_tenant_migration(p_job uuid,p_lease uuid,p_now timestamptz)
                RETURNS boolean LANGUAGE plpgsql VOLATILE SECURITY DEFINER SET search_path=pg_catalog,enterprise AS $fn$
                DECLARE j enterprise.tenant_migration_jobs%ROWTYPE; s enterprise.tenant_placements%ROWTYPE;
                    v_destination uuid; v_cutover timestamptz;
                BEGIN
                    SELECT job.* INTO j FROM enterprise.tenant_migration_jobs job
                    JOIN enterprise.tenant_migration_execution e ON e.job_id=job.id
                    WHERE job.id=p_job AND job.status=5 AND e.lease_id=p_lease AND e.leased_until>p_now FOR UPDATE OF job;
                    IF j.id IS NULL THEN RETURN false; END IF;
                    SELECT * INTO s FROM enterprise.tenant_placements WHERE tenant_id=j.tenant_id AND id=j.source_placement_id FOR UPDATE;
                    SELECT destination_placement_id INTO v_destination FROM enterprise.tenant_migration_execution WHERE job_id=j.id;
                    v_cutover := greatest(p_now,s.effective_from+interval '1 microsecond');
                    UPDATE enterprise.tenant_placements SET is_primary=false,
                        effective_to=coalesce(effective_to,greatest(v_cutover,effective_from+interval '1 microsecond')),
                        updated_at_utc=v_cutover,version=version+1 WHERE tenant_id=j.tenant_id AND is_primary AND effective_to IS NULL;
                    INSERT INTO enterprise.tenant_placements(id,region,database_key,mode,residency_country,is_primary,
                        effective_from,effective_to,tenant_id,created_at_utc,updated_at_utc,version)
                    VALUES(v_destination,j.destination_region,j.destination_database_key,j.destination_mode,s.residency_country,
                        true,v_cutover,NULL,j.tenant_id,v_cutover,v_cutover,1)
                    ON CONFLICT(id) DO UPDATE SET is_primary=true,effective_to=NULL,updated_at_utc=v_cutover,version=enterprise.tenant_placements.version+1;
                    UPDATE enterprise.tenant_migration_jobs SET status=6,completed_at=v_cutover,updated_at_utc=v_cutover,version=version+1 WHERE id=j.id;
                    UPDATE enterprise.tenant_migration_execution SET lease_id=NULL,leased_until=NULL,updated_at=v_cutover WHERE job_id=j.id;
                    RETURN true;
                END $fn$;

                CREATE OR REPLACE FUNCTION enterprise.fail_tenant_migration_execution(p_job uuid,p_lease uuid,p_code text,
                    p_message text,p_maximum integer,p_now timestamptz)
                RETURNS boolean LANGUAGE plpgsql VOLATILE SECURITY DEFINER SET search_path=pg_catalog,enterprise AS $fn$
                DECLARE v_attempt integer;
                BEGIN
                    SELECT attempts INTO v_attempt FROM enterprise.tenant_migration_execution
                    WHERE job_id=p_job AND lease_id=p_lease FOR UPDATE;
                    IF v_attempt IS NULL THEN RETURN false; END IF;
                    UPDATE enterprise.tenant_migration_jobs SET status=CASE WHEN v_attempt>=p_maximum THEN 7 ELSE status END,
                        completed_at=CASE WHEN v_attempt>=p_maximum THEN p_now ELSE completed_at END,
                        failure_code=left(p_code,80),failure_message=left(p_message,2000),updated_at_utc=p_now,version=version+1
                    WHERE id=p_job;
                    UPDATE enterprise.tenant_migration_execution SET lease_id=NULL,leased_until=NULL,
                        available_at=p_now+make_interval(secs=>least(3600,5*(1<<least(10,v_attempt-1)))),
                        last_error=left(p_message,2000),updated_at=p_now WHERE job_id=p_job;
                    RETURN true;
                END $fn$;

                REVOKE ALL ON FUNCTION enterprise.resolve_tenant_database_key(uuid) FROM PUBLIC;
                REVOKE ALL ON FUNCTION enterprise.tenant_migration_write_blocked(uuid) FROM PUBLIC;
                REVOKE ALL ON FUNCTION enterprise.claim_tenant_migration(uuid,integer,integer,timestamptz) FROM PUBLIC;
                REVOKE ALL ON FUNCTION enterprise.mark_tenant_migration_exported(uuid,uuid,text,text,text,bigint,bigint,timestamptz) FROM PUBLIC;
                REVOKE ALL ON FUNCTION enterprise.advance_tenant_migration(uuid,uuid,integer,integer,timestamptz) FROM PUBLIC;
                REVOKE ALL ON FUNCTION enterprise.complete_tenant_migration(uuid,uuid,timestamptz) FROM PUBLIC;
                REVOKE ALL ON FUNCTION enterprise.fail_tenant_migration_execution(uuid,uuid,text,text,integer,timestamptz) FROM PUBLIC;
                DO $grant$ BEGIN IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname='murimios_app') THEN
                    GRANT SELECT,INSERT,UPDATE,DELETE ON enterprise.tenant_migration_execution TO murimios_app;
                    GRANT EXECUTE ON FUNCTION enterprise.resolve_tenant_database_key(uuid),
                        enterprise.tenant_migration_write_blocked(uuid),
                        enterprise.claim_tenant_migration(uuid,integer,integer,timestamptz),
                        enterprise.mark_tenant_migration_exported(uuid,uuid,text,text,text,bigint,bigint,timestamptz),
                        enterprise.advance_tenant_migration(uuid,uuid,integer,integer,timestamptz),
                        enterprise.complete_tenant_migration(uuid,uuid,timestamptz),
                        enterprise.fail_tenant_migration_execution(uuid,uuid,text,text,integer,timestamptz) TO murimios_app;
                END IF; END $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP FUNCTION IF EXISTS enterprise.fail_tenant_migration_execution(uuid,uuid,text,text,integer,timestamptz);
                DROP FUNCTION IF EXISTS enterprise.complete_tenant_migration(uuid,uuid,timestamptz);
                DROP FUNCTION IF EXISTS enterprise.advance_tenant_migration(uuid,uuid,integer,integer,timestamptz);
                DROP FUNCTION IF EXISTS enterprise.mark_tenant_migration_exported(uuid,uuid,text,text,text,bigint,bigint,timestamptz);
                DROP FUNCTION IF EXISTS enterprise.claim_tenant_migration(uuid,integer,integer,timestamptz);
                DROP FUNCTION IF EXISTS enterprise.tenant_migration_write_blocked(uuid);
                DROP FUNCTION IF EXISTS enterprise.resolve_tenant_database_key(uuid);
                DROP TABLE IF EXISTS enterprise.tenant_migration_execution;
                """);
        }
    }
}
