using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddEpcisInteroperability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "epcis_exchanges",
                schema: "traceability",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    direction = table.Column<int>(type: "integer", nullable: false),
                    document_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    document_id = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    schema_version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    cbv_version = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    event_count = table.Column<int>(type: "integer", nullable: false),
                    first_event_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_event_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    sender = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    receiver = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    payload_json = table.Column<string>(type: "jsonb", nullable: false),
                    exchanged_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    exchanged_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_epcis_exchanges", x => x.id);
                    table.UniqueConstraint("ak_epcis_exchanges_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_epcis_exchanges_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "epcis_captured_events",
                schema: "traceability",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    exchange_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_id = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    event_type = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    event_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    record_time = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    business_step = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    disposition = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    object_identifiers_json = table.Column<string>(type: "jsonb", nullable: false),
                    sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    payload_json = table.Column<string>(type: "jsonb", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_epcis_captured_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_epcis_captured_events_epcis_exchanges_tenant_id_exchange_id",
                        columns: x => new { x.tenant_id, x.exchange_id },
                        principalSchema: "traceability",
                        principalTable: "epcis_exchanges",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_epcis_captured_events_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_epcis_captured_events_tenant_id",
                schema: "traceability",
                table: "epcis_captured_events",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_epcis_captured_events_tenant_id_event_id",
                schema: "traceability",
                table: "epcis_captured_events",
                columns: new[] { "tenant_id", "event_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_epcis_captured_events_tenant_id_exchange_id",
                schema: "traceability",
                table: "epcis_captured_events",
                columns: new[] { "tenant_id", "exchange_id" });

            migrationBuilder.CreateIndex(
                name: "ix_epcis_captured_events_tenant_id_farm_id_event_time",
                schema: "traceability",
                table: "epcis_captured_events",
                columns: new[] { "tenant_id", "farm_id", "event_time" });

            migrationBuilder.CreateIndex(
                name: "ix_epcis_exchanges_tenant_id",
                schema: "traceability",
                table: "epcis_exchanges",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_epcis_exchanges_tenant_id_direction_sha256",
                schema: "traceability",
                table: "epcis_exchanges",
                columns: new[] { "tenant_id", "direction", "sha256" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_epcis_exchanges_tenant_id_document_id",
                schema: "traceability",
                table: "epcis_exchanges",
                columns: new[] { "tenant_id", "document_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_epcis_exchanges_tenant_id_farm_id_exchanged_at",
                schema: "traceability",
                table: "epcis_exchanges",
                columns: new[] { "tenant_id", "farm_id", "exchanged_at" });

            migrationBuilder.Sql("""
                ALTER TABLE traceability.epcis_exchanges
                    ADD CONSTRAINT ck_epcis_exchange_direction CHECK (direction IN (1,2)),
                    ADD CONSTRAINT ck_epcis_exchange_document_type CHECK (
                        (direction = 1 AND document_type = 'EPCISDocument') OR
                        (direction = 2 AND document_type = 'EPCISQueryDocument')),
                    ADD CONSTRAINT ck_epcis_exchange_versions CHECK (schema_version = '2.0' AND cbv_version = '2.0.0'),
                    ADD CONSTRAINT ck_epcis_exchange_hash CHECK (sha256 ~ '^[0-9a-f]{64}$'),
                    ADD CONSTRAINT ck_epcis_exchange_payload CHECK (jsonb_typeof(payload_json) = 'object'),
                    ADD CONSTRAINT ck_epcis_exchange_event_range CHECK (
                        (event_count = 0 AND first_event_at IS NULL AND last_event_at IS NULL) OR
                        (event_count > 0 AND first_event_at IS NOT NULL AND last_event_at IS NOT NULL
                         AND first_event_at <= last_event_at)),
                    ADD CONSTRAINT ck_epcis_capture_has_events CHECK (direction <> 1 OR event_count > 0);

                ALTER TABLE traceability.epcis_captured_events
                    ADD CONSTRAINT ck_epcis_captured_event_type CHECK (event_type IN (
                        'ObjectEvent','AggregationEvent','AssociationEvent','TransformationEvent','TransactionEvent')),
                    ADD CONSTRAINT ck_epcis_captured_event_hash CHECK (sha256 ~ '^[0-9a-f]{64}$'),
                    ADD CONSTRAINT ck_epcis_captured_event_objects CHECK (jsonb_typeof(object_identifiers_json) = 'array'),
                    ADD CONSTRAINT ck_epcis_captured_event_payload CHECK (jsonb_typeof(payload_json) = 'object');

                CREATE TRIGGER epcis_exchanges_immutable
                    BEFORE UPDATE OR DELETE ON traceability.epcis_exchanges
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER epcis_captured_events_immutable
                    BEFORE UPDATE OR DELETE ON traceability.epcis_captured_events
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();

                ALTER TABLE traceability.epcis_exchanges ENABLE ROW LEVEL SECURITY;
                ALTER TABLE traceability.epcis_exchanges FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON traceability.epcis_exchanges
                    USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid)
                    WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);
                ALTER TABLE traceability.epcis_captured_events ENABLE ROW LEVEL SECURITY;
                ALTER TABLE traceability.epcis_captured_events FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON traceability.epcis_captured_events
                    USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid)
                    WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);

                DO $grants$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname='murimios_app') THEN
                        GRANT SELECT,INSERT ON traceability.epcis_exchanges,traceability.epcis_captured_events TO murimios_app;
                        REVOKE UPDATE,DELETE ON traceability.epcis_exchanges,traceability.epcis_captured_events FROM murimios_app;
                    END IF;
                END $grants$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "epcis_captured_events",
                schema: "traceability");

            migrationBuilder.DropTable(
                name: "epcis_exchanges",
                schema: "traceability");
        }
    }
}
