using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddGeospatialImportOnboarding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "imports");

            migrationBuilder.CreateTable(
                name: "geospatial_jobs",
                schema: "imports",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    format = table.Column<int>(type: "integer", nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content_type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    byte_size = table.Column<long>(type: "bigint", nullable: false),
                    sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    object_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    parser_version = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    feature_count = table.Column<int>(type: "integer", nullable: false),
                    eligible_feature_count = table.Column<int>(type: "integer", nullable: false),
                    warning_count = table.Column<int>(type: "integer", nullable: false),
                    error_count = table.Column<int>(type: "integer", nullable: false),
                    failure_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    failure_detail = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    validated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    applied_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    applied_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_geospatial_jobs", x => x.id);
                    table.UniqueConstraint("ak_geospatial_jobs_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_geospatial_jobs_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "geospatial_features",
                schema: "imports",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ordinal = table.Column<int>(type: "integer", nullable: false),
                    source_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    boundary = table.Column<MultiPolygon>(type: "geography (multipolygon, 4326)", nullable: false),
                    area_ha = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: false, computedColumnSql: "ST_Area(boundary) / 10000.0", stored: true),
                    properties_json = table.Column<string>(type: "jsonb", nullable: false),
                    issues_json = table.Column<string>(type: "jsonb", nullable: false),
                    is_eligible = table.Column<bool>(type: "boolean", nullable: false),
                    applied_field_id = table.Column<Guid>(type: "uuid", nullable: true),
                    applied_boundary_version_id = table.Column<Guid>(type: "uuid", nullable: true),
                    applied_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_geospatial_features", x => x.id);
                    table.ForeignKey(
                        name: "fk_geospatial_features_field_boundary_versions_tenant_id_appli",
                        columns: x => new { x.tenant_id, x.applied_boundary_version_id },
                        principalSchema: "farm",
                        principalTable: "field_boundary_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_geospatial_features_fields_tenant_id_applied_field_id",
                        columns: x => new { x.tenant_id, x.applied_field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_geospatial_features_geospatial_jobs_tenant_id_job_id",
                        columns: x => new { x.tenant_id, x.job_id },
                        principalSchema: "imports",
                        principalTable: "geospatial_jobs",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_geospatial_features_tenant_id",
                schema: "imports",
                table: "geospatial_features",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_geospatial_features_tenant_id_applied_boundary_version_id",
                schema: "imports",
                table: "geospatial_features",
                columns: new[] { "tenant_id", "applied_boundary_version_id" });

            migrationBuilder.CreateIndex(
                name: "ix_geospatial_features_tenant_id_applied_field_id",
                schema: "imports",
                table: "geospatial_features",
                columns: new[] { "tenant_id", "applied_field_id" });

            migrationBuilder.CreateIndex(
                name: "ix_geospatial_features_tenant_id_job_id_is_eligible",
                schema: "imports",
                table: "geospatial_features",
                columns: new[] { "tenant_id", "job_id", "is_eligible" });

            migrationBuilder.CreateIndex(
                name: "ix_geospatial_features_tenant_id_job_id_ordinal",
                schema: "imports",
                table: "geospatial_features",
                columns: new[] { "tenant_id", "job_id", "ordinal" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_geospatial_jobs_tenant_id",
                schema: "imports",
                table: "geospatial_jobs",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_geospatial_jobs_tenant_id_farm_id_created_at_utc",
                schema: "imports",
                table: "geospatial_jobs",
                columns: new[] { "tenant_id", "farm_id", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_geospatial_jobs_tenant_id_sha256_farm_id",
                schema: "imports",
                table: "geospatial_jobs",
                columns: new[] { "tenant_id", "sha256", "farm_id" });

            migrationBuilder.CreateIndex(
                name: "ix_geospatial_jobs_tenant_id_status_created_at_utc",
                schema: "imports",
                table: "geospatial_jobs",
                columns: new[] { "tenant_id", "status", "created_at_utc" });

            migrationBuilder.Sql(
                """
                ALTER TABLE imports.geospatial_jobs
                  ADD CONSTRAINT ck_geo_jobs_format CHECK (format BETWEEN 1 AND 4),
                  ADD CONSTRAINT ck_geo_jobs_status CHECK (status BETWEEN 1 AND 5),
                  ADD CONSTRAINT ck_geo_jobs_size CHECK (byte_size BETWEEN 1 AND 52428800),
                  ADD CONSTRAINT ck_geo_jobs_sha256 CHECK (sha256 ~ '^[0-9a-f]{64}$'),
                  ADD CONSTRAINT ck_geo_jobs_counts CHECK (feature_count >= 0 AND eligible_feature_count BETWEEN 0 AND feature_count AND warning_count >= 0 AND error_count >= 0),
                  ADD CONSTRAINT ck_geo_jobs_outcome CHECK (
                    (status = 1 AND validated_at IS NULL AND applied_at IS NULL)
                    OR (status IN (2,3) AND validated_at IS NOT NULL AND failure_code IS NULL AND applied_at IS NULL)
                    OR (status = 4 AND validated_at IS NOT NULL AND applied_at IS NOT NULL AND applied_by IS NOT NULL)
                    OR (status = 5 AND validated_at IS NOT NULL AND failure_code IS NOT NULL AND failure_detail IS NOT NULL AND applied_at IS NULL));

                ALTER TABLE imports.geospatial_features
                  ADD CONSTRAINT ck_geo_features_ordinal CHECK (ordinal > 0),
                  ADD CONSTRAINT ck_geo_features_json CHECK (jsonb_typeof(properties_json) = 'object' AND jsonb_typeof(issues_json) = 'array'),
                  ADD CONSTRAINT ck_geo_features_geometry CHECK (ST_SRID(boundary::geometry) = 4326 AND GeometryType(boundary::geometry) = 'MULTIPOLYGON' AND NOT ST_IsEmpty(boundary::geometry)),
                  ADD CONSTRAINT ck_geo_features_application CHECK (
                    (applied_field_id IS NULL AND applied_boundary_version_id IS NULL AND applied_at IS NULL)
                    OR (is_eligible AND applied_field_id IS NOT NULL AND applied_boundary_version_id IS NOT NULL AND applied_at IS NOT NULL));

                CREATE OR REPLACE FUNCTION imports.validate_geospatial_scope() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_TABLE_NAME = 'geospatial_jobs' THEN
                        IF NOT EXISTS (SELECT 1 FROM farm.farms f WHERE f.tenant_id = NEW.tenant_id AND f.id = NEW.farm_id) THEN
                            RAISE EXCEPTION 'Import farm must belong to the current tenant.' USING ERRCODE = '23514';
                        END IF;
                    ELSE
                        IF NOT EXISTS (SELECT 1 FROM imports.geospatial_jobs j WHERE j.tenant_id = NEW.tenant_id AND j.id = NEW.job_id) THEN
                            RAISE EXCEPTION 'Import feature must belong to its job.' USING ERRCODE = '23514';
                        END IF;
                        IF NEW.applied_field_id IS NOT NULL AND NOT EXISTS (
                            SELECT 1 FROM imports.geospatial_jobs j
                            JOIN farm.fields f ON f.tenant_id = j.tenant_id AND f.farm_id = j.farm_id AND f.id = NEW.applied_field_id
                            JOIN farm.field_boundary_versions b ON b.tenant_id = f.tenant_id AND b.field_id = f.id AND b.id = NEW.applied_boundary_version_id
                            WHERE j.tenant_id = NEW.tenant_id AND j.id = NEW.job_id) THEN
                            RAISE EXCEPTION 'Applied field and boundary must belong to the import farm.' USING ERRCODE = '23514';
                        END IF;
                    END IF;
                    RETURN NEW;
                END $$;

                CREATE OR REPLACE FUNCTION imports.guard_geospatial_lifecycle() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_TABLE_NAME = 'geospatial_jobs' THEN
                        IF OLD.status = 4 OR NEW.status NOT IN (OLD.status, 4) OR (NEW.status = 4 AND OLD.status <> 2) THEN
                            RAISE EXCEPTION 'Invalid geospatial import lifecycle transition.' USING ERRCODE = '23514';
                        END IF;
                        IF NEW.farm_id <> OLD.farm_id OR NEW.format <> OLD.format OR NEW.file_name <> OLD.file_name
                           OR NEW.content_type <> OLD.content_type OR NEW.byte_size <> OLD.byte_size OR NEW.sha256 <> OLD.sha256
                           OR NEW.object_key <> OLD.object_key OR NEW.parser_version <> OLD.parser_version
                           OR NEW.feature_count <> OLD.feature_count OR NEW.eligible_feature_count <> OLD.eligible_feature_count
                           OR NEW.warning_count <> OLD.warning_count OR NEW.error_count <> OLD.error_count
                           OR NEW.failure_code IS DISTINCT FROM OLD.failure_code OR NEW.failure_detail IS DISTINCT FROM OLD.failure_detail
                           OR NEW.validated_at IS DISTINCT FROM OLD.validated_at THEN
                            RAISE EXCEPTION 'Validated import provenance and results are immutable.' USING ERRCODE = '23514';
                        END IF;
                    ELSE
                        IF OLD.applied_at IS NOT NULL OR NEW.job_id <> OLD.job_id OR NEW.ordinal <> OLD.ordinal
                           OR NEW.source_name <> OLD.source_name OR NOT ST_Equals(NEW.boundary::geometry, OLD.boundary::geometry)
                           OR NEW.properties_json <> OLD.properties_json OR NEW.issues_json <> OLD.issues_json
                           OR NEW.is_eligible <> OLD.is_eligible THEN
                            RAISE EXCEPTION 'Import preview features are immutable and can only be applied once.' USING ERRCODE = '23514';
                        END IF;
                    END IF;
                    RETURN NEW;
                END $$;

                CREATE TRIGGER geospatial_job_scope BEFORE INSERT ON imports.geospatial_jobs FOR EACH ROW EXECUTE FUNCTION imports.validate_geospatial_scope();
                CREATE TRIGGER geospatial_feature_scope BEFORE INSERT OR UPDATE OF applied_field_id, applied_boundary_version_id, applied_at ON imports.geospatial_features FOR EACH ROW EXECUTE FUNCTION imports.validate_geospatial_scope();
                CREATE TRIGGER geospatial_job_lifecycle BEFORE UPDATE ON imports.geospatial_jobs FOR EACH ROW EXECUTE FUNCTION imports.guard_geospatial_lifecycle();
                CREATE TRIGGER geospatial_feature_lifecycle BEFORE UPDATE ON imports.geospatial_features FOR EACH ROW EXECUTE FUNCTION imports.guard_geospatial_lifecycle();

                ALTER TABLE imports.geospatial_jobs ENABLE ROW LEVEL SECURITY;
                ALTER TABLE imports.geospatial_jobs FORCE ROW LEVEL SECURITY;
                CREATE POLICY geospatial_jobs_tenant ON imports.geospatial_jobs
                    USING (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid)
                    WITH CHECK (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid);
                ALTER TABLE imports.geospatial_features ENABLE ROW LEVEL SECURITY;
                ALTER TABLE imports.geospatial_features FORCE ROW LEVEL SECURITY;
                CREATE POLICY geospatial_features_tenant ON imports.geospatial_features
                    USING (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid)
                    WITH CHECK (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid);

                GRANT USAGE ON SCHEMA imports TO murimios_app;
                GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA imports TO murimios_app;
                ALTER DEFAULT PRIVILEGES IN SCHEMA imports GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO murimios_app;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "geospatial_features",
                schema: "imports");

            migrationBuilder.DropTable(
                name: "geospatial_jobs",
                schema: "imports");

            migrationBuilder.Sql(
                """
                DROP FUNCTION IF EXISTS imports.validate_geospatial_scope();
                DROP FUNCTION IF EXISTS imports.guard_geospatial_lifecycle();
                """);
        }
    }
}
