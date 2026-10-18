using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddComplianceAuditPackExports : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "audit_pack_exports",
                schema: "compliance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    assessment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    standard_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    revision = table.Column<int>(type: "integer", nullable: false),
                    content_schema_version = table.Column<int>(type: "integer", nullable: false),
                    scope_statement = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    includes_evidence_files = table.Column<bool>(type: "boolean", nullable: false),
                    format = table.Column<int>(type: "integer", nullable: false),
                    object_key = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    byte_size = table.Column<long>(type: "bigint", nullable: false),
                    content_entry_count = table.Column<int>(type: "integer", nullable: false),
                    evidence_file_count = table.Column<int>(type: "integer", nullable: false),
                    artifact_sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    manifest_sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    signature_algorithm = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    signature_key_id = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    signature_base64 = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    manifest_json = table.Column<string>(type: "text", nullable: false),
                    generated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    generated_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_audit_pack_exports", x => x.id);
                    table.ForeignKey(
                        name: "fk_audit_pack_exports_assessments_tenant_id_assessment_id",
                        columns: x => new { x.tenant_id, x.assessment_id },
                        principalSchema: "compliance",
                        principalTable: "assessments",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_audit_pack_exports_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_audit_pack_exports_standard_versions_tenant_id_standard_ver",
                        columns: x => new { x.tenant_id, x.standard_version_id },
                        principalSchema: "compliance",
                        principalTable: "standard_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_audit_pack_exports_tenant_id",
                schema: "compliance",
                table: "audit_pack_exports",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_audit_pack_exports_tenant_id_assessment_id_revision",
                schema: "compliance",
                table: "audit_pack_exports",
                columns: new[] { "tenant_id", "assessment_id", "revision" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_audit_pack_exports_tenant_id_farm_id_generated_at",
                schema: "compliance",
                table: "audit_pack_exports",
                columns: new[] { "tenant_id", "farm_id", "generated_at" });

            migrationBuilder.CreateIndex(
                name: "ix_audit_pack_exports_tenant_id_standard_version_id",
                schema: "compliance",
                table: "audit_pack_exports",
                columns: new[] { "tenant_id", "standard_version_id" });

            migrationBuilder.Sql("""
                ALTER TABLE compliance.audit_pack_exports
                    ADD CONSTRAINT ck_audit_pack_exports_versions
                        CHECK (revision > 0 AND content_schema_version > 0),
                    ADD CONSTRAINT ck_audit_pack_exports_format CHECK (format = 1),
                    ADD CONSTRAINT ck_audit_pack_exports_content
                        CHECK (byte_size > 0 AND content_entry_count > 0
                            AND evidence_file_count >= 0 AND evidence_file_count <= content_entry_count),
                    ADD CONSTRAINT ck_audit_pack_exports_hashes CHECK (
                        artifact_sha256 ~ '^[0-9a-f]{64}$' AND manifest_sha256 ~ '^[0-9a-f]{64}$'),
                    ADD CONSTRAINT ck_audit_pack_exports_signature
                        CHECK (signature_algorithm = 'HMAC-SHA256' AND length(signature_key_id) > 0
                            AND length(signature_base64) > 0),
                    ADD CONSTRAINT ck_audit_pack_exports_manifest
                        CHECK (jsonb_typeof(manifest_json::jsonb) = 'object');

                ALTER TABLE compliance.audit_pack_exports ENABLE ROW LEVEL SECURITY;
                ALTER TABLE compliance.audit_pack_exports FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON compliance.audit_pack_exports
                    USING (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid)
                    WITH CHECK (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid);
                CREATE TRIGGER audit_pack_exports_immutable
                    BEFORE UPDATE OR DELETE ON compliance.audit_pack_exports
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();

                DO $grant$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        GRANT SELECT, INSERT ON compliance.audit_pack_exports TO murimios_app;
                        REVOKE UPDATE, DELETE ON compliance.audit_pack_exports FROM murimios_app;
                    END IF;
                END $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP TRIGGER IF EXISTS audit_pack_exports_immutable ON compliance.audit_pack_exports;");
            migrationBuilder.DropTable(
                name: "audit_pack_exports",
                schema: "compliance");
        }
    }
}
