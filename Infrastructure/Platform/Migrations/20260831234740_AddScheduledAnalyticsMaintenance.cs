using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddScheduledAnalyticsMaintenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "projection_schedules",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    frequency = table.Column<int>(type: "integer", nullable: false),
                    lookback_days = table.Column<int>(type: "integer", nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    projection_version = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    next_run_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_scheduled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_projection_schedules", x => x.id);
                    table.ForeignKey(
                        name: "fk_projection_schedules_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "retention_policies",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    raw_sensor_days = table.Column<int>(type: "integer", nullable: false),
                    raw_telemetry_days = table.Column<int>(type: "integer", nullable: false),
                    failed_projection_days = table.Column<int>(type: "integer", nullable: false),
                    next_run_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_run_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    configured_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_retention_policies", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_projection_schedules_is_active_next_run_at",
                schema: "analytics",
                table: "projection_schedules",
                columns: new[] { "is_active", "next_run_at" });

            migrationBuilder.CreateIndex(
                name: "ix_projection_schedules_tenant_id",
                schema: "analytics",
                table: "projection_schedules",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_projection_schedules_tenant_id_farm_id_name",
                schema: "analytics",
                table: "projection_schedules",
                columns: new[] { "tenant_id", "farm_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_retention_policies_is_active_next_run_at",
                schema: "analytics",
                table: "retention_policies",
                columns: new[] { "is_active", "next_run_at" });

            migrationBuilder.CreateIndex(
                name: "ix_retention_policies_tenant_id",
                schema: "analytics",
                table: "retention_policies",
                column: "tenant_id",
                unique: true,
                filter: "is_active");

            migrationBuilder.Sql("""
                ALTER TABLE analytics.projection_schedules
                    ADD CONSTRAINT ck_projection_schedules_frequency CHECK (frequency BETWEEN 1 AND 3),
                    ADD CONSTRAINT ck_projection_schedules_lookback CHECK (lookback_days BETWEEN 1 AND 366),
                    ADD CONSTRAINT ck_projection_schedules_currency CHECK (currency ~ '^[A-Z]{3}$');
                ALTER TABLE analytics.retention_policies
                    ADD CONSTRAINT ck_retention_periods CHECK (
                        raw_sensor_days BETWEEN 30 AND 3650 AND raw_telemetry_days BETWEEN 30 AND 3650
                        AND failed_projection_days BETWEEN 30 AND 3650);

                CREATE TABLE analytics.projection_execution (
                    tenant_id uuid NOT NULL,
                    run_id uuid NOT NULL,
                    lease_id uuid NULL,
                    attempt integer NOT NULL DEFAULT 0,
                    leased_until timestamptz NULL,
                    last_claimed_at timestamptz NULL,
                    completed_at timestamptz NULL,
                    last_error varchar(2000) NULL,
                    created_at timestamptz NOT NULL,
                    updated_at timestamptz NOT NULL,
                    CONSTRAINT pk_projection_execution PRIMARY KEY (tenant_id,run_id),
                    CONSTRAINT fk_projection_execution_run FOREIGN KEY (tenant_id,run_id)
                        REFERENCES analytics.projection_runs(tenant_id,id) ON DELETE CASCADE,
                    CONSTRAINT ck_projection_execution_attempt CHECK (attempt >= 0),
                    CONSTRAINT ck_projection_execution_lease CHECK ((lease_id IS NULL) = (leased_until IS NULL))
                );
                CREATE INDEX ix_projection_execution_claim ON analytics.projection_execution(leased_until,attempt);

                CREATE TABLE analytics.farm_metric_rollups (
                    tenant_id uuid NOT NULL,
                    farm_id uuid NOT NULL,
                    period_end date NOT NULL,
                    metric_code varchar(120) NOT NULL,
                    unit varchar(40) NOT NULL,
                    value_kind integer NOT NULL,
                    total numeric(38,8) NOT NULL,
                    average numeric(38,8) NOT NULL,
                    minimum numeric(38,8) NOT NULL,
                    maximum numeric(38,8) NOT NULL,
                    data_quality_percent numeric(8,4) NOT NULL,
                    grain_count integer NOT NULL,
                    source_record_count bigint NOT NULL,
                    refreshed_at timestamptz NOT NULL,
                    CONSTRAINT pk_farm_metric_rollups PRIMARY KEY (tenant_id,farm_id,period_end,metric_code,unit,value_kind),
                    CONSTRAINT fk_farm_metric_rollups_farm FOREIGN KEY (tenant_id,farm_id)
                        REFERENCES farm.farms(tenant_id,id) ON DELETE RESTRICT,
                    CONSTRAINT ck_farm_metric_rollups_kind CHECK (value_kind BETWEEN 1 AND 3),
                    CONSTRAINT ck_farm_metric_rollups_quality CHECK (data_quality_percent BETWEEN 0 AND 100)
                );
                CREATE INDEX ix_farm_metric_rollups_metric_period ON analytics.farm_metric_rollups(tenant_id,metric_code,period_end);

                CREATE TABLE analytics.warehouse_refresh_runs (
                    id uuid PRIMARY KEY,
                    started_at timestamptz NOT NULL,
                    completed_at timestamptz NOT NULL,
                    fact_count bigint NOT NULL,
                    aggregate_count integer NOT NULL,
                    status varchar(20) NOT NULL,
                    error varchar(2000) NULL
                );
                CREATE INDEX ix_warehouse_refresh_runs_completed ON analytics.warehouse_refresh_runs(completed_at DESC);

                CREATE TABLE analytics.sensor_daily_rollups (
                    tenant_id uuid NOT NULL,
                    farm_id uuid NOT NULL,
                    device_id uuid NOT NULL,
                    channel_id uuid NOT NULL,
                    metric_code varchar(120) NOT NULL,
                    unit varchar(40) NOT NULL,
                    observed_on date NOT NULL,
                    reading_count bigint NOT NULL,
                    value_sum numeric(38,8) NOT NULL,
                    minimum numeric(24,8) NOT NULL,
                    maximum numeric(24,8) NOT NULL,
                    quality_percent numeric(8,4) NOT NULL,
                    last_observed_at timestamptz NOT NULL,
                    refreshed_at timestamptz NOT NULL,
                    CONSTRAINT pk_sensor_daily_rollups PRIMARY KEY (tenant_id,device_id,channel_id,metric_code,unit,observed_on),
                    CONSTRAINT fk_sensor_daily_rollups_farm FOREIGN KEY (tenant_id,farm_id)
                        REFERENCES farm.farms(tenant_id,id) ON DELETE RESTRICT,
                    CONSTRAINT ck_sensor_daily_rollups_count CHECK (reading_count > 0),
                    CONSTRAINT ck_sensor_daily_rollups_quality CHECK (quality_percent BETWEEN 0 AND 100)
                );
                CREATE INDEX ix_sensor_daily_rollups_farm_metric ON analytics.sensor_daily_rollups(tenant_id,farm_id,metric_code,observed_on);

                CREATE TABLE analytics.machinery_daily_rollups (
                    tenant_id uuid NOT NULL,
                    farm_id uuid NOT NULL,
                    equipment_id uuid NOT NULL,
                    observed_on date NOT NULL,
                    reading_count bigint NOT NULL,
                    maximum_engine_hours numeric(18,4) NULL,
                    speed_sum numeric(38,8) NOT NULL,
                    speed_sample_count bigint NOT NULL,
                    fuel_sum numeric(38,8) NOT NULL,
                    fuel_sample_count bigint NOT NULL,
                    last_observed_at timestamptz NOT NULL,
                    refreshed_at timestamptz NOT NULL,
                    CONSTRAINT pk_machinery_daily_rollups PRIMARY KEY (tenant_id,equipment_id,observed_on),
                    CONSTRAINT fk_machinery_daily_rollups_farm FOREIGN KEY (tenant_id,farm_id)
                        REFERENCES farm.farms(tenant_id,id) ON DELETE RESTRICT,
                    CONSTRAINT ck_machinery_daily_rollups_counts CHECK (
                        reading_count > 0 AND speed_sample_count >= 0 AND fuel_sample_count >= 0)
                );
                CREATE INDEX ix_machinery_daily_rollups_farm_day ON analytics.machinery_daily_rollups(tenant_id,farm_id,observed_on);

                CREATE TABLE analytics.retention_runs (
                    id uuid NOT NULL PRIMARY KEY,
                    tenant_id uuid NOT NULL,
                    policy_id uuid NOT NULL,
                    sensor_cutoff timestamptz NOT NULL,
                    telemetry_cutoff timestamptz NOT NULL,
                    failed_projection_cutoff timestamptz NOT NULL,
                    sensor_rows_deleted bigint NOT NULL,
                    telemetry_rows_deleted bigint NOT NULL,
                    projection_rows_deleted bigint NOT NULL,
                    started_at timestamptz NOT NULL,
                    completed_at timestamptz NOT NULL,
                    status varchar(20) NOT NULL,
                    CONSTRAINT fk_retention_runs_policy FOREIGN KEY (policy_id)
                        REFERENCES analytics.retention_policies(id) ON DELETE RESTRICT
                );
                CREATE INDEX ix_retention_runs_tenant_completed ON analytics.retention_runs(tenant_id,completed_at DESC);

                DO $rls$
                DECLARE table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY[
                        'projection_schedules','retention_policies','projection_execution','farm_metric_rollups',
                        'sensor_daily_rollups','machinery_daily_rollups','retention_runs'] LOOP
                        EXECUTE format('ALTER TABLE analytics.%I ENABLE ROW LEVEL SECURITY',table_name);
                        EXECUTE format('ALTER TABLE analytics.%I FORCE ROW LEVEL SECURITY',table_name);
                        EXECUTE format('CREATE POLICY tenant_isolation ON analytics.%I USING (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid) WITH CHECK (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid)',table_name);
                    END LOOP;
                END
                $rls$;

                CREATE OR REPLACE FUNCTION analytics.schedule_due_projections(p_now timestamptz,p_batch_size integer)
                RETURNS integer LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,analytics,public AS $function$
                DECLARE schedule_record record; inserted_count integer:=0; affected integer;
                DECLARE period_finish date; period_begin date;
                BEGIN
                    IF p_batch_size NOT BETWEEN 1 AND 1000 THEN RAISE EXCEPTION 'Invalid analytics schedule batch size.' USING ERRCODE='22023'; END IF;
                    FOR schedule_record IN
                        SELECT s.* FROM analytics.projection_schedules s
                        WHERE s.is_active AND s.next_run_at<=p_now
                        ORDER BY s.next_run_at,s.id FOR UPDATE SKIP LOCKED LIMIT p_batch_size
                    LOOP
                        period_finish := (p_now AT TIME ZONE 'UTC')::date - 1;
                        period_begin := period_finish - (schedule_record.lookback_days - 1);
                        INSERT INTO analytics.projection_runs(
                            id,farm_id,period_start,period_end,currency,projection_version,scheduled_for,requested_by,
                            status,started_at,completed_at,fact_count,data_quality_percent,error_code,error_message,
                            tenant_id,created_at_utc,updated_at_utc,version)
                        VALUES(pg_catalog.gen_random_uuid(),schedule_record.farm_id,period_begin,period_finish,
                            schedule_record.currency,schedule_record.projection_version,p_now,
                            'analytics-schedule:'||schedule_record.id::text,1,NULL,NULL,0,NULL,NULL,NULL,
                            schedule_record.tenant_id,p_now,p_now,1)
                        ON CONFLICT (tenant_id,farm_id,period_start,period_end,projection_version) DO NOTHING;
                        GET DIAGNOSTICS affected=ROW_COUNT;
                        inserted_count:=inserted_count+affected;
                        UPDATE analytics.projection_schedules SET last_scheduled_at=p_now,
                            next_run_at=CASE frequency WHEN 1 THEN greatest(next_run_at,p_now)+interval '1 day'
                                WHEN 2 THEN greatest(next_run_at,p_now)+interval '7 days'
                                ELSE greatest(next_run_at,p_now)+interval '1 month' END,
                            updated_at_utc=p_now,version=version+1 WHERE id=schedule_record.id;
                    END LOOP;
                    RETURN inserted_count;
                END $function$;

                CREATE OR REPLACE FUNCTION analytics.claim_projection(
                    p_lease_id uuid,p_lock_seconds integer,p_maximum_attempts integer,p_now timestamptz)
                RETURNS TABLE(claimed_tenant_id uuid,claimed_run_id uuid,claimed_lease_id uuid,claimed_attempt integer)
                LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,analytics AS $function$
                #variable_conflict use_column
                BEGIN
                    IF p_lease_id IS NULL OR p_lock_seconds NOT BETWEEN 30 AND 7200 OR p_maximum_attempts NOT BETWEEN 1 AND 20 THEN
                        RAISE EXCEPTION 'Invalid analytics projection lease.' USING ERRCODE='22023'; END IF;
                    RETURN QUERY WITH candidate AS (
                        SELECT r.tenant_id,r.id FROM analytics.projection_runs r
                        LEFT JOIN analytics.projection_execution e ON e.tenant_id=r.tenant_id AND e.run_id=r.id
                        WHERE ((r.status=1 AND r.scheduled_for<=p_now) OR (r.status=2 AND e.leased_until<=p_now))
                          AND coalesce(e.attempt,0)<p_maximum_attempts
                        ORDER BY r.scheduled_for,r.id FOR UPDATE OF r SKIP LOCKED LIMIT 1
                    ), claimed_run AS (
                        UPDATE analytics.projection_runs r SET status=2,started_at=coalesce(r.started_at,p_now),
                            completed_at=NULL,error_code=NULL,error_message=NULL,updated_at_utc=p_now,version=r.version+1
                        FROM candidate c WHERE r.tenant_id=c.tenant_id AND r.id=c.id RETURNING r.tenant_id,r.id
                    ), claimed_execution AS (
                        INSERT INTO analytics.projection_execution(
                            tenant_id,run_id,lease_id,attempt,leased_until,last_claimed_at,completed_at,last_error,created_at,updated_at)
                        SELECT c.tenant_id,c.id,p_lease_id,1,p_now+make_interval(secs=>p_lock_seconds),p_now,NULL,NULL,p_now,p_now
                        FROM claimed_run c ON CONFLICT (tenant_id,run_id) DO UPDATE SET
                            lease_id=p_lease_id,attempt=analytics.projection_execution.attempt+1,
                            leased_until=p_now+make_interval(secs=>p_lock_seconds),last_claimed_at=p_now,
                            completed_at=NULL,last_error=NULL,updated_at=p_now
                        RETURNING analytics.projection_execution.tenant_id,analytics.projection_execution.run_id,
                            analytics.projection_execution.lease_id,analytics.projection_execution.attempt)
                    SELECT c.tenant_id,c.run_id,c.lease_id,c.attempt FROM claimed_execution c;
                END $function$;

                CREATE OR REPLACE FUNCTION analytics.complete_projection(p_run_id uuid,p_lease_id uuid,p_completed_at timestamptz)
                RETURNS integer LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,analytics AS $function$
                DECLARE affected integer;
                BEGIN
                    UPDATE analytics.projection_execution e SET lease_id=NULL,leased_until=NULL,completed_at=p_completed_at,
                        last_error=NULL,updated_at=p_completed_at
                    WHERE e.run_id=p_run_id AND e.lease_id=p_lease_id
                      AND EXISTS(SELECT 1 FROM analytics.projection_runs r WHERE r.tenant_id=e.tenant_id AND r.id=e.run_id AND r.status=3);
                    GET DIAGNOSTICS affected=ROW_COUNT; RETURN affected;
                END $function$;

                CREATE OR REPLACE FUNCTION analytics.fail_projection(
                    p_run_id uuid,p_lease_id uuid,p_failed_at timestamptz,p_maximum_attempts integer,p_error text)
                RETURNS integer LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,analytics AS $function$
                DECLARE execution_record record; affected integer;
                BEGIN
                    SELECT * INTO execution_record FROM analytics.projection_execution
                    WHERE run_id=p_run_id AND lease_id=p_lease_id FOR UPDATE;
                    IF execution_record IS NULL THEN RETURN 0; END IF;
                    UPDATE analytics.projection_runs SET
                        status=CASE WHEN execution_record.attempt>=p_maximum_attempts THEN 4 ELSE 1 END,
                        scheduled_for=CASE WHEN execution_record.attempt>=p_maximum_attempts THEN scheduled_for
                            ELSE p_failed_at+make_interval(secs=>least(3600,10*power(2,greatest(0,execution_record.attempt-1)))::integer) END,
                        started_at=CASE WHEN execution_record.attempt>=p_maximum_attempts THEN started_at ELSE NULL END,
                        completed_at=CASE WHEN execution_record.attempt>=p_maximum_attempts THEN p_failed_at ELSE NULL END,
                        error_code='projection_failed',error_message=left(coalesce(p_error,'Unknown projection failure.'),2000),
                        updated_at_utc=p_failed_at,version=version+1
                    WHERE tenant_id=execution_record.tenant_id AND id=p_run_id AND status=2;
                    GET DIAGNOSTICS affected=ROW_COUNT;
                    UPDATE analytics.projection_execution SET lease_id=NULL,leased_until=NULL,last_error=left(coalesce(p_error,'Unknown projection failure.'),2000),
                        updated_at=p_failed_at WHERE tenant_id=execution_record.tenant_id AND run_id=p_run_id;
                    RETURN affected;
                END $function$;

                CREATE OR REPLACE FUNCTION analytics.refresh_warehouse(p_now timestamptz)
                RETURNS integer LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,analytics,public AS $function$
                DECLARE refresh_id uuid:=pg_catalog.gen_random_uuid(); aggregate_count integer; fact_count bigint;
                BEGIN
                    SELECT count(*) INTO fact_count FROM analytics.facts;
                    INSERT INTO analytics.farm_metric_rollups(
                        tenant_id,farm_id,period_end,metric_code,unit,value_kind,total,average,minimum,maximum,
                        data_quality_percent,grain_count,source_record_count,refreshed_at)
                    SELECT tenant_id,farm_id,period_end,metric_code,unit,value_kind,sum(value),avg(value),min(value),max(value),
                        avg(data_quality_percent),count(*)::integer,sum(source_record_count),p_now
                    FROM analytics.facts GROUP BY tenant_id,farm_id,period_end,metric_code,unit,value_kind
                    ON CONFLICT (tenant_id,farm_id,period_end,metric_code,unit,value_kind) DO UPDATE SET
                        total=excluded.total,average=excluded.average,minimum=excluded.minimum,maximum=excluded.maximum,
                        data_quality_percent=excluded.data_quality_percent,grain_count=excluded.grain_count,
                        source_record_count=excluded.source_record_count,refreshed_at=excluded.refreshed_at;
                    GET DIAGNOSTICS aggregate_count=ROW_COUNT;
                    INSERT INTO analytics.warehouse_refresh_runs VALUES(refresh_id,p_now,clock_timestamp(),fact_count,aggregate_count,'Succeeded',NULL);
                    RETURN aggregate_count;
                EXCEPTION WHEN OTHERS THEN
                    INSERT INTO analytics.warehouse_refresh_runs VALUES(refresh_id,p_now,clock_timestamp(),0,0,'Failed',left(SQLERRM,2000));
                    RAISE;
                END $function$;

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

                CREATE OR REPLACE FUNCTION analytics.run_due_retention(p_now timestamptz,p_batch_size integer)
                RETURNS integer LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,analytics,connected,public AS $function$
                DECLARE policy_record record; completed integer:=0; sensor_deleted bigint; telemetry_deleted bigint; projection_deleted bigint;
                DECLARE sensor_cutoff timestamptz; telemetry_cutoff timestamptz; projection_cutoff timestamptz; run_id uuid;
                BEGIN
                    IF p_batch_size NOT BETWEEN 1 AND 100 THEN RAISE EXCEPTION 'Invalid retention batch size.' USING ERRCODE='22023'; END IF;
                    PERFORM set_config('app.analytics_retention','on',true);
                    FOR policy_record IN SELECT p.* FROM analytics.retention_policies p
                        WHERE p.is_active AND p.next_run_at<=p_now ORDER BY p.next_run_at,p.id
                        FOR UPDATE SKIP LOCKED LIMIT p_batch_size
                    LOOP
                        run_id:=pg_catalog.gen_random_uuid();
                        sensor_cutoff:=p_now-make_interval(days=>policy_record.raw_sensor_days);
                        telemetry_cutoff:=p_now-make_interval(days=>policy_record.raw_telemetry_days);
                        projection_cutoff:=p_now-make_interval(days=>policy_record.failed_projection_days);

                        INSERT INTO analytics.sensor_daily_rollups(
                            tenant_id,farm_id,device_id,channel_id,metric_code,unit,observed_on,reading_count,value_sum,
                            minimum,maximum,quality_percent,last_observed_at,refreshed_at)
                        SELECT tenant_id,farm_id,device_id,channel_id,metric_code,unit,(observed_at AT TIME ZONE 'UTC')::date,
                            count(*),sum(value),min(value),max(value),100.0*count(*) FILTER(WHERE quality=1)/count(*),max(observed_at),p_now
                        FROM connected.sensor_readings WHERE tenant_id=policy_record.tenant_id AND observed_at<sensor_cutoff
                        GROUP BY tenant_id,farm_id,device_id,channel_id,metric_code,unit,(observed_at AT TIME ZONE 'UTC')::date
                        ON CONFLICT (tenant_id,device_id,channel_id,metric_code,unit,observed_on) DO UPDATE SET
                            reading_count=excluded.reading_count,value_sum=excluded.value_sum,minimum=excluded.minimum,
                            maximum=excluded.maximum,quality_percent=excluded.quality_percent,
                            last_observed_at=excluded.last_observed_at,refreshed_at=excluded.refreshed_at;
                        DELETE FROM connected.sensor_readings WHERE tenant_id=policy_record.tenant_id AND observed_at<sensor_cutoff;
                        GET DIAGNOSTICS sensor_deleted=ROW_COUNT;

                        INSERT INTO analytics.machinery_daily_rollups(
                            tenant_id,farm_id,equipment_id,observed_on,reading_count,maximum_engine_hours,
                            speed_sum,speed_sample_count,fuel_sum,fuel_sample_count,last_observed_at,refreshed_at)
                        SELECT tenant_id,farm_id,equipment_id,(observed_at AT TIME ZONE 'UTC')::date,count(*),max(engine_hours),
                            coalesce(sum(speed_kph),0),count(speed_kph),coalesce(sum(fuel_level_percent),0),count(fuel_level_percent),max(observed_at),p_now
                        FROM connected.machinery_telemetry WHERE tenant_id=policy_record.tenant_id AND observed_at<telemetry_cutoff
                        GROUP BY tenant_id,farm_id,equipment_id,(observed_at AT TIME ZONE 'UTC')::date
                        ON CONFLICT (tenant_id,equipment_id,observed_on) DO UPDATE SET
                            reading_count=excluded.reading_count,maximum_engine_hours=excluded.maximum_engine_hours,
                            speed_sum=excluded.speed_sum,speed_sample_count=excluded.speed_sample_count,
                            fuel_sum=excluded.fuel_sum,fuel_sample_count=excluded.fuel_sample_count,
                            last_observed_at=excluded.last_observed_at,refreshed_at=excluded.refreshed_at;
                        DELETE FROM connected.machinery_telemetry WHERE tenant_id=policy_record.tenant_id AND observed_at<telemetry_cutoff;
                        GET DIAGNOSTICS telemetry_deleted=ROW_COUNT;

                        DELETE FROM analytics.projection_execution e USING analytics.projection_runs r
                            WHERE e.tenant_id=r.tenant_id AND e.run_id=r.id AND r.tenant_id=policy_record.tenant_id
                              AND r.status IN (4,5) AND r.completed_at<projection_cutoff
                              AND NOT EXISTS(SELECT 1 FROM analytics.facts f WHERE f.tenant_id=r.tenant_id AND f.projection_run_id=r.id);
                        DELETE FROM analytics.projection_runs r WHERE r.tenant_id=policy_record.tenant_id
                            AND r.status IN (4,5) AND r.completed_at<projection_cutoff
                            AND NOT EXISTS(SELECT 1 FROM analytics.facts f WHERE f.tenant_id=r.tenant_id AND f.projection_run_id=r.id);
                        GET DIAGNOSTICS projection_deleted=ROW_COUNT;

                        INSERT INTO analytics.retention_runs VALUES(run_id,policy_record.tenant_id,policy_record.id,
                            sensor_cutoff,telemetry_cutoff,projection_cutoff,sensor_deleted,telemetry_deleted,projection_deleted,
                            p_now,clock_timestamp(),'Succeeded');
                        UPDATE analytics.retention_policies SET last_run_at=p_now,next_run_at=p_now+interval '1 day',
                            updated_at_utc=p_now,version=version+1 WHERE id=policy_record.id;
                        completed:=completed+1;
                    END LOOP;
                    PERFORM set_config('app.analytics_retention','off',true);
                    RETURN completed;
                END $function$;

                REVOKE ALL ON FUNCTION analytics.schedule_due_projections(timestamptz,integer) FROM PUBLIC;
                REVOKE ALL ON FUNCTION analytics.claim_projection(uuid,integer,integer,timestamptz) FROM PUBLIC;
                REVOKE ALL ON FUNCTION analytics.complete_projection(uuid,uuid,timestamptz) FROM PUBLIC;
                REVOKE ALL ON FUNCTION analytics.fail_projection(uuid,uuid,timestamptz,integer,text) FROM PUBLIC;
                REVOKE ALL ON FUNCTION analytics.refresh_warehouse(timestamptz) FROM PUBLIC;
                REVOKE ALL ON FUNCTION analytics.run_due_retention(timestamptz,integer) FROM PUBLIC;
                DO $grant$ BEGIN IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname='murimios_app') THEN
                    GRANT USAGE ON SCHEMA analytics TO murimios_app;
                    GRANT SELECT,INSERT,UPDATE,DELETE ON analytics.projection_schedules,analytics.retention_policies,
                        analytics.projection_execution,analytics.farm_metric_rollups,analytics.sensor_daily_rollups,
                        analytics.machinery_daily_rollups,analytics.retention_runs TO murimios_app;
                    GRANT SELECT ON analytics.warehouse_refresh_runs TO murimios_app;
                    GRANT EXECUTE ON FUNCTION analytics.schedule_due_projections(timestamptz,integer),
                        analytics.claim_projection(uuid,integer,integer,timestamptz),analytics.complete_projection(uuid,uuid,timestamptz),
                        analytics.fail_projection(uuid,uuid,timestamptz,integer,text),analytics.refresh_warehouse(timestamptz),
                        analytics.run_due_retention(timestamptz,integer) TO murimios_app;
                END IF; END $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP FUNCTION IF EXISTS analytics.run_due_retention(timestamptz,integer);
                DROP FUNCTION IF EXISTS analytics.refresh_warehouse(timestamptz);
                DROP FUNCTION IF EXISTS analytics.fail_projection(uuid,uuid,timestamptz,integer,text);
                DROP FUNCTION IF EXISTS analytics.complete_projection(uuid,uuid,timestamptz);
                DROP FUNCTION IF EXISTS analytics.claim_projection(uuid,integer,integer,timestamptz);
                DROP FUNCTION IF EXISTS analytics.schedule_due_projections(timestamptz,integer);
                DROP TABLE IF EXISTS analytics.retention_runs;
                DROP TABLE IF EXISTS analytics.machinery_daily_rollups;
                DROP TABLE IF EXISTS analytics.sensor_daily_rollups;
                DROP TABLE IF EXISTS analytics.warehouse_refresh_runs;
                DROP TABLE IF EXISTS analytics.farm_metric_rollups;
                DROP TABLE IF EXISTS analytics.projection_execution;
                CREATE OR REPLACE FUNCTION platform.guard_immutable_projection_record() RETURNS trigger
                LANGUAGE plpgsql AS $$ BEGIN RAISE EXCEPTION '% records are immutable.',TG_TABLE_NAME USING ERRCODE='55000'; END $$;
                """);
            migrationBuilder.DropTable(
                name: "projection_schedules",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "retention_policies",
                schema: "analytics");
        }
    }
}
