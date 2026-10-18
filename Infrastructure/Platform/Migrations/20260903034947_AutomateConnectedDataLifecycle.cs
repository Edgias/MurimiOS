using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AutomateConnectedDataLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "derived_media_days",
                schema: "analytics",
                table: "retention_policies",
                type: "integer",
                nullable: false,
                defaultValue: 90);

            migrationBuilder.AddColumn<int>(
                name: "expired_map_layer_days",
                schema: "analytics",
                table: "retention_policies",
                type: "integer",
                nullable: false,
                defaultValue: 90);

            migrationBuilder.CreateTable(
                name: "derived_media_assets",
                schema: "connected",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: true),
                    kind = table.Column<int>(type: "integer", nullable: false),
                    source_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    object_key = table.Column<string>(type: "character varying(1024)", maxLength: 1024, nullable: false),
                    content_type = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    byte_size = table.Column<long>(type: "bigint", nullable: false),
                    sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    generated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    legal_hold = table.Column<bool>(type: "boolean", nullable: false),
                    legal_hold_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    legal_hold_changed_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    legal_hold_changed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deleted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    deletion_attempts = table.Column<int>(type: "integer", nullable: false),
                    last_deletion_error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    deletion_lease_id = table.Column<Guid>(type: "uuid", nullable: true),
                    deletion_leased_until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_derived_media_assets", x => x.id);
                    table.ForeignKey(
                        name: "fk_derived_media_assets_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_derived_media_assets_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_derived_media_assets_status_legal_hold_generated_at",
                schema: "connected",
                table: "derived_media_assets",
                columns: new[] { "status", "legal_hold", "generated_at" });

            migrationBuilder.CreateIndex(
                name: "ix_derived_media_assets_tenant_id",
                schema: "connected",
                table: "derived_media_assets",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_derived_media_assets_tenant_id_farm_id",
                schema: "connected",
                table: "derived_media_assets",
                columns: new[] { "tenant_id", "farm_id" });

            migrationBuilder.CreateIndex(
                name: "ix_derived_media_assets_tenant_id_field_id",
                schema: "connected",
                table: "derived_media_assets",
                columns: new[] { "tenant_id", "field_id" });

            migrationBuilder.CreateIndex(
                name: "ix_derived_media_assets_tenant_id_object_key",
                schema: "connected",
                table: "derived_media_assets",
                columns: new[] { "tenant_id", "object_key" },
                unique: true);

            migrationBuilder.Sql("""
                ALTER TABLE analytics.retention_policies
                    ADD CONSTRAINT ck_retention_derived_media_days CHECK (derived_media_days BETWEEN 30 AND 3650),
                    ADD CONSTRAINT ck_retention_expired_map_layer_days CHECK (expired_map_layer_days BETWEEN 30 AND 3650);
                ALTER TABLE connected.derived_media_assets
                    ADD CONSTRAINT ck_derived_media_kind CHECK (kind BETWEEN 1 AND 5),
                    ADD CONSTRAINT ck_derived_media_status CHECK (status BETWEEN 1 AND 4),
                    ADD CONSTRAINT ck_derived_media_byte_size CHECK (byte_size > 0),
                    ADD CONSTRAINT ck_derived_media_sha256 CHECK (sha256 ~ '^[0-9a-f]{64}$'),
                    ADD CONSTRAINT ck_derived_media_hold CHECK (
                        (legal_hold AND legal_hold_reason IS NOT NULL AND legal_hold_changed_by IS NOT NULL AND legal_hold_changed_at IS NOT NULL)
                        OR (NOT legal_hold AND legal_hold_reason IS NULL)),
                    ADD CONSTRAINT ck_derived_media_deleted CHECK (
                        (status=3 AND deleted_at IS NOT NULL) OR (status<>3 AND deleted_at IS NULL));

                CREATE TABLE connected.partition_maintenance_runs (
                    id uuid PRIMARY KEY,
                    requested_at timestamptz NOT NULL,
                    current_year integer NOT NULL,
                    years_ahead integer NOT NULL,
                    partitions_created integer NOT NULL,
                    status text NOT NULL,
                    error_message text NULL,
                    completed_at timestamptz NOT NULL,
                    CONSTRAINT ck_partition_maintenance_horizon CHECK (years_ahead BETWEEN 1 AND 10),
                    CONSTRAINT ck_partition_maintenance_status CHECK (status IN ('Succeeded','Partial','Failed'))
                );
                CREATE INDEX ix_partition_maintenance_completed ON connected.partition_maintenance_runs(completed_at DESC);

                CREATE TABLE connected.lifecycle_retention_runs (
                    id uuid PRIMARY KEY,
                    tenant_id uuid NOT NULL,
                    policy_id uuid NOT NULL,
                    derived_media_cutoff timestamptz NOT NULL,
                    map_layer_cutoff timestamptz NOT NULL,
                    derived_media_queued bigint NOT NULL,
                    map_layers_deleted bigint NOT NULL,
                    completed_at timestamptz NOT NULL,
                    CONSTRAINT fk_lifecycle_retention_policy FOREIGN KEY(policy_id)
                        REFERENCES analytics.retention_policies(id) ON DELETE RESTRICT
                );
                CREATE INDEX ix_lifecycle_retention_tenant_completed
                    ON connected.lifecycle_retention_runs(tenant_id,completed_at DESC);

                ALTER TABLE connected.derived_media_assets ENABLE ROW LEVEL SECURITY;
                ALTER TABLE connected.derived_media_assets FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON connected.derived_media_assets
                    USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid)
                    WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);
                ALTER TABLE connected.lifecycle_retention_runs ENABLE ROW LEVEL SECURITY;
                ALTER TABLE connected.lifecycle_retention_runs FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON connected.lifecycle_retention_runs
                    USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid)
                    WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);

                CREATE OR REPLACE FUNCTION connected.ensure_future_partitions(p_now timestamptz,p_years_ahead integer)
                RETURNS integer LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,connected AS $function$
                DECLARE parent_name text; partition_name text; default_name text; year_value integer;
                DECLARE start_at timestamptz; finish_at timestamptz; created_count integer:=0; failed_count integer:=0;
                DECLARE errors text:=''; run_id uuid:=pg_catalog.gen_random_uuid();
                BEGIN
                    IF p_now IS NULL OR p_years_ahead NOT BETWEEN 1 AND 10 THEN
                        RAISE EXCEPTION 'Invalid connected-data partition horizon.' USING ERRCODE='22023';
                    END IF;
                    PERFORM pg_catalog.pg_advisory_xact_lock(pg_catalog.hashtext('murimios.connected.partitions'));
                    FOR parent_name IN SELECT unnest(ARRAY['sensor_readings','machinery_telemetry']) LOOP
                        FOR year_value IN SELECT generate_series(extract(year FROM p_now AT TIME ZONE 'UTC')::integer,
                            extract(year FROM p_now AT TIME ZONE 'UTC')::integer+p_years_ahead) LOOP
                            partition_name:=parent_name||'_'||year_value::text;
                            default_name:=parent_name||'_default';
                            IF pg_catalog.to_regclass('connected.'||partition_name) IS NULL THEN
                                start_at:=pg_catalog.make_timestamptz(year_value,1,1,0,0,0,'UTC');
                                finish_at:=pg_catalog.make_timestamptz(year_value+1,1,1,0,0,0,'UTC');
                                BEGIN
                                    EXECUTE format('CREATE TABLE connected.%I PARTITION OF connected.%I FOR VALUES FROM (%L) TO (%L)',
                                        partition_name,parent_name,start_at,finish_at);
                                    EXECUTE format('ALTER TABLE connected.%I ENABLE ROW LEVEL SECURITY',partition_name);
                                    EXECUTE format('ALTER TABLE connected.%I FORCE ROW LEVEL SECURITY',partition_name);
                                    EXECUTE format('CREATE POLICY tenant_isolation ON connected.%I USING (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid) WITH CHECK (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid)',partition_name);
                                    created_count:=created_count+1;
                                EXCEPTION WHEN OTHERS THEN
                                    failed_count:=failed_count+1;
                                    errors:=left(errors||CASE WHEN errors='' THEN '' ELSE '; ' END||partition_name||': '||SQLERRM,2000);
                                END;
                            END IF;
                        END LOOP;
                    END LOOP;
                    INSERT INTO connected.partition_maintenance_runs VALUES(run_id,p_now,
                        extract(year FROM p_now AT TIME ZONE 'UTC')::integer,p_years_ahead,created_count,
                        CASE WHEN failed_count=0 THEN 'Succeeded' WHEN created_count=0 THEN 'Failed' ELSE 'Partial' END,
                        nullif(errors,''),clock_timestamp());
                    RETURN CASE WHEN failed_count>0 THEN -failed_count ELSE created_count END;
                END $function$;

                CREATE OR REPLACE FUNCTION connected.claim_derived_media(
                    p_lease_id uuid,p_lock_seconds integer,p_maximum_attempts integer,p_now timestamptz)
                RETURNS TABLE(claimed_tenant_id uuid,claimed_asset_id uuid,claimed_lease_id uuid,
                    claimed_attempt integer,claimed_object_key character varying(1024))
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

                CREATE OR REPLACE FUNCTION connected.complete_derived_media_deletion(
                    p_asset_id uuid,p_lease_id uuid,p_completed_at timestamptz)
                RETURNS integer LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,connected AS $function$
                DECLARE affected integer;
                BEGIN
                    UPDATE connected.derived_media_assets SET status=3,deleted_at=p_completed_at,
                        deletion_lease_id=NULL,deletion_leased_until=NULL,last_deletion_error=NULL,
                        updated_at_utc=p_completed_at,version=version+1
                    WHERE id=p_asset_id AND status=2 AND NOT legal_hold AND deletion_lease_id=p_lease_id;
                    GET DIAGNOSTICS affected=ROW_COUNT; RETURN affected;
                END $function$;

                CREATE OR REPLACE FUNCTION connected.fail_derived_media_deletion(
                    p_asset_id uuid,p_lease_id uuid,p_failed_at timestamptz,p_error text)
                RETURNS integer LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,connected AS $function$
                DECLARE affected integer;
                BEGIN
                    UPDATE connected.derived_media_assets SET status=4,deletion_lease_id=NULL,
                        deletion_leased_until=p_failed_at+make_interval(secs=>least(3600,10*power(2,greatest(0,deletion_attempts-1)))::integer),
                        last_deletion_error=left(coalesce(p_error,'Unknown object-storage deletion failure.'),2000),
                        updated_at_utc=p_failed_at,version=version+1
                    WHERE id=p_asset_id AND status=2 AND deletion_lease_id=p_lease_id;
                    GET DIAGNOSTICS affected=ROW_COUNT; RETURN affected;
                END $function$;

                CREATE OR REPLACE FUNCTION connected.apply_extended_retention() RETURNS trigger
                LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,connected,insights AS $function$
                DECLARE queued bigint:=0; layer_deleted bigint:=0; derived_cutoff timestamptz; layer_cutoff timestamptz;
                BEGIN
                    IF NEW.last_run_at IS NOT DISTINCT FROM OLD.last_run_at THEN RETURN NEW; END IF;
                    derived_cutoff:=NEW.last_run_at-make_interval(days=>NEW.derived_media_days);
                    layer_cutoff:=NEW.last_run_at-make_interval(days=>NEW.expired_map_layer_days);
                    UPDATE connected.derived_media_assets SET status=2,deletion_lease_id=NULL,
                        deletion_leased_until=NULL,last_deletion_error=NULL,updated_at_utc=NEW.last_run_at,version=version+1
                    WHERE tenant_id=NEW.tenant_id AND status=1 AND NOT legal_hold AND generated_at<derived_cutoff;
                    GET DIAGNOSTICS queued=ROW_COUNT;
                    DELETE FROM insights.map_layers
                    WHERE tenant_id=NEW.tenant_id AND valid_to IS NOT NULL AND valid_to<layer_cutoff;
                    GET DIAGNOSTICS layer_deleted=ROW_COUNT;
                    INSERT INTO connected.lifecycle_retention_runs VALUES(pg_catalog.gen_random_uuid(),NEW.tenant_id,
                        NEW.id,derived_cutoff,layer_cutoff,queued,layer_deleted,clock_timestamp());
                    RETURN NEW;
                END $function$;
                CREATE TRIGGER retention_policy_extended_lifecycle
                    AFTER UPDATE OF last_run_at ON analytics.retention_policies
                    FOR EACH ROW EXECUTE FUNCTION connected.apply_extended_retention();

                CREATE OR REPLACE FUNCTION platform.guard_immutable_projection_record() RETURNS trigger
                LANGUAGE plpgsql AS $function$
                BEGIN
                    IF TG_OP='DELETE' AND current_user<>session_user
                       AND current_setting('app.analytics_retention',true)='on'
                       AND TG_TABLE_SCHEMA='connected'
                       AND (TG_TABLE_NAME IN ('sensor_readings','machinery_telemetry')
                            OR TG_TABLE_NAME LIKE 'sensor_readings\_%' ESCAPE '\'
                            OR TG_TABLE_NAME LIKE 'machinery_telemetry\_%' ESCAPE '\') THEN
                        RETURN OLD;
                    END IF;
                    RAISE EXCEPTION '% records are immutable.',TG_TABLE_NAME USING ERRCODE='55000';
                END $function$;

                REVOKE ALL ON FUNCTION connected.ensure_future_partitions(timestamptz,integer) FROM PUBLIC;
                REVOKE ALL ON FUNCTION connected.claim_derived_media(uuid,integer,integer,timestamptz) FROM PUBLIC;
                REVOKE ALL ON FUNCTION connected.complete_derived_media_deletion(uuid,uuid,timestamptz) FROM PUBLIC;
                REVOKE ALL ON FUNCTION connected.fail_derived_media_deletion(uuid,uuid,timestamptz,text) FROM PUBLIC;
                DO $grant$ BEGIN IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname='murimios_app') THEN
                    GRANT SELECT,INSERT,UPDATE,DELETE ON connected.derived_media_assets TO murimios_app;
                    GRANT SELECT ON connected.partition_maintenance_runs,connected.lifecycle_retention_runs TO murimios_app;
                    GRANT EXECUTE ON FUNCTION connected.ensure_future_partitions(timestamptz,integer),
                        connected.claim_derived_media(uuid,integer,integer,timestamptz),
                        connected.complete_derived_media_deletion(uuid,uuid,timestamptz),
                        connected.fail_derived_media_deletion(uuid,uuid,timestamptz,text) TO murimios_app;
                END IF; END $grant$;

                SELECT connected.ensure_future_partitions(clock_timestamp(),3);
                """);

            migrationBuilder.CreateIndex(
                name: "ix_derived_media_assets_tenant_id_source_type_source_id_kind",
                schema: "connected",
                table: "derived_media_assets",
                columns: new[] { "tenant_id", "source_type", "source_id", "kind" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP TRIGGER IF EXISTS retention_policy_extended_lifecycle ON analytics.retention_policies;
                DROP FUNCTION IF EXISTS connected.apply_extended_retention();
                DROP FUNCTION IF EXISTS connected.fail_derived_media_deletion(uuid,uuid,timestamptz,text);
                DROP FUNCTION IF EXISTS connected.complete_derived_media_deletion(uuid,uuid,timestamptz);
                DROP FUNCTION IF EXISTS connected.claim_derived_media(uuid,integer,integer,timestamptz);
                DROP FUNCTION IF EXISTS connected.ensure_future_partitions(timestamptz,integer);
                DROP TABLE IF EXISTS connected.lifecycle_retention_runs;
                DROP TABLE IF EXISTS connected.partition_maintenance_runs;
                CREATE OR REPLACE FUNCTION platform.guard_immutable_projection_record() RETURNS trigger
                LANGUAGE plpgsql AS $function$
                BEGIN
                    IF TG_OP='DELETE' AND current_user<>session_user
                       AND current_setting('app.analytics_retention',true)='on'
                       AND TG_TABLE_SCHEMA='connected' AND TG_TABLE_NAME IN ('sensor_readings','machinery_telemetry') THEN
                        RETURN OLD;
                    END IF;
                    RAISE EXCEPTION '% records are immutable.',TG_TABLE_NAME USING ERRCODE='55000';
                END $function$;
                """);
            migrationBuilder.DropTable(
                name: "derived_media_assets",
                schema: "connected");

            migrationBuilder.DropColumn(
                name: "derived_media_days",
                schema: "analytics",
                table: "retention_policies");

            migrationBuilder.DropColumn(
                name: "expired_map_layer_days",
                schema: "analytics",
                table: "retention_policies");
        }
    }
}
