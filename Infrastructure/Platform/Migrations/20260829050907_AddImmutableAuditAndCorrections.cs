using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddImmutableAuditAndCorrections : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "audit");

            migrationBuilder.AddColumn<string>(
                name: "correction_reason",
                schema: "livestock",
                table: "measurements",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "superseded_by_id",
                schema: "livestock",
                table: "measurements",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "correction_reason",
                schema: "operations",
                table: "field_operations",
                type: "character varying(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "superseded_by_id",
                schema: "operations",
                table: "field_operations",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "ak_measurements_tenant_id_id",
                schema: "livestock",
                table: "measurements",
                columns: new[] { "tenant_id", "id" });

            migrationBuilder.CreateTable(
                name: "corrections",
                schema: "audit",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_type = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    original_entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    replacement_entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    corrected_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    corrected_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_corrections", x => x.id);
                    table.ForeignKey(
                        name: "fk_corrections_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "events",
                schema: "audit",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    actor_subject = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    membership_id = table.Column<Guid>(type: "uuid", nullable: true),
                    action = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    entity_type = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: true),
                    organisation_node_id = table.Column<Guid>(type: "uuid", nullable: true),
                    changes = table.Column<string>(type: "jsonb", nullable: false),
                    correlation_id = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_events", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "ix_measurements_tenant_id_superseded_by_id",
                schema: "livestock",
                table: "measurements",
                columns: new[] { "tenant_id", "superseded_by_id" });

            migrationBuilder.CreateIndex(
                name: "ix_field_operations_tenant_id_superseded_by_id",
                schema: "operations",
                table: "field_operations",
                columns: new[] { "tenant_id", "superseded_by_id" });

            migrationBuilder.CreateIndex(
                name: "ix_corrections_tenant_id",
                schema: "audit",
                table: "corrections",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_corrections_tenant_id_entity_type_original_entity_id",
                schema: "audit",
                table: "corrections",
                columns: new[] { "tenant_id", "entity_type", "original_entity_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_corrections_tenant_id_entity_type_replacement_entity_id",
                schema: "audit",
                table: "corrections",
                columns: new[] { "tenant_id", "entity_type", "replacement_entity_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_corrections_tenant_id_farm_id_corrected_at",
                schema: "audit",
                table: "corrections",
                columns: new[] { "tenant_id", "farm_id", "corrected_at" });

            migrationBuilder.CreateIndex(
                name: "ix_events_tenant_id_entity_type_entity_id_occurred_at",
                schema: "audit",
                table: "events",
                columns: new[] { "tenant_id", "entity_type", "entity_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_events_tenant_id_farm_id_occurred_at",
                schema: "audit",
                table: "events",
                columns: new[] { "tenant_id", "farm_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_events_tenant_id_occurred_at",
                schema: "audit",
                table: "events",
                columns: new[] { "tenant_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_events_tenant_id_organisation_node_id_occurred_at",
                schema: "audit",
                table: "events",
                columns: new[] { "tenant_id", "organisation_node_id", "occurred_at" });

            migrationBuilder.AddForeignKey(
                name: "fk_field_operations_field_operations_tenant_id_superseded_by_id",
                schema: "operations",
                table: "field_operations",
                columns: new[] { "tenant_id", "superseded_by_id" },
                principalSchema: "operations",
                principalTable: "field_operations",
                principalColumns: new[] { "tenant_id", "id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_measurements_measurements_tenant_id_superseded_by_id",
                schema: "livestock",
                table: "measurements",
                columns: new[] { "tenant_id", "superseded_by_id" },
                principalSchema: "livestock",
                principalTable: "measurements",
                principalColumns: new[] { "tenant_id", "id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql(
                """
                ALTER TABLE audit.events ENABLE ROW LEVEL SECURITY;
                ALTER TABLE audit.events FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON audit.events
                    USING (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid)
                    WITH CHECK (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid);

                ALTER TABLE audit.corrections ENABLE ROW LEVEL SECURITY;
                ALTER TABLE audit.corrections FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON audit.corrections
                    USING (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid)
                    WITH CHECK (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid);

                CREATE OR REPLACE FUNCTION audit.reject_record_mutation()
                RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    RAISE EXCEPTION '% is append-only; % is forbidden', TG_TABLE_NAME, TG_OP
                        USING ERRCODE = '55000';
                END;
                $$;

                CREATE TRIGGER audit_events_immutable
                    BEFORE UPDATE OR DELETE ON audit.events
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER audit_corrections_immutable
                    BEFORE UPDATE OR DELETE ON audit.corrections
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER report_exports_immutable
                    BEFORE UPDATE OR DELETE ON reporting.exports
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();

                CREATE OR REPLACE FUNCTION audit.allow_supersession_only()
                RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_OP = 'DELETE' THEN
                        RAISE EXCEPTION '% records cannot be deleted; create a correction', TG_TABLE_NAME
                            USING ERRCODE = '55000';
                    END IF;
                    IF OLD.superseded_by_id IS NOT NULL
                       OR NEW.superseded_by_id IS NULL
                       OR NEW.superseded_by_id = OLD.id
                       OR nullif(btrim(NEW.correction_reason), '') IS NULL
                       OR (to_jsonb(NEW) - ARRAY['superseded_by_id', 'correction_reason', 'updated_at_utc', 'version'])
                          IS DISTINCT FROM
                          (to_jsonb(OLD) - ARRAY['superseded_by_id', 'correction_reason', 'updated_at_utc', 'version']) THEN
                        RAISE EXCEPTION '% records may only be updated once to record supersession', TG_TABLE_NAME
                            USING ERRCODE = '55000';
                    END IF;
                    RETURN NEW;
                END;
                $$;

                CREATE TRIGGER field_operations_supersession_only
                    BEFORE UPDATE OR DELETE ON operations.field_operations
                    FOR EACH ROW EXECUTE FUNCTION audit.allow_supersession_only();
                CREATE TRIGGER livestock_measurements_supersession_only
                    BEFORE UPDATE OR DELETE ON livestock.measurements
                    FOR EACH ROW EXECUTE FUNCTION audit.allow_supersession_only();
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP TRIGGER IF EXISTS field_operations_supersession_only ON operations.field_operations;
                DROP TRIGGER IF EXISTS livestock_measurements_supersession_only ON livestock.measurements;
                DROP TRIGGER IF EXISTS report_exports_immutable ON reporting.exports;
                DROP TRIGGER IF EXISTS audit_corrections_immutable ON audit.corrections;
                DROP TRIGGER IF EXISTS audit_events_immutable ON audit.events;
                DROP FUNCTION IF EXISTS audit.allow_supersession_only();
                DROP FUNCTION IF EXISTS audit.reject_record_mutation();
                """);

            migrationBuilder.DropForeignKey(
                name: "fk_field_operations_field_operations_tenant_id_superseded_by_id",
                schema: "operations",
                table: "field_operations");

            migrationBuilder.DropForeignKey(
                name: "fk_measurements_measurements_tenant_id_superseded_by_id",
                schema: "livestock",
                table: "measurements");

            migrationBuilder.DropTable(
                name: "corrections",
                schema: "audit");

            migrationBuilder.DropTable(
                name: "events",
                schema: "audit");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_measurements_tenant_id_id",
                schema: "livestock",
                table: "measurements");

            migrationBuilder.DropIndex(
                name: "ix_measurements_tenant_id_superseded_by_id",
                schema: "livestock",
                table: "measurements");

            migrationBuilder.DropIndex(
                name: "ix_field_operations_tenant_id_superseded_by_id",
                schema: "operations",
                table: "field_operations");

            migrationBuilder.DropColumn(
                name: "correction_reason",
                schema: "livestock",
                table: "measurements");

            migrationBuilder.DropColumn(
                name: "superseded_by_id",
                schema: "livestock",
                table: "measurements");

            migrationBuilder.DropColumn(
                name: "correction_reason",
                schema: "operations",
                table: "field_operations");

            migrationBuilder.DropColumn(
                name: "superseded_by_id",
                schema: "operations",
                table: "field_operations");
        }
    }
}
