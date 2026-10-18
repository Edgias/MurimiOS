using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class FixWebhookClaimProjectionTypes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION external_api.claim_webhook_deliveries(p_worker uuid,p_batch integer,p_lock_seconds integer,p_now timestamptz)
                RETURNS TABLE(id uuid,tenant_id uuid,subscription_id uuid,endpoint text,signing_key_hash text,event_type text,payload text,occurred_at timestamptz,attempt integer,lease_id uuid)
                LANGUAGE plpgsql VOLATILE SECURITY DEFINER SET search_path=pg_catalog,external_api AS $fn$
                BEGIN
                    IF p_worker IS NULL OR p_batch NOT BETWEEN 1 AND 200 OR p_lock_seconds NOT BETWEEN 5 AND 3600 THEN RAISE EXCEPTION 'Invalid webhook claim parameters.' USING ERRCODE='22023'; END IF;
                    RETURN QUERY WITH candidates AS (
                        SELECT d.id FROM external_api.webhook_deliveries d JOIN external_api.webhook_subscriptions s ON s.tenant_id=d.tenant_id AND s.id=d.subscription_id
                        WHERE s.status=1 AND ((d.status IN (1,3) AND d.available_at<=p_now) OR (d.status=2 AND d.leased_until<=p_now))
                        ORDER BY d.available_at,d.occurred_at FOR UPDATE OF d SKIP LOCKED LIMIT p_batch
                    ), claimed AS (
                        UPDATE external_api.webhook_deliveries d SET status=2,lease_id=p_worker,leased_until=p_now+make_interval(secs=>p_lock_seconds),attempts=d.attempts+1,updated_at_utc=p_now,version=d.version+1
                        FROM candidates c WHERE d.id=c.id RETURNING d.*
                    ) SELECT c.id,c.tenant_id,c.subscription_id,s.endpoint::text,s.signing_key_hash::text,c.event_type::text,c.payload::text,c.occurred_at,c.attempts,c.lease_id
                      FROM claimed c JOIN external_api.webhook_subscriptions s ON s.tenant_id=c.tenant_id AND s.id=c.subscription_id;
                END $fn$;
                REVOKE ALL ON FUNCTION external_api.claim_webhook_deliveries(uuid,integer,integer,timestamptz) FROM PUBLIC;
                DO $grant$ BEGIN IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname='murimios_app') THEN GRANT EXECUTE ON FUNCTION external_api.claim_webhook_deliveries(uuid,integer,integer,timestamptz) TO murimios_app; END IF; END $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
