using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddLivestockTreatmentsAndWithdrawalControls : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "dispatches",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    purpose = table.Column<int>(type: "integer", nullable: false),
                    dispatched_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    destination = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    withdrawal_hold_until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    override_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    override_authorized_by = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_dispatches", x => x.id);
                    table.ForeignKey(
                        name: "fk_dispatches_subjects_tenant_id_subject_id",
                        columns: x => new { x.tenant_id, x.subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "medicines",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    active_ingredient = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    meat_withdrawal_days = table.Column<int>(type: "integer", nullable: false),
                    milk_withdrawal_hours = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_medicines", x => x.id);
                    table.UniqueConstraint("ak_medicines_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_medicines_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "medicine_stock_lots",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    medicine_id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_number = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    received_on = table.Column<DateOnly>(type: "date", nullable: false),
                    expires_on = table.Column<DateOnly>(type: "date", nullable: true),
                    initial_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    quantity_on_hand = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_medicine_stock_lots", x => x.id);
                    table.UniqueConstraint("ak_medicine_stock_lots_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_medicine_stock_lots_medicines_tenant_id_medicine_id",
                        columns: x => new { x.tenant_id, x.medicine_id },
                        principalSchema: "livestock",
                        principalTable: "medicines",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "treatments",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    medicine_stock_lot_id = table.Column<Guid>(type: "uuid", nullable: false),
                    dose = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    route = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    administered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    recorded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    meat_hold_until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    milk_hold_until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    source_command_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_treatments", x => x.id);
                    table.ForeignKey(
                        name: "fk_treatments_medicine_stock_lots_tenant_id_medicine_stock_lot",
                        columns: x => new { x.tenant_id, x.medicine_stock_lot_id },
                        principalSchema: "livestock",
                        principalTable: "medicine_stock_lots",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_treatments_subjects_tenant_id_subject_id",
                        columns: x => new { x.tenant_id, x.subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_dispatches_tenant_id",
                schema: "livestock",
                table: "dispatches",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_dispatches_tenant_id_subject_id_dispatched_at",
                schema: "livestock",
                table: "dispatches",
                columns: new[] { "tenant_id", "subject_id", "dispatched_at" });

            migrationBuilder.CreateIndex(
                name: "ix_medicine_stock_lots_tenant_id",
                schema: "livestock",
                table: "medicine_stock_lots",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_medicine_stock_lots_tenant_id_batch_number",
                schema: "livestock",
                table: "medicine_stock_lots",
                columns: new[] { "tenant_id", "batch_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_medicine_stock_lots_tenant_id_medicine_id_received_on",
                schema: "livestock",
                table: "medicine_stock_lots",
                columns: new[] { "tenant_id", "medicine_id", "received_on" });

            migrationBuilder.CreateIndex(
                name: "ix_medicines_tenant_id",
                schema: "livestock",
                table: "medicines",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_medicines_tenant_id_farm_id_name",
                schema: "livestock",
                table: "medicines",
                columns: new[] { "tenant_id", "farm_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_treatments_tenant_id",
                schema: "livestock",
                table: "treatments",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_treatments_tenant_id_medicine_stock_lot_id",
                schema: "livestock",
                table: "treatments",
                columns: new[] { "tenant_id", "medicine_stock_lot_id" });

            migrationBuilder.CreateIndex(
                name: "ix_treatments_tenant_id_source_command_id",
                schema: "livestock",
                table: "treatments",
                columns: new[] { "tenant_id", "source_command_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_treatments_tenant_id_subject_id_administered_at",
                schema: "livestock",
                table: "treatments",
                columns: new[] { "tenant_id", "subject_id", "administered_at" });

            migrationBuilder.Sql(
                """
                DO $rls$
                DECLARE
                    table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY[
                        'livestock.medicines',
                        'livestock.medicine_stock_lots',
                        'livestock.treatments',
                        'livestock.dispatches'
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
                name: "dispatches",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "treatments",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "medicine_stock_lots",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "medicines",
                schema: "livestock");
        }
    }
}
