using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddCropTraceabilityCore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "traceability");

            migrationBuilder.CreateTable(
                name: "deliveries",
                schema: "traceability",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    delivery_number = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    recipient = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    destination = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    dispatched_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    received_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    transaction_reference = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_deliveries", x => x.id);
                    table.UniqueConstraint("ak_deliveries_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_deliveries_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "events",
                schema: "traceability",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_type = table.Column<int>(type: "integer", nullable: false),
                    business_step = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    disposition = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    recorded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    location_type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    location_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source_type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_reference = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    payload_json = table.Column<string>(type: "jsonb", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_events1", x => x.id);
                    table.UniqueConstraint("ak_events_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_events_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "quality_assessments",
                schema: "traceability",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    object_type = table.Column<int>(type: "integer", nullable: false),
                    object_id = table.Column<Guid>(type: "uuid", nullable: false),
                    metric = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    numeric_result = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    text_result = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    passed = table.Column<bool>(type: "boolean", nullable: false),
                    assessed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_quality_assessments", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "recalls",
                schema: "traceability",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    reference = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    initiated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recalls", x => x.id);
                    table.UniqueConstraint("ak_recalls_tenant_id_id", x => new { x.tenant_id, x.id });
                });

            migrationBuilder.CreateTable(
                name: "storage_lots",
                schema: "traceability",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lot_code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    crop = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    variety = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    entered_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    entered_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    display_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    packed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    storage_location = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_storage_lots", x => x.id);
                    table.ForeignKey(
                        name: "fk_storage_lots_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "delivery_lines",
                schema: "traceability",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    delivery_id = table.Column<Guid>(type: "uuid", nullable: false),
                    source_type = table.Column<int>(type: "integer", nullable: false),
                    source_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    entered_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    entered_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_delivery_lines", x => x.id);
                    table.ForeignKey(
                        name: "fk_delivery_lines_deliveries_tenant_id_delivery_id",
                        columns: x => new { x.tenant_id, x.delivery_id },
                        principalSchema: "traceability",
                        principalTable: "deliveries",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "event_objects",
                schema: "traceability",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    object_type = table.Column<int>(type: "integer", nullable: false),
                    object_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    entered_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    entered_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_event_objects", x => x.id);
                    table.ForeignKey(
                        name: "fk_event_objects_events_tenant_id_event_id",
                        columns: x => new { x.tenant_id, x.event_id },
                        principalSchema: "traceability",
                        principalTable: "events",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "links",
                schema: "traceability",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_type = table.Column<int>(type: "integer", nullable: false),
                    from_id = table.Column<Guid>(type: "uuid", nullable: false),
                    to_type = table.Column<int>(type: "integer", nullable: false),
                    to_id = table.Column<Guid>(type: "uuid", nullable: false),
                    relationship = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_links", x => x.id);
                    table.ForeignKey(
                        name: "fk_links_events_tenant_id_event_id",
                        columns: x => new { x.tenant_id, x.event_id },
                        principalSchema: "traceability",
                        principalTable: "events",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "recall_sources",
                schema: "traceability",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    recall_case_id = table.Column<Guid>(type: "uuid", nullable: false),
                    object_type = table.Column<int>(type: "integer", nullable: false),
                    object_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recall_sources", x => x.id);
                    table.ForeignKey(
                        name: "fk_recall_sources_recalls_tenant_id_recall_case_id",
                        columns: x => new { x.tenant_id, x.recall_case_id },
                        principalSchema: "traceability",
                        principalTable: "recalls",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_deliveries_tenant_id",
                schema: "traceability",
                table: "deliveries",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_deliveries_tenant_id_delivery_number",
                schema: "traceability",
                table: "deliveries",
                columns: new[] { "tenant_id", "delivery_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_deliveries_tenant_id_farm_id_dispatched_at",
                schema: "traceability",
                table: "deliveries",
                columns: new[] { "tenant_id", "farm_id", "dispatched_at" });

            migrationBuilder.CreateIndex(
                name: "ix_delivery_lines_tenant_id",
                schema: "traceability",
                table: "delivery_lines",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_delivery_lines_tenant_id_delivery_id_source_type_source_id",
                schema: "traceability",
                table: "delivery_lines",
                columns: new[] { "tenant_id", "delivery_id", "source_type", "source_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_delivery_lines_tenant_id_source_type_source_id",
                schema: "traceability",
                table: "delivery_lines",
                columns: new[] { "tenant_id", "source_type", "source_id" });

            migrationBuilder.CreateIndex(
                name: "ix_event_objects_tenant_id",
                schema: "traceability",
                table: "event_objects",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_event_objects_tenant_id_event_id_object_type_object_id_role",
                schema: "traceability",
                table: "event_objects",
                columns: new[] { "tenant_id", "event_id", "object_type", "object_id", "role" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_event_objects_tenant_id_object_type_object_id",
                schema: "traceability",
                table: "event_objects",
                columns: new[] { "tenant_id", "object_type", "object_id" });

            migrationBuilder.CreateIndex(
                name: "ix_events_tenant_id",
                schema: "traceability",
                table: "events",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_events_tenant_id_farm_id_occurred_at1",
                schema: "traceability",
                table: "events",
                columns: new[] { "tenant_id", "farm_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_events_tenant_id_source_type_source_id",
                schema: "traceability",
                table: "events",
                columns: new[] { "tenant_id", "source_type", "source_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_links_tenant_id",
                schema: "traceability",
                table: "links",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_links_tenant_id_event_id_from_type_from_id_to_type_to_id",
                schema: "traceability",
                table: "links",
                columns: new[] { "tenant_id", "event_id", "from_type", "from_id", "to_type", "to_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_links_tenant_id_from_type_from_id",
                schema: "traceability",
                table: "links",
                columns: new[] { "tenant_id", "from_type", "from_id" });

            migrationBuilder.CreateIndex(
                name: "ix_links_tenant_id_to_type_to_id",
                schema: "traceability",
                table: "links",
                columns: new[] { "tenant_id", "to_type", "to_id" });

            migrationBuilder.CreateIndex(
                name: "ix_quality_assessments_tenant_id",
                schema: "traceability",
                table: "quality_assessments",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_quality_assessments_tenant_id_object_type_object_id_assesse",
                schema: "traceability",
                table: "quality_assessments",
                columns: new[] { "tenant_id", "object_type", "object_id", "assessed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_recall_sources_tenant_id",
                schema: "traceability",
                table: "recall_sources",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_recall_sources_tenant_id_recall_case_id_object_type_object_",
                schema: "traceability",
                table: "recall_sources",
                columns: new[] { "tenant_id", "recall_case_id", "object_type", "object_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_recalls_tenant_id",
                schema: "traceability",
                table: "recalls",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_recalls_tenant_id_reference",
                schema: "traceability",
                table: "recalls",
                columns: new[] { "tenant_id", "reference" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_recalls_tenant_id_status_initiated_at",
                schema: "traceability",
                table: "recalls",
                columns: new[] { "tenant_id", "status", "initiated_at" });

            migrationBuilder.CreateIndex(
                name: "ix_storage_lots_tenant_id",
                schema: "traceability",
                table: "storage_lots",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_storage_lots_tenant_id_farm_id_packed_at",
                schema: "traceability",
                table: "storage_lots",
                columns: new[] { "tenant_id", "farm_id", "packed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_storage_lots_tenant_id_lot_code",
                schema: "traceability",
                table: "storage_lots",
                columns: new[] { "tenant_id", "lot_code" },
                unique: true);

            migrationBuilder.Sql(
                """
                DO $grant$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        GRANT USAGE ON SCHEMA traceability TO murimios_app;
                        GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA traceability TO murimios_app;
                    END IF;
                END
                $grant$;

                DO $rls$
                DECLARE table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY[
                        'deliveries', 'delivery_lines', 'events', 'event_objects', 'links',
                        'quality_assessments', 'recalls', 'recall_sources', 'storage_lots'
                    ] LOOP
                        EXECUTE format('ALTER TABLE traceability.%I ENABLE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('ALTER TABLE traceability.%I FORCE ROW LEVEL SECURITY', table_name);
                        EXECUTE format(
                            'CREATE POLICY tenant_isolation ON traceability.%I USING (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid) WITH CHECK (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid)',
                            table_name);
                    END LOOP;
                END
                $rls$;

                CREATE TRIGGER trace_events_immutable
                    BEFORE UPDATE OR DELETE ON traceability.events
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER trace_event_objects_immutable
                    BEFORE UPDATE OR DELETE ON traceability.event_objects
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER trace_links_immutable
                    BEFORE UPDATE OR DELETE ON traceability.links
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER trace_storage_lots_immutable
                    BEFORE UPDATE OR DELETE ON traceability.storage_lots
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER trace_delivery_lines_immutable
                    BEFORE UPDATE OR DELETE ON traceability.delivery_lines
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER trace_recall_sources_immutable
                    BEFORE UPDATE OR DELETE ON traceability.recall_sources
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER trace_quality_assessments_immutable
                    BEFORE UPDATE OR DELETE ON traceability.quality_assessments
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "delivery_lines",
                schema: "traceability");

            migrationBuilder.DropTable(
                name: "event_objects",
                schema: "traceability");

            migrationBuilder.DropTable(
                name: "links",
                schema: "traceability");

            migrationBuilder.DropTable(
                name: "quality_assessments",
                schema: "traceability");

            migrationBuilder.DropTable(
                name: "recall_sources",
                schema: "traceability");

            migrationBuilder.DropTable(
                name: "storage_lots",
                schema: "traceability");

            migrationBuilder.DropTable(
                name: "deliveries",
                schema: "traceability");

            migrationBuilder.DropTable(
                name: "events",
                schema: "traceability");

            migrationBuilder.DropTable(
                name: "recalls",
                schema: "traceability");
        }
    }
}
