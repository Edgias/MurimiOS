using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddReportExportAudit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "reporting");

            migrationBuilder.CreateTable(
                name: "exports",
                schema: "reporting",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    crop_zone_id = table.Column<Guid>(type: "uuid", nullable: false),
                    report_type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    format = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    generated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    requested_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exports", x => x.id);
                    table.ForeignKey(
                        name: "fk_exports_crop_zones_tenant_id_crop_zone_id",
                        columns: x => new { x.tenant_id, x.crop_zone_id },
                        principalSchema: "production",
                        principalTable: "crop_zones",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_exports_tenant_id",
                schema: "reporting",
                table: "exports",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_exports_tenant_id_crop_zone_id_generated_at",
                schema: "reporting",
                table: "exports",
                columns: new[] { "tenant_id", "crop_zone_id", "generated_at" });

            migrationBuilder.CreateIndex(
                name: "ix_exports_tenant_id_sha256",
                schema: "reporting",
                table: "exports",
                columns: new[] { "tenant_id", "sha256" });

            migrationBuilder.Sql(
                """
                ALTER TABLE reporting.exports ENABLE ROW LEVEL SECURITY;
                ALTER TABLE reporting.exports FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON reporting.exports
                    USING (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid)
                    WITH CHECK (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "exports",
                schema: "reporting");
        }
    }
}
