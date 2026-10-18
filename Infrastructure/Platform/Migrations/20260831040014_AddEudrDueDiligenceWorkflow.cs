using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddEudrDueDiligenceWorkflow : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "eudr");

            migrationBuilder.AddUniqueConstraint(
                name: "ak_harvest_lots_tenant_id_id",
                schema: "production",
                table: "harvest_lots",
                columns: new[] { "tenant_id", "id" });

            migrationBuilder.CreateTable(
                name: "due_diligence_cases",
                schema: "eudr",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    case_number = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    commodity = table.Column<int>(type: "integer", nullable: false),
                    product_description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    hs_code = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    entered_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    entered_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    production_country = table.Column<string>(type: "character(2)", fixedLength: true, maxLength: 2, nullable: false),
                    production_from = table.Column<DateOnly>(type: "date", nullable: false),
                    production_to = table.Column<DateOnly>(type: "date", nullable: false),
                    operator_name = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    operator_registration_number = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    operator_address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    market = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    assessed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ready_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    exported_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    submitted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    submission_reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    cancelled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancellation_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_due_diligence_cases", x => x.id);
                    table.UniqueConstraint("ak_eudr_cases_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_due_diligence_cases_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "plot_evidence",
                schema: "eudr",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    case_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: false),
                    boundary_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    harvest_lot_id = table.Column<Guid>(type: "uuid", nullable: true),
                    plot_reference = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    area_ha = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: false),
                    production_from = table.Column<DateOnly>(type: "date", nullable: false),
                    production_to = table.Column<DateOnly>(type: "date", nullable: false),
                    boundary_geo_json = table.Column<string>(type: "jsonb", nullable: false),
                    boundary_sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    captured_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    captured_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plot_evidence", x => x.id);
                    table.UniqueConstraint("ak_eudr_plot_evidence_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_plot_evidence_due_diligence_cases_tenant_id_case_id",
                        columns: x => new { x.tenant_id, x.case_id },
                        principalSchema: "eudr",
                        principalTable: "due_diligence_cases",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_plot_evidence_field_boundary_versions_tenant_id_boundary_ve",
                        columns: x => new { x.tenant_id, x.boundary_version_id },
                        principalSchema: "farm",
                        principalTable: "field_boundary_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_plot_evidence_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_plot_evidence_harvest_lots_tenant_id_harvest_lot_id",
                        columns: x => new { x.tenant_id, x.harvest_lot_id },
                        principalSchema: "production",
                        principalTable: "harvest_lots",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "risk_decisions",
                schema: "eudr",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    case_id = table.Column<Guid>(type: "uuid", nullable: false),
                    revision = table.Column<int>(type: "integer", nullable: false),
                    conclusion = table.Column<int>(type: "integer", nullable: false),
                    legal_production_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    rationale = table.Column<string>(type: "character varying(8000)", maxLength: 8000, nullable: false),
                    mitigation_json = table.Column<string>(type: "jsonb", nullable: true),
                    legality_evidence_json = table.Column<string>(type: "jsonb", nullable: false),
                    evidence_snapshot_json = table.Column<string>(type: "jsonb", nullable: false),
                    evidence_sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    decided_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    decided_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_risk_decisions", x => x.id);
                    table.ForeignKey(
                        name: "fk_risk_decisions_due_diligence_cases_tenant_id_case_id",
                        columns: x => new { x.tenant_id, x.case_id },
                        principalSchema: "eudr",
                        principalTable: "due_diligence_cases",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "statements",
                schema: "eudr",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    case_id = table.Column<Guid>(type: "uuid", nullable: false),
                    statement_version = table.Column<int>(type: "integer", nullable: false),
                    schema_version = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    payload_json = table.Column<string>(type: "jsonb", nullable: false),
                    payload_sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    assembled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    assembled_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    exported_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    submitted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    submission_reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_statements", x => x.id);
                    table.UniqueConstraint("ak_eudr_statements_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_statements_due_diligence_cases_tenant_id_case_id",
                        columns: x => new { x.tenant_id, x.case_id },
                        principalSchema: "eudr",
                        principalTable: "due_diligence_cases",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "deforestation_screenings",
                schema: "eudr",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    plot_evidence_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    dataset_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    dataset_version = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    observation_from = table.Column<DateOnly>(type: "date", nullable: false),
                    observation_through = table.Column<DateOnly>(type: "date", nullable: false),
                    coverage_percent = table.Column<decimal>(type: "numeric(6,3)", precision: 6, scale: 3, nullable: false),
                    tree_cover_loss_ha = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: false),
                    overlap_ha = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: false),
                    confidence_percent = table.Column<decimal>(type: "numeric(6,3)", precision: 6, scale: 3, nullable: false),
                    result = table.Column<int>(type: "integer", nullable: false),
                    source_reference = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    evidence_attachment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    result_json = table.Column<string>(type: "jsonb", nullable: false),
                    evidence_sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    screened_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    screened_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_deforestation_screenings", x => x.id);
                    table.ForeignKey(
                        name: "fk_deforestation_screenings_evidence_attachments_tenant_id_evi",
                        columns: x => new { x.tenant_id, x.evidence_attachment_id },
                        principalSchema: "evidence",
                        principalTable: "attachments",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_deforestation_screenings_plot_evidence_tenant_id_plot_evide",
                        columns: x => new { x.tenant_id, x.plot_evidence_id },
                        principalSchema: "eudr",
                        principalTable: "plot_evidence",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "statement_exports",
                schema: "eudr",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    statement_id = table.Column<Guid>(type: "uuid", nullable: false),
                    format = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    sha256 = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    exported_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    exported_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_statement_exports", x => x.id);
                    table.ForeignKey(
                        name: "fk_statement_exports_statements_tenant_id_statement_id",
                        columns: x => new { x.tenant_id, x.statement_id },
                        principalSchema: "eudr",
                        principalTable: "statements",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_deforestation_screenings_tenant_id",
                schema: "eudr",
                table: "deforestation_screenings",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_deforestation_screenings_tenant_id_evidence_attachment_id",
                schema: "eudr",
                table: "deforestation_screenings",
                columns: new[] { "tenant_id", "evidence_attachment_id" });

            migrationBuilder.CreateIndex(
                name: "ix_deforestation_screenings_tenant_id_evidence_sha256",
                schema: "eudr",
                table: "deforestation_screenings",
                columns: new[] { "tenant_id", "evidence_sha256" });

            migrationBuilder.CreateIndex(
                name: "ix_deforestation_screenings_tenant_id_plot_evidence_id_screene",
                schema: "eudr",
                table: "deforestation_screenings",
                columns: new[] { "tenant_id", "plot_evidence_id", "screened_at" });

            migrationBuilder.CreateIndex(
                name: "ix_due_diligence_cases_tenant_id",
                schema: "eudr",
                table: "due_diligence_cases",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_due_diligence_cases_tenant_id_case_number",
                schema: "eudr",
                table: "due_diligence_cases",
                columns: new[] { "tenant_id", "case_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_due_diligence_cases_tenant_id_farm_id_status_created_at",
                schema: "eudr",
                table: "due_diligence_cases",
                columns: new[] { "tenant_id", "farm_id", "status", "created_at" });

            migrationBuilder.CreateIndex(
                name: "ix_plot_evidence_tenant_id",
                schema: "eudr",
                table: "plot_evidence",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_plot_evidence_tenant_id_boundary_sha256",
                schema: "eudr",
                table: "plot_evidence",
                columns: new[] { "tenant_id", "boundary_sha256" });

            migrationBuilder.CreateIndex(
                name: "ix_plot_evidence_tenant_id_boundary_version_id",
                schema: "eudr",
                table: "plot_evidence",
                columns: new[] { "tenant_id", "boundary_version_id" });

            migrationBuilder.CreateIndex(
                name: "ix_plot_evidence_tenant_id_case_id_boundary_version_id",
                schema: "eudr",
                table: "plot_evidence",
                columns: new[] { "tenant_id", "case_id", "boundary_version_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_plot_evidence_tenant_id_case_id_plot_reference",
                schema: "eudr",
                table: "plot_evidence",
                columns: new[] { "tenant_id", "case_id", "plot_reference" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_plot_evidence_tenant_id_field_id",
                schema: "eudr",
                table: "plot_evidence",
                columns: new[] { "tenant_id", "field_id" });

            migrationBuilder.CreateIndex(
                name: "ix_plot_evidence_tenant_id_harvest_lot_id",
                schema: "eudr",
                table: "plot_evidence",
                columns: new[] { "tenant_id", "harvest_lot_id" });

            migrationBuilder.CreateIndex(
                name: "ix_risk_decisions_tenant_id",
                schema: "eudr",
                table: "risk_decisions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_risk_decisions_tenant_id_case_id_revision",
                schema: "eudr",
                table: "risk_decisions",
                columns: new[] { "tenant_id", "case_id", "revision" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_statement_exports_tenant_id",
                schema: "eudr",
                table: "statement_exports",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_statement_exports_tenant_id_statement_id_exported_at",
                schema: "eudr",
                table: "statement_exports",
                columns: new[] { "tenant_id", "statement_id", "exported_at" });

            migrationBuilder.CreateIndex(
                name: "ix_statements_tenant_id",
                schema: "eudr",
                table: "statements",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_statements_tenant_id_case_id_statement_version",
                schema: "eudr",
                table: "statements",
                columns: new[] { "tenant_id", "case_id", "statement_version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_statements_tenant_id_payload_sha256",
                schema: "eudr",
                table: "statements",
                columns: new[] { "tenant_id", "payload_sha256" });

            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION eudr.guard_case_lifecycle() RETURNS trigger LANGUAGE plpgsql AS $fn$
                BEGIN
                    IF TG_OP='DELETE' THEN RAISE EXCEPTION 'EUDR cases are retained.' USING ERRCODE='55000'; END IF;
                    IF (to_jsonb(NEW)-ARRAY['status','assessed_at','ready_at','exported_at','submitted_at','submission_reference','cancelled_at','cancellation_reason','updated_at_utc','version'])
                       IS DISTINCT FROM
                       (to_jsonb(OLD)-ARRAY['status','assessed_at','ready_at','exported_at','submitted_at','submission_reference','cancelled_at','cancellation_reason','updated_at_utc','version'])
                    THEN RAISE EXCEPTION 'EUDR case identity and declared product data are immutable.' USING ERRCODE='55000'; END IF;
                    IF NOT ((OLD.status=1 AND NEW.status IN (2,7)) OR (OLD.status=2 AND NEW.status IN (3,7)) OR
                            (OLD.status=3 AND NEW.status IN (4,7)) OR (OLD.status=4 AND NEW.status IN (5,7)) OR
                            (OLD.status=5 AND NEW.status IN (5,6,7)))
                    THEN RAISE EXCEPTION 'Invalid EUDR case lifecycle transition.' USING ERRCODE='55000'; END IF;
                    RETURN NEW;
                END $fn$;

                CREATE OR REPLACE FUNCTION eudr.guard_statement_lifecycle() RETURNS trigger LANGUAGE plpgsql AS $fn$
                BEGIN
                    IF TG_OP='DELETE' THEN RAISE EXCEPTION 'EUDR statements are retained.' USING ERRCODE='55000'; END IF;
                    IF (to_jsonb(NEW)-ARRAY['status','exported_at','submitted_at','submission_reference','updated_at_utc','version'])
                       IS DISTINCT FROM
                       (to_jsonb(OLD)-ARRAY['status','exported_at','submitted_at','submission_reference','updated_at_utc','version'])
                    THEN RAISE EXCEPTION 'Assembled DDS content is immutable.' USING ERRCODE='55000'; END IF;
                    IF NOT ((OLD.status=1 AND NEW.status=2) OR (OLD.status=2 AND NEW.status IN (2,3)))
                    THEN RAISE EXCEPTION 'Invalid DDS lifecycle transition.' USING ERRCODE='55000'; END IF;
                    RETURN NEW;
                END $fn$;

                CREATE TRIGGER eudr_cases_lifecycle BEFORE UPDATE OR DELETE ON eudr.due_diligence_cases FOR EACH ROW EXECUTE FUNCTION eudr.guard_case_lifecycle();
                CREATE TRIGGER eudr_statements_lifecycle BEFORE UPDATE OR DELETE ON eudr.statements FOR EACH ROW EXECUTE FUNCTION eudr.guard_statement_lifecycle();
                CREATE TRIGGER eudr_plot_evidence_immutable BEFORE UPDATE OR DELETE ON eudr.plot_evidence FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER eudr_screenings_immutable BEFORE UPDATE OR DELETE ON eudr.deforestation_screenings FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER eudr_risk_decisions_immutable BEFORE UPDATE OR DELETE ON eudr.risk_decisions FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER eudr_statement_exports_immutable BEFORE UPDATE OR DELETE ON eudr.statement_exports FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();

                DO $rls$ DECLARE item record; BEGIN
                    FOR item IN SELECT * FROM (VALUES
                        ('eudr','due_diligence_cases'),('eudr','plot_evidence'),('eudr','deforestation_screenings'),
                        ('eudr','risk_decisions'),('eudr','statements'),('eudr','statement_exports')) AS t(schema_name,table_name)
                    LOOP
                        EXECUTE format('ALTER TABLE %I.%I ENABLE ROW LEVEL SECURITY',item.schema_name,item.table_name);
                        EXECUTE format('ALTER TABLE %I.%I FORCE ROW LEVEL SECURITY',item.schema_name,item.table_name);
                        EXECUTE format('CREATE POLICY tenant_isolation ON %I.%I USING (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid) WITH CHECK (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid)',item.schema_name,item.table_name);
                    END LOOP;
                END $rls$;

                DO $grant$ BEGIN IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname='murimios_app') THEN
                    GRANT USAGE ON SCHEMA eudr TO murimios_app;
                    GRANT SELECT,INSERT,UPDATE,DELETE ON ALL TABLES IN SCHEMA eudr TO murimios_app;
                END IF; END $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP FUNCTION IF EXISTS eudr.guard_statement_lifecycle() CASCADE;
                DROP FUNCTION IF EXISTS eudr.guard_case_lifecycle() CASCADE;
                """);

            migrationBuilder.DropTable(
                name: "deforestation_screenings",
                schema: "eudr");

            migrationBuilder.DropTable(
                name: "risk_decisions",
                schema: "eudr");

            migrationBuilder.DropTable(
                name: "statement_exports",
                schema: "eudr");

            migrationBuilder.DropTable(
                name: "plot_evidence",
                schema: "eudr");

            migrationBuilder.DropTable(
                name: "statements",
                schema: "eudr");

            migrationBuilder.DropTable(
                name: "due_diligence_cases",
                schema: "eudr");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_harvest_lots_tenant_id_id",
                schema: "production",
                table: "harvest_lots");
        }
    }
}
