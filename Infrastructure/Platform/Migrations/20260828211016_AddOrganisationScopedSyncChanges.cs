using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddOrganisationScopedSyncChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "farm_id",
                schema: "sync",
                table: "change_log",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "ix_change_log_tenant_id_farm_id_id",
                schema: "sync",
                table: "change_log",
                columns: new[] { "tenant_id", "farm_id", "id" });

            migrationBuilder.AddForeignKey(
                name: "fk_change_log_farms_tenant_id_farm_id",
                schema: "sync",
                table: "change_log",
                columns: new[] { "tenant_id", "farm_id" },
                principalSchema: "farm",
                principalTable: "farms",
                principalColumns: new[] { "tenant_id", "id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_change_log_farms_tenant_id_farm_id",
                schema: "sync",
                table: "change_log");

            migrationBuilder.DropIndex(
                name: "ix_change_log_tenant_id_farm_id_id",
                schema: "sync",
                table: "change_log");

            migrationBuilder.DropColumn(
                name: "farm_id",
                schema: "sync",
                table: "change_log");
        }
    }
}
