using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddFarmDigitalTwinDepth : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "infrastructure_assets",
                schema: "farm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: true),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    location = table.Column<Point>(type: "geography (point, 4326)", nullable: true),
                    installed_on = table.Column<DateOnly>(type: "date", nullable: true),
                    capacity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    capacity_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    details_json = table.Column<string>(type: "jsonb", nullable: false),
                    decommissioned_on = table.Column<DateOnly>(type: "date", nullable: true),
                    decommissioned_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    decommissioned_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    decommission_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_infrastructure_assets", x => x.id);
                    table.ForeignKey(
                        name: "fk_infrastructure_assets_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_infrastructure_assets_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "land_tenure_records",
                schema: "farm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: true),
                    type = table.Column<int>(type: "integer", nullable: false),
                    holder_name = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    counterparty_name = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    reference = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    registered_area_ha = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: true),
                    valid_from = table.Column<DateOnly>(type: "date", nullable: false),
                    valid_to = table.Column<DateOnly>(type: "date", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    document_reference = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    closed_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_land_tenure_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_land_tenure_records_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_land_tenure_records_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "soil_zones",
                schema: "farm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    classification = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    management_guidance = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    retired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    retired_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_soil_zones", x => x.id);
                    table.UniqueConstraint("ak_soil_zones_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_soil_zones_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_soil_zones_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "soil_zone_boundary_versions",
                schema: "farm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    soil_zone_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_boundary_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    boundary = table.Column<MultiPolygon>(type: "geography (multipolygon, 4326)", nullable: false),
                    area_ha = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: false, computedColumnSql: "ST_Area(boundary) / 10000.0", stored: true),
                    valid_from = table.Column<DateOnly>(type: "date", nullable: false),
                    valid_to = table.Column<DateOnly>(type: "date", nullable: true),
                    source = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_soil_zone_boundary_versions", x => x.id);
                    table.UniqueConstraint("ak_soil_zone_boundaries_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_soil_zone_boundary_versions_field_boundary_versions_tenant_",
                        columns: x => new { x.tenant_id, x.field_boundary_version_id },
                        principalSchema: "farm",
                        principalTable: "field_boundary_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_soil_zone_boundary_versions_soil_zones_tenant_id_soil_zone_",
                        columns: x => new { x.tenant_id, x.soil_zone_id },
                        principalSchema: "farm",
                        principalTable: "soil_zones",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "soil_samples",
                schema: "farm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_boundary_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    soil_zone_boundary_version_id = table.Column<Guid>(type: "uuid", nullable: true),
                    sample_code = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    location = table.Column<Point>(type: "geography (point, 4326)", nullable: false),
                    sampled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    sampled_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    depth_from_cm = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    depth_to_cm = table.Column<decimal>(type: "numeric(8,2)", precision: 8, scale: 2, nullable: false),
                    laboratory_name = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    laboratory_reference = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    submitted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    analysed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    rejected_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_soil_samples", x => x.id);
                    table.UniqueConstraint("ak_soil_samples_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_soil_samples_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_soil_samples_field_boundary_versions_tenant_id_field_bounda",
                        columns: x => new { x.tenant_id, x.field_boundary_version_id },
                        principalSchema: "farm",
                        principalTable: "field_boundary_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_soil_samples_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_soil_samples_soil_zone_boundary_versions_tenant_id_soil_zon",
                        columns: x => new { x.tenant_id, x.soil_zone_boundary_version_id },
                        principalSchema: "farm",
                        principalTable: "soil_zone_boundary_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "soil_sample_results",
                schema: "farm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    sample_id = table.Column<Guid>(type: "uuid", nullable: false),
                    analyte_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    analyte_name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    value = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    unit = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    method = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    detection_limit = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    result_flag = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: true),
                    recorded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    recorded_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_soil_sample_results", x => x.id);
                    table.ForeignKey(
                        name: "fk_soil_sample_results_soil_samples_tenant_id_sample_id",
                        columns: x => new { x.tenant_id, x.sample_id },
                        principalSchema: "farm",
                        principalTable: "soil_samples",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_assets_tenant_id",
                schema: "farm",
                table: "infrastructure_assets",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_assets_tenant_id_farm_id_code",
                schema: "farm",
                table: "infrastructure_assets",
                columns: new[] { "tenant_id", "farm_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_assets_tenant_id_farm_id_status_type",
                schema: "farm",
                table: "infrastructure_assets",
                columns: new[] { "tenant_id", "farm_id", "status", "type" });

            migrationBuilder.CreateIndex(
                name: "ix_infrastructure_assets_tenant_id_field_id",
                schema: "farm",
                table: "infrastructure_assets",
                columns: new[] { "tenant_id", "field_id" });

            migrationBuilder.CreateIndex(
                name: "ix_land_tenure_records_tenant_id",
                schema: "farm",
                table: "land_tenure_records",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_land_tenure_records_tenant_id_farm_id_field_id_valid_from_v",
                schema: "farm",
                table: "land_tenure_records",
                columns: new[] { "tenant_id", "farm_id", "field_id", "valid_from", "valid_to" });

            migrationBuilder.CreateIndex(
                name: "ix_land_tenure_records_tenant_id_field_id",
                schema: "farm",
                table: "land_tenure_records",
                columns: new[] { "tenant_id", "field_id" });

            migrationBuilder.CreateIndex(
                name: "ix_land_tenure_records_tenant_id_reference",
                schema: "farm",
                table: "land_tenure_records",
                columns: new[] { "tenant_id", "reference" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_soil_sample_results_tenant_id",
                schema: "farm",
                table: "soil_sample_results",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_soil_sample_results_tenant_id_sample_id_analyte_code",
                schema: "farm",
                table: "soil_sample_results",
                columns: new[] { "tenant_id", "sample_id", "analyte_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_soil_samples_tenant_id",
                schema: "farm",
                table: "soil_samples",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_soil_samples_tenant_id_farm_id_sample_code",
                schema: "farm",
                table: "soil_samples",
                columns: new[] { "tenant_id", "farm_id", "sample_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_soil_samples_tenant_id_field_boundary_version_id",
                schema: "farm",
                table: "soil_samples",
                columns: new[] { "tenant_id", "field_boundary_version_id" });

            migrationBuilder.CreateIndex(
                name: "ix_soil_samples_tenant_id_field_id_sampled_at",
                schema: "farm",
                table: "soil_samples",
                columns: new[] { "tenant_id", "field_id", "sampled_at" });

            migrationBuilder.CreateIndex(
                name: "ix_soil_samples_tenant_id_soil_zone_boundary_version_id",
                schema: "farm",
                table: "soil_samples",
                columns: new[] { "tenant_id", "soil_zone_boundary_version_id" });

            migrationBuilder.CreateIndex(
                name: "ix_soil_samples_tenant_id_status_sampled_at",
                schema: "farm",
                table: "soil_samples",
                columns: new[] { "tenant_id", "status", "sampled_at" });

            migrationBuilder.CreateIndex(
                name: "ix_soil_zone_boundary_versions_tenant_id",
                schema: "farm",
                table: "soil_zone_boundary_versions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_soil_zone_boundary_versions_tenant_id_field_boundary_versio",
                schema: "farm",
                table: "soil_zone_boundary_versions",
                columns: new[] { "tenant_id", "field_boundary_version_id" });

            migrationBuilder.CreateIndex(
                name: "ix_soil_zone_boundary_versions_tenant_id_soil_zone_id_valid_fr",
                schema: "farm",
                table: "soil_zone_boundary_versions",
                columns: new[] { "tenant_id", "soil_zone_id", "valid_from" });

            migrationBuilder.CreateIndex(
                name: "ix_soil_zones_tenant_id",
                schema: "farm",
                table: "soil_zones",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_soil_zones_tenant_id_farm_id_status",
                schema: "farm",
                table: "soil_zones",
                columns: new[] { "tenant_id", "farm_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_soil_zones_tenant_id_field_id_code",
                schema: "farm",
                table: "soil_zones",
                columns: new[] { "tenant_id", "field_id", "code" },
                unique: true);

            migrationBuilder.Sql("""
                ALTER TABLE farm.land_tenure_records
                    ADD CONSTRAINT ck_land_tenure_type CHECK (type BETWEEN 1 AND 6),
                    ADD CONSTRAINT ck_land_tenure_status CHECK (status BETWEEN 1 AND 4),
                    ADD CONSTRAINT ck_land_tenure_area CHECK (registered_area_ha IS NULL OR registered_area_ha > 0),
                    ADD CONSTRAINT ck_land_tenure_dates CHECK (valid_to IS NULL OR valid_to >= valid_from),
                    ADD CONSTRAINT ck_land_tenure_lifecycle CHECK (
                        (status = 1 AND valid_to IS NULL AND closed_at IS NULL AND closed_by IS NULL) OR
                        (status = 2 AND valid_to IS NOT NULL) OR
                        (status IN (3,4) AND valid_to IS NOT NULL AND closed_at IS NOT NULL AND closed_by IS NOT NULL));

                ALTER TABLE farm.infrastructure_assets
                    ADD CONSTRAINT ck_infrastructure_type CHECK (type BETWEEN 1 AND 11),
                    ADD CONSTRAINT ck_infrastructure_status CHECK (status BETWEEN 1 AND 4),
                    ADD CONSTRAINT ck_infrastructure_capacity CHECK (
                        (capacity IS NULL AND capacity_unit IS NULL) OR
                        (capacity IS NOT NULL AND capacity > 0 AND capacity_unit IS NOT NULL)),
                    ADD CONSTRAINT ck_infrastructure_details CHECK (jsonb_typeof(details_json) = 'object'),
                    ADD CONSTRAINT ck_infrastructure_decommission CHECK (
                        (status <> 4 AND decommissioned_on IS NULL AND decommissioned_at IS NULL AND decommissioned_by IS NULL AND decommission_reason IS NULL) OR
                        (status = 4 AND decommissioned_on IS NOT NULL AND decommissioned_at IS NOT NULL AND decommissioned_by IS NOT NULL AND decommission_reason IS NOT NULL));

                ALTER TABLE farm.soil_zones
                    ADD CONSTRAINT ck_soil_zone_status CHECK (status BETWEEN 1 AND 2),
                    ADD CONSTRAINT ck_soil_zone_retirement CHECK (
                        (status = 1 AND retired_at IS NULL AND retired_by IS NULL) OR
                        (status = 2 AND retired_at IS NOT NULL AND retired_by IS NOT NULL));

                ALTER TABLE farm.soil_zone_boundary_versions
                    ADD CONSTRAINT ck_soil_zone_boundary_dates CHECK (valid_to IS NULL OR valid_to >= valid_from),
                    ADD CONSTRAINT ck_soil_zone_boundary_geometry CHECK (
                        NOT ST_IsEmpty(boundary::geometry) AND ST_IsValid(boundary::geometry));

                ALTER TABLE farm.soil_samples
                    ADD CONSTRAINT ck_soil_sample_status CHECK (status BETWEEN 1 AND 4),
                    ADD CONSTRAINT ck_soil_sample_depth CHECK (depth_from_cm >= 0 AND depth_to_cm > depth_from_cm),
                    ADD CONSTRAINT ck_soil_sample_location CHECK (NOT ST_IsEmpty(location::geometry)),
                    ADD CONSTRAINT ck_soil_sample_lifecycle CHECK (
                        (status = 1 AND submitted_at IS NULL AND analysed_at IS NULL AND rejected_at IS NULL AND rejection_reason IS NULL) OR
                        (status = 2 AND submitted_at IS NOT NULL AND analysed_at IS NULL AND rejected_at IS NULL AND rejection_reason IS NULL) OR
                        (status = 3 AND submitted_at IS NOT NULL AND analysed_at IS NOT NULL AND rejected_at IS NULL AND rejection_reason IS NULL) OR
                        (status = 4 AND analysed_at IS NULL AND rejected_at IS NOT NULL AND rejection_reason IS NOT NULL));

                ALTER TABLE farm.soil_sample_results
                    ADD CONSTRAINT ck_soil_result_detection_limit CHECK (detection_limit IS NULL OR detection_limit >= 0);

                CREATE UNIQUE INDEX ux_soil_zone_current_boundary
                    ON farm.soil_zone_boundary_versions (tenant_id, soil_zone_id) WHERE valid_to IS NULL;
                CREATE INDEX ix_infrastructure_assets_location_gist
                    ON farm.infrastructure_assets USING gist (location);
                CREATE INDEX ix_soil_zone_boundaries_boundary_gist
                    ON farm.soil_zone_boundary_versions USING gist (boundary);
                CREATE INDEX ix_soil_samples_location_gist
                    ON farm.soil_samples USING gist (location);

                CREATE OR REPLACE FUNCTION farm.validate_land_tenure_scope()
                RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF NEW.field_id IS NOT NULL AND NOT EXISTS (
                        SELECT 1 FROM farm.fields f
                        WHERE f.tenant_id = NEW.tenant_id AND f.id = NEW.field_id AND f.farm_id = NEW.farm_id) THEN
                        RAISE EXCEPTION 'land tenure field must belong to the selected farm' USING ERRCODE = '23514';
                    END IF;

                    PERFORM pg_advisory_xact_lock(hashtextextended(
                        NEW.tenant_id::text || ':' || NEW.farm_id::text || ':' || coalesce(NEW.field_id::text, 'farm'), 0));
                    IF EXISTS (
                        SELECT 1 FROM farm.land_tenure_records t
                        WHERE t.tenant_id = NEW.tenant_id
                          AND t.farm_id = NEW.farm_id
                          AND t.field_id IS NOT DISTINCT FROM NEW.field_id
                          AND t.id <> NEW.id
                          AND daterange(t.valid_from, coalesce(t.valid_to, 'infinity'::date), '[]')
                              && daterange(NEW.valid_from, coalesce(NEW.valid_to, 'infinity'::date), '[]')) THEN
                        RAISE EXCEPTION 'land tenure periods cannot overlap for the same farm or field' USING ERRCODE = '23P01';
                    END IF;
                    RETURN NEW;
                END $$;

                CREATE OR REPLACE FUNCTION farm.validate_infrastructure_scope()
                RETURNS trigger LANGUAGE plpgsql AS $$
                DECLARE field_boundary geography;
                BEGIN
                    IF NEW.field_id IS NULL THEN RETURN NEW; END IF;
                    IF NOT EXISTS (SELECT 1 FROM farm.fields f WHERE f.tenant_id=NEW.tenant_id AND f.id=NEW.field_id AND f.farm_id=NEW.farm_id) THEN
                        RAISE EXCEPTION 'infrastructure field must belong to the selected farm' USING ERRCODE = '23514';
                    END IF;
                    IF NEW.location IS NOT NULL THEN
                        SELECT b.boundary INTO field_boundary FROM farm.field_boundary_versions b
                        WHERE b.tenant_id=NEW.tenant_id AND b.field_id=NEW.field_id AND b.valid_to IS NULL
                        ORDER BY b.valid_from DESC LIMIT 1;
                        IF field_boundary IS NULL OR NOT ST_Covers(field_boundary::geometry, NEW.location::geometry) THEN
                            RAISE EXCEPTION 'field-scoped infrastructure must lie inside the current field boundary' USING ERRCODE = '23514';
                        END IF;
                    END IF;
                    RETURN NEW;
                END $$;

                CREATE OR REPLACE FUNCTION farm.validate_soil_zone_scope()
                RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM farm.fields f WHERE f.tenant_id=NEW.tenant_id AND f.id=NEW.field_id AND f.farm_id=NEW.farm_id) THEN
                        RAISE EXCEPTION 'soil zone field must belong to the selected farm' USING ERRCODE = '23514';
                    END IF;
                    RETURN NEW;
                END $$;

                CREATE OR REPLACE FUNCTION farm.validate_soil_zone_boundary()
                RETURNS trigger LANGUAGE plpgsql AS $$
                DECLARE zone_field uuid; referenced_field uuid; referenced_boundary geography;
                BEGIN
                    SELECT z.field_id INTO zone_field FROM farm.soil_zones z
                    WHERE z.tenant_id=NEW.tenant_id AND z.id=NEW.soil_zone_id;
                    SELECT b.field_id, b.boundary INTO referenced_field, referenced_boundary
                    FROM farm.field_boundary_versions b
                    WHERE b.tenant_id=NEW.tenant_id AND b.id=NEW.field_boundary_version_id;
                    IF zone_field IS NULL OR referenced_field IS DISTINCT FROM zone_field THEN
                        RAISE EXCEPTION 'soil-zone and field-boundary versions must reference the same field' USING ERRCODE = '23514';
                    END IF;
                    IF NOT ST_CoveredBy(NEW.boundary::geometry, referenced_boundary::geometry) THEN
                        RAISE EXCEPTION 'soil-zone boundary must be covered by its field boundary version' USING ERRCODE = '23514';
                    END IF;
                    IF NEW.valid_to IS NULL AND EXISTS (
                        SELECT 1 FROM farm.soil_zone_boundary_versions other
                        JOIN farm.soil_zones other_zone ON other_zone.tenant_id=other.tenant_id AND other_zone.id=other.soil_zone_id
                        WHERE other.tenant_id=NEW.tenant_id AND other.id<>NEW.id AND other.valid_to IS NULL
                          AND other_zone.field_id=zone_field AND other_zone.status=1
                          AND ST_Area(ST_Intersection(other.boundary::geometry, NEW.boundary::geometry)) > 0.000000000001) THEN
                        RAISE EXCEPTION 'active soil-management zones in a field cannot overlap' USING ERRCODE = '23P01';
                    END IF;
                    RETURN NEW;
                END $$;

                CREATE OR REPLACE FUNCTION farm.validate_soil_sample_scope()
                RETURNS trigger LANGUAGE plpgsql AS $$
                DECLARE boundary_field uuid; field_boundary geography; zone_field uuid; zone_boundary geography;
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM farm.fields f WHERE f.tenant_id=NEW.tenant_id AND f.id=NEW.field_id AND f.farm_id=NEW.farm_id) THEN
                        RAISE EXCEPTION 'soil sample field must belong to the selected farm' USING ERRCODE = '23514';
                    END IF;
                    SELECT b.field_id, b.boundary INTO boundary_field, field_boundary FROM farm.field_boundary_versions b
                    WHERE b.tenant_id=NEW.tenant_id AND b.id=NEW.field_boundary_version_id;
                    IF boundary_field IS DISTINCT FROM NEW.field_id OR NOT ST_Covers(field_boundary::geometry, NEW.location::geometry) THEN
                        RAISE EXCEPTION 'soil sample must lie inside its exact field-boundary version' USING ERRCODE = '23514';
                    END IF;
                    IF NEW.soil_zone_boundary_version_id IS NOT NULL THEN
                        SELECT z.field_id, b.boundary INTO zone_field, zone_boundary
                        FROM farm.soil_zone_boundary_versions b
                        JOIN farm.soil_zones z ON z.tenant_id=b.tenant_id AND z.id=b.soil_zone_id
                        WHERE b.tenant_id=NEW.tenant_id AND b.id=NEW.soil_zone_boundary_version_id;
                        IF zone_field IS DISTINCT FROM NEW.field_id OR NOT ST_Covers(zone_boundary::geometry, NEW.location::geometry) THEN
                            RAISE EXCEPTION 'soil sample must lie inside its exact soil-zone boundary version' USING ERRCODE = '23514';
                        END IF;
                    END IF;
                    RETURN NEW;
                END $$;

                CREATE TRIGGER validate_land_tenure_scope BEFORE INSERT OR UPDATE ON farm.land_tenure_records
                    FOR EACH ROW EXECUTE FUNCTION farm.validate_land_tenure_scope();
                CREATE TRIGGER validate_infrastructure_scope BEFORE INSERT OR UPDATE ON farm.infrastructure_assets
                    FOR EACH ROW EXECUTE FUNCTION farm.validate_infrastructure_scope();
                CREATE TRIGGER validate_soil_zone_scope BEFORE INSERT OR UPDATE ON farm.soil_zones
                    FOR EACH ROW EXECUTE FUNCTION farm.validate_soil_zone_scope();
                CREATE TRIGGER validate_soil_zone_boundary BEFORE INSERT OR UPDATE ON farm.soil_zone_boundary_versions
                    FOR EACH ROW EXECUTE FUNCTION farm.validate_soil_zone_boundary();
                CREATE TRIGGER validate_soil_sample_scope BEFORE INSERT OR UPDATE ON farm.soil_samples
                    FOR EACH ROW EXECUTE FUNCTION farm.validate_soil_sample_scope();

                CREATE TRIGGER land_tenure_no_delete BEFORE DELETE ON farm.land_tenure_records
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER infrastructure_no_delete BEFORE DELETE ON farm.infrastructure_assets
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER soil_zone_no_delete BEFORE DELETE ON farm.soil_zones
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER soil_zone_boundary_no_delete BEFORE DELETE ON farm.soil_zone_boundary_versions
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER soil_sample_no_delete BEFORE DELETE ON farm.soil_samples
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER soil_sample_results_immutable BEFORE UPDATE OR DELETE ON farm.soil_sample_results
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();

                DO $rls$
                DECLARE table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY['land_tenure_records','infrastructure_assets','soil_zones',
                        'soil_zone_boundary_versions','soil_samples','soil_sample_results'] LOOP
                        EXECUTE format('ALTER TABLE farm.%I ENABLE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('ALTER TABLE farm.%I FORCE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('CREATE POLICY tenant_isolation ON farm.%I USING (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid) WITH CHECK (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid)', table_name);
                    END LOOP;
                END $rls$;

                DO $grants$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname='murimios_app') THEN
                        GRANT SELECT,INSERT,UPDATE ON farm.land_tenure_records,farm.infrastructure_assets,
                            farm.soil_zones,farm.soil_zone_boundary_versions,farm.soil_samples TO murimios_app;
                        GRANT SELECT,INSERT ON farm.soil_sample_results TO murimios_app;
                        REVOKE DELETE ON farm.land_tenure_records,farm.infrastructure_assets,farm.soil_zones,
                            farm.soil_zone_boundary_versions,farm.soil_samples,farm.soil_sample_results FROM murimios_app;
                        REVOKE UPDATE ON farm.soil_sample_results FROM murimios_app;
                    END IF;
                END $grants$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP FUNCTION IF EXISTS farm.validate_soil_sample_scope();
                DROP FUNCTION IF EXISTS farm.validate_soil_zone_boundary();
                DROP FUNCTION IF EXISTS farm.validate_soil_zone_scope();
                DROP FUNCTION IF EXISTS farm.validate_infrastructure_scope();
                DROP FUNCTION IF EXISTS farm.validate_land_tenure_scope();
                """);

            migrationBuilder.DropTable(
                name: "infrastructure_assets",
                schema: "farm");

            migrationBuilder.DropTable(
                name: "land_tenure_records",
                schema: "farm");

            migrationBuilder.DropTable(
                name: "soil_sample_results",
                schema: "farm");

            migrationBuilder.DropTable(
                name: "soil_samples",
                schema: "farm");

            migrationBuilder.DropTable(
                name: "soil_zone_boundary_versions",
                schema: "farm");

            migrationBuilder.DropTable(
                name: "soil_zones",
                schema: "farm");
        }
    }
}
