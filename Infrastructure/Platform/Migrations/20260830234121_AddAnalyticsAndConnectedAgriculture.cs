using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddAnalyticsAndConnectedAgriculture : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "analytics");

            migrationBuilder.EnsureSchema(
                name: "connected");

            migrationBuilder.CreateTable(
                name: "benchmark_enrollments",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    metric_code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    cohort_key = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    minimum_cohort_size = table.Column<int>(type: "integer", nullable: false),
                    enrolled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    enrolled_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revoked_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_benchmark_enrollments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "business_documents",
                schema: "connected",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    connection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_type = table.Column<int>(type: "integer", nullable: false),
                    direction = table.Column<int>(type: "integer", nullable: false),
                    external_document_id = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    schema_version = table.Column<int>(type: "integer", nullable: false),
                    mapping_version = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    payload_json = table.Column<string>(type: "jsonb", nullable: false),
                    payload_sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    received_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    processed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    applied_entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_business_documents", x => x.id);
                    table.ForeignKey(
                        name: "fk_business_documents_connections_tenant_id_connection_id",
                        columns: x => new { x.tenant_id, x.connection_id },
                        principalSchema: "integrations",
                        principalTable: "connections",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_business_documents_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "equipment",
                schema: "connected",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    manufacturer = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    model = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    serial_number = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    model_year = table.Column<int>(type: "integer", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    retired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_equipment", x => x.id);
                    table.UniqueConstraint("ak_connected_equipment_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_equipment_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "kpi_definitions",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    series_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    formula = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    unit = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    time_grain = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    owner = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    exclusions = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    quality_rule = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    definition_version = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    activated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    retired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_kpi_definitions", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "projection_runs",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_start = table.Column<DateOnly>(type: "date", nullable: false),
                    period_end = table.Column<DateOnly>(type: "date", nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    projection_version = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    scheduled_for = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    requested_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    fact_count = table.Column<int>(type: "integer", nullable: false),
                    data_quality_percent = table.Column<decimal>(type: "numeric(8,4)", precision: 8, scale: 4, nullable: true),
                    error_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_projection_runs", x => x.id);
                    table.UniqueConstraint("ak_analytics_projection_runs_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_projection_runs_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "devices",
                schema: "connected",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_key = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    kind = table.Column<int>(type: "integer", nullable: false),
                    protocol = table.Column<int>(type: "integer", nullable: false),
                    connection_id = table.Column<Guid>(type: "uuid", nullable: true),
                    equipment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    field_id = table.Column<Guid>(type: "uuid", nullable: true),
                    holding_id = table.Column<Guid>(type: "uuid", nullable: true),
                    firmware_version = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    provisioned_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    activated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_seen_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    retired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_devices", x => x.id);
                    table.UniqueConstraint("ak_connected_devices_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_devices_connections_tenant_id_connection_id",
                        columns: x => new { x.tenant_id, x.connection_id },
                        principalSchema: "integrations",
                        principalTable: "connections",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_devices_equipment_tenant_id_equipment_id",
                        columns: x => new { x.tenant_id, x.equipment_id },
                        principalSchema: "connected",
                        principalTable: "equipment",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_devices_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_devices_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_devices_livestock_holdings_tenant_id_holding_id",
                        columns: x => new { x.tenant_id, x.holding_id },
                        principalSchema: "livestock",
                        principalTable: "holdings",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "machinery_task_files",
                schema: "connected",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    equipment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    field_id = table.Column<Guid>(type: "uuid", nullable: true),
                    direction = table.Column<int>(type: "integer", nullable: false),
                    format = table.Column<int>(type: "integer", nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    object_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    byte_size = table.Column<long>(type: "bigint", nullable: false),
                    sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    schema_version = table.Column<int>(type: "integer", nullable: false),
                    mapping_version = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    external_task_id = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    processed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_machinery_task_files", x => x.id);
                    table.ForeignKey(
                        name: "fk_machinery_task_files_equipment_tenant_id_equipment_id",
                        columns: x => new { x.tenant_id, x.equipment_id },
                        principalSchema: "connected",
                        principalTable: "equipment",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_machinery_task_files_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_machinery_task_files_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_machinery_task_files_work_orders_tenant_id_work_order_id",
                        columns: x => new { x.tenant_id, x.work_order_id },
                        principalSchema: "operations",
                        principalTable: "work_orders",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "facts",
                schema: "analytics",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    projection_run_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organisation_node_id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<int>(type: "integer", nullable: false),
                    value_kind = table.Column<int>(type: "integer", nullable: false),
                    metric_code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    value = table.Column<decimal>(type: "numeric(28,8)", precision: 28, scale: 8, nullable: false),
                    unit = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    period_start = table.Column<DateOnly>(type: "date", nullable: false),
                    period_end = table.Column<DateOnly>(type: "date", nullable: false),
                    data_quality_percent = table.Column<decimal>(type: "numeric(8,4)", precision: 8, scale: 4, nullable: false),
                    source_record_count = table.Column<int>(type: "integer", nullable: false),
                    formula_version = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    grain_key = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: false),
                    season_id = table.Column<Guid>(type: "uuid", nullable: true),
                    field_id = table.Column<Guid>(type: "uuid", nullable: true),
                    crop_zone_id = table.Column<Guid>(type: "uuid", nullable: true),
                    livestock_subject_id = table.Column<Guid>(type: "uuid", nullable: true),
                    supplier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    crop = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    variety = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    species = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    dimensions_json = table.Column<string>(type: "jsonb", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_facts", x => x.id);
                    table.ForeignKey(
                        name: "fk_facts_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_facts_org_nodes_tenant_id_organisation_node_id",
                        columns: x => new { x.tenant_id, x.organisation_node_id },
                        principalSchema: "platform",
                        principalTable: "org_nodes",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_facts_projection_runs_tenant_id_projection_run_id",
                        columns: x => new { x.tenant_id, x.projection_run_id },
                        principalSchema: "analytics",
                        principalTable: "projection_runs",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "machinery_telemetry",
                schema: "connected",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    observed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    equipment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_id = table.Column<Guid>(type: "uuid", nullable: true),
                    received_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    engine_hours = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    speed_kph = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    fuel_level_percent = table.Column<decimal>(type: "numeric(8,4)", precision: 8, scale: 4, nullable: true),
                    state = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: true),
                    longitude = table.Column<double>(type: "double precision", nullable: true),
                    external_record_id = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    payload_sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    ingestion_record_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_machinery_telemetry", x => new { x.tenant_id, x.id, x.observed_at });
                    table.ForeignKey(
                        name: "fk_machinery_telemetry_devices_tenant_id_device_id",
                        columns: x => new { x.tenant_id, x.device_id },
                        principalSchema: "connected",
                        principalTable: "devices",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_machinery_telemetry_equipment_tenant_id_equipment_id",
                        columns: x => new { x.tenant_id, x.equipment_id },
                        principalSchema: "connected",
                        principalTable: "equipment",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_machinery_telemetry_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_machinery_telemetry_ingestion_records_tenant_id_ingestion_r",
                        columns: x => new { x.tenant_id, x.ingestion_record_id },
                        principalSchema: "integrations",
                        principalTable: "ingestion_records",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sensor_channels",
                schema: "connected",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_key = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    metric_code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    unit = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    minimum_valid_value = table.Column<decimal>(type: "numeric(24,8)", precision: 24, scale: 8, nullable: true),
                    maximum_valid_value = table.Column<decimal>(type: "numeric(24,8)", precision: 24, scale: 8, nullable: true),
                    sampling_interval_seconds = table.Column<int>(type: "integer", nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sensor_channels", x => x.id);
                    table.UniqueConstraint("ak_sensor_channels_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_sensor_channels_devices_tenant_id_device_id",
                        columns: x => new { x.tenant_id, x.device_id },
                        principalSchema: "connected",
                        principalTable: "devices",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "sensor_readings",
                schema: "connected",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    observed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_id = table.Column<Guid>(type: "uuid", nullable: false),
                    channel_id = table.Column<Guid>(type: "uuid", nullable: false),
                    metric_code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    value = table.Column<decimal>(type: "numeric(24,8)", precision: 24, scale: 8, nullable: false),
                    unit = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    received_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    quality = table.Column<int>(type: "integer", nullable: false),
                    external_reading_id = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    payload_sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    ingestion_record_id = table.Column<Guid>(type: "uuid", nullable: true),
                    latitude = table.Column<double>(type: "double precision", nullable: true),
                    longitude = table.Column<double>(type: "double precision", nullable: true),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sensor_readings", x => new { x.tenant_id, x.id, x.observed_at });
                    table.ForeignKey(
                        name: "fk_sensor_readings_devices_tenant_id_device_id",
                        columns: x => new { x.tenant_id, x.device_id },
                        principalSchema: "connected",
                        principalTable: "devices",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_sensor_readings_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_sensor_readings_ingestion_records_tenant_id_ingestion_recor",
                        columns: x => new { x.tenant_id, x.ingestion_record_id },
                        principalSchema: "integrations",
                        principalTable: "ingestion_records",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_sensor_readings_sensor_channels_tenant_id_channel_id",
                        columns: x => new { x.tenant_id, x.channel_id },
                        principalSchema: "connected",
                        principalTable: "sensor_channels",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_benchmark_enrollments_metric_code_cohort_key_is_active",
                schema: "analytics",
                table: "benchmark_enrollments",
                columns: new[] { "metric_code", "cohort_key", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_benchmark_enrollments_tenant_id",
                schema: "analytics",
                table: "benchmark_enrollments",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_benchmark_enrollments_tenant_id_metric_code_cohort_key",
                schema: "analytics",
                table: "benchmark_enrollments",
                columns: new[] { "tenant_id", "metric_code", "cohort_key" },
                unique: true,
                filter: "is_active");

            migrationBuilder.CreateIndex(
                name: "ix_business_documents_tenant_id",
                schema: "connected",
                table: "business_documents",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_business_documents_tenant_id_connection_id_document_type_di",
                schema: "connected",
                table: "business_documents",
                columns: new[] { "tenant_id", "connection_id", "document_type", "direction", "external_document_id", "payload_sha256" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_business_documents_tenant_id_farm_id_status_occurred_at",
                schema: "connected",
                table: "business_documents",
                columns: new[] { "tenant_id", "farm_id", "status", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_devices_tenant_id",
                schema: "connected",
                table: "devices",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_devices_tenant_id_connection_id",
                schema: "connected",
                table: "devices",
                columns: new[] { "tenant_id", "connection_id" });

            migrationBuilder.CreateIndex(
                name: "ix_devices_tenant_id_device_key",
                schema: "connected",
                table: "devices",
                columns: new[] { "tenant_id", "device_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_devices_tenant_id_equipment_id",
                schema: "connected",
                table: "devices",
                columns: new[] { "tenant_id", "equipment_id" });

            migrationBuilder.CreateIndex(
                name: "ix_devices_tenant_id_farm_id_status_kind",
                schema: "connected",
                table: "devices",
                columns: new[] { "tenant_id", "farm_id", "status", "kind" });

            migrationBuilder.CreateIndex(
                name: "ix_devices_tenant_id_field_id",
                schema: "connected",
                table: "devices",
                columns: new[] { "tenant_id", "field_id" });

            migrationBuilder.CreateIndex(
                name: "ix_devices_tenant_id_holding_id",
                schema: "connected",
                table: "devices",
                columns: new[] { "tenant_id", "holding_id" });

            migrationBuilder.CreateIndex(
                name: "ix_equipment_tenant_id",
                schema: "connected",
                table: "equipment",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_equipment_tenant_id_farm_id_code",
                schema: "connected",
                table: "equipment",
                columns: new[] { "tenant_id", "farm_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_equipment_tenant_id_farm_id_status_type",
                schema: "connected",
                table: "equipment",
                columns: new[] { "tenant_id", "farm_id", "status", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_facts_tenant_id",
                schema: "analytics",
                table: "facts",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_facts_tenant_id_farm_id_metric_code_period_start_period_end",
                schema: "analytics",
                table: "facts",
                columns: new[] { "tenant_id", "farm_id", "metric_code", "period_start", "period_end" });

            migrationBuilder.CreateIndex(
                name: "ix_facts_tenant_id_organisation_node_id_metric_code_period_end",
                schema: "analytics",
                table: "facts",
                columns: new[] { "tenant_id", "organisation_node_id", "metric_code", "period_end" });

            migrationBuilder.CreateIndex(
                name: "ix_facts_tenant_id_projection_run_id_metric_code_value_kind_gr",
                schema: "analytics",
                table: "facts",
                columns: new[] { "tenant_id", "projection_run_id", "metric_code", "value_kind", "grain_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_kpi_definitions_tenant_id",
                schema: "analytics",
                table: "kpi_definitions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_kpi_definitions_tenant_id_code_status",
                schema: "analytics",
                table: "kpi_definitions",
                columns: new[] { "tenant_id", "code", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_kpi_definitions_tenant_id_series_id_definition_version",
                schema: "analytics",
                table: "kpi_definitions",
                columns: new[] { "tenant_id", "series_id", "definition_version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_machinery_task_files_tenant_id",
                schema: "connected",
                table: "machinery_task_files",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_machinery_task_files_tenant_id_equipment_id_direction_exter",
                schema: "connected",
                table: "machinery_task_files",
                columns: new[] { "tenant_id", "equipment_id", "direction", "external_task_id" },
                unique: true,
                filter: "external_task_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_machinery_task_files_tenant_id_farm_id_status_created_at",
                schema: "connected",
                table: "machinery_task_files",
                columns: new[] { "tenant_id", "farm_id", "status", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_machinery_task_files_tenant_id_field_id",
                schema: "connected",
                table: "machinery_task_files",
                columns: new[] { "tenant_id", "field_id" });

            migrationBuilder.CreateIndex(
                name: "ix_machinery_task_files_tenant_id_work_order_id",
                schema: "connected",
                table: "machinery_task_files",
                columns: new[] { "tenant_id", "work_order_id" });

            migrationBuilder.CreateIndex(
                name: "ix_machinery_telemetry_tenant_id",
                schema: "connected",
                table: "machinery_telemetry",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_machinery_telemetry_tenant_id_device_id",
                schema: "connected",
                table: "machinery_telemetry",
                columns: new[] { "tenant_id", "device_id" });

            migrationBuilder.CreateIndex(
                name: "ix_machinery_telemetry_tenant_id_equipment_id_external_record_",
                schema: "connected",
                table: "machinery_telemetry",
                columns: new[] { "tenant_id", "equipment_id", "external_record_id", "observed_at" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_machinery_telemetry_tenant_id_farm_id_observed_at",
                schema: "connected",
                table: "machinery_telemetry",
                columns: new[] { "tenant_id", "farm_id", "observed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_machinery_telemetry_tenant_id_ingestion_record_id",
                schema: "connected",
                table: "machinery_telemetry",
                columns: new[] { "tenant_id", "ingestion_record_id" });

            migrationBuilder.CreateIndex(
                name: "ix_projection_runs_tenant_id",
                schema: "analytics",
                table: "projection_runs",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_projection_runs_tenant_id_farm_id_period_start_period_end_p",
                schema: "analytics",
                table: "projection_runs",
                columns: new[] { "tenant_id", "farm_id", "period_start", "period_end", "projection_version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_projection_runs_tenant_id_status_scheduled_for",
                schema: "analytics",
                table: "projection_runs",
                columns: new[] { "tenant_id", "status", "scheduled_for" });

            migrationBuilder.CreateIndex(
                name: "ix_sensor_channels_tenant_id",
                schema: "connected",
                table: "sensor_channels",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_sensor_channels_tenant_id_device_id_channel_key",
                schema: "connected",
                table: "sensor_channels",
                columns: new[] { "tenant_id", "device_id", "channel_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sensor_channels_tenant_id_metric_code_is_active",
                schema: "connected",
                table: "sensor_channels",
                columns: new[] { "tenant_id", "metric_code", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_sensor_readings_tenant_id",
                schema: "connected",
                table: "sensor_readings",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_sensor_readings_tenant_id_channel_id",
                schema: "connected",
                table: "sensor_readings",
                columns: new[] { "tenant_id", "channel_id" });

            migrationBuilder.CreateIndex(
                name: "ix_sensor_readings_tenant_id_device_id_channel_id_external_rea",
                schema: "connected",
                table: "sensor_readings",
                columns: new[] { "tenant_id", "device_id", "channel_id", "external_reading_id", "observed_at" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sensor_readings_tenant_id_farm_id_metric_code_observed_at",
                schema: "connected",
                table: "sensor_readings",
                columns: new[] { "tenant_id", "farm_id", "metric_code", "observed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_sensor_readings_tenant_id_ingestion_record_id",
                schema: "connected",
                table: "sensor_readings",
                columns: new[] { "tenant_id", "ingestion_record_id" });

            migrationBuilder.Sql("""
                -- Replace the two high-volume observation tables with native time-range partitions.
                DROP TABLE connected.sensor_readings;
                CREATE TABLE connected.sensor_readings (
                    id uuid NOT NULL, tenant_id uuid NOT NULL, observed_at timestamptz NOT NULL,
                    farm_id uuid NOT NULL, device_id uuid NOT NULL, channel_id uuid NOT NULL,
                    metric_code varchar(120) NOT NULL, value numeric(24,8) NOT NULL, unit varchar(40) NOT NULL,
                    received_at timestamptz NOT NULL, quality integer NOT NULL, external_reading_id varchar(240) NOT NULL,
                    payload_sha256 char(64) NOT NULL, ingestion_record_id uuid NULL,
                    latitude double precision NULL, longitude double precision NULL,
                    created_at_utc timestamptz NOT NULL, updated_at_utc timestamptz NOT NULL, version bigint NOT NULL,
                    CONSTRAINT pk_sensor_readings PRIMARY KEY (tenant_id,id,observed_at),
                    CONSTRAINT fk_sensor_readings_devices_tenant_id_device_id FOREIGN KEY (tenant_id,device_id) REFERENCES connected.devices(tenant_id,id) ON DELETE RESTRICT,
                    CONSTRAINT fk_sensor_readings_farms_tenant_id_farm_id FOREIGN KEY (tenant_id,farm_id) REFERENCES farm.farms(tenant_id,id) ON DELETE RESTRICT,
                    CONSTRAINT fk_sensor_readings_ingestion_records_tenant_id_ingestion_recor FOREIGN KEY (tenant_id,ingestion_record_id) REFERENCES integrations.ingestion_records(tenant_id,id) ON DELETE RESTRICT,
                    CONSTRAINT fk_sensor_readings_sensor_channels_tenant_id_channel_id FOREIGN KEY (tenant_id,channel_id) REFERENCES connected.sensor_channels(tenant_id,id) ON DELETE RESTRICT,
                    CONSTRAINT ck_sensor_readings_quality CHECK (quality BETWEEN 1 AND 3),
                    CONSTRAINT ck_sensor_readings_time CHECK (observed_at <= received_at + interval '5 minutes'),
                    CONSTRAINT ck_sensor_readings_location CHECK ((latitude IS NULL) = (longitude IS NULL) AND (latitude IS NULL OR (latitude BETWEEN -90 AND 90 AND longitude BETWEEN -180 AND 180))),
                    CONSTRAINT ck_sensor_readings_hash CHECK (payload_sha256 ~ '^[0-9a-f]{64}$')
                ) PARTITION BY RANGE (observed_at);
                CREATE TABLE connected.sensor_readings_2026 PARTITION OF connected.sensor_readings FOR VALUES FROM ('2026-01-01') TO ('2027-01-01');
                CREATE TABLE connected.sensor_readings_default PARTITION OF connected.sensor_readings DEFAULT;
                CREATE INDEX ix_sensor_readings_tenant_id ON connected.sensor_readings(tenant_id);
                CREATE INDEX ix_sensor_readings_tenant_id_channel_id ON connected.sensor_readings(tenant_id,channel_id);
                CREATE UNIQUE INDEX ix_sensor_readings_tenant_id_device_id_channel_id_external_rea ON connected.sensor_readings(tenant_id,device_id,channel_id,external_reading_id,observed_at);
                CREATE INDEX ix_sensor_readings_tenant_id_farm_id_metric_code_observed_at ON connected.sensor_readings(tenant_id,farm_id,metric_code,observed_at);
                CREATE INDEX ix_sensor_readings_tenant_id_ingestion_record_id ON connected.sensor_readings(tenant_id,ingestion_record_id);

                DROP TABLE connected.machinery_telemetry;
                CREATE TABLE connected.machinery_telemetry (
                    id uuid NOT NULL, tenant_id uuid NOT NULL, observed_at timestamptz NOT NULL,
                    farm_id uuid NOT NULL, equipment_id uuid NOT NULL, device_id uuid NULL, received_at timestamptz NOT NULL,
                    engine_hours numeric(18,4) NULL, speed_kph numeric(18,4) NULL, fuel_level_percent numeric(8,4) NULL,
                    state varchar(80) NOT NULL, latitude double precision NULL, longitude double precision NULL,
                    external_record_id varchar(240) NOT NULL, payload_sha256 char(64) NOT NULL, ingestion_record_id uuid NULL,
                    created_at_utc timestamptz NOT NULL, updated_at_utc timestamptz NOT NULL, version bigint NOT NULL,
                    CONSTRAINT pk_machinery_telemetry PRIMARY KEY (tenant_id,id,observed_at),
                    CONSTRAINT fk_machinery_telemetry_devices_tenant_id_device_id FOREIGN KEY (tenant_id,device_id) REFERENCES connected.devices(tenant_id,id) ON DELETE RESTRICT,
                    CONSTRAINT fk_machinery_telemetry_equipment_tenant_id_equipment_id FOREIGN KEY (tenant_id,equipment_id) REFERENCES connected.equipment(tenant_id,id) ON DELETE RESTRICT,
                    CONSTRAINT fk_machinery_telemetry_farms_tenant_id_farm_id FOREIGN KEY (tenant_id,farm_id) REFERENCES farm.farms(tenant_id,id) ON DELETE RESTRICT,
                    CONSTRAINT fk_machinery_telemetry_ingestion_records_tenant_id_ingestion_r FOREIGN KEY (tenant_id,ingestion_record_id) REFERENCES integrations.ingestion_records(tenant_id,id) ON DELETE RESTRICT,
                    CONSTRAINT ck_machinery_telemetry_time CHECK (observed_at <= received_at + interval '5 minutes'),
                    CONSTRAINT ck_machinery_telemetry_values CHECK ((engine_hours IS NULL OR engine_hours >= 0) AND (speed_kph IS NULL OR speed_kph >= 0) AND (fuel_level_percent IS NULL OR fuel_level_percent BETWEEN 0 AND 100)),
                    CONSTRAINT ck_machinery_telemetry_location CHECK ((latitude IS NULL) = (longitude IS NULL) AND (latitude IS NULL OR (latitude BETWEEN -90 AND 90 AND longitude BETWEEN -180 AND 180))),
                    CONSTRAINT ck_machinery_telemetry_hash CHECK (payload_sha256 ~ '^[0-9a-f]{64}$')
                ) PARTITION BY RANGE (observed_at);
                CREATE TABLE connected.machinery_telemetry_2026 PARTITION OF connected.machinery_telemetry FOR VALUES FROM ('2026-01-01') TO ('2027-01-01');
                CREATE TABLE connected.machinery_telemetry_default PARTITION OF connected.machinery_telemetry DEFAULT;
                CREATE INDEX ix_machinery_telemetry_tenant_id ON connected.machinery_telemetry(tenant_id);
                CREATE INDEX ix_machinery_telemetry_tenant_id_device_id ON connected.machinery_telemetry(tenant_id,device_id);
                CREATE UNIQUE INDEX ix_machinery_telemetry_tenant_id_equipment_id_external_record_ ON connected.machinery_telemetry(tenant_id,equipment_id,external_record_id,observed_at);
                CREATE INDEX ix_machinery_telemetry_tenant_id_farm_id_observed_at ON connected.machinery_telemetry(tenant_id,farm_id,observed_at);
                CREATE INDEX ix_machinery_telemetry_tenant_id_ingestion_record_id ON connected.machinery_telemetry(tenant_id,ingestion_record_id);
                """);

            migrationBuilder.Sql("""
                ALTER TABLE connected.equipment
                    ADD CONSTRAINT ck_equipment_type CHECK (type BETWEEN 1 AND 7), ADD CONSTRAINT ck_equipment_status CHECK (status BETWEEN 1 AND 3),
                    ADD CONSTRAINT ck_equipment_model_year CHECK (model_year IS NULL OR model_year BETWEEN 1900 AND 2200);
                ALTER TABLE connected.devices
                    ADD CONSTRAINT ck_devices_kind CHECK (kind BETWEEN 1 AND 6), ADD CONSTRAINT ck_devices_protocol CHECK (protocol BETWEEN 1 AND 6),
                    ADD CONSTRAINT ck_devices_status CHECK (status BETWEEN 1 AND 4), ADD CONSTRAINT ck_devices_single_target CHECK (num_nonnulls(equipment_id,field_id,holding_id) <= 1);
                ALTER TABLE connected.sensor_channels
                    ADD CONSTRAINT ck_sensor_channels_interval CHECK (sampling_interval_seconds BETWEEN 1 AND 2592000),
                    ADD CONSTRAINT ck_sensor_channels_range CHECK (maximum_valid_value IS NULL OR minimum_valid_value IS NULL OR maximum_valid_value >= minimum_valid_value);
                ALTER TABLE connected.machinery_task_files
                    ADD CONSTRAINT ck_task_files_direction CHECK (direction BETWEEN 1 AND 2), ADD CONSTRAINT ck_task_files_format CHECK (format BETWEEN 1 AND 5),
                    ADD CONSTRAINT ck_task_files_status CHECK (status BETWEEN 1 AND 5), ADD CONSTRAINT ck_task_files_size CHECK (byte_size > 0 AND schema_version > 0),
                    ADD CONSTRAINT ck_task_files_hash CHECK (sha256 ~ '^[0-9a-f]{64}$');
                ALTER TABLE connected.business_documents
                    ADD CONSTRAINT ck_business_documents_type CHECK (document_type BETWEEN 1 AND 8), ADD CONSTRAINT ck_business_documents_direction CHECK (direction BETWEEN 1 AND 2),
                    ADD CONSTRAINT ck_business_documents_status CHECK (status BETWEEN 1 AND 5), ADD CONSTRAINT ck_business_documents_schema CHECK (schema_version > 0),
                    ADD CONSTRAINT ck_business_documents_time CHECK (occurred_at <= received_at + interval '5 minutes'),
                    ADD CONSTRAINT ck_business_documents_hash CHECK (payload_sha256 ~ '^[0-9a-f]{64}$');
                ALTER TABLE analytics.kpi_definitions ADD CONSTRAINT ck_kpi_definitions_status CHECK (status BETWEEN 1 AND 3), ADD CONSTRAINT ck_kpi_definitions_version CHECK (definition_version > 0);
                ALTER TABLE analytics.projection_runs
                    ADD CONSTRAINT ck_projection_runs_status CHECK (status BETWEEN 1 AND 5), ADD CONSTRAINT ck_projection_runs_period CHECK (period_end >= period_start),
                    ADD CONSTRAINT ck_projection_runs_currency CHECK (currency ~ '^[A-Z]{3}$'), ADD CONSTRAINT ck_projection_runs_quality CHECK (data_quality_percent IS NULL OR data_quality_percent BETWEEN 0 AND 100),
                    ADD CONSTRAINT ck_projection_runs_count CHECK (fact_count >= 0);
                ALTER TABLE analytics.facts
                    ADD CONSTRAINT ck_facts_kind CHECK (kind BETWEEN 1 AND 6), ADD CONSTRAINT ck_facts_value_kind CHECK (value_kind BETWEEN 1 AND 3),
                    ADD CONSTRAINT ck_facts_period CHECK (period_end >= period_start), ADD CONSTRAINT ck_facts_quality CHECK (data_quality_percent BETWEEN 0 AND 100),
                    ADD CONSTRAINT ck_facts_sources CHECK (source_record_count >= 0);
                ALTER TABLE analytics.benchmark_enrollments ADD CONSTRAINT ck_benchmark_minimum CHECK (minimum_cohort_size BETWEEN 5 AND 100);

                CREATE OR REPLACE FUNCTION connected.validate_scope() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_TABLE_NAME = 'devices' THEN
                        IF NEW.equipment_id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM connected.equipment e WHERE e.tenant_id=NEW.tenant_id AND e.id=NEW.equipment_id AND e.farm_id=NEW.farm_id) THEN RAISE EXCEPTION 'Device equipment farm scope is invalid.' USING ERRCODE='23514'; END IF;
                        IF NEW.field_id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM farm.fields f WHERE f.tenant_id=NEW.tenant_id AND f.id=NEW.field_id AND f.farm_id=NEW.farm_id) THEN RAISE EXCEPTION 'Device field farm scope is invalid.' USING ERRCODE='23514'; END IF;
                        IF NEW.holding_id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM livestock.holdings h WHERE h.tenant_id=NEW.tenant_id AND h.id=NEW.holding_id AND h.farm_id=NEW.farm_id) THEN RAISE EXCEPTION 'Device holding farm scope is invalid.' USING ERRCODE='23514'; END IF;
                    ELSIF TG_TABLE_NAME = 'sensor_readings' THEN
                        IF NOT EXISTS (SELECT 1 FROM connected.devices d JOIN connected.sensor_channels c ON c.tenant_id=d.tenant_id AND c.device_id=d.id WHERE d.tenant_id=NEW.tenant_id AND d.id=NEW.device_id AND d.farm_id=NEW.farm_id AND c.id=NEW.channel_id AND c.metric_code=NEW.metric_code AND lower(c.unit)=lower(NEW.unit) AND c.is_active) THEN RAISE EXCEPTION 'Reading device/channel scope or metric contract is invalid.' USING ERRCODE='23514'; END IF;
                    ELSIF TG_TABLE_NAME = 'machinery_telemetry' THEN
                        IF NOT EXISTS (SELECT 1 FROM connected.equipment e WHERE e.tenant_id=NEW.tenant_id AND e.id=NEW.equipment_id AND e.farm_id=NEW.farm_id) OR (NEW.device_id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM connected.devices d WHERE d.tenant_id=NEW.tenant_id AND d.id=NEW.device_id AND d.equipment_id=NEW.equipment_id AND d.farm_id=NEW.farm_id)) THEN RAISE EXCEPTION 'Telemetry equipment/device farm scope is invalid.' USING ERRCODE='23514'; END IF;
                    ELSIF TG_TABLE_NAME = 'machinery_task_files' THEN
                        IF NOT EXISTS (SELECT 1 FROM connected.equipment e WHERE e.tenant_id=NEW.tenant_id AND e.id=NEW.equipment_id AND e.farm_id=NEW.farm_id) THEN RAISE EXCEPTION 'Task-file equipment scope is invalid.' USING ERRCODE='23514'; END IF;
                        IF NEW.work_order_id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM operations.work_orders w WHERE w.tenant_id=NEW.tenant_id AND w.id=NEW.work_order_id AND w.farm_id=NEW.farm_id) THEN RAISE EXCEPTION 'Task-file work-order scope is invalid.' USING ERRCODE='23514'; END IF;
                    ELSIF TG_TABLE_NAME = 'business_documents' THEN
                        IF NOT EXISTS (SELECT 1 FROM integrations.connections c WHERE c.tenant_id=NEW.tenant_id AND c.id=NEW.connection_id AND c.category IN (5,6)) THEN RAISE EXCEPTION 'Business documents require an ERP or accounting connection.' USING ERRCODE='23514'; END IF;
                    END IF; RETURN NEW;
                END $$;

                CREATE OR REPLACE FUNCTION analytics.validate_fact_scope() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM analytics.projection_runs r JOIN farm.farms f ON f.tenant_id=r.tenant_id AND f.id=r.farm_id WHERE r.tenant_id=NEW.tenant_id AND r.id=NEW.projection_run_id AND r.farm_id=NEW.farm_id AND f.organisation_node_id=NEW.organisation_node_id AND r.period_start=NEW.period_start AND r.period_end=NEW.period_end) THEN RAISE EXCEPTION 'Analytics fact run, farm, organisation, or period scope is invalid.' USING ERRCODE='23514'; END IF;
                    IF NEW.crop_zone_id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM production.crop_zones z JOIN production.seasons s ON s.tenant_id=z.tenant_id AND s.id=z.season_id WHERE z.tenant_id=NEW.tenant_id AND z.id=NEW.crop_zone_id AND s.farm_id=NEW.farm_id) THEN RAISE EXCEPTION 'Analytics crop-zone scope is invalid.' USING ERRCODE='23514'; END IF;
                    IF NEW.livestock_subject_id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM livestock.subjects l WHERE l.tenant_id=NEW.tenant_id AND l.id=NEW.livestock_subject_id AND l.farm_id=NEW.farm_id) THEN RAISE EXCEPTION 'Analytics livestock scope is invalid.' USING ERRCODE='23514'; END IF;
                    RETURN NEW;
                END $$;

                CREATE OR REPLACE FUNCTION platform.guard_immutable_projection_record() RETURNS trigger LANGUAGE plpgsql AS $$ BEGIN RAISE EXCEPTION '% records are immutable.',TG_TABLE_NAME USING ERRCODE='55000'; END $$;
                CREATE OR REPLACE FUNCTION connected.guard_exchange_lifecycle() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN IF TG_OP='DELETE' THEN RAISE EXCEPTION 'Exchange records are retained.' USING ERRCODE='55000'; END IF;
                    IF (to_jsonb(NEW)-ARRAY['status','processed_at','applied_entity_id','rejection_reason','updated_at_utc','version'])<>(to_jsonb(OLD)-ARRAY['status','processed_at','applied_entity_id','rejection_reason','updated_at_utc','version']) THEN RAISE EXCEPTION 'Exchange payload and lineage are immutable.' USING ERRCODE='55000'; END IF; RETURN NEW; END $$;
                CREATE OR REPLACE FUNCTION analytics.guard_definition_lifecycle() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN IF TG_OP='DELETE' THEN RAISE EXCEPTION 'Analytics definitions and enrollments are retained.' USING ERRCODE='55000'; END IF;
                    IF TG_TABLE_NAME='kpi_definitions' AND (to_jsonb(NEW)-ARRAY['status','activated_at','retired_at','updated_at_utc','version'])<>(to_jsonb(OLD)-ARRAY['status','activated_at','retired_at','updated_at_utc','version']) THEN RAISE EXCEPTION 'KPI definition content is versioned and immutable.' USING ERRCODE='55000'; END IF;
                    IF TG_TABLE_NAME='benchmark_enrollments' AND (to_jsonb(NEW)-ARRAY['is_active','revoked_at','revoked_by','updated_at_utc','version'])<>(to_jsonb(OLD)-ARRAY['is_active','revoked_at','revoked_by','updated_at_utc','version']) THEN RAISE EXCEPTION 'Benchmark enrollment scope is immutable.' USING ERRCODE='55000'; END IF; RETURN NEW; END $$;

                CREATE TRIGGER connected_devices_scope BEFORE INSERT OR UPDATE ON connected.devices FOR EACH ROW EXECUTE FUNCTION connected.validate_scope();
                CREATE TRIGGER sensor_readings_scope BEFORE INSERT ON connected.sensor_readings FOR EACH ROW EXECUTE FUNCTION connected.validate_scope();
                CREATE TRIGGER machinery_telemetry_scope BEFORE INSERT ON connected.machinery_telemetry FOR EACH ROW EXECUTE FUNCTION connected.validate_scope();
                CREATE TRIGGER machinery_task_files_scope BEFORE INSERT ON connected.machinery_task_files FOR EACH ROW EXECUTE FUNCTION connected.validate_scope();
                CREATE TRIGGER business_documents_scope BEFORE INSERT ON connected.business_documents FOR EACH ROW EXECUTE FUNCTION connected.validate_scope();
                CREATE TRIGGER analytics_facts_scope BEFORE INSERT ON analytics.facts FOR EACH ROW EXECUTE FUNCTION analytics.validate_fact_scope();
                CREATE TRIGGER sensor_readings_immutable BEFORE UPDATE OR DELETE ON connected.sensor_readings FOR EACH ROW EXECUTE FUNCTION platform.guard_immutable_projection_record();
                CREATE TRIGGER machinery_telemetry_immutable BEFORE UPDATE OR DELETE ON connected.machinery_telemetry FOR EACH ROW EXECUTE FUNCTION platform.guard_immutable_projection_record();
                CREATE TRIGGER analytics_facts_immutable BEFORE UPDATE OR DELETE ON analytics.facts FOR EACH ROW EXECUTE FUNCTION platform.guard_immutable_projection_record();
                CREATE TRIGGER machinery_task_files_lifecycle BEFORE UPDATE OR DELETE ON connected.machinery_task_files FOR EACH ROW EXECUTE FUNCTION connected.guard_exchange_lifecycle();
                CREATE TRIGGER business_documents_lifecycle BEFORE UPDATE OR DELETE ON connected.business_documents FOR EACH ROW EXECUTE FUNCTION connected.guard_exchange_lifecycle();
                CREATE TRIGGER kpi_definition_lifecycle BEFORE UPDATE OR DELETE ON analytics.kpi_definitions FOR EACH ROW EXECUTE FUNCTION analytics.guard_definition_lifecycle();
                CREATE TRIGGER benchmark_enrollment_lifecycle BEFORE UPDATE OR DELETE ON analytics.benchmark_enrollments FOR EACH ROW EXECUTE FUNCTION analytics.guard_definition_lifecycle();

                DO $$ DECLARE table_name text; BEGIN
                    FOREACH table_name IN ARRAY ARRAY['equipment','devices','sensor_channels','sensor_readings','machinery_telemetry','machinery_task_files','business_documents'] LOOP
                        EXECUTE format('ALTER TABLE connected.%I ENABLE ROW LEVEL SECURITY',table_name); EXECUTE format('ALTER TABLE connected.%I FORCE ROW LEVEL SECURITY',table_name);
                        EXECUTE format('CREATE POLICY tenant_isolation ON connected.%I USING (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid) WITH CHECK (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid)',table_name); END LOOP;
                    FOREACH table_name IN ARRAY ARRAY['kpi_definitions','projection_runs','facts','benchmark_enrollments'] LOOP
                        EXECUTE format('ALTER TABLE analytics.%I ENABLE ROW LEVEL SECURITY',table_name); EXECUTE format('ALTER TABLE analytics.%I FORCE ROW LEVEL SECURITY',table_name);
                        EXECUTE format('CREATE POLICY tenant_isolation ON analytics.%I USING (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid) WITH CHECK (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid)',table_name); END LOOP;
                END $$;

                CREATE OR REPLACE FUNCTION analytics.benchmark_metric(p_metric_code text,p_cohort_key text,p_from date,p_to date)
                RETURNS TABLE(cohort_size integer,minimum numeric,percentile25 numeric,median numeric,percentile75 numeric,maximum numeric,average numeric,average_data_quality_percent numeric)
                LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,analytics AS $$
                DECLARE requesting_tenant uuid; required_size integer; BEGIN
                    requesting_tenant:=nullif(current_setting('app.tenant_id',true),'')::uuid;
                    SELECT minimum_cohort_size INTO required_size FROM analytics.benchmark_enrollments WHERE tenant_id=requesting_tenant AND metric_code=lower(p_metric_code) AND cohort_key=lower(p_cohort_key) AND is_active;
                    IF required_size IS NULL THEN RAISE EXCEPTION 'Active benchmark enrollment required.' USING ERRCODE='42501'; END IF;
                    RETURN QUERY WITH tenant_values AS (SELECT f.tenant_id,avg(f.value) value,avg(f.data_quality_percent) quality FROM analytics.facts f JOIN analytics.benchmark_enrollments e ON e.tenant_id=f.tenant_id AND e.metric_code=f.metric_code AND e.cohort_key=lower(p_cohort_key) AND e.is_active WHERE f.metric_code=lower(p_metric_code) AND f.value_kind=1 AND f.period_end>=p_from AND f.period_start<=p_to AND f.data_quality_percent>=60 GROUP BY f.tenant_id)
                    SELECT count(*)::integer,min(value),percentile_cont(0.25) within group(order by value)::numeric,percentile_cont(0.5) within group(order by value)::numeric,percentile_cont(0.75) within group(order by value)::numeric,max(value),avg(value),avg(quality) FROM tenant_values HAVING count(*)>=required_size;
                END $$;
                REVOKE ALL ON FUNCTION analytics.benchmark_metric(text,text,date,date) FROM PUBLIC;
                GRANT USAGE ON SCHEMA connected,analytics TO murimios_app;
                GRANT SELECT,INSERT,UPDATE,DELETE ON ALL TABLES IN SCHEMA connected,analytics TO murimios_app;
                GRANT EXECUTE ON FUNCTION analytics.benchmark_metric(text,text,date,date) TO murimios_app;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP FUNCTION IF EXISTS analytics.benchmark_metric(text,text,date,date);
                DROP FUNCTION IF EXISTS analytics.guard_definition_lifecycle() CASCADE;
                DROP FUNCTION IF EXISTS connected.guard_exchange_lifecycle() CASCADE;
                DROP FUNCTION IF EXISTS analytics.validate_fact_scope() CASCADE;
                DROP FUNCTION IF EXISTS connected.validate_scope() CASCADE;
                DROP FUNCTION IF EXISTS platform.guard_immutable_projection_record() CASCADE;
                """);
            migrationBuilder.DropTable(
                name: "benchmark_enrollments",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "business_documents",
                schema: "connected");

            migrationBuilder.DropTable(
                name: "facts",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "kpi_definitions",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "machinery_task_files",
                schema: "connected");

            migrationBuilder.DropTable(
                name: "machinery_telemetry",
                schema: "connected");

            migrationBuilder.DropTable(
                name: "sensor_readings",
                schema: "connected");

            migrationBuilder.DropTable(
                name: "projection_runs",
                schema: "analytics");

            migrationBuilder.DropTable(
                name: "sensor_channels",
                schema: "connected");

            migrationBuilder.DropTable(
                name: "devices",
                schema: "connected");

            migrationBuilder.DropTable(
                name: "equipment",
                schema: "connected");
        }
    }
}
