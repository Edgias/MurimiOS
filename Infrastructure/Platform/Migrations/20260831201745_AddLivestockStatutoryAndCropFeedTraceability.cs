using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddLivestockStatutoryAndCropFeedTraceability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddUniqueConstraint(
                name: "ak_movements_tenant_id_id",
                schema: "livestock",
                table: "movements",
                columns: new[] { "tenant_id", "id" });

            migrationBuilder.CreateTable(
                name: "crop_feed_transfers",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_type = table.Column<int>(type: "integer", nullable: false),
                    source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feed_batch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    trace_event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    entered_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    entered_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    transferred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    transferred_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_crop_feed_transfers", x => x.id);
                    table.ForeignKey(
                        name: "fk_crop_feed_transfers_events_tenant_id_trace_event_id",
                        columns: x => new { x.tenant_id, x.trace_event_id },
                        principalSchema: "traceability",
                        principalTable: "events",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_crop_feed_transfers_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_crop_feed_transfers_feed_batches_tenant_id_feed_batch_id",
                        columns: x => new { x.tenant_id, x.feed_batch_id },
                        principalSchema: "livestock",
                        principalTable: "feed_batches",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "statutory_movement_documents",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    movement_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    jurisdiction = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    provider_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    schema_version = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    document_number = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    snapshot_json = table.Column<string>(type: "jsonb", nullable: false),
                    payload_sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    reporting_due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    register_due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    submitted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
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
                    table.PrimaryKey("pk_statutory_movement_documents", x => x.id);
                    table.UniqueConstraint("ak_statutory_movement_documents_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_statutory_movement_documents_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_statutory_movement_documents_movements_tenant_id_movement_id",
                        columns: x => new { x.tenant_id, x.movement_id },
                        principalSchema: "livestock",
                        principalTable: "movements",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_statutory_movement_documents_subjects_tenant_id_subject_id",
                        columns: x => new { x.tenant_id, x.subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "statutory_submission_attempts",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    document_id = table.Column<Guid>(type: "uuid", nullable: false),
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
                    table.PrimaryKey("pk_statutory_submission_attempts", x => x.id);
                    table.ForeignKey(
                        name: "fk_statutory_submission_attempts_statutory_movement_documents_",
                        columns: x => new { x.tenant_id, x.document_id },
                        principalSchema: "livestock",
                        principalTable: "statutory_movement_documents",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_crop_feed_transfers_tenant_id",
                schema: "livestock",
                table: "crop_feed_transfers",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_crop_feed_transfers_tenant_id_farm_id",
                schema: "livestock",
                table: "crop_feed_transfers",
                columns: new[] { "tenant_id", "farm_id" });

            migrationBuilder.CreateIndex(
                name: "ix_crop_feed_transfers_tenant_id_feed_batch_id",
                schema: "livestock",
                table: "crop_feed_transfers",
                columns: new[] { "tenant_id", "feed_batch_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_crop_feed_transfers_tenant_id_source_type_source_id_transfe",
                schema: "livestock",
                table: "crop_feed_transfers",
                columns: new[] { "tenant_id", "source_type", "source_id", "transferred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_crop_feed_transfers_tenant_id_trace_event_id",
                schema: "livestock",
                table: "crop_feed_transfers",
                columns: new[] { "tenant_id", "trace_event_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_statutory_movement_documents_tenant_id",
                schema: "livestock",
                table: "statutory_movement_documents",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_statutory_movement_documents_tenant_id_farm_id",
                schema: "livestock",
                table: "statutory_movement_documents",
                columns: new[] { "tenant_id", "farm_id" });

            migrationBuilder.CreateIndex(
                name: "ix_statutory_movement_documents_tenant_id_movement_id_provider",
                schema: "livestock",
                table: "statutory_movement_documents",
                columns: new[] { "tenant_id", "movement_id", "provider_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_statutory_movement_documents_tenant_id_status_reporting_due",
                schema: "livestock",
                table: "statutory_movement_documents",
                columns: new[] { "tenant_id", "status", "reporting_due_at" });

            migrationBuilder.CreateIndex(
                name: "ix_statutory_movement_documents_tenant_id_subject_id",
                schema: "livestock",
                table: "statutory_movement_documents",
                columns: new[] { "tenant_id", "subject_id" });

            migrationBuilder.CreateIndex(
                name: "ix_statutory_submission_attempts_tenant_id",
                schema: "livestock",
                table: "statutory_submission_attempts",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_statutory_submission_attempts_tenant_id_document_id_attempt",
                schema: "livestock",
                table: "statutory_submission_attempts",
                columns: new[] { "tenant_id", "document_id", "attempt_number" },
                unique: true);

            migrationBuilder.Sql(
                """
                ALTER TABLE livestock.crop_feed_transfers
                    ADD CONSTRAINT ck_crop_feed_transfers_source_type CHECK (source_type IN (1, 2)),
                    ADD CONSTRAINT ck_crop_feed_transfers_quantity CHECK (quantity > 0 AND entered_quantity > 0);
                ALTER TABLE livestock.statutory_movement_documents
                    ADD CONSTRAINT ck_statutory_movement_documents_status CHECK (status IN (1, 2, 3)),
                    ADD CONSTRAINT ck_statutory_movement_documents_hash CHECK (payload_sha256 ~ '^[0-9a-f]{64}$'),
                    ADD CONSTRAINT ck_statutory_movement_documents_decision CHECK (
                        (status = 1 AND submitted_at IS NULL AND decided_at IS NULL AND external_reference IS NULL AND rejection_code IS NULL)
                        OR (status = 2 AND submitted_at IS NOT NULL AND decided_at >= submitted_at AND external_reference IS NOT NULL AND rejection_code IS NULL)
                        OR (status = 3 AND submitted_at IS NOT NULL AND decided_at >= submitted_at AND external_reference IS NULL AND rejection_code IS NOT NULL));
                ALTER TABLE livestock.statutory_submission_attempts
                    ADD CONSTRAINT ck_statutory_submission_attempts_outcome CHECK (outcome IN (1, 2, 3)),
                    ADD CONSTRAINT ck_statutory_submission_attempts_time CHECK (attempt_number > 0 AND completed_at >= started_at),
                    ADD CONSTRAINT ck_statutory_submission_attempts_http CHECK (http_status IS NULL OR http_status BETWEEN 100 AND 599),
                    ADD CONSTRAINT ck_statutory_submission_attempts_hash CHECK (request_sha256 ~ '^[0-9a-f]{64}$');

                DO $rls$
                DECLARE table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY['crop_feed_transfers', 'statutory_movement_documents', 'statutory_submission_attempts'] LOOP
                        EXECUTE format('ALTER TABLE livestock.%I ENABLE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('ALTER TABLE livestock.%I FORCE ROW LEVEL SECURITY', table_name);
                        EXECUTE format(
                            'CREATE POLICY tenant_isolation ON livestock.%I USING (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid) WITH CHECK (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid)',
                            table_name);
                    END LOOP;
                END
                $rls$;

                CREATE TRIGGER livestock_crop_feed_transfers_immutable
                    BEFORE UPDATE OR DELETE ON livestock.crop_feed_transfers
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER livestock_statutory_submission_attempts_immutable
                    BEFORE UPDATE OR DELETE ON livestock.statutory_submission_attempts
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();

                CREATE OR REPLACE FUNCTION livestock.guard_statutory_document()
                RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_OP = 'DELETE'
                       OR OLD.status <> 1
                       OR NEW.status NOT IN (2, 3)
                       OR (to_jsonb(NEW) - ARRAY['status','submitted_at','decided_at','external_reference','rejection_code','rejection_message','updated_at_utc','version'])
                          IS DISTINCT FROM
                          (to_jsonb(OLD) - ARRAY['status','submitted_at','decided_at','external_reference','rejection_code','rejection_message','updated_at_utc','version']) THEN
                        RAISE EXCEPTION 'statutory movement document snapshots are immutable and may only be finalized once' USING ERRCODE = '55000';
                    END IF;
                    RETURN NEW;
                END;
                $$;
                CREATE TRIGGER livestock_statutory_document_guard
                    BEFORE UPDATE OR DELETE ON livestock.statutory_movement_documents
                    FOR EACH ROW EXECUTE FUNCTION livestock.guard_statutory_document();

                DO $grant$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        GRANT SELECT, INSERT ON livestock.crop_feed_transfers, livestock.statutory_submission_attempts TO murimios_app;
                        GRANT SELECT, INSERT, UPDATE ON livestock.statutory_movement_documents TO murimios_app;
                    END IF;
                END
                $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "crop_feed_transfers",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "statutory_submission_attempts",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "statutory_movement_documents",
                schema: "livestock");

            migrationBuilder.Sql("DROP FUNCTION IF EXISTS livestock.guard_statutory_document();");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_movements_tenant_id_id",
                schema: "livestock",
                table: "movements");
        }
    }
}
