using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduledProviderAdapters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                ALTER TABLE integrations.connections DROP CONSTRAINT ck_integration_connections_state;
                ALTER TABLE integrations.connections ADD CONSTRAINT ck_integration_connections_state CHECK (
                    category BETWEEN 1 AND 9 AND status BETWEEN 1 AND 4 AND health BETWEEN 1 AND 4
                    AND (rate_limit_remaining IS NULL OR rate_limit_remaining >= 0)
                    AND ((status = 4) = (disabled_at IS NOT NULL AND disabled_by IS NOT NULL)));

                ALTER TABLE integrations.feeds DROP CONSTRAINT ck_integration_feeds_state;
                ALTER TABLE integrations.feeds ADD CONSTRAINT ck_integration_feeds_state CHECK (
                    feed_type BETWEEN 1 AND 9 AND target_type BETWEEN 1 AND 5 AND status BETWEEN 1 AND 3
                    AND polling_interval_minutes BETWEEN 1 AND 43200
                    AND freshness_threshold_minutes BETWEEN 1 AND 129600
                    AND ((target_type = 1 AND target_id IS NULL) OR (target_type <> 1 AND target_id IS NOT NULL)));

                CREATE OR REPLACE FUNCTION integrations.schedule_due_jobs(
                    p_now timestamptz,
                    p_batch_size integer,
                    p_maximum_attempts integer)
                RETURNS integer
                LANGUAGE plpgsql
                SECURITY DEFINER
                SET search_path = pg_catalog, integrations, public
                AS $function$
                DECLARE inserted_count integer;
                BEGIN
                    IF p_batch_size NOT BETWEEN 1 AND 1000 OR p_maximum_attempts NOT BETWEEN 1 AND 20 THEN
                        RAISE EXCEPTION 'Invalid ingestion scheduling limits.' USING ERRCODE = '22023';
                    END IF;
                    WITH due AS (
                        SELECT f.tenant_id, f.id AS feed_id, f.farm_id
                        FROM integrations.feeds f
                        JOIN integrations.connections c ON c.tenant_id = f.tenant_id AND c.id = f.connection_id
                        WHERE f.status = 1 AND c.status = 2 AND f.next_poll_at <= p_now
                          AND NOT EXISTS (
                              SELECT 1 FROM integrations.ingestion_jobs j
                              WHERE j.tenant_id = f.tenant_id AND j.feed_id = f.id AND j.status IN (1,2,4))
                        ORDER BY f.next_poll_at, f.id
                        FOR UPDATE OF f SKIP LOCKED
                        LIMIT p_batch_size
                    )
                    INSERT INTO integrations.ingestion_jobs (
                        id, feed_id, farm_id, kind, idempotency_key, status, requested_at, requested_by,
                        available_at, attempts, maximum_attempts, tenant_id, created_at_utc, updated_at_utc, version)
                    SELECT pg_catalog.gen_random_uuid(), feed_id, farm_id, 1,
                           'scheduled:' || feed_id::text || ':' || extract(epoch FROM date_trunc('minute', p_now))::bigint,
                           1, p_now, 'provider-ingestion-worker', p_now, 0, p_maximum_attempts,
                           tenant_id, p_now, p_now, 1
                    FROM due
                    ON CONFLICT (tenant_id, feed_id, idempotency_key) DO NOTHING;
                    GET DIAGNOSTICS inserted_count = ROW_COUNT;
                    RETURN inserted_count;
                END
                $function$;

                CREATE OR REPLACE FUNCTION integrations.claim_scheduled_job(
                    p_lease_id uuid,
                    p_lock_seconds integer,
                    p_now timestamptz)
                RETURNS TABLE(tenant_id uuid, job_id uuid, lease_id uuid)
                LANGUAGE plpgsql
                SECURITY DEFINER
                SET search_path = pg_catalog, integrations, public
                AS $function$
                BEGIN
                    IF p_lease_id IS NULL OR p_lock_seconds NOT BETWEEN 10 AND 3600 THEN
                        RAISE EXCEPTION 'Invalid ingestion lease.' USING ERRCODE = '22023';
                    END IF;
                    RETURN QUERY
                    WITH candidate AS (
                        SELECT j.tenant_id, j.id
                        FROM integrations.ingestion_jobs j
                        JOIN integrations.feeds f ON f.tenant_id = j.tenant_id AND f.id = j.feed_id
                        JOIN integrations.connections c ON c.tenant_id = f.tenant_id AND c.id = f.connection_id
                        WHERE j.attempts < j.maximum_attempts
                          AND ((j.status IN (1,4) AND j.available_at <= p_now)
                               OR (j.status = 2 AND j.leased_until <= p_now))
                          AND f.status = 1 AND c.status = 2
                        ORDER BY j.available_at, j.requested_at, j.id
                        FOR UPDATE OF j SKIP LOCKED
                        LIMIT 1
                    ), claimed AS (
                        UPDATE integrations.ingestion_jobs j
                        SET status = 2, lease_id = p_lease_id, last_claimed_at = p_now,
                            leased_until = p_now + make_interval(secs => p_lock_seconds), attempts = j.attempts + 1,
                            updated_at_utc = p_now, version = j.version + 1
                        FROM candidate x
                        WHERE j.tenant_id = x.tenant_id AND j.id = x.id
                        RETURNING j.tenant_id, j.id
                    )
                    SELECT claimed.tenant_id, claimed.id, p_lease_id FROM claimed;
                END
                $function$;

                REVOKE ALL ON FUNCTION integrations.schedule_due_jobs(timestamptz, integer, integer) FROM PUBLIC;
                REVOKE ALL ON FUNCTION integrations.claim_scheduled_job(uuid, integer, timestamptz) FROM PUBLIC;
                DO $grant$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        GRANT EXECUTE ON FUNCTION integrations.schedule_due_jobs(timestamptz, integer, integer) TO murimios_app;
                        GRANT EXECUTE ON FUNCTION integrations.claim_scheduled_job(uuid, integer, timestamptz) TO murimios_app;
                    END IF;
                END
                $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP FUNCTION IF EXISTS integrations.claim_scheduled_job(uuid, integer, timestamptz);
                DROP FUNCTION IF EXISTS integrations.schedule_due_jobs(timestamptz, integer, integer);
                ALTER TABLE integrations.connections DROP CONSTRAINT ck_integration_connections_state;
                ALTER TABLE integrations.connections ADD CONSTRAINT ck_integration_connections_state CHECK (
                    category BETWEEN 1 AND 8 AND status BETWEEN 1 AND 4 AND health BETWEEN 1 AND 4
                    AND (rate_limit_remaining IS NULL OR rate_limit_remaining >= 0)
                    AND ((status = 4) = (disabled_at IS NOT NULL AND disabled_by IS NOT NULL)));
                ALTER TABLE integrations.feeds DROP CONSTRAINT ck_integration_feeds_state;
                ALTER TABLE integrations.feeds ADD CONSTRAINT ck_integration_feeds_state CHECK (
                    feed_type BETWEEN 1 AND 8 AND target_type BETWEEN 1 AND 5 AND status BETWEEN 1 AND 3
                    AND polling_interval_minutes BETWEEN 1 AND 43200
                    AND freshness_threshold_minutes BETWEEN 1 AND 129600
                    AND ((target_type = 1 AND target_id IS NULL) OR (target_type <> 1 AND target_id IS NOT NULL)));
                """);
        }
    }
}
