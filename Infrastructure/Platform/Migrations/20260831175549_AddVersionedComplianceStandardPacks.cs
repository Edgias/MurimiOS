using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddVersionedComplianceStandardPacks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "manifest_sha256",
                schema: "compliance",
                table: "standard_versions",
                type: "character(64)",
                fixedLength: true,
                maxLength: 64,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "pack_key",
                schema: "compliance",
                table: "standard_versions",
                type: "character varying(120)",
                maxLength: 120,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "source_uri",
                schema: "compliance",
                table: "standard_versions",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_standard_versions_tenant_id_pack_key_manifest_sha256",
                schema: "compliance",
                table: "standard_versions",
                columns: new[] { "tenant_id", "pack_key", "manifest_sha256" },
                unique: true,
                filter: "pack_key IS NOT NULL AND manifest_sha256 IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "ix_standard_versions_tenant_id_pack_key_manifest_sha256",
                schema: "compliance",
                table: "standard_versions");

            migrationBuilder.DropColumn(
                name: "manifest_sha256",
                schema: "compliance",
                table: "standard_versions");

            migrationBuilder.DropColumn(
                name: "pack_key",
                schema: "compliance",
                table: "standard_versions");

            migrationBuilder.DropColumn(
                name: "source_uri",
                schema: "compliance",
                table: "standard_versions");
        }
    }
}
