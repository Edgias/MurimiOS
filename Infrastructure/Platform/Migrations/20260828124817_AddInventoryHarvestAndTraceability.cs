using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddInventoryHarvestAndTraceability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "inventory");

            migrationBuilder.AddUniqueConstraint(
                name: "ak_field_operations_tenant_id_id",
                schema: "operations",
                table: "field_operations",
                columns: new[] { "tenant_id", "id" });

            migrationBuilder.CreateTable(
                name: "harvest_lots",
                schema: "production",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    crop_zone_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lot_code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    harvested_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    source_command_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_harvest_lots", x => x.id);
                    table.ForeignKey(
                        name: "fk_harvest_lots_crop_zones_tenant_id_crop_zone_id",
                        columns: x => new { x.tenant_id, x.crop_zone_id },
                        principalSchema: "production",
                        principalTable: "crop_zones",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "inputs",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    category = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_inputs", x => x.id);
                    table.UniqueConstraint("ak_inputs_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_inputs_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "input_stock_lots",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    input_id = table.Column<Guid>(type: "uuid", nullable: false),
                    lot_code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
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
                    table.PrimaryKey("pk_input_stock_lots", x => x.id);
                    table.UniqueConstraint("ak_input_stock_lots_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_input_stock_lots_inputs_tenant_id_input_id",
                        columns: x => new { x.tenant_id, x.input_id },
                        principalSchema: "inventory",
                        principalTable: "inputs",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "movements",
                schema: "inventory",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    stock_lot_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    operation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    source_command_id = table.Column<Guid>(type: "uuid", nullable: true),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movements", x => x.id);
                    table.ForeignKey(
                        name: "fk_movements_field_operations_tenant_id_operation_id",
                        columns: x => new { x.tenant_id, x.operation_id },
                        principalSchema: "operations",
                        principalTable: "field_operations",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_movements_input_stock_lots_tenant_id_stock_lot_id",
                        columns: x => new { x.tenant_id, x.stock_lot_id },
                        principalSchema: "inventory",
                        principalTable: "input_stock_lots",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_harvest_lots_tenant_id",
                schema: "production",
                table: "harvest_lots",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_harvest_lots_tenant_id_crop_zone_id_harvested_at",
                schema: "production",
                table: "harvest_lots",
                columns: new[] { "tenant_id", "crop_zone_id", "harvested_at" });

            migrationBuilder.CreateIndex(
                name: "ix_harvest_lots_tenant_id_lot_code",
                schema: "production",
                table: "harvest_lots",
                columns: new[] { "tenant_id", "lot_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_harvest_lots_tenant_id_source_command_id",
                schema: "production",
                table: "harvest_lots",
                columns: new[] { "tenant_id", "source_command_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_input_stock_lots_tenant_id",
                schema: "inventory",
                table: "input_stock_lots",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_input_stock_lots_tenant_id_input_id_received_on",
                schema: "inventory",
                table: "input_stock_lots",
                columns: new[] { "tenant_id", "input_id", "received_on" });

            migrationBuilder.CreateIndex(
                name: "ix_input_stock_lots_tenant_id_lot_code",
                schema: "inventory",
                table: "input_stock_lots",
                columns: new[] { "tenant_id", "lot_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_inputs_tenant_id",
                schema: "inventory",
                table: "inputs",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_inputs_tenant_id_farm_id_name",
                schema: "inventory",
                table: "inputs",
                columns: new[] { "tenant_id", "farm_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_movements_tenant_id",
                schema: "inventory",
                table: "movements",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_movements_tenant_id_operation_id",
                schema: "inventory",
                table: "movements",
                columns: new[] { "tenant_id", "operation_id" });

            migrationBuilder.CreateIndex(
                name: "ix_movements_tenant_id_source_command_id",
                schema: "inventory",
                table: "movements",
                columns: new[] { "tenant_id", "source_command_id" },
                unique: true,
                filter: "source_command_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_movements_tenant_id_stock_lot_id_occurred_at",
                schema: "inventory",
                table: "movements",
                columns: new[] { "tenant_id", "stock_lot_id", "occurred_at" });

            migrationBuilder.Sql(
                """
                DO $rls$
                DECLARE
                    table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY[
                        'inventory.inputs',
                        'inventory.input_stock_lots',
                        'inventory.movements',
                        'production.harvest_lots'
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
                name: "harvest_lots",
                schema: "production");

            migrationBuilder.DropTable(
                name: "movements",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "input_stock_lots",
                schema: "inventory");

            migrationBuilder.DropTable(
                name: "inputs",
                schema: "inventory");

            migrationBuilder.DropUniqueConstraint(
                name: "ak_field_operations_tenant_id_id",
                schema: "operations",
                table: "field_operations");
        }
    }
}
