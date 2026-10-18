using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddComplianceConformancePolicies : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "conformance_policy_json",
                schema: "compliance",
                table: "standard_versions",
                type: "jsonb",
                nullable: false,
                defaultValueSql: "'{\"minimumScorePercent\":80,\"criticalControlsMustConform\":true}'::jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "conformance_policy_json",
                schema: "compliance",
                table: "standard_versions");
        }
    }
}
