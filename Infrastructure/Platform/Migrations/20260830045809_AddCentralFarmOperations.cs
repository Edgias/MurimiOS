using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddCentralFarmOperations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "work_orders",
                schema: "operations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    number = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    work_type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    scheduled_start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    scheduled_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    instructions = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: true),
                    boundary_version_id = table.Column<Guid>(type: "uuid", nullable: true),
                    crop_zone_id = table.Column<Guid>(type: "uuid", nullable: true),
                    livestock_subject_id = table.Column<Guid>(type: "uuid", nullable: true),
                    crop_plan_id = table.Column<Guid>(type: "uuid", nullable: true),
                    planned_operation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    previous_work_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    superseded_by_work_order_id = table.Column<Guid>(type: "uuid", nullable: true),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    drafted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    planned_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    assigned_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    verified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    verified_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    rejected_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    rejected_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    cancelled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancelled_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    cancellation_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    completion_notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_orders", x => x.id);
                    table.UniqueConstraint("ak_operations_work_orders_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_work_orders_crop_plans_tenant_id_crop_plan_id",
                        columns: x => new { x.tenant_id, x.crop_plan_id },
                        principalSchema: "planning",
                        principalTable: "crop_plans",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_work_orders_crop_zones_tenant_id_crop_zone_id",
                        columns: x => new { x.tenant_id, x.crop_zone_id },
                        principalSchema: "production",
                        principalTable: "crop_zones",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_work_orders_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_work_orders_field_boundary_versions_tenant_id_boundary_vers",
                        columns: x => new { x.tenant_id, x.boundary_version_id },
                        principalSchema: "farm",
                        principalTable: "field_boundary_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_work_orders_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_work_orders_livestock_subjects_tenant_id_livestock_subject_",
                        columns: x => new { x.tenant_id, x.livestock_subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_work_orders_operations_tenant_id_crop_plan_id_planned_opera",
                        columns: x => new { x.tenant_id, x.crop_plan_id, x.planned_operation_id },
                        principalSchema: "planning",
                        principalTable: "operations",
                        principalColumns: new[] { "tenant_id", "plan_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_work_orders_work_orders_tenant_id_previous_work_order_id",
                        columns: x => new { x.tenant_id, x.previous_work_order_id },
                        principalSchema: "operations",
                        principalTable: "work_orders",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_work_orders_work_orders_tenant_id_superseded_by_work_order_",
                        columns: x => new { x.tenant_id, x.superseded_by_work_order_id },
                        principalSchema: "operations",
                        principalTable: "work_orders",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "work_order_assignments",
                schema: "operations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    membership_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assignment_role = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    assigned_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    assigned_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    unassigned_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    unassigned_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_order_assignments", x => x.id);
                    table.ForeignKey(
                        name: "fk_work_order_assignments_memberships_tenant_id_membership_id",
                        columns: x => new { x.tenant_id, x.membership_id },
                        principalSchema: "access",
                        principalTable: "memberships",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_work_order_assignments_work_orders_tenant_id_work_order_id",
                        columns: x => new { x.tenant_id, x.work_order_id },
                        principalSchema: "operations",
                        principalTable: "work_orders",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "work_order_resources",
                schema: "operations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    resource_type = table.Column<int>(type: "integer", nullable: false),
                    resource_name = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    resource_reference = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    agricultural_input_id = table.Column<Guid>(type: "uuid", nullable: true),
                    stock_lot_id = table.Column<Guid>(type: "uuid", nullable: true),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    entered_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    entered_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    unit_cost = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_order_resources", x => x.id);
                    table.ForeignKey(
                        name: "fk_work_order_resources_agricultural_inputs_tenant_id_agricult",
                        columns: x => new { x.tenant_id, x.agricultural_input_id },
                        principalSchema: "inventory",
                        principalTable: "inputs",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_work_order_resources_input_stock_lots_tenant_id_stock_lot_id",
                        columns: x => new { x.tenant_id, x.stock_lot_id },
                        principalSchema: "inventory",
                        principalTable: "input_stock_lots",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_work_order_resources_work_orders_tenant_id_work_order_id",
                        columns: x => new { x.tenant_id, x.work_order_id },
                        principalSchema: "operations",
                        principalTable: "work_orders",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "work_order_results",
                schema: "operations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    result_type = table.Column<int>(type: "integer", nullable: false),
                    result_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    recorded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    recorded_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_order_results", x => x.id);
                    table.ForeignKey(
                        name: "fk_work_order_results_work_orders_tenant_id_work_order_id",
                        columns: x => new { x.tenant_id, x.work_order_id },
                        principalSchema: "operations",
                        principalTable: "work_orders",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "work_order_status_history",
                schema: "operations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    work_order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    from_status = table.Column<int>(type: "integer", nullable: false),
                    to_status = table.Column<int>(type: "integer", nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    actor = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_work_order_status_history", x => x.id);
                    table.ForeignKey(
                        name: "fk_work_order_status_history_work_orders_tenant_id_work_order_",
                        columns: x => new { x.tenant_id, x.work_order_id },
                        principalSchema: "operations",
                        principalTable: "work_orders",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_work_order_assignments_tenant_id",
                schema: "operations",
                table: "work_order_assignments",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_order_assignments_tenant_id_membership_id_unassigned_at",
                schema: "operations",
                table: "work_order_assignments",
                columns: new[] { "tenant_id", "membership_id", "unassigned_at" });

            migrationBuilder.CreateIndex(
                name: "ix_work_order_assignments_tenant_id_work_order_id_is_primary",
                schema: "operations",
                table: "work_order_assignments",
                columns: new[] { "tenant_id", "work_order_id", "is_primary" },
                unique: true,
                filter: "unassigned_at IS NULL AND is_primary");

            migrationBuilder.CreateIndex(
                name: "ix_work_order_assignments_tenant_id_work_order_id_membership_id",
                schema: "operations",
                table: "work_order_assignments",
                columns: new[] { "tenant_id", "work_order_id", "membership_id" },
                unique: true,
                filter: "unassigned_at IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_work_order_resources_tenant_id",
                schema: "operations",
                table: "work_order_resources",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_order_resources_tenant_id_agricultural_input_id",
                schema: "operations",
                table: "work_order_resources",
                columns: new[] { "tenant_id", "agricultural_input_id" });

            migrationBuilder.CreateIndex(
                name: "ix_work_order_resources_tenant_id_resource_type_resource_refer",
                schema: "operations",
                table: "work_order_resources",
                columns: new[] { "tenant_id", "resource_type", "resource_reference", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_work_order_resources_tenant_id_stock_lot_id_status",
                schema: "operations",
                table: "work_order_resources",
                columns: new[] { "tenant_id", "stock_lot_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_work_order_resources_tenant_id_work_order_id_resource_type",
                schema: "operations",
                table: "work_order_resources",
                columns: new[] { "tenant_id", "work_order_id", "resource_type" });

            migrationBuilder.CreateIndex(
                name: "ix_work_order_results_tenant_id",
                schema: "operations",
                table: "work_order_results",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_order_results_tenant_id_result_type_result_id",
                schema: "operations",
                table: "work_order_results",
                columns: new[] { "tenant_id", "result_type", "result_id" });

            migrationBuilder.CreateIndex(
                name: "ix_work_order_results_tenant_id_work_order_id_result_type_resu",
                schema: "operations",
                table: "work_order_results",
                columns: new[] { "tenant_id", "work_order_id", "result_type", "result_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_work_order_status_history_tenant_id",
                schema: "operations",
                table: "work_order_status_history",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_order_status_history_tenant_id_work_order_id_occurred_",
                schema: "operations",
                table: "work_order_status_history",
                columns: new[] { "tenant_id", "work_order_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_tenant_id",
                schema: "operations",
                table: "work_orders",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_tenant_id_boundary_version_id",
                schema: "operations",
                table: "work_orders",
                columns: new[] { "tenant_id", "boundary_version_id" });

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_tenant_id_crop_plan_id_planned_operation_id",
                schema: "operations",
                table: "work_orders",
                columns: new[] { "tenant_id", "crop_plan_id", "planned_operation_id" },
                unique: true,
                filter: "planned_operation_id IS NOT NULL AND status <> 9");

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_tenant_id_crop_zone_id",
                schema: "operations",
                table: "work_orders",
                columns: new[] { "tenant_id", "crop_zone_id" });

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_tenant_id_farm_id_number",
                schema: "operations",
                table: "work_orders",
                columns: new[] { "tenant_id", "farm_id", "number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_tenant_id_farm_id_status_scheduled_start",
                schema: "operations",
                table: "work_orders",
                columns: new[] { "tenant_id", "farm_id", "status", "scheduled_start" });

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_tenant_id_field_id",
                schema: "operations",
                table: "work_orders",
                columns: new[] { "tenant_id", "field_id" });

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_tenant_id_livestock_subject_id",
                schema: "operations",
                table: "work_orders",
                columns: new[] { "tenant_id", "livestock_subject_id" });

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_tenant_id_previous_work_order_id",
                schema: "operations",
                table: "work_orders",
                columns: new[] { "tenant_id", "previous_work_order_id" });

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_tenant_id_scheduled_start_scheduled_end",
                schema: "operations",
                table: "work_orders",
                columns: new[] { "tenant_id", "scheduled_start", "scheduled_end" });

            migrationBuilder.CreateIndex(
                name: "ix_work_orders_tenant_id_superseded_by_work_order_id",
                schema: "operations",
                table: "work_orders",
                columns: new[] { "tenant_id", "superseded_by_work_order_id" });

            migrationBuilder.Sql(
                """
                ALTER TABLE operations.work_orders
                    ADD CONSTRAINT ck_work_orders_priority CHECK (priority BETWEEN 1 AND 4),
                    ADD CONSTRAINT ck_work_orders_status CHECK (status BETWEEN 1 AND 9),
                    ADD CONSTRAINT ck_work_orders_schedule CHECK (scheduled_end > scheduled_start),
                    ADD CONSTRAINT ck_work_orders_target CHECK (
                        (field_id IS NULL AND boundary_version_id IS NULL AND crop_zone_id IS NULL)
                        OR (field_id IS NOT NULL AND boundary_version_id IS NOT NULL)
                    ),
                    ADD CONSTRAINT ck_work_orders_target_exclusive CHECK (field_id IS NULL OR livestock_subject_id IS NULL),
                    ADD CONSTRAINT ck_work_orders_plan_link CHECK ((crop_plan_id IS NULL) = (planned_operation_id IS NULL)),
                    ADD CONSTRAINT ck_work_orders_replacement CHECK (
                        previous_work_order_id IS NULL OR previous_work_order_id <> id
                    ),
                    ADD CONSTRAINT ck_work_orders_superseded CHECK (
                        (status = 9 AND superseded_by_work_order_id IS NOT NULL)
                        OR (status <> 9 AND superseded_by_work_order_id IS NULL)
                    );

                ALTER TABLE operations.work_order_assignments
                    ADD CONSTRAINT ck_work_order_assignment_close CHECK (
                        (unassigned_at IS NULL AND unassigned_by IS NULL)
                        OR (unassigned_at IS NOT NULL AND unassigned_by IS NOT NULL AND unassigned_at >= assigned_at)
                    );

                ALTER TABLE operations.work_order_resources
                    ADD CONSTRAINT ck_work_order_resource_type CHECK (resource_type BETWEEN 1 AND 6),
                    ADD CONSTRAINT ck_work_order_resource_status CHECK (status BETWEEN 1 AND 3),
                    ADD CONSTRAINT ck_work_order_resource_quantity CHECK (quantity > 0 AND entered_quantity > 0),
                    ADD CONSTRAINT ck_work_order_resource_cost CHECK (unit_cost >= 0),
                    ADD CONSTRAINT ck_work_order_resource_currency CHECK (currency ~ '^[A-Z]{3}$'),
                    ADD CONSTRAINT ck_work_order_resource_input CHECK (
                        (resource_type = 1)
                        OR (agricultural_input_id IS NULL AND stock_lot_id IS NULL)
                    ),
                    ADD CONSTRAINT ck_work_order_resource_stock_lot CHECK (stock_lot_id IS NULL OR agricultural_input_id IS NOT NULL);

                ALTER TABLE operations.work_order_results
                    ADD CONSTRAINT ck_work_order_result_type CHECK (result_type BETWEEN 1 AND 6);

                ALTER TABLE operations.work_order_status_history
                    ADD CONSTRAINT ck_work_order_history_status CHECK (
                        from_status BETWEEN 1 AND 9 AND to_status BETWEEN 1 AND 9 AND from_status <> to_status
                    ),
                    ADD CONSTRAINT ck_work_order_history_transition CHECK (
                        (from_status, to_status) IN ((1,2),(1,8),(2,3),(2,8),(3,2),(3,4),(3,8),
                            (4,5),(5,6),(5,7),(7,3),(7,8),(6,9))
                    );

                CREATE OR REPLACE FUNCTION operations.guard_work_order()
                RETURNS trigger LANGUAGE plpgsql SET search_path = pg_catalog, operations
                AS $function$
                BEGIN
                    IF TG_OP = 'DELETE' THEN
                        RAISE EXCEPTION 'Work orders are retained; cancel or supersede them instead.' USING ERRCODE = '55000';
                    END IF;
                    IF TG_OP = 'INSERT' THEN
                        IF NEW.status NOT IN (1, 2) OR (NEW.status = 2 AND NEW.planned_at IS NULL) THEN
                            RAISE EXCEPTION 'A work order must be inserted as draft or planned.' USING ERRCODE = '23514';
                        END IF;
                        RETURN NEW;
                    END IF;
                    IF (to_jsonb(NEW) - ARRAY['title','work_type','priority','scheduled_start','scheduled_end','instructions',
                        'status','planned_at','assigned_at','started_at','completed_at','verified_at','verified_by',
                        'rejected_at','rejected_by','rejection_reason','cancelled_at','cancelled_by','cancellation_reason',
                        'completion_notes','superseded_by_work_order_id','updated_at_utc','version']) IS DISTINCT FROM
                       (to_jsonb(OLD) - ARRAY['title','work_type','priority','scheduled_start','scheduled_end','instructions',
                        'status','planned_at','assigned_at','started_at','completed_at','verified_at','verified_by',
                        'rejected_at','rejected_by','rejection_reason','cancelled_at','cancelled_by','cancellation_reason',
                        'completion_notes','superseded_by_work_order_id','updated_at_utc','version']) THEN
                        RAISE EXCEPTION 'Work-order identity, scope, plan link, and correction lineage are immutable.' USING ERRCODE = '55000';
                    END IF;
                    IF OLD.status = NEW.status THEN
                        IF NEW.status NOT IN (1,2) THEN
                            RAISE EXCEPTION 'Only draft and planned work orders can be edited without a lifecycle transition.' USING ERRCODE = '55000';
                        END IF;
                        RETURN NEW;
                    END IF;
                    IF NOT ((OLD.status, NEW.status) IN ((1,2),(1,8),(2,3),(2,8),(3,2),(3,4),(3,8),
                        (4,5),(5,6),(5,7),(7,3),(7,8),(6,9))) THEN
                        RAISE EXCEPTION 'Invalid work-order lifecycle transition from % to %.', OLD.status, NEW.status USING ERRCODE = '55000';
                    END IF;
                    IF NEW.status = 2 AND NEW.planned_at IS NULL
                       OR NEW.status = 3 AND NEW.assigned_at IS NULL
                       OR NEW.status = 4 AND NEW.started_at IS NULL
                       OR NEW.status = 5 AND NEW.completed_at IS NULL
                       OR NEW.status = 6 AND (NEW.verified_at IS NULL OR nullif(btrim(NEW.verified_by), '') IS NULL)
                       OR NEW.status = 7 AND (NEW.rejected_at IS NULL OR nullif(btrim(NEW.rejected_by), '') IS NULL OR nullif(btrim(NEW.rejection_reason), '') IS NULL)
                       OR NEW.status = 8 AND (NEW.cancelled_at IS NULL OR nullif(btrim(NEW.cancelled_by), '') IS NULL OR nullif(btrim(NEW.cancellation_reason), '') IS NULL)
                       OR NEW.status = 9 AND NEW.superseded_by_work_order_id IS NULL THEN
                        RAISE EXCEPTION 'Work-order lifecycle metadata is incomplete.' USING ERRCODE = '23514';
                    END IF;
                    RETURN NEW;
                END;
                $function$;
                CREATE TRIGGER work_order_guard BEFORE INSERT OR UPDATE OR DELETE ON operations.work_orders
                    FOR EACH ROW EXECUTE FUNCTION operations.guard_work_order();

                CREATE OR REPLACE FUNCTION operations.guard_work_order_assignment()
                RETURNS trigger LANGUAGE plpgsql SET search_path = pg_catalog, operations
                AS $function$
                DECLARE parent_status integer;
                BEGIN
                    IF TG_OP = 'DELETE' THEN RAISE EXCEPTION 'Assignment history is retained.' USING ERRCODE = '55000'; END IF;
                    SELECT status INTO parent_status FROM operations.work_orders WHERE tenant_id = NEW.tenant_id AND id = NEW.work_order_id;
                    IF TG_OP = 'INSERT' THEN
                        IF parent_status NOT IN (2,3) OR NEW.unassigned_at IS NOT NULL THEN
                            RAISE EXCEPTION 'Assignments can only be added to planned or assigned work.' USING ERRCODE = '23514';
                        END IF;
                        RETURN NEW;
                    END IF;
                    IF OLD.unassigned_at IS NOT NULL
                       OR NEW.unassigned_at IS NULL OR NEW.unassigned_by IS NULL
                       OR (to_jsonb(NEW) - ARRAY['unassigned_at','unassigned_by','updated_at_utc','version']) IS DISTINCT FROM
                          (to_jsonb(OLD) - ARRAY['unassigned_at','unassigned_by','updated_at_utc','version']) THEN
                        RAISE EXCEPTION 'Assignments are append-only and may only be closed once.' USING ERRCODE = '55000';
                    END IF;
                    RETURN NEW;
                END;
                $function$;
                CREATE TRIGGER work_order_assignment_guard BEFORE INSERT OR UPDATE OR DELETE ON operations.work_order_assignments
                    FOR EACH ROW EXECUTE FUNCTION operations.guard_work_order_assignment();

                CREATE OR REPLACE FUNCTION operations.guard_work_order_resource()
                RETURNS trigger LANGUAGE plpgsql SET search_path = pg_catalog, operations, inventory
                AS $function$
                DECLARE parent_status integer; lot_input uuid; on_hand numeric; reserved numeric;
                BEGIN
                    IF TG_OP = 'DELETE' THEN RAISE EXCEPTION 'Work-order resources are retained.' USING ERRCODE = '55000'; END IF;
                    SELECT status INTO parent_status FROM operations.work_orders WHERE tenant_id = NEW.tenant_id AND id = NEW.work_order_id;
                    IF TG_OP = 'INSERT' AND (parent_status NOT IN (1,2,3) OR NEW.status <> 1) THEN
                        RAISE EXCEPTION 'Resources must be reserved before work starts.' USING ERRCODE = '23514';
                    ELSIF TG_OP = 'UPDATE' AND (OLD.status <> 1 OR NEW.status NOT IN (2,3)
                        OR (to_jsonb(NEW) - ARRAY['status','updated_at_utc','version']) IS DISTINCT FROM
                           (to_jsonb(OLD) - ARRAY['status','updated_at_utc','version'])) THEN
                        RAISE EXCEPTION 'A resource reservation may only be consumed or released once.' USING ERRCODE = '55000';
                    END IF;
                    IF NEW.stock_lot_id IS NOT NULL AND NEW.status = 1 THEN
                        PERFORM pg_advisory_xact_lock(hashtextextended(NEW.tenant_id::text || NEW.stock_lot_id::text, 0));
                        SELECT input_id, quantity_on_hand INTO lot_input, on_hand
                          FROM inventory.input_stock_lots WHERE tenant_id = NEW.tenant_id AND id = NEW.stock_lot_id;
                        IF lot_input IS NULL OR lot_input <> NEW.agricultural_input_id THEN
                            RAISE EXCEPTION 'The reserved stock lot does not belong to the selected input.' USING ERRCODE = '23514';
                        END IF;
                        SELECT coalesce(sum(quantity), 0) INTO reserved
                          FROM operations.work_order_resources
                         WHERE tenant_id = NEW.tenant_id AND stock_lot_id = NEW.stock_lot_id AND status = 1 AND id <> NEW.id;
                        IF reserved + NEW.quantity > on_hand THEN
                            RAISE EXCEPTION 'The reservation exceeds stock on hand.' USING ERRCODE = '23514';
                        END IF;
                    END IF;
                    RETURN NEW;
                END;
                $function$;
                CREATE TRIGGER work_order_resource_guard BEFORE INSERT OR UPDATE OR DELETE ON operations.work_order_resources
                    FOR EACH ROW EXECUTE FUNCTION operations.guard_work_order_resource();

                CREATE OR REPLACE FUNCTION operations.validate_work_order_child_insert()
                RETURNS trigger LANGUAGE plpgsql SET search_path = pg_catalog, operations
                AS $function$
                DECLARE parent_status integer;
                BEGIN
                    SELECT status INTO parent_status FROM operations.work_orders WHERE tenant_id = NEW.tenant_id AND id = NEW.work_order_id;
                    IF TG_TABLE_NAME = 'work_order_results' AND parent_status NOT IN (4,5) THEN
                        RAISE EXCEPTION 'Results can only be attached while completing in-progress work.' USING ERRCODE = '23514';
                    END IF;
                    RETURN NEW;
                END;
                $function$;
                CREATE TRIGGER work_order_result_validate BEFORE INSERT ON operations.work_order_results
                    FOR EACH ROW EXECUTE FUNCTION operations.validate_work_order_child_insert();
                CREATE TRIGGER work_order_results_immutable BEFORE UPDATE OR DELETE ON operations.work_order_results
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER work_order_history_immutable BEFORE UPDATE OR DELETE ON operations.work_order_status_history
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();

                DO $rls$
                DECLARE table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY['work_orders', 'work_order_assignments', 'work_order_resources',
                        'work_order_results', 'work_order_status_history'] LOOP
                        EXECUTE format('ALTER TABLE operations.%I ENABLE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('ALTER TABLE operations.%I FORCE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('CREATE POLICY tenant_isolation ON operations.%I USING (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid) WITH CHECK (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid)', table_name);
                    END LOOP;
                END
                $rls$;

                DO $grant$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        GRANT USAGE ON SCHEMA operations TO murimios_app;
                        GRANT SELECT, INSERT, UPDATE ON operations.work_orders, operations.work_order_assignments,
                            operations.work_order_resources, operations.work_order_results, operations.work_order_status_history TO murimios_app;
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
                DROP TRIGGER IF EXISTS work_order_result_validate ON operations.work_order_results;
                DROP TRIGGER IF EXISTS work_order_results_immutable ON operations.work_order_results;
                DROP TRIGGER IF EXISTS work_order_history_immutable ON operations.work_order_status_history;
                DROP TRIGGER IF EXISTS work_order_resource_guard ON operations.work_order_resources;
                DROP TRIGGER IF EXISTS work_order_assignment_guard ON operations.work_order_assignments;
                DROP TRIGGER IF EXISTS work_order_guard ON operations.work_orders;
                DROP FUNCTION IF EXISTS operations.validate_work_order_child_insert();
                DROP FUNCTION IF EXISTS operations.guard_work_order_resource();
                DROP FUNCTION IF EXISTS operations.guard_work_order_assignment();
                DROP FUNCTION IF EXISTS operations.guard_work_order();
                """);
            migrationBuilder.DropTable(
                name: "work_order_assignments",
                schema: "operations");

            migrationBuilder.DropTable(
                name: "work_order_resources",
                schema: "operations");

            migrationBuilder.DropTable(
                name: "work_order_results",
                schema: "operations");

            migrationBuilder.DropTable(
                name: "work_order_status_history",
                schema: "operations");

            migrationBuilder.DropTable(
                name: "work_orders",
                schema: "operations");
        }
    }
}
