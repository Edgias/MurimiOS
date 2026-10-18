using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddRealTimeFieldIntelligence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "insights");

            migrationBuilder.AddUniqueConstraint(
                name: "ak_ingestion_records_tenant_id_id",
                schema: "integrations",
                table: "ingestion_records",
                columns: new[] { "tenant_id", "id" });

            migrationBuilder.AddUniqueConstraint(
                name: "ak_evidence_attachments_tenant_id_id",
                schema: "evidence",
                table: "attachments",
                columns: new[] { "tenant_id", "id" });

            migrationBuilder.CreateTable(
                name: "alerts",
                schema: "insights",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: false),
                    boundary_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    alert_key = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    source_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    detected_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    observed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    acknowledged_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    acknowledged_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    resolved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    resolved_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    resolution = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    dismissed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    dismissed_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    dismissal_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    follow_up_work_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_alerts", x => x.id);
                    table.ForeignKey(
                        name: "fk_alerts_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_alerts_field_boundary_versions_tenant_id_boundary_version_id",
                        columns: x => new { x.tenant_id, x.boundary_version_id },
                        principalSchema: "farm",
                        principalTable: "field_boundary_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_alerts_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_alerts_work_orders_tenant_id_follow_up_work_order_id",
                        columns: x => new { x.tenant_id, x.follow_up_work_order_id },
                        principalSchema: "operations",
                        principalTable: "work_orders",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "map_layers",
                schema: "insights",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: false),
                    boundary_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    source_type = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_observed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    generated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    valid_from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    valid_to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    geo_json = table.Column<string>(type: "jsonb", nullable: false),
                    style_json = table.Column<string>(type: "jsonb", nullable: false),
                    quality = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_map_layers", x => x.id);
                    table.ForeignKey(
                        name: "fk_map_layers_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_map_layers_field_boundary_versions_tenant_id_boundary_versi",
                        columns: x => new { x.tenant_id, x.boundary_version_id },
                        principalSchema: "farm",
                        principalTable: "field_boundary_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_map_layers_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "risk_rules",
                schema: "insights",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_key = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    rule_version = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    crop = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    metric = table.Column<int>(type: "integer", nullable: false),
                    aggregation = table.Column<int>(type: "integer", nullable: false),
                    comparison = table.Column<int>(type: "integer", nullable: false),
                    threshold = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    lookback_hours = table.Column<int>(type: "integer", nullable: false),
                    minimum_samples = table.Column<int>(type: "integer", nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    message = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    effective_from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    effective_to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
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
                    table.PrimaryKey("pk_risk_rules", x => x.id);
                    table.UniqueConstraint("ak_field_risk_rules_tenant_id_id", x => new { x.tenant_id, x.id });
                });

            migrationBuilder.CreateTable(
                name: "satellite_field_statistics",
                schema: "insights",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: false),
                    boundary_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feed_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ingestion_record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider_acquisition_id = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    index = table.Column<int>(type: "integer", nullable: false),
                    custom_index = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    acquired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    processed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    mean = table.Column<decimal>(type: "numeric(10,5)", precision: 10, scale: 5, nullable: false),
                    minimum = table.Column<decimal>(type: "numeric(10,5)", precision: 10, scale: 5, nullable: false),
                    maximum = table.Column<decimal>(type: "numeric(10,5)", precision: 10, scale: 5, nullable: false),
                    standard_deviation = table.Column<decimal>(type: "numeric(10,5)", precision: 10, scale: 5, nullable: false),
                    valid_pixel_percent = table.Column<decimal>(type: "numeric(10,5)", precision: 10, scale: 5, nullable: false),
                    cloud_percent = table.Column<decimal>(type: "numeric(10,5)", precision: 10, scale: 5, nullable: false),
                    quality = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_satellite_field_statistics", x => x.id);
                    table.ForeignKey(
                        name: "fk_satellite_field_statistics_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_satellite_field_statistics_feeds_tenant_id_feed_id",
                        columns: x => new { x.tenant_id, x.feed_id },
                        principalSchema: "integrations",
                        principalTable: "feeds",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_satellite_field_statistics_field_boundary_versions_tenant_i",
                        columns: x => new { x.tenant_id, x.boundary_version_id },
                        principalSchema: "farm",
                        principalTable: "field_boundary_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_satellite_field_statistics_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_satellite_field_statistics_ingestion_records_tenant_id_inge",
                        columns: x => new { x.tenant_id, x.ingestion_record_id },
                        principalSchema: "integrations",
                        principalTable: "ingestion_records",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "scouting_observations",
                schema: "insights",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: false),
                    boundary_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    crop_zone_id = table.Column<Guid>(type: "uuid", nullable: true),
                    category = table.Column<int>(type: "integer", nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    issue_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    observed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    recorded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    recorded_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    location = table.Column<Point>(type: "geometry(Point,4326)", nullable: true),
                    evidence_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source_command_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_scouting_observations", x => x.id);
                    table.ForeignKey(
                        name: "fk_scouting_observations_crop_zones_tenant_id_crop_zone_id",
                        columns: x => new { x.tenant_id, x.crop_zone_id },
                        principalSchema: "production",
                        principalTable: "crop_zones",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_scouting_observations_evidence_attachments_tenant_id_eviden",
                        columns: x => new { x.tenant_id, x.evidence_id },
                        principalSchema: "evidence",
                        principalTable: "attachments",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_scouting_observations_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_scouting_observations_field_boundary_versions_tenant_id_bou",
                        columns: x => new { x.tenant_id, x.boundary_version_id },
                        principalSchema: "farm",
                        principalTable: "field_boundary_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_scouting_observations_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "weather_locations",
                schema: "insights",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: true),
                    grid_key = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    latitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    longitude = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    elevation_metres = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_weather_locations", x => x.id);
                    table.UniqueConstraint("ak_weather_locations_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_weather_locations_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_weather_locations_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "risk_assessments",
                schema: "insights",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: false),
                    boundary_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_version = table.Column<int>(type: "integer", nullable: false),
                    assessed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    window_start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    window_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    observed_value = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    sample_count = table.Column<int>(type: "integer", nullable: false),
                    triggered = table.Column<bool>(type: "boolean", nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    evaluator_version = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    input_snapshot_json = table.Column<string>(type: "jsonb", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_risk_assessments", x => x.id);
                    table.ForeignKey(
                        name: "fk_risk_assessments_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_risk_assessments_field_boundary_versions_tenant_id_boundary",
                        columns: x => new { x.tenant_id, x.boundary_version_id },
                        principalSchema: "farm",
                        principalTable: "field_boundary_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_risk_assessments_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_risk_assessments_risk_rules_tenant_id_rule_id",
                        columns: x => new { x.tenant_id, x.rule_id },
                        principalSchema: "insights",
                        principalTable: "risk_rules",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "weather_forecasts",
                schema: "insights",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: true),
                    feed_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ingestion_record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider_forecast_id = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    issued_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    valid_from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    valid_to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    minimum_temperature_c = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: true),
                    maximum_temperature_c = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: true),
                    precipitation_mm = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: true),
                    precipitation_probability_percent = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: true),
                    wind_speed_mps = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: true),
                    relative_humidity_percent = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: true),
                    quality = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_weather_forecasts", x => x.id);
                    table.ForeignKey(
                        name: "fk_weather_forecasts_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_weather_forecasts_feeds_tenant_id_feed_id",
                        columns: x => new { x.tenant_id, x.feed_id },
                        principalSchema: "integrations",
                        principalTable: "feeds",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_weather_forecasts_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_weather_forecasts_ingestion_records_tenant_id_ingestion_rec",
                        columns: x => new { x.tenant_id, x.ingestion_record_id },
                        principalSchema: "integrations",
                        principalTable: "ingestion_records",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_weather_forecasts_weather_locations_tenant_id_location_id",
                        columns: x => new { x.tenant_id, x.location_id },
                        principalSchema: "insights",
                        principalTable: "weather_locations",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "weather_observations",
                schema: "insights",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: true),
                    feed_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ingestion_record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider_observation_id = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    observed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    received_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    temperature_c = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: true),
                    relative_humidity_percent = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: true),
                    precipitation_mm = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: true),
                    wind_speed_mps = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: true),
                    wind_direction_degrees = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: true),
                    pressure_hpa = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: true),
                    soil_temperature_c = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: true),
                    soil_moisture_percent = table.Column<decimal>(type: "numeric(10,3)", precision: 10, scale: 3, nullable: true),
                    quality = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_weather_observations", x => x.id);
                    table.ForeignKey(
                        name: "fk_weather_observations_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_weather_observations_feeds_tenant_id_feed_id",
                        columns: x => new { x.tenant_id, x.feed_id },
                        principalSchema: "integrations",
                        principalTable: "feeds",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_weather_observations_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_weather_observations_ingestion_records_tenant_id_ingestion_",
                        columns: x => new { x.tenant_id, x.ingestion_record_id },
                        principalSchema: "integrations",
                        principalTable: "ingestion_records",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_weather_observations_weather_locations_tenant_id_location_id",
                        columns: x => new { x.tenant_id, x.location_id },
                        principalSchema: "insights",
                        principalTable: "weather_locations",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_alerts_tenant_id",
                schema: "insights",
                table: "alerts",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_alerts_tenant_id_alert_key_status",
                schema: "insights",
                table: "alerts",
                columns: new[] { "tenant_id", "alert_key", "status" },
                unique: true,
                filter: "status IN (1,2)");

            migrationBuilder.CreateIndex(
                name: "ix_alerts_tenant_id_boundary_version_id",
                schema: "insights",
                table: "alerts",
                columns: new[] { "tenant_id", "boundary_version_id" });

            migrationBuilder.CreateIndex(
                name: "ix_alerts_tenant_id_farm_id_status_severity_detected_at",
                schema: "insights",
                table: "alerts",
                columns: new[] { "tenant_id", "farm_id", "status", "severity", "detected_at" });

            migrationBuilder.CreateIndex(
                name: "ix_alerts_tenant_id_field_id",
                schema: "insights",
                table: "alerts",
                columns: new[] { "tenant_id", "field_id" });

            migrationBuilder.CreateIndex(
                name: "ix_alerts_tenant_id_follow_up_work_order_id",
                schema: "insights",
                table: "alerts",
                columns: new[] { "tenant_id", "follow_up_work_order_id" });

            migrationBuilder.CreateIndex(
                name: "ix_map_layers_tenant_id",
                schema: "insights",
                table: "map_layers",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_map_layers_tenant_id_boundary_version_id",
                schema: "insights",
                table: "map_layers",
                columns: new[] { "tenant_id", "boundary_version_id" });

            migrationBuilder.CreateIndex(
                name: "ix_map_layers_tenant_id_farm_id",
                schema: "insights",
                table: "map_layers",
                columns: new[] { "tenant_id", "farm_id" });

            migrationBuilder.CreateIndex(
                name: "ix_map_layers_tenant_id_field_id_kind_valid_from",
                schema: "insights",
                table: "map_layers",
                columns: new[] { "tenant_id", "field_id", "kind", "valid_from" });

            migrationBuilder.CreateIndex(
                name: "ix_map_layers_tenant_id_source_type_source_id_kind",
                schema: "insights",
                table: "map_layers",
                columns: new[] { "tenant_id", "source_type", "source_id", "kind" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_risk_assessments_tenant_id",
                schema: "insights",
                table: "risk_assessments",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_risk_assessments_tenant_id_boundary_version_id",
                schema: "insights",
                table: "risk_assessments",
                columns: new[] { "tenant_id", "boundary_version_id" });

            migrationBuilder.CreateIndex(
                name: "ix_risk_assessments_tenant_id_farm_id",
                schema: "insights",
                table: "risk_assessments",
                columns: new[] { "tenant_id", "farm_id" });

            migrationBuilder.CreateIndex(
                name: "ix_risk_assessments_tenant_id_field_id_assessed_at",
                schema: "insights",
                table: "risk_assessments",
                columns: new[] { "tenant_id", "field_id", "assessed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_risk_assessments_tenant_id_rule_id_field_id_assessed_at",
                schema: "insights",
                table: "risk_assessments",
                columns: new[] { "tenant_id", "rule_id", "field_id", "assessed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_risk_rules_tenant_id",
                schema: "insights",
                table: "risk_rules",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_risk_rules_tenant_id_rule_key_rule_version",
                schema: "insights",
                table: "risk_rules",
                columns: new[] { "tenant_id", "rule_key", "rule_version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_risk_rules_tenant_id_rule_key_status",
                schema: "insights",
                table: "risk_rules",
                columns: new[] { "tenant_id", "rule_key", "status" },
                unique: true,
                filter: "status = 2");

            migrationBuilder.CreateIndex(
                name: "ix_risk_rules_tenant_id_status_effective_from_effective_to",
                schema: "insights",
                table: "risk_rules",
                columns: new[] { "tenant_id", "status", "effective_from", "effective_to" });

            migrationBuilder.CreateIndex(
                name: "ix_satellite_field_statistics_tenant_id",
                schema: "insights",
                table: "satellite_field_statistics",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_satellite_field_statistics_tenant_id_boundary_version_id",
                schema: "insights",
                table: "satellite_field_statistics",
                columns: new[] { "tenant_id", "boundary_version_id" });

            migrationBuilder.CreateIndex(
                name: "ix_satellite_field_statistics_tenant_id_farm_id",
                schema: "insights",
                table: "satellite_field_statistics",
                columns: new[] { "tenant_id", "farm_id" });

            migrationBuilder.CreateIndex(
                name: "ix_satellite_field_statistics_tenant_id_feed_id_provider_acqui",
                schema: "insights",
                table: "satellite_field_statistics",
                columns: new[] { "tenant_id", "feed_id", "provider_acquisition_id", "index", "custom_index" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_satellite_field_statistics_tenant_id_field_id_index_acquire",
                schema: "insights",
                table: "satellite_field_statistics",
                columns: new[] { "tenant_id", "field_id", "index", "acquired_at" });

            migrationBuilder.CreateIndex(
                name: "ix_satellite_field_statistics_tenant_id_ingestion_record_id",
                schema: "insights",
                table: "satellite_field_statistics",
                columns: new[] { "tenant_id", "ingestion_record_id" });

            migrationBuilder.CreateIndex(
                name: "ix_scouting_observations_tenant_id",
                schema: "insights",
                table: "scouting_observations",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_scouting_observations_tenant_id_boundary_version_id",
                schema: "insights",
                table: "scouting_observations",
                columns: new[] { "tenant_id", "boundary_version_id" });

            migrationBuilder.CreateIndex(
                name: "ix_scouting_observations_tenant_id_crop_zone_id",
                schema: "insights",
                table: "scouting_observations",
                columns: new[] { "tenant_id", "crop_zone_id" });

            migrationBuilder.CreateIndex(
                name: "ix_scouting_observations_tenant_id_evidence_id",
                schema: "insights",
                table: "scouting_observations",
                columns: new[] { "tenant_id", "evidence_id" });

            migrationBuilder.CreateIndex(
                name: "ix_scouting_observations_tenant_id_farm_id",
                schema: "insights",
                table: "scouting_observations",
                columns: new[] { "tenant_id", "farm_id" });

            migrationBuilder.CreateIndex(
                name: "ix_scouting_observations_tenant_id_field_id_observed_at",
                schema: "insights",
                table: "scouting_observations",
                columns: new[] { "tenant_id", "field_id", "observed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_scouting_observations_tenant_id_source_command_id",
                schema: "insights",
                table: "scouting_observations",
                columns: new[] { "tenant_id", "source_command_id" },
                unique: true,
                filter: "source_command_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_weather_forecasts_tenant_id",
                schema: "insights",
                table: "weather_forecasts",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_weather_forecasts_tenant_id_farm_id",
                schema: "insights",
                table: "weather_forecasts",
                columns: new[] { "tenant_id", "farm_id" });

            migrationBuilder.CreateIndex(
                name: "ix_weather_forecasts_tenant_id_feed_id_provider_forecast_id",
                schema: "insights",
                table: "weather_forecasts",
                columns: new[] { "tenant_id", "feed_id", "provider_forecast_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_weather_forecasts_tenant_id_field_id_valid_from_valid_to",
                schema: "insights",
                table: "weather_forecasts",
                columns: new[] { "tenant_id", "field_id", "valid_from", "valid_to" });

            migrationBuilder.CreateIndex(
                name: "ix_weather_forecasts_tenant_id_ingestion_record_id",
                schema: "insights",
                table: "weather_forecasts",
                columns: new[] { "tenant_id", "ingestion_record_id" });

            migrationBuilder.CreateIndex(
                name: "ix_weather_forecasts_tenant_id_location_id",
                schema: "insights",
                table: "weather_forecasts",
                columns: new[] { "tenant_id", "location_id" });

            migrationBuilder.CreateIndex(
                name: "ix_weather_locations_tenant_id",
                schema: "insights",
                table: "weather_locations",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_weather_locations_tenant_id_farm_id_grid_key",
                schema: "insights",
                table: "weather_locations",
                columns: new[] { "tenant_id", "farm_id", "grid_key" },
                unique: true,
                filter: "field_id IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_weather_locations_tenant_id_field_id_grid_key",
                schema: "insights",
                table: "weather_locations",
                columns: new[] { "tenant_id", "field_id", "grid_key" },
                unique: true,
                filter: "field_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_weather_observations_tenant_id",
                schema: "insights",
                table: "weather_observations",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_weather_observations_tenant_id_farm_id",
                schema: "insights",
                table: "weather_observations",
                columns: new[] { "tenant_id", "farm_id" });

            migrationBuilder.CreateIndex(
                name: "ix_weather_observations_tenant_id_feed_id_provider_observation",
                schema: "insights",
                table: "weather_observations",
                columns: new[] { "tenant_id", "feed_id", "provider_observation_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_weather_observations_tenant_id_field_id_observed_at",
                schema: "insights",
                table: "weather_observations",
                columns: new[] { "tenant_id", "field_id", "observed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_weather_observations_tenant_id_ingestion_record_id",
                schema: "insights",
                table: "weather_observations",
                columns: new[] { "tenant_id", "ingestion_record_id" });

            migrationBuilder.CreateIndex(
                name: "ix_weather_observations_tenant_id_location_id_observed_at",
                schema: "insights",
                table: "weather_observations",
                columns: new[] { "tenant_id", "location_id", "observed_at" });

            migrationBuilder.Sql(
                """
                ALTER TABLE insights.weather_locations ADD CONSTRAINT ck_weather_location_coordinates CHECK (
                    latitude BETWEEN -90 AND 90 AND longitude BETWEEN -180 AND 180
                    AND (elevation_metres IS NULL OR elevation_metres BETWEEN -500 AND 9000));
                ALTER TABLE insights.weather_observations ADD CONSTRAINT ck_weather_observation_values CHECK (
                    quality BETWEEN 1 AND 4
                    AND (temperature_c IS NULL OR temperature_c BETWEEN -100 AND 70)
                    AND (relative_humidity_percent IS NULL OR relative_humidity_percent BETWEEN 0 AND 100)
                    AND (precipitation_mm IS NULL OR precipitation_mm BETWEEN 0 AND 10000)
                    AND (wind_speed_mps IS NULL OR wind_speed_mps BETWEEN 0 AND 150)
                    AND (wind_direction_degrees IS NULL OR wind_direction_degrees BETWEEN 0 AND 360)
                    AND (pressure_hpa IS NULL OR pressure_hpa BETWEEN 500 AND 1200)
                    AND (soil_temperature_c IS NULL OR soil_temperature_c BETWEEN -100 AND 100)
                    AND (soil_moisture_percent IS NULL OR soil_moisture_percent BETWEEN 0 AND 100)
                    AND num_nonnulls(temperature_c, relative_humidity_percent, precipitation_mm, wind_speed_mps,
                        pressure_hpa, soil_temperature_c, soil_moisture_percent) > 0);
                ALTER TABLE insights.weather_forecasts ADD CONSTRAINT ck_weather_forecast_values CHECK (
                    quality BETWEEN 1 AND 4 AND valid_to > valid_from
                    AND (minimum_temperature_c IS NULL OR minimum_temperature_c BETWEEN -100 AND 70)
                    AND (maximum_temperature_c IS NULL OR maximum_temperature_c BETWEEN -100 AND 70)
                    AND (minimum_temperature_c IS NULL OR maximum_temperature_c IS NULL OR minimum_temperature_c <= maximum_temperature_c)
                    AND (precipitation_mm IS NULL OR precipitation_mm BETWEEN 0 AND 10000)
                    AND (precipitation_probability_percent IS NULL OR precipitation_probability_percent BETWEEN 0 AND 100)
                    AND (wind_speed_mps IS NULL OR wind_speed_mps BETWEEN 0 AND 150)
                    AND (relative_humidity_percent IS NULL OR relative_humidity_percent BETWEEN 0 AND 100));
                ALTER TABLE insights.satellite_field_statistics ADD CONSTRAINT ck_satellite_statistic_values CHECK (
                    index BETWEEN 1 AND 6 AND quality BETWEEN 1 AND 4
                    AND minimum <= mean AND mean <= maximum AND standard_deviation >= 0
                    AND valid_pixel_percent BETWEEN 0 AND 100 AND cloud_percent BETWEEN 0 AND 100
                    AND ((index = 6 AND custom_index IS NOT NULL) OR (index <> 6 AND custom_index IS NULL)));
                ALTER TABLE insights.scouting_observations ADD CONSTRAINT ck_scouting_observation_values CHECK (
                    category BETWEEN 1 AND 8 AND severity BETWEEN 1 AND 4
                    AND (location IS NULL OR (ST_SRID(location) = 4326 AND ST_IsValid(location))));
                ALTER TABLE insights.risk_rules ADD CONSTRAINT ck_risk_rule_state CHECK (
                    rule_version > 0 AND metric BETWEEN 1 AND 8 AND aggregation BETWEEN 1 AND 5
                    AND comparison BETWEEN 1 AND 5 AND lookback_hours BETWEEN 1 AND 8760
                    AND minimum_samples BETWEEN 1 AND 100000 AND severity BETWEEN 1 AND 4 AND status BETWEEN 1 AND 3
                    AND (effective_to IS NULL OR effective_to > effective_from)
                    AND ((status = 1 AND activated_at IS NULL AND retired_at IS NULL)
                      OR (status = 2 AND activated_at IS NOT NULL AND retired_at IS NULL)
                      OR (status = 3 AND activated_at IS NOT NULL AND retired_at IS NOT NULL)));
                ALTER TABLE insights.risk_assessments ADD CONSTRAINT ck_risk_assessment_values CHECK (
                    rule_version > 0 AND window_end > window_start AND sample_count >= 0
                    AND severity BETWEEN 1 AND 4 AND (NOT triggered OR observed_value IS NOT NULL));
                ALTER TABLE insights.alerts ADD CONSTRAINT ck_field_insight_alert_state CHECK (
                    severity BETWEEN 1 AND 4 AND status BETWEEN 1 AND 4
                    AND (expires_at IS NULL OR expires_at > detected_at)
                    AND ((status = 1 AND acknowledged_at IS NULL AND resolved_at IS NULL AND dismissed_at IS NULL)
                      OR (status = 2 AND acknowledged_at IS NOT NULL AND acknowledged_by IS NOT NULL AND resolved_at IS NULL AND dismissed_at IS NULL)
                      OR (status = 3 AND resolved_at IS NOT NULL AND resolved_by IS NOT NULL AND resolution IS NOT NULL AND dismissed_at IS NULL)
                      OR (status = 4 AND dismissed_at IS NOT NULL AND dismissed_by IS NOT NULL AND dismissal_reason IS NOT NULL AND resolved_at IS NULL)));
                ALTER TABLE insights.map_layers ADD CONSTRAINT ck_field_map_layer_values CHECK (
                    kind BETWEEN 1 AND 4 AND quality BETWEEN 1 AND 4 AND (valid_to IS NULL OR valid_to > valid_from)
                    AND geo_json ? 'type');

                CREATE OR REPLACE FUNCTION insights.reject_immutable_mutation() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    RAISE EXCEPTION 'Field-intelligence source, assessment, and projection records are immutable.' USING ERRCODE = '55000';
                END $$;
                CREATE TRIGGER weather_locations_immutable BEFORE UPDATE OR DELETE ON insights.weather_locations FOR EACH ROW EXECUTE FUNCTION insights.reject_immutable_mutation();
                CREATE TRIGGER weather_observations_immutable BEFORE UPDATE OR DELETE ON insights.weather_observations FOR EACH ROW EXECUTE FUNCTION insights.reject_immutable_mutation();
                CREATE TRIGGER weather_forecasts_immutable BEFORE UPDATE OR DELETE ON insights.weather_forecasts FOR EACH ROW EXECUTE FUNCTION insights.reject_immutable_mutation();
                CREATE TRIGGER satellite_statistics_immutable BEFORE UPDATE OR DELETE ON insights.satellite_field_statistics FOR EACH ROW EXECUTE FUNCTION insights.reject_immutable_mutation();
                CREATE TRIGGER scouting_observations_immutable BEFORE UPDATE OR DELETE ON insights.scouting_observations FOR EACH ROW EXECUTE FUNCTION insights.reject_immutable_mutation();
                CREATE TRIGGER risk_assessments_immutable BEFORE UPDATE OR DELETE ON insights.risk_assessments FOR EACH ROW EXECUTE FUNCTION insights.reject_immutable_mutation();
                CREATE TRIGGER map_layers_immutable BEFORE UPDATE OR DELETE ON insights.map_layers FOR EACH ROW EXECUTE FUNCTION insights.reject_immutable_mutation();

                CREATE OR REPLACE FUNCTION insights.guard_risk_rule_update() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_OP = 'DELETE' THEN RAISE EXCEPTION 'Risk-rule versions cannot be deleted.' USING ERRCODE = '55000'; END IF;
                    IF OLD.tenant_id <> NEW.tenant_id OR OLD.id <> NEW.id OR OLD.rule_key <> NEW.rule_key
                       OR OLD.rule_version <> NEW.rule_version OR OLD.name <> NEW.name OR OLD.crop IS DISTINCT FROM NEW.crop
                       OR OLD.metric <> NEW.metric OR OLD.aggregation <> NEW.aggregation OR OLD.comparison <> NEW.comparison
                       OR OLD.threshold <> NEW.threshold OR OLD.lookback_hours <> NEW.lookback_hours
                       OR OLD.minimum_samples <> NEW.minimum_samples OR OLD.severity <> NEW.severity
                       OR OLD.message <> NEW.message OR OLD.effective_from <> NEW.effective_from
                       OR OLD.effective_to IS DISTINCT FROM NEW.effective_to OR OLD.created_by <> NEW.created_by THEN
                        RAISE EXCEPTION 'Risk-rule configuration is immutable; create a new version.' USING ERRCODE = '55000';
                    END IF;
                    IF NOT ((OLD.status = 1 AND NEW.status = 2) OR (OLD.status = 2 AND NEW.status = 3)) THEN
                        RAISE EXCEPTION 'Invalid risk-rule lifecycle transition.' USING ERRCODE = '55000';
                    END IF;
                    RETURN NEW;
                END $$;
                CREATE TRIGGER risk_rule_guard BEFORE UPDATE OR DELETE ON insights.risk_rules FOR EACH ROW EXECUTE FUNCTION insights.guard_risk_rule_update();

                CREATE OR REPLACE FUNCTION insights.guard_alert_update() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_OP = 'DELETE' THEN RAISE EXCEPTION 'Field-intelligence alerts cannot be deleted.' USING ERRCODE = '55000'; END IF;
                    IF OLD.tenant_id <> NEW.tenant_id OR OLD.id <> NEW.id OR OLD.farm_id <> NEW.farm_id
                       OR OLD.field_id <> NEW.field_id OR OLD.boundary_version_id <> NEW.boundary_version_id
                       OR OLD.alert_key <> NEW.alert_key OR OLD.source_type <> NEW.source_type OR OLD.source_id <> NEW.source_id
                       OR OLD.severity <> NEW.severity OR OLD.title <> NEW.title OR OLD.message <> NEW.message
                       OR OLD.detected_at <> NEW.detected_at OR OLD.observed_at <> NEW.observed_at
                       OR OLD.expires_at IS DISTINCT FROM NEW.expires_at THEN
                        RAISE EXCEPTION 'Alert identity, source, and detected content are immutable.' USING ERRCODE = '55000';
                    END IF;
                    IF NEW.follow_up_work_order_id IS DISTINCT FROM OLD.follow_up_work_order_id
                       AND NOT (OLD.follow_up_work_order_id IS NULL AND NEW.follow_up_work_order_id IS NOT NULL) THEN
                        RAISE EXCEPTION 'A follow-up work order can be linked once.' USING ERRCODE = '55000';
                    END IF;
                    IF NEW.status <> OLD.status AND NOT ((OLD.status = 1 AND NEW.status IN (2,3,4)) OR (OLD.status = 2 AND NEW.status IN (3,4))) THEN
                        RAISE EXCEPTION 'Invalid alert lifecycle transition.' USING ERRCODE = '55000';
                    END IF;
                    IF NEW.status = OLD.status AND NEW.follow_up_work_order_id IS NOT DISTINCT FROM OLD.follow_up_work_order_id THEN
                        RAISE EXCEPTION 'No permitted alert transition was supplied.' USING ERRCODE = '55000';
                    END IF;
                    RETURN NEW;
                END $$;
                CREATE TRIGGER field_insight_alert_guard BEFORE UPDATE OR DELETE ON insights.alerts FOR EACH ROW EXECUTE FUNCTION insights.guard_alert_update();

                DO $$
                DECLARE table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY['weather_locations','weather_observations','weather_forecasts',
                        'satellite_field_statistics','scouting_observations','risk_rules','risk_assessments','alerts','map_layers'] LOOP
                        EXECUTE format('ALTER TABLE insights.%I ENABLE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('ALTER TABLE insights.%I FORCE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('CREATE POLICY tenant_isolation ON insights.%I USING (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid) WITH CHECK (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid)', table_name);
                    END LOOP;
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        GRANT USAGE ON SCHEMA insights TO murimios_app;
                        GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA insights TO murimios_app;
                        GRANT USAGE, SELECT, UPDATE ON ALL SEQUENCES IN SCHEMA insights TO murimios_app;
                        ALTER DEFAULT PRIVILEGES IN SCHEMA insights GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO murimios_app;
                        ALTER DEFAULT PRIVILEGES IN SCHEMA insights GRANT USAGE, SELECT, UPDATE ON SEQUENCES TO murimios_app;
                    END IF;
                END $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP TRIGGER IF EXISTS weather_locations_immutable ON insights.weather_locations;
                DROP TRIGGER IF EXISTS weather_observations_immutable ON insights.weather_observations;
                DROP TRIGGER IF EXISTS weather_forecasts_immutable ON insights.weather_forecasts;
                DROP TRIGGER IF EXISTS satellite_statistics_immutable ON insights.satellite_field_statistics;
                DROP TRIGGER IF EXISTS scouting_observations_immutable ON insights.scouting_observations;
                DROP TRIGGER IF EXISTS risk_assessments_immutable ON insights.risk_assessments;
                DROP TRIGGER IF EXISTS map_layers_immutable ON insights.map_layers;
                DROP TRIGGER IF EXISTS risk_rule_guard ON insights.risk_rules;
                DROP TRIGGER IF EXISTS field_insight_alert_guard ON insights.alerts;
                DROP FUNCTION IF EXISTS insights.reject_immutable_mutation();
                DROP FUNCTION IF EXISTS insights.guard_risk_rule_update();
                DROP FUNCTION IF EXISTS insights.guard_alert_update();
                """);

            migrationBuilder.DropTable(
                name: "alerts",
                schema: "insights");

            migrationBuilder.DropTable(
                name: "map_layers",
                schema: "insights");

            migrationBuilder.DropTable(
                name: "risk_assessments",
                schema: "insights");

            migrationBuilder.DropTable(
                name: "satellite_field_statistics",
                schema: "insights");

            migrationBuilder.DropTable(
                name: "scouting_observations",
                schema: "insights");

            migrationBuilder.DropTable(
                name: "weather_forecasts",
                schema: "insights");

            migrationBuilder.DropTable(
                name: "weather_observations",
                schema: "insights");

            migrationBuilder.DropTable(
                name: "risk_rules",
                schema: "insights");

            migrationBuilder.DropTable(
                name: "weather_locations",
                schema: "insights");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_ingestion_records_tenant_id_id",
                schema: "integrations",
                table: "ingestion_records");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_evidence_attachments_tenant_id_id",
                schema: "evidence",
                table: "attachments");
        }
    }
}
