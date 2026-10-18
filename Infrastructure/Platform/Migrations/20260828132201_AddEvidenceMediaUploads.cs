using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddEvidenceMediaUploads : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "evidence");

            migrationBuilder.CreateTable(
                name: "attachments",
                schema: "evidence",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    crop_zone_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content_type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    byte_size = table.Column<long>(type: "bigint", nullable: false),
                    sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    object_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    captured_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: true),
                    longitude = table.Column<double>(type: "double precision", nullable: true),
                    accuracy_metres = table.Column<double>(type: "double precision", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_attachments", x => x.id);
                    table.ForeignKey(
                        name: "fk_attachments_crop_zones_tenant_id_crop_zone_id",
                        columns: x => new { x.tenant_id, x.crop_zone_id },
                        principalSchema: "production",
                        principalTable: "crop_zones",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "upload_sessions",
                schema: "evidence",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    evidence_id = table.Column<Guid>(type: "uuid", nullable: false),
                    crop_zone_id = table.Column<Guid>(type: "uuid", nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    content_type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    expected_bytes = table.Column<long>(type: "bigint", nullable: false),
                    expected_sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    received_bytes = table.Column<long>(type: "bigint", nullable: false),
                    captured_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    latitude = table.Column<double>(type: "double precision", nullable: true),
                    longitude = table.Column<double>(type: "double precision", nullable: true),
                    accuracy_metres = table.Column<double>(type: "double precision", nullable: true),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_upload_sessions", x => x.id);
                    table.ForeignKey(
                        name: "fk_upload_sessions_crop_zones_tenant_id_crop_zone_id",
                        columns: x => new { x.tenant_id, x.crop_zone_id },
                        principalSchema: "production",
                        principalTable: "crop_zones",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_attachments_tenant_id",
                schema: "evidence",
                table: "attachments",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_attachments_tenant_id_crop_zone_id_captured_at",
                schema: "evidence",
                table: "attachments",
                columns: new[] { "tenant_id", "crop_zone_id", "captured_at" });

            migrationBuilder.CreateIndex(
                name: "ix_attachments_tenant_id_sha256",
                schema: "evidence",
                table: "attachments",
                columns: new[] { "tenant_id", "sha256" });

            migrationBuilder.CreateIndex(
                name: "ix_upload_sessions_tenant_id",
                schema: "evidence",
                table: "upload_sessions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_upload_sessions_tenant_id_crop_zone_id",
                schema: "evidence",
                table: "upload_sessions",
                columns: new[] { "tenant_id", "crop_zone_id" });

            migrationBuilder.CreateIndex(
                name: "ix_upload_sessions_tenant_id_evidence_id",
                schema: "evidence",
                table: "upload_sessions",
                columns: new[] { "tenant_id", "evidence_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_upload_sessions_tenant_id_status_expires_at",
                schema: "evidence",
                table: "upload_sessions",
                columns: new[] { "tenant_id", "status", "expires_at" });

            migrationBuilder.Sql(
                """
                DO $rls$
                DECLARE
                    table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY[
                        'evidence.attachments',
                        'evidence.upload_sessions'
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "attachments",
                schema: "evidence");

            migrationBuilder.DropTable(
                name: "upload_sessions",
                schema: "evidence");
        }
    }
}
