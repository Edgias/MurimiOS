using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddAdvancedCropPlanning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "planning");

            migrationBuilder.CreateTable(
                name: "crop_plans",
                schema: "planning",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    planning_group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scenario_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    season_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    scenario_name = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    revision = table.Column<int>(type: "integer", nullable: false),
                    previous_plan_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    drafted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    submitted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    submitted_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    approved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    approved_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    rejected_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    rejected_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    superseded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    superseded_by_plan_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_crop_plans", x => x.id);
                    table.UniqueConstraint("ak_crop_plans_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_crop_plans_crop_plans_tenant_id_previous_plan_id",
                        columns: x => new { x.tenant_id, x.previous_plan_id },
                        principalSchema: "planning",
                        principalTable: "crop_plans",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_crop_plans_crop_plans_tenant_id_superseded_by_plan_id",
                        columns: x => new { x.tenant_id, x.superseded_by_plan_id },
                        principalSchema: "planning",
                        principalTable: "crop_plans",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_crop_plans_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_crop_plans_seasons_tenant_id_season_id",
                        columns: x => new { x.tenant_id, x.season_id },
                        principalSchema: "production",
                        principalTable: "seasons",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "product_application_rules",
                schema: "planning",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    input_id = table.Column<Guid>(type: "uuid", nullable: false),
                    crop = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    jurisdiction = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    maximum_rate_per_ha = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    rate_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    entered_maximum_rate_per_ha = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    entered_rate_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    re_entry_hours = table.Column<int>(type: "integer", nullable: false),
                    pre_harvest_interval_days = table.Column<int>(type: "integer", nullable: false),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_to = table.Column<DateOnly>(type: "date", nullable: true),
                    authorization_reference = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product_application_rules", x => x.id);
                    table.ForeignKey(
                        name: "fk_product_application_rules_agricultural_inputs_tenant_id_inp",
                        columns: x => new { x.tenant_id, x.input_id },
                        principalSchema: "inventory",
                        principalTable: "inputs",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "allocations",
                schema: "planning",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: false),
                    boundary_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    crop_zone_id = table.Column<Guid>(type: "uuid", nullable: true),
                    crop = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    variety = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    planned_area_ha = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: false),
                    entered_area = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: false),
                    entered_area_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    target_yield = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    target_yield_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    entered_target_yield = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    entered_target_yield_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    target_price_per_canonical_unit = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    planned_sowing_date = table.Column<DateOnly>(type: "date", nullable: false),
                    planned_harvest_date = table.Column<DateOnly>(type: "date", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_allocations", x => x.id);
                    table.UniqueConstraint("ak_planning_allocations_tenant_plan_id", x => new { x.tenant_id, x.plan_id, x.id });
                    table.ForeignKey(
                        name: "fk_allocations_crop_plans_tenant_id_plan_id",
                        columns: x => new { x.tenant_id, x.plan_id },
                        principalSchema: "planning",
                        principalTable: "crop_plans",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_allocations_crop_zones_tenant_id_crop_zone_id",
                        columns: x => new { x.tenant_id, x.crop_zone_id },
                        principalSchema: "production",
                        principalTable: "crop_zones",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_allocations_field_boundary_versions_tenant_id_boundary_vers",
                        columns: x => new { x.tenant_id, x.boundary_version_id },
                        principalSchema: "farm",
                        principalTable: "field_boundary_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_allocations_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "financial_entries",
                schema: "planning",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    allocation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    entry_type = table.Column<int>(type: "integer", nullable: false),
                    category = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    reference = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    recorded_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_financial_entries", x => x.id);
                    table.ForeignKey(
                        name: "fk_financial_entries_allocations_tenant_id_plan_id_allocation_",
                        columns: x => new { x.tenant_id, x.plan_id, x.allocation_id },
                        principalSchema: "planning",
                        principalTable: "allocations",
                        principalColumns: new[] { "tenant_id", "plan_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_financial_entries_crop_plans_tenant_id_plan_id",
                        columns: x => new { x.tenant_id, x.plan_id },
                        principalSchema: "planning",
                        principalTable: "crop_plans",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "operations",
                schema: "planning",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    allocation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    operation_type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    planned_start = table.Column<DateOnly>(type: "date", nullable: false),
                    planned_end = table.Column<DateOnly>(type: "date", nullable: false),
                    sequence = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_operations", x => x.id);
                    table.UniqueConstraint("ak_planning_operations_tenant_plan_id", x => new { x.tenant_id, x.plan_id, x.id });
                    table.ForeignKey(
                        name: "fk_operations_allocations_tenant_id_plan_id_allocation_id",
                        columns: x => new { x.tenant_id, x.plan_id, x.allocation_id },
                        principalSchema: "planning",
                        principalTable: "allocations",
                        principalColumns: new[] { "tenant_id", "plan_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_operations_crop_plans_tenant_id_plan_id",
                        columns: x => new { x.tenant_id, x.plan_id },
                        principalSchema: "planning",
                        principalTable: "crop_plans",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "resource_budgets",
                schema: "planning",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_id = table.Column<Guid>(type: "uuid", nullable: false),
                    allocation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    planned_operation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    resource_type = table.Column<int>(type: "integer", nullable: false),
                    agricultural_input_id = table.Column<Guid>(type: "uuid", nullable: true),
                    resource_name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    entered_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    entered_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    unit_cost_per_canonical_unit = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_resource_budgets", x => x.id);
                    table.ForeignKey(
                        name: "fk_resource_budgets_agricultural_inputs_tenant_id_agricultural",
                        columns: x => new { x.tenant_id, x.agricultural_input_id },
                        principalSchema: "inventory",
                        principalTable: "inputs",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_resource_budgets_allocations_tenant_id_plan_id_allocation_id",
                        columns: x => new { x.tenant_id, x.plan_id, x.allocation_id },
                        principalSchema: "planning",
                        principalTable: "allocations",
                        principalColumns: new[] { "tenant_id", "plan_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_resource_budgets_crop_plans_tenant_id_plan_id",
                        columns: x => new { x.tenant_id, x.plan_id },
                        principalSchema: "planning",
                        principalTable: "crop_plans",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_resource_budgets_operations_tenant_id_plan_id_planned_opera",
                        columns: x => new { x.tenant_id, x.plan_id, x.planned_operation_id },
                        principalSchema: "planning",
                        principalTable: "operations",
                        principalColumns: new[] { "tenant_id", "plan_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_allocations_tenant_id",
                schema: "planning",
                table: "allocations",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_allocations_tenant_id_boundary_version_id",
                schema: "planning",
                table: "allocations",
                columns: new[] { "tenant_id", "boundary_version_id" });

            migrationBuilder.CreateIndex(
                name: "ix_allocations_tenant_id_crop_zone_id",
                schema: "planning",
                table: "allocations",
                columns: new[] { "tenant_id", "crop_zone_id" });

            migrationBuilder.CreateIndex(
                name: "ix_allocations_tenant_id_field_id",
                schema: "planning",
                table: "allocations",
                columns: new[] { "tenant_id", "field_id" });

            migrationBuilder.CreateIndex(
                name: "ix_allocations_tenant_id_plan_id_crop_zone_id",
                schema: "planning",
                table: "allocations",
                columns: new[] { "tenant_id", "plan_id", "crop_zone_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_allocations_tenant_id_plan_id_field_id",
                schema: "planning",
                table: "allocations",
                columns: new[] { "tenant_id", "plan_id", "field_id" });

            migrationBuilder.CreateIndex(
                name: "ix_crop_plans_tenant_id",
                schema: "planning",
                table: "crop_plans",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_crop_plans_tenant_id_farm_id_season_id_status",
                schema: "planning",
                table: "crop_plans",
                columns: new[] { "tenant_id", "farm_id", "season_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_crop_plans_tenant_id_planning_group_id_status",
                schema: "planning",
                table: "crop_plans",
                columns: new[] { "tenant_id", "planning_group_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_crop_plans_tenant_id_previous_plan_id",
                schema: "planning",
                table: "crop_plans",
                columns: new[] { "tenant_id", "previous_plan_id" });

            migrationBuilder.CreateIndex(
                name: "ix_crop_plans_tenant_id_scenario_id_revision",
                schema: "planning",
                table: "crop_plans",
                columns: new[] { "tenant_id", "scenario_id", "revision" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_crop_plans_tenant_id_season_id",
                schema: "planning",
                table: "crop_plans",
                columns: new[] { "tenant_id", "season_id" });

            migrationBuilder.CreateIndex(
                name: "ix_crop_plans_tenant_id_superseded_by_plan_id",
                schema: "planning",
                table: "crop_plans",
                columns: new[] { "tenant_id", "superseded_by_plan_id" });

            migrationBuilder.CreateIndex(
                name: "ix_financial_entries_tenant_id",
                schema: "planning",
                table: "financial_entries",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_financial_entries_tenant_id_plan_id_allocation_id",
                schema: "planning",
                table: "financial_entries",
                columns: new[] { "tenant_id", "plan_id", "allocation_id" });

            migrationBuilder.CreateIndex(
                name: "ix_financial_entries_tenant_id_plan_id_occurred_at",
                schema: "planning",
                table: "financial_entries",
                columns: new[] { "tenant_id", "plan_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_financial_entries_tenant_id_reference",
                schema: "planning",
                table: "financial_entries",
                columns: new[] { "tenant_id", "reference" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_operations_tenant_id",
                schema: "planning",
                table: "operations",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_operations_tenant_id_plan_id_allocation_id_sequence",
                schema: "planning",
                table: "operations",
                columns: new[] { "tenant_id", "plan_id", "allocation_id", "sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_operations_tenant_id_planned_start_planned_end",
                schema: "planning",
                table: "operations",
                columns: new[] { "tenant_id", "planned_start", "planned_end" });

            migrationBuilder.CreateIndex(
                name: "ix_product_application_rules_tenant_id",
                schema: "planning",
                table: "product_application_rules",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_product_application_rules_tenant_id_input_id_crop_effective",
                schema: "planning",
                table: "product_application_rules",
                columns: new[] { "tenant_id", "input_id", "crop", "effective_from", "effective_to" });

            migrationBuilder.CreateIndex(
                name: "ix_product_application_rules_tenant_id_input_id_crop_jurisdict",
                schema: "planning",
                table: "product_application_rules",
                columns: new[] { "tenant_id", "input_id", "crop", "jurisdiction", "effective_from" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_resource_budgets_tenant_id",
                schema: "planning",
                table: "resource_budgets",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_resource_budgets_tenant_id_agricultural_input_id",
                schema: "planning",
                table: "resource_budgets",
                columns: new[] { "tenant_id", "agricultural_input_id" });

            migrationBuilder.CreateIndex(
                name: "ix_resource_budgets_tenant_id_plan_id_allocation_id_resource_t",
                schema: "planning",
                table: "resource_budgets",
                columns: new[] { "tenant_id", "plan_id", "allocation_id", "resource_type" });

            migrationBuilder.CreateIndex(
                name: "ix_resource_budgets_tenant_id_plan_id_planned_operation_id",
                schema: "planning",
                table: "resource_budgets",
                columns: new[] { "tenant_id", "plan_id", "planned_operation_id" });

            migrationBuilder.Sql(
                """
                CREATE UNIQUE INDEX ux_planning_one_approved_plan_per_season
                    ON planning.crop_plans (tenant_id, farm_id, season_id)
                    WHERE status = 3;

                ALTER TABLE planning.crop_plans
                    ADD CONSTRAINT ck_crop_plan_revision CHECK (revision > 0),
                    ADD CONSTRAINT ck_crop_plan_status CHECK (status BETWEEN 1 AND 5),
                    ADD CONSTRAINT ck_crop_plan_currency CHECK (currency ~ '^[A-Z]{3}$');
                ALTER TABLE planning.allocations
                    ADD CONSTRAINT ck_plan_allocation_area CHECK (planned_area_ha > 0),
                    ADD CONSTRAINT ck_plan_allocation_yield CHECK (target_yield > 0),
                    ADD CONSTRAINT ck_plan_allocation_price CHECK (target_price_per_canonical_unit >= 0),
                    ADD CONSTRAINT ck_plan_allocation_dates CHECK (planned_harvest_date >= planned_sowing_date),
                    ADD CONSTRAINT ck_plan_allocation_currency CHECK (currency ~ '^[A-Z]{3}$');
                ALTER TABLE planning.operations
                    ADD CONSTRAINT ck_planned_operation_dates CHECK (planned_end >= planned_start),
                    ADD CONSTRAINT ck_planned_operation_sequence CHECK (sequence > 0);
                ALTER TABLE planning.resource_budgets
                    ADD CONSTRAINT ck_resource_budget_type CHECK (resource_type BETWEEN 1 AND 6),
                    ADD CONSTRAINT ck_resource_budget_quantity CHECK (quantity > 0),
                    ADD CONSTRAINT ck_resource_budget_cost CHECK (unit_cost_per_canonical_unit >= 0),
                    ADD CONSTRAINT ck_resource_budget_currency CHECK (currency ~ '^[A-Z]{3}$'),
                    ADD CONSTRAINT ck_resource_budget_input CHECK (
                        (resource_type = 1 AND agricultural_input_id IS NOT NULL)
                        OR (resource_type <> 1 AND agricultural_input_id IS NULL));
                ALTER TABLE planning.product_application_rules
                    ADD CONSTRAINT ck_product_rule_rate CHECK (maximum_rate_per_ha > 0),
                    ADD CONSTRAINT ck_product_rule_intervals CHECK (re_entry_hours >= 0 AND pre_harvest_interval_days >= 0),
                    ADD CONSTRAINT ck_product_rule_dates CHECK (effective_to IS NULL OR effective_to >= effective_from),
                    ADD CONSTRAINT ex_product_rule_effective_period EXCLUDE USING gist (
                        tenant_id WITH =,
                        input_id WITH =,
                        lower(crop) WITH =,
                        jurisdiction WITH =,
                        daterange(effective_from, effective_to, '[]') WITH &&);
                ALTER TABLE planning.financial_entries
                    ADD CONSTRAINT ck_plan_financial_type CHECK (entry_type IN (1, 2)),
                    ADD CONSTRAINT ck_plan_financial_amount CHECK (amount > 0),
                    ADD CONSTRAINT ck_plan_financial_currency CHECK (currency ~ '^[A-Z]{3}$');

                DO $rls$
                DECLARE table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY[
                        'crop_plans', 'allocations', 'operations', 'resource_budgets',
                        'product_application_rules', 'financial_entries'
                    ] LOOP
                        EXECUTE format('ALTER TABLE planning.%I ENABLE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('ALTER TABLE planning.%I FORCE ROW LEVEL SECURITY', table_name);
                        EXECUTE format(
                            'CREATE POLICY tenant_isolation ON planning.%I USING (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid) WITH CHECK (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid)',
                            table_name);
                    END LOOP;
                END
                $rls$;

                CREATE OR REPLACE FUNCTION planning.guard_crop_plan_lifecycle()
                RETURNS trigger
                LANGUAGE plpgsql
                SET search_path = pg_catalog, planning, production
                AS $function$
                DECLARE
                    previous_row planning.crop_plans%ROWTYPE;
                BEGIN
                    IF TG_OP = 'DELETE' THEN
                        RAISE EXCEPTION 'Crop plans are retained and cannot be deleted.' USING ERRCODE = '55000';
                    END IF;
                    IF TG_OP = 'INSERT' THEN
                        IF NEW.status <> 1 OR NEW.revision <= 0 THEN
                            RAISE EXCEPTION 'A crop plan must start as revisioned draft.' USING ERRCODE = '23514';
                        END IF;
                        IF NOT EXISTS (
                            SELECT 1 FROM production.seasons AS season
                            WHERE season.tenant_id = NEW.tenant_id AND season.id = NEW.season_id
                              AND season.farm_id = NEW.farm_id) THEN
                            RAISE EXCEPTION 'Crop plan season must belong to its farm.' USING ERRCODE = '23514';
                        END IF;
                        IF EXISTS (SELECT 1 FROM planning.crop_plans AS grouped WHERE grouped.tenant_id = NEW.tenant_id
                            AND grouped.planning_group_id = NEW.planning_group_id
                            AND (grouped.farm_id <> NEW.farm_id OR grouped.season_id <> NEW.season_id)) THEN
                            RAISE EXCEPTION 'Planning groups cannot span farms or seasons.' USING ERRCODE = '23514';
                        END IF;
                        IF NEW.previous_plan_id IS NULL THEN
                            IF NEW.revision <> 1 OR NEW.scenario_id <> NEW.id THEN
                                RAISE EXCEPTION 'An initial scenario must start at revision one.' USING ERRCODE = '23514';
                            END IF;
                        ELSE
                            SELECT * INTO previous_row FROM planning.crop_plans
                            WHERE tenant_id = NEW.tenant_id AND id = NEW.previous_plan_id;
                            IF NOT FOUND OR previous_row.scenario_id <> NEW.scenario_id
                               OR previous_row.planning_group_id <> NEW.planning_group_id
                               OR previous_row.farm_id <> NEW.farm_id OR previous_row.season_id <> NEW.season_id
                               OR NEW.revision <> previous_row.revision + 1 THEN
                                RAISE EXCEPTION 'Crop plan revision lineage is invalid.' USING ERRCODE = '23514';
                            END IF;
                        END IF;
                        RETURN NEW;
                    END IF;
                    IF (to_jsonb(NEW) - ARRAY['status', 'submitted_at', 'submitted_by', 'approved_at', 'approved_by',
                        'rejected_at', 'rejected_by', 'rejection_reason', 'superseded_at', 'superseded_by_plan_id',
                        'updated_at_utc', 'version']) IS DISTINCT FROM
                       (to_jsonb(OLD) - ARRAY['status', 'submitted_at', 'submitted_by', 'approved_at', 'approved_by',
                        'rejected_at', 'rejected_by', 'rejection_reason', 'superseded_at', 'superseded_by_plan_id',
                        'updated_at_utc', 'version'])
                       OR NEW.version <> OLD.version + 1 OR NEW.updated_at_utc < OLD.updated_at_utc THEN
                        RAISE EXCEPTION 'Crop plan identity and revision details are immutable.' USING ERRCODE = '55000';
                    END IF;
                    IF OLD.status = 1 AND NEW.status = 2 AND NEW.submitted_at IS NOT NULL AND NEW.submitted_by IS NOT NULL THEN
                        RETURN NEW;
                    ELSIF OLD.status = 2 AND NEW.status = 3 AND NEW.approved_at IS NOT NULL AND NEW.approved_by IS NOT NULL THEN
                        RETURN NEW;
                    ELSIF OLD.status = 2 AND NEW.status = 4 AND NEW.rejected_at IS NOT NULL
                          AND NEW.rejected_by IS NOT NULL AND nullif(btrim(NEW.rejection_reason), '') IS NOT NULL THEN
                        RETURN NEW;
                    ELSIF OLD.status = 3 AND NEW.status = 5 AND NEW.superseded_at IS NOT NULL
                          AND NEW.superseded_by_plan_id IS NOT NULL THEN
                        RETURN NEW;
                    END IF;
                    RAISE EXCEPTION 'Invalid crop plan lifecycle transition.' USING ERRCODE = '55000';
                END;
                $function$;

                CREATE TRIGGER crop_plan_lifecycle_guard
                    BEFORE INSERT OR UPDATE OR DELETE ON planning.crop_plans
                    FOR EACH ROW EXECUTE FUNCTION planning.guard_crop_plan_lifecycle();

                CREATE OR REPLACE FUNCTION planning.guard_draft_plan_line()
                RETURNS trigger
                LANGUAGE plpgsql
                SET search_path = pg_catalog, planning
                AS $function$
                DECLARE
                    target_plan_id uuid := CASE WHEN TG_OP = 'DELETE' THEN OLD.plan_id ELSE NEW.plan_id END;
                    target_tenant_id uuid := CASE WHEN TG_OP = 'DELETE' THEN OLD.tenant_id ELSE NEW.tenant_id END;
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM planning.crop_plans AS plan
                        WHERE plan.tenant_id = target_tenant_id AND plan.id = target_plan_id AND plan.status = 1) THEN
                        RAISE EXCEPTION 'Plan allocations, operations, and budgets can change only while the plan is draft.' USING ERRCODE = '55000';
                    END IF;
                    IF TG_OP = 'DELETE' THEN
                        RETURN OLD;
                    END IF;
                    RETURN NEW;
                END;
                $function$;

                CREATE TRIGGER plan_allocation_draft_guard BEFORE INSERT OR UPDATE OR DELETE ON planning.allocations
                    FOR EACH ROW EXECUTE FUNCTION planning.guard_draft_plan_line();
                CREATE TRIGGER planned_operation_draft_guard BEFORE INSERT OR UPDATE OR DELETE ON planning.operations
                    FOR EACH ROW EXECUTE FUNCTION planning.guard_draft_plan_line();
                CREATE TRIGGER resource_budget_draft_guard BEFORE INSERT OR UPDATE OR DELETE ON planning.resource_budgets
                    FOR EACH ROW EXECUTE FUNCTION planning.guard_draft_plan_line();

                CREATE OR REPLACE FUNCTION planning.guard_financial_entry_insert()
                RETURNS trigger
                LANGUAGE plpgsql
                SET search_path = pg_catalog, planning
                AS $function$
                DECLARE plan_currency text;
                BEGIN
                    SELECT currency INTO plan_currency FROM planning.crop_plans AS plan
                    WHERE plan.tenant_id = NEW.tenant_id AND plan.id = NEW.plan_id AND plan.status IN (3, 5);
                    IF NOT FOUND OR plan_currency <> NEW.currency THEN
                        RAISE EXCEPTION 'Financial entries require an approved retained plan and matching currency.' USING ERRCODE = '23514';
                    END IF;
                    RETURN NEW;
                END;
                $function$;

                CREATE TRIGGER plan_financial_insert_guard BEFORE INSERT ON planning.financial_entries
                    FOR EACH ROW EXECUTE FUNCTION planning.guard_financial_entry_insert();
                CREATE TRIGGER plan_financial_immutable BEFORE UPDATE OR DELETE ON planning.financial_entries
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER product_application_rule_immutable BEFORE UPDATE OR DELETE ON planning.product_application_rules
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();

                DO $grant$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        GRANT USAGE ON SCHEMA planning TO murimios_app;
                        GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA planning TO murimios_app;
                    END IF;
                END
                $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP TRIGGER IF EXISTS plan_financial_insert_guard ON planning.financial_entries;
                DROP TRIGGER IF EXISTS resource_budget_draft_guard ON planning.resource_budgets;
                DROP TRIGGER IF EXISTS planned_operation_draft_guard ON planning.operations;
                DROP TRIGGER IF EXISTS plan_allocation_draft_guard ON planning.allocations;
                DROP TRIGGER IF EXISTS crop_plan_lifecycle_guard ON planning.crop_plans;
                DROP FUNCTION IF EXISTS planning.guard_financial_entry_insert();
                DROP FUNCTION IF EXISTS planning.guard_draft_plan_line();
                DROP FUNCTION IF EXISTS planning.guard_crop_plan_lifecycle();
                """);

            migrationBuilder.DropTable(
                name: "financial_entries",
                schema: "planning");

            migrationBuilder.DropTable(
                name: "product_application_rules",
                schema: "planning");

            migrationBuilder.DropTable(
                name: "resource_budgets",
                schema: "planning");

            migrationBuilder.DropTable(
                name: "operations",
                schema: "planning");

            migrationBuilder.DropTable(
                name: "allocations",
                schema: "planning");

            migrationBuilder.DropTable(
                name: "crop_plans",
                schema: "planning");
        }
    }
}
