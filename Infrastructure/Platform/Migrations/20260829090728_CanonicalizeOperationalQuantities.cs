using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class CanonicalizeOperationalQuantities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "display_unit",
                schema: "livestock",
                table: "treatments",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "entered_dose",
                schema: "livestock",
                table: "treatments",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "entered_unit",
                schema: "livestock",
                table: "treatments",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "display_unit",
                schema: "inventory",
                table: "movements",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "entered_quantity",
                schema: "inventory",
                table: "movements",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "entered_unit",
                schema: "inventory",
                table: "movements",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "unit",
                schema: "inventory",
                table: "movements",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "display_unit",
                schema: "livestock",
                table: "medicines",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "display_unit",
                schema: "livestock",
                table: "medicine_stock_lots",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "entered_initial_quantity",
                schema: "livestock",
                table: "medicine_stock_lots",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "entered_unit",
                schema: "livestock",
                table: "medicine_stock_lots",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "display_unit",
                schema: "livestock",
                table: "measurements",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "entered_unit",
                schema: "livestock",
                table: "measurements",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "entered_value",
                schema: "livestock",
                table: "measurements",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "display_unit",
                schema: "inventory",
                table: "inputs",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "display_unit",
                schema: "inventory",
                table: "input_stock_lots",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "entered_initial_quantity",
                schema: "inventory",
                table: "input_stock_lots",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "entered_unit",
                schema: "inventory",
                table: "input_stock_lots",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "display_unit",
                schema: "production",
                table: "harvest_lots",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<decimal>(
                name: "entered_quantity",
                schema: "production",
                table: "harvest_lots",
                type: "numeric(18,4)",
                precision: 18,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "entered_unit",
                schema: "production",
                table: "harvest_lots",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                ALTER TABLE livestock.measurements DISABLE TRIGGER livestock_measurements_supersession_only;

                UPDATE inventory.input_stock_lots lot
                SET entered_initial_quantity = lot.initial_quantity,
                    entered_unit = input.unit,
                    display_unit = input.unit,
                    initial_quantity = CASE lower(input.unit)
                        WHEN 'g' THEN lot.initial_quantity * 0.001 WHEN 't' THEN lot.initial_quantity * 1000
                        WHEN 'lb' THEN lot.initial_quantity * 0.45359237 WHEN 'ml' THEN lot.initial_quantity * 0.001
                        WHEN 'm3' THEN lot.initial_quantity * 1000 WHEN 'gal_us' THEN lot.initial_quantity * 3.785411784
                        ELSE lot.initial_quantity END,
                    quantity_on_hand = CASE lower(input.unit)
                        WHEN 'g' THEN lot.quantity_on_hand * 0.001 WHEN 't' THEN lot.quantity_on_hand * 1000
                        WHEN 'lb' THEN lot.quantity_on_hand * 0.45359237 WHEN 'ml' THEN lot.quantity_on_hand * 0.001
                        WHEN 'm3' THEN lot.quantity_on_hand * 1000 WHEN 'gal_us' THEN lot.quantity_on_hand * 3.785411784
                        ELSE lot.quantity_on_hand END
                FROM inventory.inputs input
                WHERE input.id = lot.input_id AND input.tenant_id = lot.tenant_id;

                UPDATE inventory.movements movement
                SET entered_quantity = movement.quantity,
                    entered_unit = input.unit,
                    display_unit = input.unit,
                    unit = CASE lower(input.unit)
                        WHEN 'g' THEN 'kg' WHEN 't' THEN 'kg' WHEN 'lb' THEN 'kg'
                        WHEN 'ml' THEN 'L' WHEN 'm3' THEN 'L' WHEN 'gal_us' THEN 'L' ELSE input.unit END,
                    quantity = CASE lower(input.unit)
                        WHEN 'g' THEN movement.quantity * 0.001 WHEN 't' THEN movement.quantity * 1000
                        WHEN 'lb' THEN movement.quantity * 0.45359237 WHEN 'ml' THEN movement.quantity * 0.001
                        WHEN 'm3' THEN movement.quantity * 1000 WHEN 'gal_us' THEN movement.quantity * 3.785411784
                        ELSE movement.quantity END
                FROM inventory.input_stock_lots lot, inventory.inputs input
                WHERE lot.id = movement.stock_lot_id AND lot.tenant_id = movement.tenant_id
                  AND input.id = lot.input_id AND input.tenant_id = lot.tenant_id;

                UPDATE inventory.inputs
                SET display_unit = unit,
                    unit = CASE lower(unit)
                        WHEN 'g' THEN 'kg' WHEN 't' THEN 'kg' WHEN 'lb' THEN 'kg'
                        WHEN 'ml' THEN 'L' WHEN 'm3' THEN 'L' WHEN 'gal_us' THEN 'L' ELSE unit END;

                UPDATE livestock.medicine_stock_lots lot
                SET entered_initial_quantity = lot.initial_quantity,
                    entered_unit = medicine.unit,
                    display_unit = medicine.unit,
                    initial_quantity = CASE lower(medicine.unit)
                        WHEN 'g' THEN lot.initial_quantity * 0.001 WHEN 't' THEN lot.initial_quantity * 1000
                        WHEN 'lb' THEN lot.initial_quantity * 0.45359237 WHEN 'ml' THEN lot.initial_quantity * 0.001
                        WHEN 'm3' THEN lot.initial_quantity * 1000 WHEN 'gal_us' THEN lot.initial_quantity * 3.785411784
                        ELSE lot.initial_quantity END,
                    quantity_on_hand = CASE lower(medicine.unit)
                        WHEN 'g' THEN lot.quantity_on_hand * 0.001 WHEN 't' THEN lot.quantity_on_hand * 1000
                        WHEN 'lb' THEN lot.quantity_on_hand * 0.45359237 WHEN 'ml' THEN lot.quantity_on_hand * 0.001
                        WHEN 'm3' THEN lot.quantity_on_hand * 1000 WHEN 'gal_us' THEN lot.quantity_on_hand * 3.785411784
                        ELSE lot.quantity_on_hand END
                FROM livestock.medicines medicine
                WHERE medicine.id = lot.medicine_id AND medicine.tenant_id = lot.tenant_id;

                UPDATE livestock.medicines
                SET display_unit = unit,
                    unit = CASE lower(unit)
                        WHEN 'g' THEN 'kg' WHEN 't' THEN 'kg' WHEN 'lb' THEN 'kg'
                        WHEN 'ml' THEN 'L' WHEN 'm3' THEN 'L' WHEN 'gal_us' THEN 'L' ELSE unit END;

                UPDATE production.harvest_lots
                SET entered_quantity = quantity, entered_unit = unit, display_unit = unit,
                    quantity = CASE lower(unit)
                        WHEN 'g' THEN quantity * 0.001 WHEN 't' THEN quantity * 1000
                        WHEN 'lb' THEN quantity * 0.45359237 WHEN 'ml' THEN quantity * 0.001
                        WHEN 'm3' THEN quantity * 1000 WHEN 'gal_us' THEN quantity * 3.785411784 ELSE quantity END,
                    unit = CASE lower(unit)
                        WHEN 'g' THEN 'kg' WHEN 't' THEN 'kg' WHEN 'lb' THEN 'kg'
                        WHEN 'ml' THEN 'L' WHEN 'm3' THEN 'L' WHEN 'gal_us' THEN 'L' ELSE unit END;

                UPDATE livestock.measurements
                SET entered_value = value, entered_unit = unit, display_unit = unit,
                    value = CASE lower(unit)
                        WHEN 'g' THEN value * 0.001 WHEN 't' THEN value * 1000 WHEN 'lb' THEN value * 0.45359237
                        WHEN 'ml' THEN value * 0.001 WHEN 'm3' THEN value * 1000 WHEN 'gal_us' THEN value * 3.785411784
                        WHEN 'acre' THEN value * 0.40468564224 WHEN 'm2' THEN value * 0.0001
                        WHEN 'f' THEN value * 0.5555555555555555555555555556 - 17.777777777777777777777777778
                        WHEN 'k' THEN value - 273.15 ELSE value END,
                    unit = CASE lower(unit)
                        WHEN 'g' THEN 'kg' WHEN 't' THEN 'kg' WHEN 'lb' THEN 'kg'
                        WHEN 'ml' THEN 'L' WHEN 'm3' THEN 'L' WHEN 'gal_us' THEN 'L'
                        WHEN 'acre' THEN 'ha' WHEN 'm2' THEN 'ha' WHEN 'f' THEN 'C' WHEN 'k' THEN 'C' ELSE unit END;

                UPDATE livestock.treatments
                SET entered_dose = dose, entered_unit = unit, display_unit = unit,
                    dose = CASE lower(unit)
                        WHEN 'g' THEN dose * 0.001 WHEN 't' THEN dose * 1000 WHEN 'lb' THEN dose * 0.45359237
                        WHEN 'ml' THEN dose * 0.001 WHEN 'm3' THEN dose * 1000
                        WHEN 'gal_us' THEN dose * 3.785411784 ELSE dose END,
                    unit = CASE lower(unit)
                        WHEN 'g' THEN 'kg' WHEN 't' THEN 'kg' WHEN 'lb' THEN 'kg'
                        WHEN 'ml' THEN 'L' WHEN 'm3' THEN 'L' WHEN 'gal_us' THEN 'L' ELSE unit END;

                ALTER TABLE livestock.measurements ENABLE TRIGGER livestock_measurements_supersession_only;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "display_unit",
                schema: "livestock",
                table: "treatments");

            migrationBuilder.DropColumn(
                name: "entered_dose",
                schema: "livestock",
                table: "treatments");

            migrationBuilder.DropColumn(
                name: "entered_unit",
                schema: "livestock",
                table: "treatments");

            migrationBuilder.DropColumn(
                name: "display_unit",
                schema: "inventory",
                table: "movements");

            migrationBuilder.DropColumn(
                name: "entered_quantity",
                schema: "inventory",
                table: "movements");

            migrationBuilder.DropColumn(
                name: "entered_unit",
                schema: "inventory",
                table: "movements");

            migrationBuilder.DropColumn(
                name: "unit",
                schema: "inventory",
                table: "movements");

            migrationBuilder.DropColumn(
                name: "display_unit",
                schema: "livestock",
                table: "medicines");

            migrationBuilder.DropColumn(
                name: "display_unit",
                schema: "livestock",
                table: "medicine_stock_lots");

            migrationBuilder.DropColumn(
                name: "entered_initial_quantity",
                schema: "livestock",
                table: "medicine_stock_lots");

            migrationBuilder.DropColumn(
                name: "entered_unit",
                schema: "livestock",
                table: "medicine_stock_lots");

            migrationBuilder.DropColumn(
                name: "display_unit",
                schema: "livestock",
                table: "measurements");

            migrationBuilder.DropColumn(
                name: "entered_unit",
                schema: "livestock",
                table: "measurements");

            migrationBuilder.DropColumn(
                name: "entered_value",
                schema: "livestock",
                table: "measurements");

            migrationBuilder.DropColumn(
                name: "display_unit",
                schema: "inventory",
                table: "inputs");

            migrationBuilder.DropColumn(
                name: "display_unit",
                schema: "inventory",
                table: "input_stock_lots");

            migrationBuilder.DropColumn(
                name: "entered_initial_quantity",
                schema: "inventory",
                table: "input_stock_lots");

            migrationBuilder.DropColumn(
                name: "entered_unit",
                schema: "inventory",
                table: "input_stock_lots");

            migrationBuilder.DropColumn(
                name: "display_unit",
                schema: "production",
                table: "harvest_lots");

            migrationBuilder.DropColumn(
                name: "entered_quantity",
                schema: "production",
                table: "harvest_lots");

            migrationBuilder.DropColumn(
                name: "entered_unit",
                schema: "production",
                table: "harvest_lots");
        }
    }
}
