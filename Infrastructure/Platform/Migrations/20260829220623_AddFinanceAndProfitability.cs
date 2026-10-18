using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddFinanceAndProfitability : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "finance");

            migrationBuilder.CreateTable(
                name: "accounting_imports",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    external_batch_id = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    statement_start = table.Column<DateOnly>(type: "date", nullable: false),
                    statement_end = table.Column<DateOnly>(type: "date", nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    imported_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    imported_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    reconciled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    reconciled_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accounting_imports", x => x.id);
                    table.UniqueConstraint("ak_finance_accounting_imports_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_accounting_imports_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cost_centres",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: true),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    closed_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cost_centres", x => x.id);
                    table.UniqueConstraint("ak_finance_cost_centres_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_cost_centres_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transactions",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entry_type = table.Column<int>(type: "integer", nullable: false),
                    category = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    source_type = table.Column<int>(type: "integer", nullable: false),
                    source_id = table.Column<Guid>(type: "uuid", nullable: true),
                    reference = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    counterparty = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    description = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    posted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    posted_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    reversal_of_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transactions", x => x.id);
                    table.UniqueConstraint("ak_finance_transactions_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_transactions_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_transactions_transactions_tenant_id_reversal_of_id",
                        columns: x => new { x.tenant_id, x.reversal_of_id },
                        principalSchema: "finance",
                        principalTable: "transactions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "accounting_statement_lines",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_line_id = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    entry_type = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    account_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    reference = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accounting_statement_lines", x => x.id);
                    table.UniqueConstraint("ak_finance_statement_lines_tenant_batch_id", x => new { x.tenant_id, x.batch_id, x.id });
                    table.ForeignKey(
                        name: "fk_accounting_statement_lines_accounting_imports_tenant_id_bat",
                        columns: x => new { x.tenant_id, x.batch_id },
                        principalSchema: "finance",
                        principalTable: "accounting_imports",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "cost_allocations",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cost_centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    percentage = table.Column<decimal>(type: "numeric(9,6)", precision: 9, scale: 6, nullable: false),
                    method = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_cost_allocations", x => x.id);
                    table.ForeignKey(
                        name: "fk_cost_allocations_cost_centres_tenant_id_cost_centre_id",
                        columns: x => new { x.tenant_id, x.cost_centre_id },
                        principalSchema: "finance",
                        principalTable: "cost_centres",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_cost_allocations_transactions_tenant_id_transaction_id",
                        columns: x => new { x.tenant_id, x.transaction_id },
                        principalSchema: "finance",
                        principalTable: "transactions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "labour_entries",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cost_centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_operation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    crop_zone_id = table.Column<Guid>(type: "uuid", nullable: true),
                    livestock_subject_id = table.Column<Guid>(type: "uuid", nullable: true),
                    worker_reference = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    duration_minutes = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    entered_duration_hours = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    hourly_rate = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    total_cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_labour_entries", x => x.id);
                    table.ForeignKey(
                        name: "fk_labour_entries_cost_centres_tenant_id_cost_centre_id",
                        columns: x => new { x.tenant_id, x.cost_centre_id },
                        principalSchema: "finance",
                        principalTable: "cost_centres",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_labour_entries_crop_zones_tenant_id_crop_zone_id",
                        columns: x => new { x.tenant_id, x.crop_zone_id },
                        principalSchema: "production",
                        principalTable: "crop_zones",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_labour_entries_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_labour_entries_field_operations_tenant_id_field_operation_id",
                        columns: x => new { x.tenant_id, x.field_operation_id },
                        principalSchema: "operations",
                        principalTable: "field_operations",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_labour_entries_livestock_subjects_tenant_id_livestock_subje",
                        columns: x => new { x.tenant_id, x.livestock_subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_labour_entries_transactions_tenant_id_transaction_id",
                        columns: x => new { x.tenant_id, x.transaction_id },
                        principalSchema: "finance",
                        principalTable: "transactions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "machinery_entries",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    cost_centre_id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_operation_id = table.Column<Guid>(type: "uuid", nullable: true),
                    crop_zone_id = table.Column<Guid>(type: "uuid", nullable: true),
                    livestock_subject_id = table.Column<Guid>(type: "uuid", nullable: true),
                    machine_reference = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    duration_minutes = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    entered_duration_hours = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    hourly_rate = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    additional_cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    total_cost = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_machinery_entries", x => x.id);
                    table.ForeignKey(
                        name: "fk_machinery_entries_cost_centres_tenant_id_cost_centre_id",
                        columns: x => new { x.tenant_id, x.cost_centre_id },
                        principalSchema: "finance",
                        principalTable: "cost_centres",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_machinery_entries_crop_zones_tenant_id_crop_zone_id",
                        columns: x => new { x.tenant_id, x.crop_zone_id },
                        principalSchema: "production",
                        principalTable: "crop_zones",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_machinery_entries_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_machinery_entries_field_operations_tenant_id_field_operatio",
                        columns: x => new { x.tenant_id, x.field_operation_id },
                        principalSchema: "operations",
                        principalTable: "field_operations",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_machinery_entries_livestock_subjects_tenant_id_livestock_su",
                        columns: x => new { x.tenant_id, x.livestock_subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_machinery_entries_transactions_tenant_id_transaction_id",
                        columns: x => new { x.tenant_id, x.transaction_id },
                        principalSchema: "finance",
                        principalTable: "transactions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "accounting_reconciliations",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    statement_line_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    difference = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    tolerance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    matched_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    matched_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    notes = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_accounting_reconciliations", x => x.id);
                    table.ForeignKey(
                        name: "fk_accounting_reconciliations_accounting_imports_tenant_id_bat",
                        columns: x => new { x.tenant_id, x.batch_id },
                        principalSchema: "finance",
                        principalTable: "accounting_imports",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accounting_reconciliations_accounting_statement_lines_tenan",
                        columns: x => new { x.tenant_id, x.batch_id, x.statement_line_id },
                        principalSchema: "finance",
                        principalTable: "accounting_statement_lines",
                        principalColumns: new[] { "tenant_id", "batch_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_accounting_reconciliations_transactions_tenant_id_transacti",
                        columns: x => new { x.tenant_id, x.transaction_id },
                        principalSchema: "finance",
                        principalTable: "transactions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_accounting_imports_tenant_id",
                schema: "finance",
                table: "accounting_imports",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounting_imports_tenant_id_farm_id_provider_external_batc",
                schema: "finance",
                table: "accounting_imports",
                columns: new[] { "tenant_id", "farm_id", "provider", "external_batch_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_accounting_reconciliations_tenant_id",
                schema: "finance",
                table: "accounting_reconciliations",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounting_reconciliations_tenant_id_batch_id_statement_lin",
                schema: "finance",
                table: "accounting_reconciliations",
                columns: new[] { "tenant_id", "batch_id", "statement_line_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_accounting_reconciliations_tenant_id_transaction_id",
                schema: "finance",
                table: "accounting_reconciliations",
                columns: new[] { "tenant_id", "transaction_id" },
                unique: true,
                filter: "transaction_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_accounting_statement_lines_tenant_id",
                schema: "finance",
                table: "accounting_statement_lines",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_accounting_statement_lines_tenant_id_batch_id_external_line",
                schema: "finance",
                table: "accounting_statement_lines",
                columns: new[] { "tenant_id", "batch_id", "external_line_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cost_allocations_tenant_id",
                schema: "finance",
                table: "cost_allocations",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_cost_allocations_tenant_id_cost_centre_id_transaction_id",
                schema: "finance",
                table: "cost_allocations",
                columns: new[] { "tenant_id", "cost_centre_id", "transaction_id" });

            migrationBuilder.CreateIndex(
                name: "ix_cost_allocations_tenant_id_transaction_id_cost_centre_id",
                schema: "finance",
                table: "cost_allocations",
                columns: new[] { "tenant_id", "transaction_id", "cost_centre_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cost_centres_tenant_id",
                schema: "finance",
                table: "cost_centres",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_cost_centres_tenant_id_farm_id_code",
                schema: "finance",
                table: "cost_centres",
                columns: new[] { "tenant_id", "farm_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_cost_centres_tenant_id_type_target_id",
                schema: "finance",
                table: "cost_centres",
                columns: new[] { "tenant_id", "type", "target_id" });

            migrationBuilder.CreateIndex(
                name: "ix_labour_entries_tenant_id",
                schema: "finance",
                table: "labour_entries",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_labour_entries_tenant_id_cost_centre_id",
                schema: "finance",
                table: "labour_entries",
                columns: new[] { "tenant_id", "cost_centre_id" });

            migrationBuilder.CreateIndex(
                name: "ix_labour_entries_tenant_id_crop_zone_id",
                schema: "finance",
                table: "labour_entries",
                columns: new[] { "tenant_id", "crop_zone_id" });

            migrationBuilder.CreateIndex(
                name: "ix_labour_entries_tenant_id_farm_id_occurred_at",
                schema: "finance",
                table: "labour_entries",
                columns: new[] { "tenant_id", "farm_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_labour_entries_tenant_id_field_operation_id",
                schema: "finance",
                table: "labour_entries",
                columns: new[] { "tenant_id", "field_operation_id" });

            migrationBuilder.CreateIndex(
                name: "ix_labour_entries_tenant_id_livestock_subject_id",
                schema: "finance",
                table: "labour_entries",
                columns: new[] { "tenant_id", "livestock_subject_id" });

            migrationBuilder.CreateIndex(
                name: "ix_labour_entries_tenant_id_transaction_id",
                schema: "finance",
                table: "labour_entries",
                columns: new[] { "tenant_id", "transaction_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_machinery_entries_tenant_id",
                schema: "finance",
                table: "machinery_entries",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_machinery_entries_tenant_id_cost_centre_id",
                schema: "finance",
                table: "machinery_entries",
                columns: new[] { "tenant_id", "cost_centre_id" });

            migrationBuilder.CreateIndex(
                name: "ix_machinery_entries_tenant_id_crop_zone_id",
                schema: "finance",
                table: "machinery_entries",
                columns: new[] { "tenant_id", "crop_zone_id" });

            migrationBuilder.CreateIndex(
                name: "ix_machinery_entries_tenant_id_farm_id_occurred_at",
                schema: "finance",
                table: "machinery_entries",
                columns: new[] { "tenant_id", "farm_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_machinery_entries_tenant_id_field_operation_id",
                schema: "finance",
                table: "machinery_entries",
                columns: new[] { "tenant_id", "field_operation_id" });

            migrationBuilder.CreateIndex(
                name: "ix_machinery_entries_tenant_id_livestock_subject_id",
                schema: "finance",
                table: "machinery_entries",
                columns: new[] { "tenant_id", "livestock_subject_id" });

            migrationBuilder.CreateIndex(
                name: "ix_machinery_entries_tenant_id_transaction_id",
                schema: "finance",
                table: "machinery_entries",
                columns: new[] { "tenant_id", "transaction_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_transactions_tenant_id",
                schema: "finance",
                table: "transactions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_tenant_id_farm_id_occurred_at",
                schema: "finance",
                table: "transactions",
                columns: new[] { "tenant_id", "farm_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_transactions_tenant_id_farm_id_reference",
                schema: "finance",
                table: "transactions",
                columns: new[] { "tenant_id", "farm_id", "reference" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_transactions_tenant_id_reversal_of_id",
                schema: "finance",
                table: "transactions",
                columns: new[] { "tenant_id", "reversal_of_id" },
                unique: true,
                filter: "reversal_of_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_transactions_tenant_id_source_type_source_id",
                schema: "finance",
                table: "transactions",
                columns: new[] { "tenant_id", "source_type", "source_id" },
                unique: true,
                filter: "source_id IS NOT NULL");

            migrationBuilder.Sql(
                """
                ALTER TABLE finance.cost_centres
                    ADD CONSTRAINT ck_finance_cost_centre_type CHECK (type BETWEEN 1 AND 5),
                    ADD CONSTRAINT ck_finance_cost_centre_target CHECK ((type = 5 AND target_id IS NULL) OR (type <> 5 AND target_id IS NOT NULL)),
                    ADD CONSTRAINT ck_finance_cost_centre_currency CHECK (currency ~ '^[A-Z]{3}$'),
                    ADD CONSTRAINT ck_finance_cost_centre_close CHECK (
                        (is_active AND closed_at IS NULL AND closed_by IS NULL)
                        OR (NOT is_active AND closed_at IS NOT NULL AND nullif(btrim(closed_by), '') IS NOT NULL));
                ALTER TABLE finance.transactions
                    ADD CONSTRAINT ck_finance_transaction_type CHECK (entry_type IN (1, 2)),
                    ADD CONSTRAINT ck_finance_transaction_source CHECK (source_type BETWEEN 1 AND 7),
                    ADD CONSTRAINT ck_finance_transaction_amount CHECK (amount > 0),
                    ADD CONSTRAINT ck_finance_transaction_currency CHECK (currency ~ '^[A-Z]{3}$'),
                    ADD CONSTRAINT ck_finance_transaction_reversal CHECK (
                        (source_type = 6 AND reversal_of_id IS NOT NULL AND source_id = reversal_of_id)
                        OR (source_type <> 6 AND reversal_of_id IS NULL));
                ALTER TABLE finance.cost_allocations
                    ADD CONSTRAINT ck_finance_allocation_amount CHECK (amount > 0),
                    ADD CONSTRAINT ck_finance_allocation_percentage CHECK (percentage > 0 AND percentage <= 100),
                    ADD CONSTRAINT ck_finance_allocation_method CHECK (method BETWEEN 1 AND 5);
                ALTER TABLE finance.labour_entries
                    ADD CONSTRAINT ck_finance_labour_duration CHECK (duration_minutes > 0 AND entered_duration_hours > 0),
                    ADD CONSTRAINT ck_finance_labour_cost CHECK (hourly_rate >= 0 AND total_cost > 0),
                    ADD CONSTRAINT ck_finance_labour_currency CHECK (currency ~ '^[A-Z]{3}$'),
                    ADD CONSTRAINT ck_finance_labour_target CHECK (crop_zone_id IS NULL OR livestock_subject_id IS NULL);
                ALTER TABLE finance.machinery_entries
                    ADD CONSTRAINT ck_finance_machinery_duration CHECK (duration_minutes > 0 AND entered_duration_hours > 0),
                    ADD CONSTRAINT ck_finance_machinery_cost CHECK (hourly_rate >= 0 AND additional_cost >= 0 AND total_cost > 0),
                    ADD CONSTRAINT ck_finance_machinery_currency CHECK (currency ~ '^[A-Z]{3}$'),
                    ADD CONSTRAINT ck_finance_machinery_target CHECK (crop_zone_id IS NULL OR livestock_subject_id IS NULL);
                ALTER TABLE finance.accounting_imports
                    ADD CONSTRAINT ck_finance_accounting_period CHECK (statement_end >= statement_start),
                    ADD CONSTRAINT ck_finance_accounting_currency CHECK (currency ~ '^[A-Z]{3}$'),
                    ADD CONSTRAINT ck_finance_accounting_status CHECK (status BETWEEN 1 AND 3),
                    ADD CONSTRAINT ck_finance_accounting_completion CHECK (
                        (status = 1 AND reconciled_at IS NULL AND reconciled_by IS NULL)
                        OR (status IN (2, 3) AND reconciled_at IS NOT NULL AND nullif(btrim(reconciled_by), '') IS NOT NULL));
                ALTER TABLE finance.accounting_statement_lines
                    ADD CONSTRAINT ck_finance_statement_type CHECK (entry_type IN (1, 2)),
                    ADD CONSTRAINT ck_finance_statement_amount CHECK (amount > 0),
                    ADD CONSTRAINT ck_finance_statement_currency CHECK (currency ~ '^[A-Z]{3}$');
                ALTER TABLE finance.accounting_reconciliations
                    ADD CONSTRAINT ck_finance_reconciliation_status CHECK (status IN (1, 2)),
                    ADD CONSTRAINT ck_finance_reconciliation_tolerance CHECK (tolerance >= 0),
                    ADD CONSTRAINT ck_finance_reconciliation_match CHECK (
                        (status = 1 AND transaction_id IS NOT NULL AND abs(difference) <= tolerance)
                        OR (status = 2 AND transaction_id IS NULL));

                CREATE OR REPLACE FUNCTION finance.guard_cost_centre()
                RETURNS trigger LANGUAGE plpgsql
                SET search_path = pg_catalog, finance, farm, production, livestock
                AS $function$
                BEGIN
                    IF TG_OP = 'DELETE' THEN
                        RAISE EXCEPTION 'Cost centres are retained and cannot be deleted.' USING ERRCODE = '55000';
                    END IF;
                    IF TG_OP = 'INSERT' THEN
                        IF NEW.type = 1 AND NEW.target_id <> NEW.farm_id THEN
                            RAISE EXCEPTION 'Farm cost centre target is invalid.' USING ERRCODE = '23514';
                        ELSIF NEW.type = 2 AND NOT EXISTS (SELECT 1 FROM farm.fields f WHERE f.tenant_id = NEW.tenant_id AND f.id = NEW.target_id AND f.farm_id = NEW.farm_id) THEN
                            RAISE EXCEPTION 'Field cost centre target is invalid.' USING ERRCODE = '23514';
                        ELSIF NEW.type = 3 AND NOT EXISTS (SELECT 1 FROM production.crop_zones z JOIN production.seasons s ON s.tenant_id = z.tenant_id AND s.id = z.season_id WHERE z.tenant_id = NEW.tenant_id AND z.id = NEW.target_id AND s.farm_id = NEW.farm_id) THEN
                            RAISE EXCEPTION 'Crop-zone cost centre target is invalid.' USING ERRCODE = '23514';
                        ELSIF NEW.type = 4 AND NOT EXISTS (SELECT 1 FROM livestock.subjects s WHERE s.tenant_id = NEW.tenant_id AND s.id = NEW.target_id AND s.farm_id = NEW.farm_id) THEN
                            RAISE EXCEPTION 'Livestock cost centre target is invalid.' USING ERRCODE = '23514';
                        END IF;
                        RETURN NEW;
                    END IF;
                    IF (to_jsonb(NEW) - ARRAY['is_active', 'closed_at', 'closed_by', 'updated_at_utc', 'version']) IS DISTINCT FROM
                       (to_jsonb(OLD) - ARRAY['is_active', 'closed_at', 'closed_by', 'updated_at_utc', 'version'])
                       OR NOT OLD.is_active OR NEW.is_active OR NEW.closed_at IS NULL OR nullif(btrim(NEW.closed_by), '') IS NULL
                       OR NEW.version <> OLD.version + 1 OR NEW.updated_at_utc < OLD.updated_at_utc THEN
                        RAISE EXCEPTION 'A cost centre may only transition once from active to closed.' USING ERRCODE = '55000';
                    END IF;
                    RETURN NEW;
                END;
                $function$;
                CREATE TRIGGER finance_cost_centre_guard BEFORE INSERT OR UPDATE OR DELETE ON finance.cost_centres
                    FOR EACH ROW EXECUTE FUNCTION finance.guard_cost_centre();

                CREATE OR REPLACE FUNCTION finance.validate_allocation()
                RETURNS trigger LANGUAGE plpgsql SET search_path = pg_catalog, finance
                AS $function$
                DECLARE transaction_row finance.transactions%ROWTYPE; centre_row finance.cost_centres%ROWTYPE;
                BEGIN
                    SELECT * INTO transaction_row FROM finance.transactions WHERE tenant_id = NEW.tenant_id AND id = NEW.transaction_id;
                    SELECT * INTO centre_row FROM finance.cost_centres WHERE tenant_id = NEW.tenant_id AND id = NEW.cost_centre_id;
                    IF transaction_row.id IS NULL OR centre_row.id IS NULL
                       OR transaction_row.farm_id <> centre_row.farm_id OR transaction_row.currency <> centre_row.currency THEN
                        RAISE EXCEPTION 'Allocation transaction and cost centre must share farm and currency.' USING ERRCODE = '23514';
                    END IF;
                    RETURN NEW;
                END;
                $function$;
                CREATE TRIGGER finance_allocation_validate BEFORE INSERT ON finance.cost_allocations
                    FOR EACH ROW EXECUTE FUNCTION finance.validate_allocation();

                CREATE OR REPLACE FUNCTION finance.validate_transaction_balance()
                RETURNS trigger LANGUAGE plpgsql SET search_path = pg_catalog, finance
                AS $function$
                DECLARE target_tenant uuid; target_transaction uuid; transaction_amount numeric; allocated_amount numeric; allocated_percentage numeric;
                BEGIN
                    IF TG_TABLE_NAME = 'transactions' THEN target_tenant := NEW.tenant_id; target_transaction := NEW.id;
                    ELSE target_tenant := NEW.tenant_id; target_transaction := NEW.transaction_id; END IF;
                    SELECT amount INTO transaction_amount FROM finance.transactions WHERE tenant_id = target_tenant AND id = target_transaction;
                    IF transaction_amount IS NULL THEN RETURN NULL; END IF;
                    SELECT COALESCE(sum(amount), 0), COALESCE(sum(percentage), 0)
                      INTO allocated_amount, allocated_percentage
                      FROM finance.cost_allocations WHERE tenant_id = target_tenant AND transaction_id = target_transaction;
                    IF allocated_amount <> transaction_amount OR abs(allocated_percentage - 100) > 0.0001 THEN
                        RAISE EXCEPTION 'Finance transaction allocations must balance amount and 100 percent.' USING ERRCODE = '23514';
                    END IF;
                    RETURN NULL;
                END;
                $function$;
                CREATE CONSTRAINT TRIGGER finance_transaction_balance AFTER INSERT ON finance.transactions
                    DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE FUNCTION finance.validate_transaction_balance();
                CREATE CONSTRAINT TRIGGER finance_allocation_balance AFTER INSERT ON finance.cost_allocations
                    DEFERRABLE INITIALLY DEFERRED FOR EACH ROW EXECUTE FUNCTION finance.validate_transaction_balance();

                CREATE OR REPLACE FUNCTION finance.validate_operational_cost()
                RETURNS trigger LANGUAGE plpgsql SET search_path = pg_catalog, finance
                AS $function$
                DECLARE transaction_row finance.transactions%ROWTYPE; expected_source integer;
                BEGIN
                    expected_source := CASE WHEN TG_TABLE_NAME = 'labour_entries' THEN 2 ELSE 3 END;
                    SELECT * INTO transaction_row FROM finance.transactions WHERE tenant_id = NEW.tenant_id AND id = NEW.transaction_id;
                    IF NOT FOUND OR transaction_row.farm_id <> NEW.farm_id OR transaction_row.currency <> NEW.currency
                       OR transaction_row.entry_type <> 1 OR transaction_row.source_type <> expected_source
                       OR transaction_row.source_id <> NEW.id OR transaction_row.amount <> NEW.total_cost THEN
                        RAISE EXCEPTION 'Operational cost must match its expense transaction.' USING ERRCODE = '23514';
                    END IF;
                    RETURN NEW;
                END;
                $function$;
                CREATE TRIGGER finance_labour_validate BEFORE INSERT ON finance.labour_entries
                    FOR EACH ROW EXECUTE FUNCTION finance.validate_operational_cost();
                CREATE TRIGGER finance_machinery_validate BEFORE INSERT ON finance.machinery_entries
                    FOR EACH ROW EXECUTE FUNCTION finance.validate_operational_cost();

                CREATE OR REPLACE FUNCTION finance.guard_accounting_import()
                RETURNS trigger LANGUAGE plpgsql SET search_path = pg_catalog, finance
                AS $function$
                BEGIN
                    IF TG_OP = 'DELETE' THEN RAISE EXCEPTION 'Accounting imports are retained.' USING ERRCODE = '55000'; END IF;
                    IF TG_OP = 'INSERT' THEN
                        IF NEW.status <> 1 THEN RAISE EXCEPTION 'Accounting imports must start imported.' USING ERRCODE = '23514'; END IF;
                        RETURN NEW;
                    END IF;
                    IF (to_jsonb(NEW) - ARRAY['status', 'reconciled_at', 'reconciled_by', 'updated_at_utc', 'version']) IS DISTINCT FROM
                       (to_jsonb(OLD) - ARRAY['status', 'reconciled_at', 'reconciled_by', 'updated_at_utc', 'version'])
                       OR OLD.status <> 1 OR NEW.status NOT IN (2, 3) OR NEW.reconciled_at IS NULL
                       OR nullif(btrim(NEW.reconciled_by), '') IS NULL OR NEW.version <> OLD.version + 1 THEN
                        RAISE EXCEPTION 'Invalid accounting reconciliation lifecycle.' USING ERRCODE = '55000';
                    END IF;
                    RETURN NEW;
                END;
                $function$;
                CREATE TRIGGER finance_accounting_import_guard BEFORE INSERT OR UPDATE OR DELETE ON finance.accounting_imports
                    FOR EACH ROW EXECUTE FUNCTION finance.guard_accounting_import();

                CREATE OR REPLACE FUNCTION finance.validate_statement_line()
                RETURNS trigger LANGUAGE plpgsql SET search_path = pg_catalog, finance
                AS $function$
                DECLARE batch_row finance.accounting_imports%ROWTYPE;
                BEGIN
                    SELECT * INTO batch_row FROM finance.accounting_imports WHERE tenant_id = NEW.tenant_id AND id = NEW.batch_id;
                    IF NOT FOUND OR batch_row.currency <> NEW.currency
                       OR NEW.occurred_at::date < batch_row.statement_start OR NEW.occurred_at::date > batch_row.statement_end THEN
                        RAISE EXCEPTION 'Statement line must match its import currency and period.' USING ERRCODE = '23514';
                    END IF;
                    RETURN NEW;
                END;
                $function$;
                CREATE TRIGGER finance_statement_line_validate BEFORE INSERT ON finance.accounting_statement_lines
                    FOR EACH ROW EXECUTE FUNCTION finance.validate_statement_line();

                CREATE OR REPLACE FUNCTION finance.validate_reconciliation()
                RETURNS trigger LANGUAGE plpgsql SET search_path = pg_catalog, finance
                AS $function$
                DECLARE batch_row finance.accounting_imports%ROWTYPE; line_row finance.accounting_statement_lines%ROWTYPE; transaction_row finance.transactions%ROWTYPE;
                BEGIN
                    SELECT * INTO batch_row FROM finance.accounting_imports WHERE tenant_id = NEW.tenant_id AND id = NEW.batch_id;
                    SELECT * INTO line_row FROM finance.accounting_statement_lines WHERE tenant_id = NEW.tenant_id AND batch_id = NEW.batch_id AND id = NEW.statement_line_id;
                    IF batch_row.id IS NULL OR line_row.id IS NULL THEN
                        RAISE EXCEPTION 'Reconciliation batch or statement line is invalid.' USING ERRCODE = '23514';
                    END IF;
                    IF NEW.transaction_id IS NOT NULL THEN
                        SELECT * INTO transaction_row FROM finance.transactions WHERE tenant_id = NEW.tenant_id AND id = NEW.transaction_id;
                        IF transaction_row.id IS NULL OR transaction_row.farm_id <> batch_row.farm_id
                           OR transaction_row.currency <> line_row.currency OR transaction_row.entry_type <> line_row.entry_type
                           OR transaction_row.amount - line_row.amount <> NEW.difference THEN
                            RAISE EXCEPTION 'Reconciled transaction must match statement farm, currency, type, and difference.' USING ERRCODE = '23514';
                        END IF;
                    END IF;
                    RETURN NEW;
                END;
                $function$;
                CREATE TRIGGER finance_reconciliation_validate BEFORE INSERT ON finance.accounting_reconciliations
                    FOR EACH ROW EXECUTE FUNCTION finance.validate_reconciliation();

                CREATE TRIGGER finance_transactions_immutable BEFORE UPDATE OR DELETE ON finance.transactions
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER finance_allocations_immutable BEFORE UPDATE OR DELETE ON finance.cost_allocations
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER finance_labour_immutable BEFORE UPDATE OR DELETE ON finance.labour_entries
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER finance_machinery_immutable BEFORE UPDATE OR DELETE ON finance.machinery_entries
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER finance_statement_lines_immutable BEFORE UPDATE OR DELETE ON finance.accounting_statement_lines
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER finance_reconciliations_immutable BEFORE UPDATE OR DELETE ON finance.accounting_reconciliations
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();

                DO $rls$
                DECLARE table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY['cost_centres', 'transactions', 'cost_allocations',
                        'labour_entries', 'machinery_entries', 'accounting_imports',
                        'accounting_statement_lines', 'accounting_reconciliations'] LOOP
                        EXECUTE format('ALTER TABLE finance.%I ENABLE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('ALTER TABLE finance.%I FORCE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('CREATE POLICY tenant_isolation ON finance.%I USING (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid) WITH CHECK (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid)', table_name);
                    END LOOP;
                END
                $rls$;

                DO $grant$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        GRANT USAGE ON SCHEMA finance TO murimios_app;
                        GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA finance TO murimios_app;
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
                DROP TRIGGER IF EXISTS finance_accounting_import_guard ON finance.accounting_imports;
                DROP TRIGGER IF EXISTS finance_machinery_validate ON finance.machinery_entries;
                DROP TRIGGER IF EXISTS finance_labour_validate ON finance.labour_entries;
                DROP TRIGGER IF EXISTS finance_allocation_balance ON finance.cost_allocations;
                DROP TRIGGER IF EXISTS finance_transaction_balance ON finance.transactions;
                DROP TRIGGER IF EXISTS finance_allocation_validate ON finance.cost_allocations;
                DROP TRIGGER IF EXISTS finance_cost_centre_guard ON finance.cost_centres;
                DROP TRIGGER IF EXISTS finance_statement_line_validate ON finance.accounting_statement_lines;
                DROP TRIGGER IF EXISTS finance_reconciliation_validate ON finance.accounting_reconciliations;
                DROP FUNCTION IF EXISTS finance.validate_reconciliation();
                DROP FUNCTION IF EXISTS finance.validate_statement_line();
                DROP FUNCTION IF EXISTS finance.guard_accounting_import();
                DROP FUNCTION IF EXISTS finance.validate_operational_cost();
                DROP FUNCTION IF EXISTS finance.validate_transaction_balance();
                DROP FUNCTION IF EXISTS finance.validate_allocation();
                DROP FUNCTION IF EXISTS finance.guard_cost_centre();
                """);
            migrationBuilder.DropTable(
                name: "accounting_reconciliations",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "cost_allocations",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "labour_entries",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "machinery_entries",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "accounting_statement_lines",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "cost_centres",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "transactions",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "accounting_imports",
                schema: "finance");
        }
    }
}
