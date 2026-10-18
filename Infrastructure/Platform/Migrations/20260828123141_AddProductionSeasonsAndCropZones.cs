using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddProductionSeasonsAndCropZones : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "production");

            migrationBuilder.AddColumn<Guid>(
                name: "crop_zone_id",
                schema: "operations",
                table: "field_operations",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "seasons",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    starts_on = table.Column<DateOnly>(type: "date", nullable: false),
                    ends_on = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_seasons", x => x.id);
                    table.UniqueConstraint("ak_seasons_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_seasons_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "crop_zones",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    season_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: false),
                    boundary_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    crop = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    variety = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    boundary = table.Column<MultiPolygon>(type: "geography (multipolygon, 4326)", nullable: false),
                    area_ha = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: false, computedColumnSql: "ST_Area(boundary) / 10000.0", stored: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_crop_zones", x => x.id);
                    table.UniqueConstraint("ak_crop_zones_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_crop_zones_field_boundary_versions_tenant_id_boundary_versi",
                        columns: x => new { x.tenant_id, x.boundary_version_id },
                        principalSchema: "farm",
                        principalTable: "field_boundary_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_crop_zones_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_crop_zones_seasons_tenant_id_season_id",
                        columns: x => new { x.tenant_id, x.season_id },
                        principalSchema: "production",
                        principalTable: "seasons",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_field_operations_tenant_id_crop_zone_id",
                schema: "operations",
                table: "field_operations",
                columns: new[] { "tenant_id", "crop_zone_id" });

            migrationBuilder.CreateIndex(
                name: "ix_crop_zones_tenant_id",
                schema: "production",
                table: "crop_zones",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_crop_zones_tenant_id_boundary_version_id",
                schema: "production",
                table: "crop_zones",
                columns: new[] { "tenant_id", "boundary_version_id" });

            migrationBuilder.CreateIndex(
                name: "ix_crop_zones_tenant_id_field_id_season_id",
                schema: "production",
                table: "crop_zones",
                columns: new[] { "tenant_id", "field_id", "season_id" });

            migrationBuilder.CreateIndex(
                name: "ix_crop_zones_tenant_id_season_id_name",
                schema: "production",
                table: "crop_zones",
                columns: new[] { "tenant_id", "season_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_seasons_tenant_id",
                schema: "production",
                table: "seasons",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_seasons_tenant_id_farm_id_name",
                schema: "production",
                table: "seasons",
                columns: new[] { "tenant_id", "farm_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_seasons_tenant_id_farm_id_starts_on_ends_on",
                schema: "production",
                table: "seasons",
                columns: new[] { "tenant_id", "farm_id", "starts_on", "ends_on" });

            migrationBuilder.Sql(
                "CREATE INDEX ix_crop_zones_boundary_gist " +
                "ON production.crop_zones USING gist (boundary);");

            migrationBuilder.Sql(
                """
                DO $rls$
                DECLARE
                    table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY[
                        'production.seasons',
                        'production.crop_zones'
                    ]
                    LOOP
                        EXECUTE format('ALTER TABLE %s ENABLE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('ALTER TABLE %s FORCE ROW LEVEL SECURITY', table_name);
                        EXECUTE format(
                            'CREATE POLICY tenant_isolation ON %s USING (' ||
                            'tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid' ||
                            ') WITH CHECK (' ||
                            'tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid' ||
                            ')',
                            table_name);
                    END LOOP;
                END
                $rls$;
                """);

            migrationBuilder.AddForeignKey(
                name: "fk_field_operations_crop_zones_tenant_id_crop_zone_id",
                schema: "operations",
                table: "field_operations",
                columns: new[] { "tenant_id", "crop_zone_id" },
                principalSchema: "production",
                principalTable: "crop_zones",
                principalColumns: new[] { "tenant_id", "id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_field_operations_crop_zones_tenant_id_crop_zone_id",
                schema: "operations",
                table: "field_operations");

            migrationBuilder.DropTable(
                name: "crop_zones",
                schema: "production");

            migrationBuilder.DropTable(
                name: "seasons",
                schema: "production");

            migrationBuilder.DropIndex(
                name: "ix_field_operations_tenant_id_crop_zone_id",
                schema: "operations",
                table: "field_operations");

            migrationBuilder.DropColumn(
                name: "crop_zone_id",
                schema: "operations",
                table: "field_operations");
        }
    }
}
