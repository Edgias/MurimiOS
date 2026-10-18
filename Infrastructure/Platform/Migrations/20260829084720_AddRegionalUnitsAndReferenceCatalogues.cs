using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddRegionalUnitsAndReferenceCatalogues : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "configuration");

            migrationBuilder.CreateTable(
                name: "reference_catalogues",
                schema: "configuration",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    catalogue_version = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    country_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: true),
                    authority = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_to = table.Column<DateOnly>(type: "date", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reference_catalogues", x => x.id);
                    table.UniqueConstraint("ak_reference_catalogues_tenant_id_id", x => new { x.tenant_id, x.id });
                });

            migrationBuilder.CreateTable(
                name: "tenant_regional_settings",
                schema: "configuration",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    country_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    currency_code = table.Column<string>(type: "character varying(3)", maxLength: 3, nullable: false),
                    locale = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    time_zone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    crop_year_start_month = table.Column<int>(type: "integer", nullable: false),
                    area_unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    mass_unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    volume_unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    temperature_unit = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_regional_settings", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "reference_catalogue_items",
                schema: "configuration",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    catalogue_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    attributes_json = table.Column<string>(type: "jsonb", nullable: false),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_to = table.Column<DateOnly>(type: "date", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_reference_catalogue_items", x => x.id);
                    table.ForeignKey(
                        name: "fk_reference_catalogue_items_reference_catalogues_tenant_id_ca",
                        columns: x => new { x.tenant_id, x.catalogue_id },
                        principalSchema: "configuration",
                        principalTable: "reference_catalogues",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_reference_catalogue_items_tenant_id",
                schema: "configuration",
                table: "reference_catalogue_items",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_reference_catalogue_items_tenant_id_catalogue_id_code",
                schema: "configuration",
                table: "reference_catalogue_items",
                columns: new[] { "tenant_id", "catalogue_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_reference_catalogue_items_tenant_id_catalogue_id_effective_",
                schema: "configuration",
                table: "reference_catalogue_items",
                columns: new[] { "tenant_id", "catalogue_id", "effective_from", "effective_to" });

            migrationBuilder.CreateIndex(
                name: "ix_reference_catalogues_tenant_id",
                schema: "configuration",
                table: "reference_catalogues",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_reference_catalogues_tenant_id_country_code_effective_from_",
                schema: "configuration",
                table: "reference_catalogues",
                columns: new[] { "tenant_id", "country_code", "effective_from", "effective_to" });

            migrationBuilder.CreateIndex(
                name: "ix_reference_catalogues_tenant_id_key_catalogue_version",
                schema: "configuration",
                table: "reference_catalogues",
                columns: new[] { "tenant_id", "key", "catalogue_version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tenant_regional_settings_tenant_id",
                schema: "configuration",
                table: "tenant_regional_settings",
                column: "tenant_id",
                unique: true);

            migrationBuilder.Sql("""
                ALTER TABLE configuration.tenant_regional_settings ENABLE ROW LEVEL SECURITY;
                ALTER TABLE configuration.tenant_regional_settings FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON configuration.tenant_regional_settings
                    USING (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid)
                    WITH CHECK (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid);

                ALTER TABLE configuration.reference_catalogues ENABLE ROW LEVEL SECURITY;
                ALTER TABLE configuration.reference_catalogues FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON configuration.reference_catalogues
                    USING (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid)
                    WITH CHECK (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid);

                ALTER TABLE configuration.reference_catalogue_items ENABLE ROW LEVEL SECURITY;
                ALTER TABLE configuration.reference_catalogue_items FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON configuration.reference_catalogue_items
                    USING (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid)
                    WITH CHECK (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid);

                DO $grant$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        GRANT USAGE ON SCHEMA configuration TO murimios_app;
                        GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA configuration TO murimios_app;
                    END IF;
                END
                $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "reference_catalogue_items",
                schema: "configuration");

            migrationBuilder.DropTable(
                name: "tenant_regional_settings",
                schema: "configuration");

            migrationBuilder.DropTable(
                name: "reference_catalogues",
                schema: "configuration");
        }
    }
}
