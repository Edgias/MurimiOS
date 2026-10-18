using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddSupplierEvidenceReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "ix_supplier_evidence_requests_tenant_id_control_point_id",
                schema: "supply",
                table: "supplier_evidence_requests",
                columns: new[] { "tenant_id", "control_point_id" });

            migrationBuilder.CreateIndex(
                name: "ix_supplier_evidence_requests_tenant_id_evidence_attachment_id",
                schema: "supply",
                table: "supplier_evidence_requests",
                columns: new[] { "tenant_id", "evidence_attachment_id" });

            migrationBuilder.CreateIndex(
                name: "ix_supplier_evidence_requests_tenant_id_standard_version_id",
                schema: "supply",
                table: "supplier_evidence_requests",
                columns: new[] { "tenant_id", "standard_version_id" });

            migrationBuilder.AddForeignKey(
                name: "fk_supplier_evidence_requests_compliance_control_points_tenant",
                schema: "supply",
                table: "supplier_evidence_requests",
                columns: new[] { "tenant_id", "control_point_id" },
                principalSchema: "compliance",
                principalTable: "control_points",
                principalColumns: new[] { "tenant_id", "id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_supplier_evidence_requests_compliance_standard_versions_ten",
                schema: "supply",
                table: "supplier_evidence_requests",
                columns: new[] { "tenant_id", "standard_version_id" },
                principalSchema: "compliance",
                principalTable: "standard_versions",
                principalColumns: new[] { "tenant_id", "id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_supplier_evidence_requests_evidence_attachments_tenant_id_e",
                schema: "supply",
                table: "supplier_evidence_requests",
                columns: new[] { "tenant_id", "evidence_attachment_id" },
                principalSchema: "evidence",
                principalTable: "attachments",
                principalColumns: new[] { "tenant_id", "id" },
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_supplier_evidence_requests_compliance_control_points_tenant",
                schema: "supply",
                table: "supplier_evidence_requests");

            migrationBuilder.DropForeignKey(
                name: "fk_supplier_evidence_requests_compliance_standard_versions_ten",
                schema: "supply",
                table: "supplier_evidence_requests");

            migrationBuilder.DropForeignKey(
                name: "fk_supplier_evidence_requests_evidence_attachments_tenant_id_e",
                schema: "supply",
                table: "supplier_evidence_requests");

            migrationBuilder.DropIndex(
                name: "ix_supplier_evidence_requests_tenant_id_control_point_id",
                schema: "supply",
                table: "supplier_evidence_requests");

            migrationBuilder.DropIndex(
                name: "ix_supplier_evidence_requests_tenant_id_evidence_attachment_id",
                schema: "supply",
                table: "supplier_evidence_requests");

            migrationBuilder.DropIndex(
                name: "ix_supplier_evidence_requests_tenant_id_standard_version_id",
                schema: "supply",
                table: "supplier_evidence_requests");
        }
    }
}
