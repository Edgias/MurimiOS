using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class FixDerivedMediaClaimResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            ReplaceClaimFunction(migrationBuilder, "character varying(1024)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            ReplaceClaimFunction(migrationBuilder, "text");
        }

        private static void ReplaceClaimFunction(MigrationBuilder migrationBuilder, string objectKeyType)
        {
            migrationBuilder.Sql($$"""
                DROP FUNCTION connected.claim_derived_media(uuid,integer,integer,timestamptz);
                CREATE FUNCTION connected.claim_derived_media(
                    p_lease_id uuid,p_lock_seconds integer,p_maximum_attempts integer,p_now timestamptz)
                RETURNS TABLE(claimed_tenant_id uuid,claimed_asset_id uuid,claimed_lease_id uuid,
                    claimed_attempt integer,claimed_object_key {{objectKeyType}})
                LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,connected AS $function$
                #variable_conflict use_column
                BEGIN
                    IF p_lease_id IS NULL OR p_lock_seconds NOT BETWEEN 30 AND 7200
                       OR p_maximum_attempts NOT BETWEEN 1 AND 20 THEN
                        RAISE EXCEPTION 'Invalid derived-media deletion lease.' USING ERRCODE='22023';
                    END IF;
                    RETURN QUERY WITH candidate AS (
                        SELECT a.id FROM connected.derived_media_assets a
                        WHERE NOT a.legal_hold AND a.deletion_attempts<p_maximum_attempts
                          AND (a.status=2 OR (a.status=4 AND coalesce(a.deletion_leased_until,'-infinity'::timestamptz)<=p_now))
                          AND (a.deletion_lease_id IS NULL OR a.deletion_leased_until<=p_now)
                        ORDER BY a.generated_at,a.id FOR UPDATE SKIP LOCKED LIMIT 1
                    )
                    UPDATE connected.derived_media_assets a SET status=2,deletion_lease_id=p_lease_id,
                        deletion_leased_until=p_now+make_interval(secs=>p_lock_seconds),
                        deletion_attempts=a.deletion_attempts+1,last_deletion_error=NULL,
                        updated_at_utc=p_now,version=version+1
                    FROM candidate c WHERE a.id=c.id
                    RETURNING a.tenant_id,a.id,a.deletion_lease_id,a.deletion_attempts,a.object_key;
                END $function$;
                REVOKE ALL ON FUNCTION connected.claim_derived_media(uuid,integer,integer,timestamptz) FROM PUBLIC;
                DO $grant$ BEGIN IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname='murimios_app') THEN
                    GRANT EXECUTE ON FUNCTION connected.claim_derived_media(uuid,integer,integer,timestamptz) TO murimios_app;
                END IF; END $grant$;
                """);
        }
    }
}
