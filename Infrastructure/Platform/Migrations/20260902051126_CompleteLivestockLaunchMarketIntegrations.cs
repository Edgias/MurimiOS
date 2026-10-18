using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class CompleteLivestockLaunchMarketIntegrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "document_media_type",
                schema: "livestock",
                table: "statutory_movement_documents",
                type: "character varying(120)",
                maxLength: 120,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "regulatory_profile_id",
                schema: "livestock",
                table: "statutory_movement_documents",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "rendered_document",
                schema: "livestock",
                table: "statutory_movement_documents",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "regulatory_profiles",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_key = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    profile_version = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    exchange_type = table.Column<int>(type: "integer", nullable: false),
                    jurisdiction = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    species = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    provider_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    schema_version = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    media_type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    reporting_deadline_hours = table.Column<int>(type: "integer", nullable: true),
                    register_deadline_hours = table.Column<int>(type: "integer", nullable: true),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_to = table.Column<DateOnly>(type: "date", nullable: true),
                    source_uri = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    is_built_in = table.Column<bool>(type: "boolean", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_regulatory_profiles", x => x.id);
                    table.UniqueConstraint("ak_livestock_regulatory_profiles_tenant_id_id", x => new { x.tenant_id, x.id });
                });

            migrationBuilder.CreateTable(
                name: "exchange_submissions",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    profile_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_record_id = table.Column<Guid>(type: "uuid", nullable: false),
                    exchange_type = table.Column<int>(type: "integer", nullable: false),
                    provider_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    schema_version = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    media_type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    payload_json = table.Column<string>(type: "jsonb", nullable: false),
                    payload_sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    reporting_due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    first_submitted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    decided_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    external_reference = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    rejection_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    rejection_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exchange_submissions", x => x.id);
                    table.UniqueConstraint("ak_livestock_exchange_submissions_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_exchange_submissions_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_exchange_submissions_regulatory_profiles_tenant_id_profile_",
                        columns: x => new { x.tenant_id, x.profile_id },
                        principalSchema: "livestock",
                        principalTable: "regulatory_profiles",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_exchange_submissions_subjects_tenant_id_subject_id",
                        columns: x => new { x.tenant_id, x.subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "exchange_attempts",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    submission_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attempt_number = table.Column<int>(type: "integer", nullable: false),
                    outcome = table.Column<int>(type: "integer", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    request_sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    http_status = table.Column<int>(type: "integer", nullable: true),
                    external_reference = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    response_json = table.Column<string>(type: "jsonb", nullable: false),
                    error_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_exchange_attempts", x => x.id);
                    table.ForeignKey(
                        name: "fk_exchange_attempts_exchange_submissions_tenant_id_submission",
                        columns: x => new { x.tenant_id, x.submission_id },
                        principalSchema: "livestock",
                        principalTable: "exchange_submissions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_statutory_movement_documents_tenant_id_regulatory_profile_id",
                schema: "livestock",
                table: "statutory_movement_documents",
                columns: new[] { "tenant_id", "regulatory_profile_id" });

            migrationBuilder.CreateIndex(
                name: "ix_exchange_attempts_tenant_id",
                schema: "livestock",
                table: "exchange_attempts",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_exchange_attempts_tenant_id_submission_id_attempt_number",
                schema: "livestock",
                table: "exchange_attempts",
                columns: new[] { "tenant_id", "submission_id", "attempt_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_exchange_submissions_tenant_id",
                schema: "livestock",
                table: "exchange_submissions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_exchange_submissions_tenant_id_exchange_type_source_record_",
                schema: "livestock",
                table: "exchange_submissions",
                columns: new[] { "tenant_id", "exchange_type", "source_record_id", "provider_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_exchange_submissions_tenant_id_farm_id",
                schema: "livestock",
                table: "exchange_submissions",
                columns: new[] { "tenant_id", "farm_id" });

            migrationBuilder.CreateIndex(
                name: "ix_exchange_submissions_tenant_id_profile_id",
                schema: "livestock",
                table: "exchange_submissions",
                columns: new[] { "tenant_id", "profile_id" });

            migrationBuilder.CreateIndex(
                name: "ix_exchange_submissions_tenant_id_status_reporting_due_at",
                schema: "livestock",
                table: "exchange_submissions",
                columns: new[] { "tenant_id", "status", "reporting_due_at" });

            migrationBuilder.CreateIndex(
                name: "ix_exchange_submissions_tenant_id_subject_id",
                schema: "livestock",
                table: "exchange_submissions",
                columns: new[] { "tenant_id", "subject_id" });

            migrationBuilder.CreateIndex(
                name: "ix_regulatory_profiles_tenant_id",
                schema: "livestock",
                table: "regulatory_profiles",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_regulatory_profiles_tenant_id_exchange_type_jurisdiction_sp",
                schema: "livestock",
                table: "regulatory_profiles",
                columns: new[] { "tenant_id", "exchange_type", "jurisdiction", "species", "effective_from" });

            migrationBuilder.CreateIndex(
                name: "ix_regulatory_profiles_tenant_id_profile_key_profile_version",
                schema: "livestock",
                table: "regulatory_profiles",
                columns: new[] { "tenant_id", "profile_key", "profile_version" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_statutory_movement_documents_regulatory_profiles_tenant_id_",
                schema: "livestock",
                table: "statutory_movement_documents",
                columns: new[] { "tenant_id", "regulatory_profile_id" },
                principalSchema: "livestock",
                principalTable: "regulatory_profiles",
                principalColumns: new[] { "tenant_id", "id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql("""
                UPDATE livestock.statutory_movement_documents
                SET document_media_type = 'application/json', rendered_document = snapshot_json::text
                WHERE document_media_type = '' OR rendered_document = '';
                ALTER TABLE livestock.statutory_movement_documents
                    ALTER COLUMN document_media_type DROP DEFAULT,
                    ALTER COLUMN rendered_document DROP DEFAULT;

                ALTER TABLE livestock.regulatory_profiles
                    ADD CONSTRAINT ck_livestock_regulatory_profile_version CHECK (profile_version > 0),
                    ADD CONSTRAINT ck_livestock_regulatory_profile_exchange CHECK (exchange_type IN (1,2,3)),
                    ADD CONSTRAINT ck_livestock_regulatory_profile_effective CHECK (effective_to IS NULL OR effective_to >= effective_from),
                    ADD CONSTRAINT ck_livestock_regulatory_profile_deadlines CHECK (
                        (exchange_type = 1 AND reporting_deadline_hours > 0 AND register_deadline_hours > 0)
                        OR (exchange_type IN (2,3) AND (reporting_deadline_hours IS NULL OR reporting_deadline_hours > 0)
                            AND register_deadline_hours IS NULL));

                ALTER TABLE livestock.exchange_submissions
                    ADD CONSTRAINT ck_livestock_exchange_type CHECK (exchange_type IN (2,3)),
                    ADD CONSTRAINT ck_livestock_exchange_hash CHECK (payload_sha256 ~ '^[0-9a-f]{64}$'),
                    ADD CONSTRAINT ck_livestock_exchange_status CHECK (status IN (1,2,3,4)),
                    ADD CONSTRAINT ck_livestock_exchange_lifecycle CHECK (
                        (status = 1 AND first_submitted_at IS NULL AND decided_at IS NULL AND external_reference IS NULL AND rejection_code IS NULL)
                        OR (status = 2 AND first_submitted_at IS NOT NULL AND decided_at IS NULL AND external_reference IS NOT NULL AND rejection_code IS NULL)
                        OR (status = 3 AND first_submitted_at IS NOT NULL AND decided_at >= first_submitted_at AND external_reference IS NOT NULL AND rejection_code IS NULL)
                        OR (status = 4 AND first_submitted_at IS NOT NULL AND decided_at >= first_submitted_at AND rejection_code IS NOT NULL));

                ALTER TABLE livestock.exchange_attempts
                    ADD CONSTRAINT ck_livestock_exchange_attempt_outcome CHECK (outcome IN (1,2,3,4)),
                    ADD CONSTRAINT ck_livestock_exchange_attempt_time CHECK (attempt_number > 0 AND completed_at >= started_at),
                    ADD CONSTRAINT ck_livestock_exchange_attempt_http CHECK (http_status IS NULL OR http_status BETWEEN 100 AND 599),
                    ADD CONSTRAINT ck_livestock_exchange_attempt_hash CHECK (request_sha256 ~ '^[0-9a-f]{64}$');

                DO $rls$
                DECLARE table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY['regulatory_profiles','exchange_submissions','exchange_attempts'] LOOP
                        EXECUTE format('ALTER TABLE livestock.%I ENABLE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('ALTER TABLE livestock.%I FORCE ROW LEVEL SECURITY', table_name);
                        EXECUTE format(
                            'CREATE POLICY tenant_isolation ON livestock.%I USING (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid) WITH CHECK (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid)',
                            table_name);
                    END LOOP;
                END
                $rls$;

                CREATE TRIGGER livestock_regulatory_profiles_immutable
                    BEFORE UPDATE OR DELETE ON livestock.regulatory_profiles
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER livestock_exchange_attempts_immutable
                    BEFORE UPDATE OR DELETE ON livestock.exchange_attempts
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();

                CREATE OR REPLACE FUNCTION livestock.guard_exchange_submission()
                RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_OP = 'DELETE'
                       OR OLD.status IN (3,4)
                       OR (OLD.status = 1 AND NEW.status NOT IN (2,3,4))
                       OR (OLD.status = 2 AND NEW.status NOT IN (2,3,4))
                       OR (to_jsonb(NEW) - ARRAY['status','first_submitted_at','decided_at','external_reference','rejection_code','rejection_message','updated_at_utc','version'])
                          IS DISTINCT FROM
                          (to_jsonb(OLD) - ARRAY['status','first_submitted_at','decided_at','external_reference','rejection_code','rejection_message','updated_at_utc','version']) THEN
                        RAISE EXCEPTION 'livestock exchange snapshots are immutable and final decisions cannot change' USING ERRCODE = '55000';
                    END IF;
                    RETURN NEW;
                END;
                $$;
                CREATE TRIGGER livestock_exchange_submission_guard
                    BEFORE UPDATE OR DELETE ON livestock.exchange_submissions
                    FOR EACH ROW EXECUTE FUNCTION livestock.guard_exchange_submission();

                DO $grant$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        GRANT SELECT, INSERT ON livestock.regulatory_profiles, livestock.exchange_attempts TO murimios_app;
                        GRANT SELECT, INSERT, UPDATE ON livestock.exchange_submissions TO murimios_app;
                        REVOKE UPDATE, DELETE ON livestock.regulatory_profiles, livestock.exchange_attempts FROM murimios_app;
                        REVOKE DELETE ON livestock.exchange_submissions FROM murimios_app;
                    END IF;
                END
                $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_statutory_movement_documents_regulatory_profiles_tenant_id_",
                schema: "livestock",
                table: "statutory_movement_documents");

            migrationBuilder.DropTable(
                name: "exchange_attempts",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "exchange_submissions",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "regulatory_profiles",
                schema: "livestock");

            migrationBuilder.Sql("DROP FUNCTION IF EXISTS livestock.guard_exchange_submission();");

            migrationBuilder.DropIndex(
                name: "ix_statutory_movement_documents_tenant_id_regulatory_profile_id",
                schema: "livestock",
                table: "statutory_movement_documents");

            migrationBuilder.DropColumn(
                name: "document_media_type",
                schema: "livestock",
                table: "statutory_movement_documents");

            migrationBuilder.DropColumn(
                name: "regulatory_profile_id",
                schema: "livestock",
                table: "statutory_movement_documents");

            migrationBuilder.DropColumn(
                name: "rendered_document",
                schema: "livestock",
                table: "statutory_movement_documents");
        }
    }
}
