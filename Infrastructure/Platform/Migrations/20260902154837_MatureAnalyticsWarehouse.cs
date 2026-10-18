using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class MatureAnalyticsWarehouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_projection_runs_tenant_id_farm_id_period_start_period_end_p",
                schema: "analytics",
                table: "projection_runs");

            migrationBuilder.AddColumn<int>(
                name: "revision",
                schema: "analytics",
                table: "projection_runs",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "source_refresh_request_id",
                schema: "analytics",
                table: "projection_runs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "superseded_at",
                schema: "analytics",
                table: "projection_runs",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "superseded_by_run_id",
                schema: "analytics",
                table: "projection_runs",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "trigger_type",
                schema: "analytics",
                table: "projection_runs",
                type: "character varying(80)",
                maxLength: 80,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "confidence_percent",
                schema: "analytics",
                table: "facts",
                type: "numeric(8,4)",
                precision: 8,
                scale: 4,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "lower_bound",
                schema: "analytics",
                table: "facts",
                type: "numeric(28,8)",
                precision: 28,
                scale: 8,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "predictive_model_definition_id",
                schema: "analytics",
                table: "facts",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "upper_bound",
                schema: "analytics",
                table: "facts",
                type: "numeric(28,8)",
                precision: 28,
                scale: 8,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "cohort_definition_id",
                schema: "analytics",
                table: "benchmark_enrollments",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "ak_analytics_facts_tenant_id_id",
                schema: "analytics",
                table: "facts",
                columns: new[] { "tenant_id", "id" });

            migrationBuilder.CreateTable(
                name: "benchmark_cohort_definitions",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    cohort_key = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    definition_version = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    inclusion_criteria_json = table.Column<string>(type: "jsonb", nullable: false),
                    minimum_cohort_size = table.Column<int>(type: "integer", nullable: false),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_to = table.Column<DateOnly>(type: "date", nullable: true),
                    owner = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    approved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    approved_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    approval_reference = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    retired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    retired_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_benchmark_cohort_definitions", x => x.id);
                    table.UniqueConstraint("ak_benchmark_cohort_definitions_tenant_id_id", x => new { x.tenant_id, x.id });
                });

            migrationBuilder.CreateTable(
                name: "dimension_snapshots",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    projection_run_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dimension_type = table.Column<int>(type: "integer", nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    grain_key = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    natural_key = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    attributes_json = table.Column<string>(type: "jsonb", nullable: false),
                    attributes_sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    valid_on = table.Column<DateOnly>(type: "date", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dimension_snapshots", x => x.id);
                    table.ForeignKey(
                        name: "fk_dimension_snapshots_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_dimension_snapshots_projection_runs_tenant_id_projection_ru",
                        columns: x => new { x.tenant_id, x.projection_run_id },
                        principalSchema: "analytics",
                        principalTable: "projection_runs",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "forecast_evaluations",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    forecast_fact_id = table.Column<Guid>(type: "uuid", nullable: false),
                    actual_fact_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    metric_code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    grain_key = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    forecast_value = table.Column<decimal>(type: "numeric(28,8)", precision: 28, scale: 8, nullable: false),
                    actual_value = table.Column<decimal>(type: "numeric(28,8)", precision: 28, scale: 8, nullable: false),
                    unit = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    absolute_error = table.Column<decimal>(type: "numeric(28,8)", precision: 28, scale: 8, nullable: false),
                    absolute_percentage_error = table.Column<decimal>(type: "numeric(18,8)", precision: 18, scale: 8, nullable: true),
                    bias = table.Column<decimal>(type: "numeric(28,8)", precision: 28, scale: 8, nullable: false),
                    horizon_days = table.Column<int>(type: "integer", nullable: false),
                    model_version = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    evaluated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_forecast_evaluations", x => x.id);
                    table.ForeignKey(
                        name: "fk_forecast_evaluations_facts_tenant_id_actual_fact_id",
                        columns: x => new { x.tenant_id, x.actual_fact_id },
                        principalSchema: "analytics",
                        principalTable: "facts",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_forecast_evaluations_facts_tenant_id_forecast_fact_id",
                        columns: x => new { x.tenant_id, x.forecast_fact_id },
                        principalSchema: "analytics",
                        principalTable: "facts",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_forecast_evaluations_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "predictive_model_definitions",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    model_key = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    model_version = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    target_metric_code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    algorithm = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    feature_schema_json = table.Column<string>(type: "jsonb", nullable: false),
                    training_window_days = table.Column<int>(type: "integer", nullable: false),
                    minimum_observations = table.Column<int>(type: "integer", nullable: false),
                    validation_mae = table.Column<decimal>(type: "numeric(28,8)", precision: 28, scale: 8, nullable: false),
                    validation_mape_percent = table.Column<decimal>(type: "numeric(10,4)", precision: 10, scale: 4, nullable: true),
                    validation_dataset_reference = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    approved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    approved_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    approval_reference = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    retired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    retired_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_predictive_model_definitions", x => x.id);
                    table.UniqueConstraint("ak_predictive_model_definitions_tenant_id_id", x => new { x.tenant_id, x.id });
                });

            migrationBuilder.CreateTable(
                name: "refresh_requests",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_outbox_message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trigger_type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    first_triggered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_triggered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    available_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    scheduled_run_id = table.Column<Guid>(type: "uuid", nullable: true),
                    processed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    event_count = table.Column<int>(type: "integer", nullable: false),
                    last_error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_refresh_requests", x => x.id);
                    table.ForeignKey(
                        name: "fk_refresh_requests_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_projection_runs_tenant_id_farm_id_period_end_superseded_at",
                schema: "analytics",
                table: "projection_runs",
                columns: new[] { "tenant_id", "farm_id", "period_end", "superseded_at" });

            migrationBuilder.CreateIndex(
                name: "ix_projection_runs_tenant_id_farm_id_period_start_period_end_p",
                schema: "analytics",
                table: "projection_runs",
                columns: new[] { "tenant_id", "farm_id", "period_start", "period_end", "projection_version", "revision" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_facts_tenant_id_predictive_model_definition_id",
                schema: "analytics",
                table: "facts",
                columns: new[] { "tenant_id", "predictive_model_definition_id" });

            migrationBuilder.CreateIndex(
                name: "ix_benchmark_enrollments_tenant_id_cohort_definition_id",
                schema: "analytics",
                table: "benchmark_enrollments",
                columns: new[] { "tenant_id", "cohort_definition_id" });

            migrationBuilder.CreateIndex(
                name: "ix_benchmark_cohort_definitions_tenant_id",
                schema: "analytics",
                table: "benchmark_cohort_definitions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_benchmark_cohort_definitions_tenant_id_cohort_key_definitio",
                schema: "analytics",
                table: "benchmark_cohort_definitions",
                columns: new[] { "tenant_id", "cohort_key", "definition_version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_benchmark_cohort_definitions_tenant_id_cohort_key_status_ef",
                schema: "analytics",
                table: "benchmark_cohort_definitions",
                columns: new[] { "tenant_id", "cohort_key", "status", "effective_from" });

            migrationBuilder.CreateIndex(
                name: "ix_dimension_snapshots_tenant_id",
                schema: "analytics",
                table: "dimension_snapshots",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_dimension_snapshots_tenant_id_dimension_type_entity_id_vali",
                schema: "analytics",
                table: "dimension_snapshots",
                columns: new[] { "tenant_id", "dimension_type", "entity_id", "valid_on" });

            migrationBuilder.CreateIndex(
                name: "ix_dimension_snapshots_tenant_id_farm_id",
                schema: "analytics",
                table: "dimension_snapshots",
                columns: new[] { "tenant_id", "farm_id" });

            migrationBuilder.CreateIndex(
                name: "ix_dimension_snapshots_tenant_id_projection_run_id_dimension_t",
                schema: "analytics",
                table: "dimension_snapshots",
                columns: new[] { "tenant_id", "projection_run_id", "dimension_type", "entity_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_forecast_evaluations_tenant_id",
                schema: "analytics",
                table: "forecast_evaluations",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_forecast_evaluations_tenant_id_actual_fact_id",
                schema: "analytics",
                table: "forecast_evaluations",
                columns: new[] { "tenant_id", "actual_fact_id" });

            migrationBuilder.CreateIndex(
                name: "ix_forecast_evaluations_tenant_id_farm_id_metric_code_evaluate",
                schema: "analytics",
                table: "forecast_evaluations",
                columns: new[] { "tenant_id", "farm_id", "metric_code", "evaluated_at" });

            migrationBuilder.CreateIndex(
                name: "ix_forecast_evaluations_tenant_id_forecast_fact_id_actual_fact",
                schema: "analytics",
                table: "forecast_evaluations",
                columns: new[] { "tenant_id", "forecast_fact_id", "actual_fact_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_predictive_model_definitions_tenant_id",
                schema: "analytics",
                table: "predictive_model_definitions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_predictive_model_definitions_tenant_id_model_key_model_vers",
                schema: "analytics",
                table: "predictive_model_definitions",
                columns: new[] { "tenant_id", "model_key", "model_version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_predictive_model_definitions_tenant_id_target_metric_code_s",
                schema: "analytics",
                table: "predictive_model_definitions",
                columns: new[] { "tenant_id", "target_metric_code", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_refresh_requests_tenant_id",
                schema: "analytics",
                table: "refresh_requests",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_requests_tenant_id_farm_id",
                schema: "analytics",
                table: "refresh_requests",
                columns: new[] { "tenant_id", "farm_id" },
                unique: true,
                filter: "status = 1");

            migrationBuilder.CreateIndex(
                name: "ix_refresh_requests_tenant_id_status_available_at",
                schema: "analytics",
                table: "refresh_requests",
                columns: new[] { "tenant_id", "status", "available_at" });

            migrationBuilder.AddForeignKey(
                name: "fk_benchmark_enrollments_benchmark_cohort_definitions_tenant_i",
                schema: "analytics",
                table: "benchmark_enrollments",
                columns: new[] { "tenant_id", "cohort_definition_id" },
                principalSchema: "analytics",
                principalTable: "benchmark_cohort_definitions",
                principalColumns: new[] { "tenant_id", "id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_facts_predictive_model_definitions_tenant_id_predictive_mod",
                schema: "analytics",
                table: "facts",
                columns: new[] { "tenant_id", "predictive_model_definition_id" },
                principalSchema: "analytics",
                principalTable: "predictive_model_definitions",
                principalColumns: new[] { "tenant_id", "id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql("""
                UPDATE analytics.projection_runs SET revision=1 WHERE revision=0;
                UPDATE analytics.projection_runs SET trigger_type='legacy' WHERE trigger_type='';
                ALTER TABLE analytics.projection_runs ALTER COLUMN revision DROP DEFAULT;
                ALTER TABLE analytics.projection_runs ALTER COLUMN trigger_type DROP DEFAULT;

                ALTER TABLE analytics.projection_runs ADD CONSTRAINT ck_projection_runs_revision
                    CHECK(revision>0);
                ALTER TABLE analytics.projection_runs ADD CONSTRAINT ck_projection_runs_trigger
                    CHECK(length(btrim(trigger_type))>0);
                ALTER TABLE analytics.projection_runs ADD CONSTRAINT ck_projection_runs_superseded
                    CHECK((superseded_at IS NULL AND superseded_by_run_id IS NULL)
                       OR (superseded_at IS NOT NULL AND superseded_by_run_id IS NOT NULL AND superseded_by_run_id<>id));
                ALTER TABLE analytics.facts ADD CONSTRAINT ck_analytics_facts_prediction
                    CHECK((value_kind<>3 AND predictive_model_definition_id IS NULL AND confidence_percent IS NULL
                            AND lower_bound IS NULL AND upper_bound IS NULL)
                       OR (value_kind=3 AND confidence_percent BETWEEN 0 AND 100
                            AND (lower_bound IS NULL OR upper_bound IS NULL OR lower_bound<=upper_bound)));
                ALTER TABLE analytics.benchmark_cohort_definitions ADD CONSTRAINT ck_benchmark_cohort_definition
                    CHECK(definition_version>0 AND minimum_cohort_size BETWEEN 5 AND 1000
                        AND status BETWEEN 1 AND 3 AND jsonb_typeof(inclusion_criteria_json)='object'
                        AND (effective_to IS NULL OR effective_to>=effective_from)
                        AND ((status=1 AND approved_at IS NULL AND retired_at IS NULL)
                          OR (status=2 AND approved_at IS NOT NULL AND approved_by IS NOT NULL
                              AND approval_reference IS NOT NULL AND retired_at IS NULL)
                          OR (status=3 AND approved_at IS NOT NULL AND retired_at IS NOT NULL AND retired_by IS NOT NULL)));
                ALTER TABLE analytics.predictive_model_definitions ADD CONSTRAINT ck_predictive_model_definition
                    CHECK(model_version>0 AND training_window_days BETWEEN 30 AND 3650
                        AND minimum_observations BETWEEN 3 AND 10000 AND validation_mae>=0
                        AND (validation_mape_percent IS NULL OR validation_mape_percent BETWEEN 0 AND 1000)
                        AND status BETWEEN 1 AND 3 AND jsonb_typeof(feature_schema_json)='object'
                        AND ((status=1 AND approved_at IS NULL AND retired_at IS NULL)
                          OR (status=2 AND approved_at IS NOT NULL AND approved_by IS NOT NULL
                              AND approval_reference IS NOT NULL AND retired_at IS NULL)
                          OR (status=3 AND approved_at IS NOT NULL AND retired_at IS NOT NULL AND retired_by IS NOT NULL)));
                ALTER TABLE analytics.dimension_snapshots ADD CONSTRAINT ck_dimension_snapshot
                    CHECK(dimension_type BETWEEN 1 AND 4 AND jsonb_typeof(attributes_json)='object'
                        AND attributes_sha256 ~ '^[0-9a-f]{64}$');
                ALTER TABLE analytics.forecast_evaluations ADD CONSTRAINT ck_forecast_evaluation
                    CHECK(horizon_days>=0 AND absolute_error>=0
                        AND (absolute_percentage_error IS NULL OR absolute_percentage_error>=0));
                ALTER TABLE analytics.refresh_requests ADD CONSTRAINT ck_refresh_request
                    CHECK(status BETWEEN 1 AND 4 AND event_count>0 AND last_triggered_at>=first_triggered_at
                        AND available_at>=first_triggered_at
                        AND ((status=1 AND scheduled_run_id IS NULL AND processed_at IS NULL)
                          OR (status=2 AND scheduled_run_id IS NOT NULL AND processed_at IS NOT NULL)
                          OR status IN (3,4)));

                DO $rls$
                DECLARE table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY[
                        'benchmark_cohort_definitions','predictive_model_definitions','dimension_snapshots',
                        'forecast_evaluations','refresh_requests'] LOOP
                        EXECUTE format('ALTER TABLE analytics.%I ENABLE ROW LEVEL SECURITY',table_name);
                        EXECUTE format('ALTER TABLE analytics.%I FORCE ROW LEVEL SECURITY',table_name);
                        EXECUTE format('CREATE POLICY tenant_isolation ON analytics.%I USING (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid) WITH CHECK (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid)',table_name);
                    END LOOP;
                END
                $rls$;

                CREATE OR REPLACE FUNCTION analytics.guard_governed_definition() RETURNS trigger
                LANGUAGE plpgsql AS $function$
                BEGIN
                    IF TG_OP='DELETE' THEN RAISE EXCEPTION '% definitions cannot be deleted.',TG_TABLE_NAME USING ERRCODE='55000'; END IF;
                    IF TG_TABLE_NAME='benchmark_cohort_definitions' THEN
                        IF (OLD.id,OLD.tenant_id,OLD.cohort_key,OLD.definition_version,OLD.name,OLD.description,
                            OLD.inclusion_criteria_json::text,OLD.minimum_cohort_size,OLD.effective_from,OLD.effective_to,
                            OLD.owner,OLD.created_by,OLD.created_at_utc)
                           IS DISTINCT FROM
                           (NEW.id,NEW.tenant_id,NEW.cohort_key,NEW.definition_version,NEW.name,NEW.description,
                            NEW.inclusion_criteria_json::text,NEW.minimum_cohort_size,NEW.effective_from,NEW.effective_to,
                            NEW.owner,NEW.created_by,NEW.created_at_utc) THEN
                            RAISE EXCEPTION 'Governed cohort definition content is immutable.' USING ERRCODE='55000';
                        END IF;
                    ELSIF TG_TABLE_NAME='predictive_model_definitions' THEN
                        IF (OLD.id,OLD.tenant_id,OLD.model_key,OLD.model_version,OLD.name,OLD.target_metric_code,
                            OLD.algorithm,OLD.feature_schema_json::text,OLD.training_window_days,OLD.minimum_observations,
                            OLD.validation_mae,OLD.validation_mape_percent,OLD.validation_dataset_reference,
                            OLD.created_by,OLD.created_at_utc)
                           IS DISTINCT FROM
                           (NEW.id,NEW.tenant_id,NEW.model_key,NEW.model_version,NEW.name,NEW.target_metric_code,
                            NEW.algorithm,NEW.feature_schema_json::text,NEW.training_window_days,NEW.minimum_observations,
                            NEW.validation_mae,NEW.validation_mape_percent,NEW.validation_dataset_reference,
                            NEW.created_by,NEW.created_at_utc) THEN
                            RAISE EXCEPTION 'Governed predictive model content is immutable.' USING ERRCODE='55000';
                        END IF;
                    END IF;
                    IF NOT ((OLD.status=1 AND NEW.status IN (1,2)) OR (OLD.status=2 AND NEW.status IN (2,3))
                            OR (OLD.status=3 AND NEW.status=3)) THEN
                        RAISE EXCEPTION 'Governed definition lifecycle cannot move backwards.' USING ERRCODE='55000';
                    END IF;
                    RETURN NEW;
                END $function$;

                CREATE OR REPLACE FUNCTION analytics.guard_refresh_request() RETURNS trigger
                LANGUAGE plpgsql AS $function$
                BEGIN
                    IF TG_OP='DELETE' OR current_setting('app.analytics_maintenance',true)<>'on' THEN
                        RAISE EXCEPTION 'Analytics refresh requests are maintenance-managed.' USING ERRCODE='55000';
                    END IF;
                    RETURN NEW;
                END $function$;

                CREATE TRIGGER benchmark_cohort_definition_lifecycle BEFORE UPDATE OR DELETE
                    ON analytics.benchmark_cohort_definitions FOR EACH ROW EXECUTE FUNCTION analytics.guard_governed_definition();
                CREATE TRIGGER predictive_model_definition_lifecycle BEFORE UPDATE OR DELETE
                    ON analytics.predictive_model_definitions FOR EACH ROW EXECUTE FUNCTION analytics.guard_governed_definition();
                CREATE TRIGGER analytics_dimension_snapshots_immutable BEFORE UPDATE OR DELETE
                    ON analytics.dimension_snapshots FOR EACH ROW EXECUTE FUNCTION platform.guard_immutable_projection_record();
                CREATE TRIGGER analytics_forecast_evaluations_immutable BEFORE UPDATE OR DELETE
                    ON analytics.forecast_evaluations FOR EACH ROW EXECUTE FUNCTION platform.guard_immutable_projection_record();
                CREATE TRIGGER analytics_refresh_requests_managed BEFORE UPDATE OR DELETE
                    ON analytics.refresh_requests FOR EACH ROW EXECUTE FUNCTION analytics.guard_refresh_request();

                CREATE OR REPLACE FUNCTION analytics.enqueue_refresh_from_outbox() RETURNS trigger
                LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,analytics,farm AS $function$
                DECLARE farm_text text; affected_farm uuid; trigger_time timestamptz;
                BEGIN
                    IF NEW.type ILIKE 'Analytics.%' OR NEW.type ILIKE 'AnalyticsProjection%' THEN RETURN NEW; END IF;
                    farm_text:=coalesce(NEW.payload->>'farmId',NEW.payload->>'FarmId');
                    IF farm_text IS NULL OR farm_text !~* '^[0-9a-f]{8}-[0-9a-f]{4}-[1-5][0-9a-f]{3}-[89ab][0-9a-f]{3}-[0-9a-f]{12}$' THEN RETURN NEW; END IF;
                    affected_farm:=farm_text::uuid;
                    IF NOT EXISTS(SELECT 1 FROM farm.farms f WHERE f.tenant_id=NEW.tenant_id AND f.id=affected_farm) THEN RETURN NEW; END IF;
                    trigger_time:=greatest(NEW.occurred_at,clock_timestamp());
                    PERFORM set_config('app.analytics_maintenance','on',true);
                    INSERT INTO analytics.refresh_requests(
                        id,farm_id,source_outbox_message_id,trigger_type,first_triggered_at,last_triggered_at,
                        available_at,status,scheduled_run_id,processed_at,event_count,last_error,
                        tenant_id,created_at_utc,updated_at_utc,version)
                    VALUES(gen_random_uuid(),affected_farm,NEW.id,NEW.type,trigger_time,trigger_time,
                        trigger_time+interval '15 seconds',1,NULL,NULL,1,NULL,NEW.tenant_id,trigger_time,trigger_time,1)
                    ON CONFLICT (tenant_id,farm_id) WHERE status=1 DO UPDATE SET
                        source_outbox_message_id=excluded.source_outbox_message_id,trigger_type=excluded.trigger_type,
                        last_triggered_at=excluded.last_triggered_at,available_at=excluded.available_at,
                        event_count=analytics.refresh_requests.event_count+1,updated_at_utc=excluded.updated_at_utc,
                        version=analytics.refresh_requests.version+1;
                    PERFORM set_config('app.analytics_maintenance','off',true);
                    RETURN NEW;
                END $function$;

                CREATE TRIGGER analytics_refresh_from_outbox AFTER INSERT ON messaging.outbox_messages
                    FOR EACH ROW EXECUTE FUNCTION analytics.enqueue_refresh_from_outbox();

                CREATE OR REPLACE FUNCTION analytics.schedule_event_refreshes(p_now timestamptz,p_batch_size integer)
                RETURNS integer LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,analytics,public AS $function$
                DECLARE request_record record; inserted_count integer:=0; period_finish date; period_begin date;
                DECLARE next_revision integer; new_run_id uuid;
                BEGIN
                    IF p_batch_size NOT BETWEEN 1 AND 1000 THEN RAISE EXCEPTION 'Invalid analytics refresh batch size.' USING ERRCODE='22023'; END IF;
                    PERFORM set_config('app.analytics_maintenance','on',true);
                    FOR request_record IN
                        SELECT q.*,s.lookback_days,s.currency,s.projection_version
                        FROM analytics.refresh_requests q
                        JOIN LATERAL (SELECT candidate.lookback_days,candidate.currency,candidate.projection_version
                            FROM analytics.projection_schedules candidate
                            WHERE candidate.tenant_id=q.tenant_id AND candidate.farm_id=q.farm_id AND candidate.is_active
                            ORDER BY candidate.next_run_at,candidate.id LIMIT 1) s ON true
                        WHERE q.status=1 AND q.available_at<=p_now
                        ORDER BY q.available_at,q.id FOR UPDATE OF q SKIP LOCKED LIMIT p_batch_size
                    LOOP
                        period_finish:=(p_now AT TIME ZONE 'UTC')::date;
                        period_begin:=period_finish-(request_record.lookback_days-1);
                        SELECT coalesce(max(r.revision),0)+1 INTO next_revision FROM analytics.projection_runs r
                            WHERE r.tenant_id=request_record.tenant_id AND r.farm_id=request_record.farm_id
                              AND r.period_start=period_begin AND r.period_end=period_finish
                              AND r.projection_version=request_record.projection_version;
                        new_run_id:=gen_random_uuid();
                        INSERT INTO analytics.projection_runs(
                            id,farm_id,period_start,period_end,currency,projection_version,scheduled_for,requested_by,
                            revision,trigger_type,source_refresh_request_id,status,started_at,completed_at,fact_count,
                            data_quality_percent,error_code,error_message,superseded_at,superseded_by_run_id,
                            tenant_id,created_at_utc,updated_at_utc,version)
                        VALUES(new_run_id,request_record.farm_id,period_begin,period_finish,request_record.currency,
                            request_record.projection_version,p_now,'analytics-event:'||request_record.id::text,
                            next_revision,'event',request_record.id,1,NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,
                            request_record.tenant_id,p_now,p_now,1);
                        UPDATE analytics.refresh_requests SET status=2,scheduled_run_id=new_run_id,processed_at=p_now,
                            updated_at_utc=p_now,version=version+1 WHERE id=request_record.id;
                        inserted_count:=inserted_count+1;
                    END LOOP;
                    PERFORM set_config('app.analytics_maintenance','off',true);
                    RETURN inserted_count;
                END $function$;

                CREATE OR REPLACE FUNCTION analytics.schedule_due_projections(p_now timestamptz,p_batch_size integer)
                RETURNS integer LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,analytics,public AS $function$
                DECLARE schedule_record record; inserted_count integer:=0; period_finish date; period_begin date;
                DECLARE next_revision integer;
                BEGIN
                    IF p_batch_size NOT BETWEEN 1 AND 1000 THEN RAISE EXCEPTION 'Invalid analytics schedule batch size.' USING ERRCODE='22023'; END IF;
                    FOR schedule_record IN SELECT s.* FROM analytics.projection_schedules s
                        WHERE s.is_active AND s.next_run_at<=p_now ORDER BY s.next_run_at,s.id
                        FOR UPDATE SKIP LOCKED LIMIT p_batch_size
                    LOOP
                        period_finish:=(p_now AT TIME ZONE 'UTC')::date-1;
                        period_begin:=period_finish-(schedule_record.lookback_days-1);
                        SELECT coalesce(max(r.revision),0)+1 INTO next_revision FROM analytics.projection_runs r
                            WHERE r.tenant_id=schedule_record.tenant_id AND r.farm_id=schedule_record.farm_id
                              AND r.period_start=period_begin AND r.period_end=period_finish
                              AND r.projection_version=schedule_record.projection_version;
                        INSERT INTO analytics.projection_runs(
                            id,farm_id,period_start,period_end,currency,projection_version,scheduled_for,requested_by,
                            revision,trigger_type,source_refresh_request_id,status,started_at,completed_at,fact_count,
                            data_quality_percent,error_code,error_message,superseded_at,superseded_by_run_id,
                            tenant_id,created_at_utc,updated_at_utc,version)
                        VALUES(gen_random_uuid(),schedule_record.farm_id,period_begin,period_finish,schedule_record.currency,
                            schedule_record.projection_version,p_now,'analytics-schedule:'||schedule_record.id::text,
                            next_revision,'scheduled',NULL,1,NULL,NULL,0,NULL,NULL,NULL,NULL,NULL,
                            schedule_record.tenant_id,p_now,p_now,1);
                        inserted_count:=inserted_count+1;
                        UPDATE analytics.projection_schedules SET last_scheduled_at=p_now,
                            next_run_at=CASE frequency WHEN 1 THEN greatest(next_run_at,p_now)+interval '1 day'
                                WHEN 2 THEN greatest(next_run_at,p_now)+interval '7 days'
                                ELSE greatest(next_run_at,p_now)+interval '1 month' END,
                            updated_at_utc=p_now,version=version+1 WHERE id=schedule_record.id;
                    END LOOP;
                    RETURN inserted_count;
                END $function$;

                CREATE OR REPLACE FUNCTION analytics.refresh_warehouse(p_now timestamptz)
                RETURNS integer LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,analytics,public AS $function$
                DECLARE refresh_id uuid:=gen_random_uuid(); aggregate_count integer; fact_count bigint;
                BEGIN
                    SELECT count(*) INTO fact_count FROM analytics.facts f JOIN analytics.projection_runs r
                        ON r.tenant_id=f.tenant_id AND r.id=f.projection_run_id WHERE r.status=3 AND r.superseded_at IS NULL;
                    INSERT INTO analytics.farm_metric_rollups(
                        tenant_id,farm_id,period_end,metric_code,unit,value_kind,total,average,minimum,maximum,
                        data_quality_percent,grain_count,source_record_count,refreshed_at)
                    SELECT f.tenant_id,f.farm_id,f.period_end,f.metric_code,f.unit,f.value_kind,sum(f.value),avg(f.value),min(f.value),max(f.value),
                        avg(f.data_quality_percent),count(*)::integer,sum(f.source_record_count),p_now
                    FROM analytics.facts f JOIN analytics.projection_runs r
                        ON r.tenant_id=f.tenant_id AND r.id=f.projection_run_id
                    WHERE r.status=3 AND r.superseded_at IS NULL
                    GROUP BY f.tenant_id,f.farm_id,f.period_end,f.metric_code,f.unit,f.value_kind
                    ON CONFLICT (tenant_id,farm_id,period_end,metric_code,unit,value_kind) DO UPDATE SET
                        total=excluded.total,average=excluded.average,minimum=excluded.minimum,maximum=excluded.maximum,
                        data_quality_percent=excluded.data_quality_percent,grain_count=excluded.grain_count,
                        source_record_count=excluded.source_record_count,refreshed_at=excluded.refreshed_at;
                    GET DIAGNOSTICS aggregate_count=ROW_COUNT;
                    INSERT INTO analytics.warehouse_refresh_runs VALUES(refresh_id,p_now,clock_timestamp(),fact_count,aggregate_count,'Succeeded',NULL);
                    RETURN aggregate_count;
                EXCEPTION WHEN OTHERS THEN
                    INSERT INTO analytics.warehouse_refresh_runs VALUES(refresh_id,p_now,clock_timestamp(),0,0,'Failed',left(SQLERRM,2000)); RAISE;
                END $function$;

                CREATE OR REPLACE FUNCTION analytics.benchmark_metric(p_metric_code text,p_cohort_key text,p_from date,p_to date)
                RETURNS TABLE(cohort_size integer,minimum numeric,percentile25 numeric,median numeric,percentile75 numeric,maximum numeric,average numeric,average_data_quality_percent numeric)
                LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,analytics AS $function$
                DECLARE requesting_tenant uuid; required_size integer;
                BEGIN
                    requesting_tenant:=nullif(current_setting('app.tenant_id',true),'')::uuid;
                    SELECT d.minimum_cohort_size INTO required_size
                    FROM analytics.benchmark_enrollments e
                    JOIN analytics.benchmark_cohort_definitions d ON d.tenant_id=e.tenant_id AND d.id=e.cohort_definition_id
                    WHERE e.tenant_id=requesting_tenant AND e.metric_code=lower(p_metric_code)
                        AND e.cohort_key=lower(p_cohort_key) AND e.is_active AND d.status=2
                        AND d.effective_from<=p_to AND (d.effective_to IS NULL OR d.effective_to>=p_from)
                    ORDER BY d.definition_version DESC LIMIT 1;
                    IF required_size IS NULL THEN RAISE EXCEPTION 'Active approved benchmark cohort enrollment required.' USING ERRCODE='42501'; END IF;
                    RETURN QUERY WITH tenant_values AS (
                        SELECT f.tenant_id,avg(f.value) value,avg(f.data_quality_percent) quality
                        FROM analytics.facts f
                        JOIN analytics.projection_runs r ON r.tenant_id=f.tenant_id AND r.id=f.projection_run_id
                            AND r.status=3 AND r.superseded_at IS NULL
                        JOIN analytics.benchmark_enrollments e ON e.tenant_id=f.tenant_id AND e.metric_code=f.metric_code
                            AND e.cohort_key=lower(p_cohort_key) AND e.is_active
                        JOIN analytics.benchmark_cohort_definitions d ON d.tenant_id=e.tenant_id
                            AND d.id=e.cohort_definition_id AND d.status=2 AND d.effective_from<=p_to
                            AND (d.effective_to IS NULL OR d.effective_to>=p_from)
                        WHERE f.metric_code=lower(p_metric_code) AND f.value_kind=1 AND f.period_end>=p_from
                            AND f.period_start<=p_to AND f.data_quality_percent>=60 GROUP BY f.tenant_id)
                    SELECT count(*)::integer,min(value),percentile_cont(0.25) within group(order by value)::numeric,
                        percentile_cont(0.5) within group(order by value)::numeric,
                        percentile_cont(0.75) within group(order by value)::numeric,max(value),avg(value),avg(quality)
                    FROM tenant_values HAVING count(*)>=required_size;
                END $function$;

                REVOKE ALL ON FUNCTION analytics.schedule_event_refreshes(timestamptz,integer) FROM PUBLIC;
                REVOKE ALL ON FUNCTION analytics.enqueue_refresh_from_outbox() FROM PUBLIC;
                REVOKE ALL ON FUNCTION analytics.schedule_due_projections(timestamptz,integer) FROM PUBLIC;
                REVOKE ALL ON FUNCTION analytics.refresh_warehouse(timestamptz) FROM PUBLIC;
                REVOKE ALL ON FUNCTION analytics.benchmark_metric(text,text,date,date) FROM PUBLIC;
                DO $grant$ BEGIN IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname='murimios_app') THEN
                    GRANT SELECT,INSERT,UPDATE ON analytics.benchmark_cohort_definitions,analytics.predictive_model_definitions TO murimios_app;
                    GRANT SELECT,INSERT ON analytics.dimension_snapshots,analytics.forecast_evaluations TO murimios_app;
                    GRANT SELECT ON analytics.refresh_requests TO murimios_app;
                    GRANT EXECUTE ON FUNCTION analytics.schedule_event_refreshes(timestamptz,integer),
                        analytics.schedule_due_projections(timestamptz,integer),analytics.refresh_warehouse(timestamptz),
                        analytics.benchmark_metric(text,text,date,date) TO murimios_app;
                END IF; END $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP TRIGGER IF EXISTS analytics_refresh_from_outbox ON messaging.outbox_messages;
                DROP FUNCTION IF EXISTS analytics.enqueue_refresh_from_outbox();
                DROP FUNCTION IF EXISTS analytics.schedule_event_refreshes(timestamptz,integer);
                DROP TRIGGER IF EXISTS benchmark_cohort_definition_lifecycle ON analytics.benchmark_cohort_definitions;
                DROP TRIGGER IF EXISTS predictive_model_definition_lifecycle ON analytics.predictive_model_definitions;
                DROP TRIGGER IF EXISTS analytics_dimension_snapshots_immutable ON analytics.dimension_snapshots;
                DROP TRIGGER IF EXISTS analytics_forecast_evaluations_immutable ON analytics.forecast_evaluations;
                DROP TRIGGER IF EXISTS analytics_refresh_requests_managed ON analytics.refresh_requests;
                DROP FUNCTION IF EXISTS analytics.guard_governed_definition();
                DROP FUNCTION IF EXISTS analytics.guard_refresh_request();
                """);

            migrationBuilder.DropForeignKey(
                name: "fk_benchmark_enrollments_benchmark_cohort_definitions_tenant_i",
                schema: "analytics",
                table: "benchmark_enrollments");

            migrationBuilder.DropForeignKey(
                name: "fk_facts_predictive_model_definitions_tenant_id_predictive_mod",
                schema: "analytics",
                table: "facts");

            migrationBuilder.DropTable(
                name: "benchmark_cohort_definitions",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "dimension_snapshots",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "forecast_evaluations",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "predictive_model_definitions",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "refresh_requests",
                schema: "analytics");

            migrationBuilder.DropIndex(
                name: "ix_projection_runs_tenant_id_farm_id_period_end_superseded_at",
                schema: "analytics",
                table: "projection_runs");

            migrationBuilder.DropIndex(
                name: "ix_projection_runs_tenant_id_farm_id_period_start_period_end_p",
                schema: "analytics",
                table: "projection_runs");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_analytics_facts_tenant_id_id",
                schema: "analytics",
                table: "facts");

            migrationBuilder.DropIndex(
                name: "ix_facts_tenant_id_predictive_model_definition_id",
                schema: "analytics",
                table: "facts");

            migrationBuilder.DropIndex(
                name: "ix_benchmark_enrollments_tenant_id_cohort_definition_id",
                schema: "analytics",
                table: "benchmark_enrollments");

            migrationBuilder.DropColumn(
                name: "revision",
                schema: "analytics",
                table: "projection_runs");

            migrationBuilder.DropColumn(
                name: "source_refresh_request_id",
                schema: "analytics",
                table: "projection_runs");

            migrationBuilder.DropColumn(
                name: "superseded_at",
                schema: "analytics",
                table: "projection_runs");

            migrationBuilder.DropColumn(
                name: "superseded_by_run_id",
                schema: "analytics",
                table: "projection_runs");

            migrationBuilder.DropColumn(
                name: "trigger_type",
                schema: "analytics",
                table: "projection_runs");

            migrationBuilder.DropColumn(
                name: "confidence_percent",
                schema: "analytics",
                table: "facts");

            migrationBuilder.DropColumn(
                name: "lower_bound",
                schema: "analytics",
                table: "facts");

            migrationBuilder.DropColumn(
                name: "predictive_model_definition_id",
                schema: "analytics",
                table: "facts");

            migrationBuilder.DropColumn(
                name: "upper_bound",
                schema: "analytics",
                table: "facts");

            migrationBuilder.DropColumn(
                name: "cohort_definition_id",
                schema: "analytics",
                table: "benchmark_enrollments");

            migrationBuilder.CreateIndex(
                name: "ix_projection_runs_tenant_id_farm_id_period_start_period_end_p",
                schema: "analytics",
                table: "projection_runs",
                columns: new[] { "tenant_id", "farm_id", "period_start", "period_end", "projection_version" },
                unique: true);

            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION analytics.schedule_due_projections(p_now timestamptz,p_batch_size integer)
                RETURNS integer LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,analytics,public AS $function$
                DECLARE schedule_record record; inserted_count integer:=0; affected integer;
                DECLARE period_finish date; period_begin date;
                BEGIN
                    IF p_batch_size NOT BETWEEN 1 AND 1000 THEN RAISE EXCEPTION 'Invalid analytics schedule batch size.' USING ERRCODE='22023'; END IF;
                    FOR schedule_record IN SELECT s.* FROM analytics.projection_schedules s
                        WHERE s.is_active AND s.next_run_at<=p_now ORDER BY s.next_run_at,s.id
                        FOR UPDATE SKIP LOCKED LIMIT p_batch_size
                    LOOP
                        period_finish:=(p_now AT TIME ZONE 'UTC')::date-1;
                        period_begin:=period_finish-(schedule_record.lookback_days-1);
                        INSERT INTO analytics.projection_runs(
                            id,farm_id,period_start,period_end,currency,projection_version,scheduled_for,requested_by,
                            status,started_at,completed_at,fact_count,data_quality_percent,error_code,error_message,
                            tenant_id,created_at_utc,updated_at_utc,version)
                        VALUES(gen_random_uuid(),schedule_record.farm_id,period_begin,period_finish,
                            schedule_record.currency,schedule_record.projection_version,p_now,
                            'analytics-schedule:'||schedule_record.id::text,1,NULL,NULL,0,NULL,NULL,NULL,
                            schedule_record.tenant_id,p_now,p_now,1)
                        ON CONFLICT (tenant_id,farm_id,period_start,period_end,projection_version) DO NOTHING;
                        GET DIAGNOSTICS affected=ROW_COUNT; inserted_count:=inserted_count+affected;
                        UPDATE analytics.projection_schedules SET last_scheduled_at=p_now,
                            next_run_at=CASE frequency WHEN 1 THEN greatest(next_run_at,p_now)+interval '1 day'
                                WHEN 2 THEN greatest(next_run_at,p_now)+interval '7 days'
                                ELSE greatest(next_run_at,p_now)+interval '1 month' END,
                            updated_at_utc=p_now,version=version+1 WHERE id=schedule_record.id;
                    END LOOP;
                    RETURN inserted_count;
                END $function$;

                CREATE OR REPLACE FUNCTION analytics.refresh_warehouse(p_now timestamptz)
                RETURNS integer LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,analytics,public AS $function$
                DECLARE refresh_id uuid:=gen_random_uuid(); aggregate_count integer; fact_count bigint;
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
                    INSERT INTO analytics.warehouse_refresh_runs VALUES(refresh_id,p_now,clock_timestamp(),0,0,'Failed',left(SQLERRM,2000)); RAISE;
                END $function$;

                CREATE OR REPLACE FUNCTION analytics.benchmark_metric(p_metric_code text,p_cohort_key text,p_from date,p_to date)
                RETURNS TABLE(cohort_size integer,minimum numeric,percentile25 numeric,median numeric,percentile75 numeric,maximum numeric,average numeric,average_data_quality_percent numeric)
                LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,analytics AS $function$
                DECLARE requesting_tenant uuid; required_size integer;
                BEGIN
                    requesting_tenant:=nullif(current_setting('app.tenant_id',true),'')::uuid;
                    SELECT minimum_cohort_size INTO required_size FROM analytics.benchmark_enrollments
                    WHERE tenant_id=requesting_tenant AND metric_code=lower(p_metric_code)
                        AND cohort_key=lower(p_cohort_key) AND is_active;
                    IF required_size IS NULL THEN RAISE EXCEPTION 'Active benchmark enrollment required.' USING ERRCODE='42501'; END IF;
                    RETURN QUERY WITH tenant_values AS (
                        SELECT f.tenant_id,avg(f.value) value,avg(f.data_quality_percent) quality
                        FROM analytics.facts f JOIN analytics.benchmark_enrollments e
                            ON e.tenant_id=f.tenant_id AND e.metric_code=f.metric_code
                            AND e.cohort_key=lower(p_cohort_key) AND e.is_active
                        WHERE f.metric_code=lower(p_metric_code) AND f.value_kind=1 AND f.period_end>=p_from
                            AND f.period_start<=p_to AND f.data_quality_percent>=60 GROUP BY f.tenant_id)
                    SELECT count(*)::integer,min(value),percentile_cont(0.25) within group(order by value)::numeric,
                        percentile_cont(0.5) within group(order by value)::numeric,
                        percentile_cont(0.75) within group(order by value)::numeric,max(value),avg(value),avg(quality)
                    FROM tenant_values HAVING count(*)>=required_size;
                END $function$;
                """);
        }
    }
}
