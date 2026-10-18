using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddComplianceAndSupplierWorkflows : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "compliance");

            migrationBuilder.CreateTable(
                name: "hosted_suppliers",
                schema: "supply",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    supplier_code = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    contact_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    phone = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    claimed_tenant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    claimed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_hosted_suppliers", x => x.id);
                    table.UniqueConstraint("ak_hosted_suppliers_tenant_id_id", x => new { x.tenant_id, x.id });
                });

            migrationBuilder.CreateTable(
                name: "standard_versions",
                schema: "compliance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    series_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    scheme = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    version_label = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    revision = table.Column<int>(type: "integer", nullable: false),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_to = table.Column<DateOnly>(type: "date", nullable: true),
                    jurisdiction = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    published_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    published_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    retired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_standard_versions", x => x.id);
                    table.UniqueConstraint("ak_standard_versions_tenant_id_id", x => new { x.tenant_id, x.id });
                });

            migrationBuilder.CreateTable(
                name: "contracts",
                schema: "supply",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contract_number = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    relationship_id = table.Column<Guid>(type: "uuid", nullable: true),
                    hosted_supplier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    commodity = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    valid_from = table.Column<DateOnly>(type: "date", nullable: false),
                    valid_to = table.Column<DateOnly>(type: "date", nullable: false),
                    contracted_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    quantity_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: true),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: true),
                    terms_json = table.Column<string>(type: "jsonb", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    activated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    termination_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contracts", x => x.id);
                    table.UniqueConstraint("ak_contracts_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_contracts_hosted_suppliers_tenant_id_hosted_supplier_id",
                        columns: x => new { x.tenant_id, x.hosted_supplier_id },
                        principalSchema: "supply",
                        principalTable: "hosted_suppliers",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_contracts_relationships_relationship_id",
                        column: x => x.relationship_id,
                        principalSchema: "supply",
                        principalTable: "relationships",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "supplier_invitations",
                schema: "supply",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    supplier_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    token_hash = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    invited_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    invited_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    hosted_supplier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    responded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_supplier_invitations", x => x.id);
                    table.ForeignKey(
                        name: "fk_supplier_invitations_hosted_suppliers_tenant_id_hosted_supp",
                        columns: x => new { x.tenant_id, x.hosted_supplier_id },
                        principalSchema: "supply",
                        principalTable: "hosted_suppliers",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "assessments",
                schema: "compliance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    standard_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    assessment_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    scheduled_on = table.Column<DateOnly>(type: "date", nullable: false),
                    auditor = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    scope_json = table.Column<string>(type: "jsonb", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    submitted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    cancelled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    score_percent = table.Column<decimal>(type: "numeric(8,4)", precision: 8, scale: 4, nullable: true),
                    conforms = table.Column<bool>(type: "boolean", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_assessments", x => x.id);
                    table.UniqueConstraint("ak_assessments_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_assessments_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_assessments_standard_versions_tenant_id_standard_version_id",
                        columns: x => new { x.tenant_id, x.standard_version_id },
                        principalSchema: "compliance",
                        principalTable: "standard_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "control_points",
                schema: "compliance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    standard_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    title = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    applicability = table.Column<int>(type: "integer", nullable: false),
                    weight = table.Column<decimal>(type: "numeric(8,4)", precision: 8, scale: 4, nullable: false),
                    critical = table.Column<bool>(type: "boolean", nullable: false),
                    evaluator_key = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    evaluator_version = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    sort_order = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_control_points", x => x.id);
                    table.UniqueConstraint("ak_control_points_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_control_points_standard_versions_tenant_id_standard_version",
                        columns: x => new { x.tenant_id, x.standard_version_id },
                        principalSchema: "compliance",
                        principalTable: "standard_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contract_allocations",
                schema: "supply",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contract_id = table.Column<Guid>(type: "uuid", nullable: false),
                    reference = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    delivery_from = table.Column<DateOnly>(type: "date", nullable: false),
                    delivery_to = table.Column<DateOnly>(type: "date", nullable: false),
                    allocated_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    fulfilled_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    quantity_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contract_allocations", x => x.id);
                    table.ForeignKey(
                        name: "fk_contract_allocations_contracts_tenant_id_contract_id",
                        columns: x => new { x.tenant_id, x.contract_id },
                        principalSchema: "supply",
                        principalTable: "contracts",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "supplier_evidence_requests",
                schema: "supply",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    relationship_id = table.Column<Guid>(type: "uuid", nullable: true),
                    hosted_supplier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    contract_id = table.Column<Guid>(type: "uuid", nullable: true),
                    standard_version_id = table.Column<Guid>(type: "uuid", nullable: true),
                    control_point_id = table.Column<Guid>(type: "uuid", nullable: true),
                    title = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    due_on = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    evidence_attachment_id = table.Column<Guid>(type: "uuid", nullable: true),
                    supplier_reference = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    review_notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    submitted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    reviewed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_supplier_evidence_requests", x => x.id);
                    table.ForeignKey(
                        name: "fk_supplier_evidence_requests_contracts_tenant_id_contract_id",
                        columns: x => new { x.tenant_id, x.contract_id },
                        principalSchema: "supply",
                        principalTable: "contracts",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_supplier_evidence_requests_hosted_suppliers_tenant_id_hoste",
                        columns: x => new { x.tenant_id, x.hosted_supplier_id },
                        principalSchema: "supply",
                        principalTable: "hosted_suppliers",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_supplier_evidence_requests_relationships_relationship_id",
                        column: x => x.relationship_id,
                        principalSchema: "supply",
                        principalTable: "relationships",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "control_evaluations",
                schema: "compliance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    assessment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    control_point_id = table.Column<Guid>(type: "uuid", nullable: false),
                    result = table.Column<int>(type: "integer", nullable: false),
                    score_percent = table.Column<decimal>(type: "numeric(8,4)", precision: 8, scale: 4, nullable: false),
                    evaluator_version = table.Column<string>(type: "character varying(60)", maxLength: 60, nullable: false),
                    evaluated_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    evaluated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    evidence_snapshot_json = table.Column<string>(type: "jsonb", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_control_evaluations", x => x.id);
                    table.UniqueConstraint("ak_control_evaluations_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_control_evaluations_assessments_tenant_id_assessment_id",
                        columns: x => new { x.tenant_id, x.assessment_id },
                        principalSchema: "compliance",
                        principalTable: "assessments",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_control_evaluations_control_points_tenant_id_control_point_",
                        columns: x => new { x.tenant_id, x.control_point_id },
                        principalSchema: "compliance",
                        principalTable: "control_points",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "evidence_requirements",
                schema: "compliance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    control_point_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    evidence_type = table.Column<int>(type: "integer", nullable: false),
                    required = table.Column<bool>(type: "boolean", nullable: false),
                    minimum_count = table.Column<int>(type: "integer", nullable: false),
                    maximum_age_days = table.Column<int>(type: "integer", nullable: true),
                    instructions = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_evidence_requirements", x => x.id);
                    table.ForeignKey(
                        name: "fk_evidence_requirements_control_points_tenant_id_control_poin",
                        columns: x => new { x.tenant_id, x.control_point_id },
                        principalSchema: "compliance",
                        principalTable: "control_points",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "findings",
                schema: "compliance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    assessment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    control_point_id = table.Column<Guid>(type: "uuid", nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    description = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    due_on = table.Column<DateOnly>(type: "date", nullable: false),
                    opened_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    closed_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    waiver_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_findings", x => x.id);
                    table.UniqueConstraint("ak_findings_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_findings_assessments_tenant_id_assessment_id",
                        columns: x => new { x.tenant_id, x.assessment_id },
                        principalSchema: "compliance",
                        principalTable: "assessments",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_findings_control_points_tenant_id_control_point_id",
                        columns: x => new { x.tenant_id, x.control_point_id },
                        principalSchema: "compliance",
                        principalTable: "control_points",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "evidence_links",
                schema: "compliance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    evaluation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    evidence_attachment_id = table.Column<Guid>(type: "uuid", nullable: false),
                    linked_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    linked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_evidence_links", x => x.id);
                    table.ForeignKey(
                        name: "fk_evidence_links_control_evaluations_tenant_id_evaluation_id",
                        columns: x => new { x.tenant_id, x.evaluation_id },
                        principalSchema: "compliance",
                        principalTable: "control_evaluations",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_evidence_links_evidence_attachments_tenant_id_evidence_atta",
                        columns: x => new { x.tenant_id, x.evidence_attachment_id },
                        principalSchema: "evidence",
                        principalTable: "attachments",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "corrective_actions",
                schema: "compliance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    finding_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    owner = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    due_on = table.Column<DateOnly>(type: "date", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    completion_notes = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: true),
                    submitted_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    verified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    verified_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_corrective_actions", x => x.id);
                    table.ForeignKey(
                        name: "fk_corrective_actions_findings_tenant_id_finding_id",
                        columns: x => new { x.tenant_id, x.finding_id },
                        principalSchema: "compliance",
                        principalTable: "findings",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_assessments_tenant_id",
                schema: "compliance",
                table: "assessments",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_assessments_tenant_id_assessment_code",
                schema: "compliance",
                table: "assessments",
                columns: new[] { "tenant_id", "assessment_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_assessments_tenant_id_farm_id_status_scheduled_on",
                schema: "compliance",
                table: "assessments",
                columns: new[] { "tenant_id", "farm_id", "status", "scheduled_on" });

            migrationBuilder.CreateIndex(
                name: "ix_assessments_tenant_id_standard_version_id",
                schema: "compliance",
                table: "assessments",
                columns: new[] { "tenant_id", "standard_version_id" });

            migrationBuilder.CreateIndex(
                name: "ix_contract_allocations_tenant_id",
                schema: "supply",
                table: "contract_allocations",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_contract_allocations_tenant_id_contract_id_reference",
                schema: "supply",
                table: "contract_allocations",
                columns: new[] { "tenant_id", "contract_id", "reference" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_contract_allocations_tenant_id_status_delivery_from_deliver",
                schema: "supply",
                table: "contract_allocations",
                columns: new[] { "tenant_id", "status", "delivery_from", "delivery_to" });

            migrationBuilder.CreateIndex(
                name: "ix_contracts_relationship_id",
                schema: "supply",
                table: "contracts",
                column: "relationship_id");

            migrationBuilder.CreateIndex(
                name: "ix_contracts_tenant_id",
                schema: "supply",
                table: "contracts",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_contracts_tenant_id_contract_number",
                schema: "supply",
                table: "contracts",
                columns: new[] { "tenant_id", "contract_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_contracts_tenant_id_hosted_supplier_id",
                schema: "supply",
                table: "contracts",
                columns: new[] { "tenant_id", "hosted_supplier_id" });

            migrationBuilder.CreateIndex(
                name: "ix_contracts_tenant_id_status_valid_from_valid_to",
                schema: "supply",
                table: "contracts",
                columns: new[] { "tenant_id", "status", "valid_from", "valid_to" });

            migrationBuilder.CreateIndex(
                name: "ix_control_evaluations_tenant_id",
                schema: "compliance",
                table: "control_evaluations",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_control_evaluations_tenant_id_assessment_id_control_point_id",
                schema: "compliance",
                table: "control_evaluations",
                columns: new[] { "tenant_id", "assessment_id", "control_point_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_control_evaluations_tenant_id_control_point_id",
                schema: "compliance",
                table: "control_evaluations",
                columns: new[] { "tenant_id", "control_point_id" });

            migrationBuilder.CreateIndex(
                name: "ix_control_points_tenant_id",
                schema: "compliance",
                table: "control_points",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_control_points_tenant_id_standard_version_id_code",
                schema: "compliance",
                table: "control_points",
                columns: new[] { "tenant_id", "standard_version_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_control_points_tenant_id_standard_version_id_sort_order",
                schema: "compliance",
                table: "control_points",
                columns: new[] { "tenant_id", "standard_version_id", "sort_order" });

            migrationBuilder.CreateIndex(
                name: "ix_corrective_actions_tenant_id",
                schema: "compliance",
                table: "corrective_actions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_corrective_actions_tenant_id_finding_id_status",
                schema: "compliance",
                table: "corrective_actions",
                columns: new[] { "tenant_id", "finding_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_corrective_actions_tenant_id_status_due_on",
                schema: "compliance",
                table: "corrective_actions",
                columns: new[] { "tenant_id", "status", "due_on" });

            migrationBuilder.CreateIndex(
                name: "ix_evidence_links_tenant_id",
                schema: "compliance",
                table: "evidence_links",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_evidence_links_tenant_id_evaluation_id_evidence_attachment_",
                schema: "compliance",
                table: "evidence_links",
                columns: new[] { "tenant_id", "evaluation_id", "evidence_attachment_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_evidence_links_tenant_id_evidence_attachment_id",
                schema: "compliance",
                table: "evidence_links",
                columns: new[] { "tenant_id", "evidence_attachment_id" });

            migrationBuilder.CreateIndex(
                name: "ix_evidence_requirements_tenant_id",
                schema: "compliance",
                table: "evidence_requirements",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_evidence_requirements_tenant_id_control_point_id_code",
                schema: "compliance",
                table: "evidence_requirements",
                columns: new[] { "tenant_id", "control_point_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_findings_tenant_id",
                schema: "compliance",
                table: "findings",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_findings_tenant_id_assessment_id_control_point_id",
                schema: "compliance",
                table: "findings",
                columns: new[] { "tenant_id", "assessment_id", "control_point_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_findings_tenant_id_control_point_id",
                schema: "compliance",
                table: "findings",
                columns: new[] { "tenant_id", "control_point_id" });

            migrationBuilder.CreateIndex(
                name: "ix_findings_tenant_id_status_severity_due_on",
                schema: "compliance",
                table: "findings",
                columns: new[] { "tenant_id", "status", "severity", "due_on" });

            migrationBuilder.CreateIndex(
                name: "ix_hosted_suppliers_tenant_id",
                schema: "supply",
                table: "hosted_suppliers",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_hosted_suppliers_tenant_id_email",
                schema: "supply",
                table: "hosted_suppliers",
                columns: new[] { "tenant_id", "email" });

            migrationBuilder.CreateIndex(
                name: "ix_hosted_suppliers_tenant_id_supplier_code",
                schema: "supply",
                table: "hosted_suppliers",
                columns: new[] { "tenant_id", "supplier_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_standard_versions_tenant_id",
                schema: "compliance",
                table: "standard_versions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_standard_versions_tenant_id_code_status",
                schema: "compliance",
                table: "standard_versions",
                columns: new[] { "tenant_id", "code", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_standard_versions_tenant_id_series_id_revision",
                schema: "compliance",
                table: "standard_versions",
                columns: new[] { "tenant_id", "series_id", "revision" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_supplier_evidence_requests_relationship_id",
                schema: "supply",
                table: "supplier_evidence_requests",
                column: "relationship_id");

            migrationBuilder.CreateIndex(
                name: "ix_supplier_evidence_requests_tenant_id",
                schema: "supply",
                table: "supplier_evidence_requests",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_supplier_evidence_requests_tenant_id_contract_id",
                schema: "supply",
                table: "supplier_evidence_requests",
                columns: new[] { "tenant_id", "contract_id" });

            migrationBuilder.CreateIndex(
                name: "ix_supplier_evidence_requests_tenant_id_hosted_supplier_id_sta",
                schema: "supply",
                table: "supplier_evidence_requests",
                columns: new[] { "tenant_id", "hosted_supplier_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_supplier_evidence_requests_tenant_id_status_due_on",
                schema: "supply",
                table: "supplier_evidence_requests",
                columns: new[] { "tenant_id", "status", "due_on" });

            migrationBuilder.CreateIndex(
                name: "ix_supplier_invitations_tenant_id",
                schema: "supply",
                table: "supplier_invitations",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_supplier_invitations_tenant_id_email_status",
                schema: "supply",
                table: "supplier_invitations",
                columns: new[] { "tenant_id", "email", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_supplier_invitations_tenant_id_hosted_supplier_id",
                schema: "supply",
                table: "supplier_invitations",
                columns: new[] { "tenant_id", "hosted_supplier_id" });

            migrationBuilder.CreateIndex(
                name: "ix_supplier_invitations_tenant_id_token_hash",
                schema: "supply",
                table: "supplier_invitations",
                columns: new[] { "tenant_id", "token_hash" },
                unique: true);

            migrationBuilder.Sql("""
                ALTER TABLE compliance.standard_versions
                    ADD CONSTRAINT ck_standard_versions_status CHECK (status BETWEEN 1 AND 3),
                    ADD CONSTRAINT ck_standard_versions_dates CHECK (effective_to IS NULL OR effective_to >= effective_from),
                    ADD CONSTRAINT ck_standard_versions_publication CHECK (
                        (status = 1 AND published_at IS NULL AND retired_at IS NULL) OR
                        (status = 2 AND published_at IS NOT NULL AND retired_at IS NULL) OR
                        (status = 3 AND published_at IS NOT NULL AND retired_at IS NOT NULL));
                ALTER TABLE compliance.control_points
                    ADD CONSTRAINT ck_control_points_applicability CHECK (applicability BETWEEN 1 AND 2),
                    ADD CONSTRAINT ck_control_points_weight CHECK (weight > 0 AND weight <= 100),
                    ADD CONSTRAINT ck_control_points_sort_order CHECK (sort_order >= 0);
                ALTER TABLE compliance.evidence_requirements
                    ADD CONSTRAINT ck_evidence_requirements_type CHECK (evidence_type BETWEEN 1 AND 5),
                    ADD CONSTRAINT ck_evidence_requirements_count CHECK (minimum_count >= 0 AND (NOT required OR minimum_count > 0)),
                    ADD CONSTRAINT ck_evidence_requirements_age CHECK (maximum_age_days IS NULL OR maximum_age_days > 0);
                ALTER TABLE compliance.assessments
                    ADD CONSTRAINT ck_assessments_status CHECK (status BETWEEN 1 AND 5),
                    ADD CONSTRAINT ck_assessments_score CHECK (score_percent IS NULL OR score_percent BETWEEN 0 AND 100);
                ALTER TABLE compliance.control_evaluations
                    ADD CONSTRAINT ck_control_evaluations_result CHECK (result BETWEEN 1 AND 3),
                    ADD CONSTRAINT ck_control_evaluations_score CHECK (score_percent BETWEEN 0 AND 100 AND (result <> 3 OR score_percent = 0));
                ALTER TABLE compliance.findings
                    ADD CONSTRAINT ck_findings_severity CHECK (severity BETWEEN 1 AND 4),
                    ADD CONSTRAINT ck_findings_status CHECK (status BETWEEN 1 AND 5);
                ALTER TABLE compliance.corrective_actions
                    ADD CONSTRAINT ck_corrective_actions_status CHECK (status BETWEEN 1 AND 6);

                ALTER TABLE supply.hosted_suppliers ADD CONSTRAINT ck_hosted_suppliers_status CHECK (status BETWEEN 1 AND 3);
                ALTER TABLE supply.supplier_invitations
                    ADD CONSTRAINT ck_supplier_invitations_status CHECK (status BETWEEN 1 AND 4),
                    ADD CONSTRAINT ck_supplier_invitations_expiry CHECK (expires_at > invited_at);
                ALTER TABLE supply.contracts
                    ADD CONSTRAINT ck_contracts_target CHECK ((relationship_id IS NULL) <> (hosted_supplier_id IS NULL)),
                    ADD CONSTRAINT ck_contracts_dates CHECK (valid_to >= valid_from),
                    ADD CONSTRAINT ck_contracts_quantity CHECK (contracted_quantity > 0),
                    ADD CONSTRAINT ck_contracts_price CHECK (unit_price IS NULL OR unit_price >= 0),
                    ADD CONSTRAINT ck_contracts_money CHECK ((unit_price IS NULL) = (currency IS NULL)),
                    ADD CONSTRAINT ck_contracts_status CHECK (status BETWEEN 1 AND 4);
                ALTER TABLE supply.contract_allocations
                    ADD CONSTRAINT ck_contract_allocations_dates CHECK (delivery_to >= delivery_from),
                    ADD CONSTRAINT ck_contract_allocations_quantity CHECK (allocated_quantity > 0 AND fulfilled_quantity >= 0 AND fulfilled_quantity <= allocated_quantity),
                    ADD CONSTRAINT ck_contract_allocations_status CHECK (status BETWEEN 1 AND 4);
                ALTER TABLE supply.supplier_evidence_requests
                    ADD CONSTRAINT ck_supplier_evidence_requests_target CHECK ((relationship_id IS NULL) <> (hosted_supplier_id IS NULL)),
                    ADD CONSTRAINT ck_supplier_evidence_requests_status CHECK (status BETWEEN 1 AND 5);

                CREATE OR REPLACE FUNCTION compliance.guard_standard_content() RETURNS trigger LANGUAGE plpgsql AS $$
                DECLARE standard_status integer; entity_tenant uuid; parent_standard uuid; parent_control uuid;
                BEGIN
                    IF TG_OP = 'DELETE' THEN entity_tenant := OLD.tenant_id; parent_standard := OLD.standard_version_id; parent_control := OLD.control_point_id;
                    ELSE entity_tenant := NEW.tenant_id; parent_standard := NEW.standard_version_id; parent_control := NEW.control_point_id; END IF;
                    IF TG_TABLE_NAME = 'control_points' THEN
                        SELECT status INTO standard_status FROM compliance.standard_versions
                        WHERE tenant_id = entity_tenant AND id = parent_standard;
                    ELSE
                        SELECT s.status INTO standard_status
                        FROM compliance.standard_versions s
                        JOIN compliance.control_points c ON c.tenant_id = s.tenant_id AND c.standard_version_id = s.id
                        WHERE c.tenant_id = entity_tenant AND c.id = parent_control;
                    END IF;
                    IF standard_status IS DISTINCT FROM 1 THEN
                        RAISE EXCEPTION 'Published or retired compliance standard content is immutable.' USING ERRCODE = '55000';
                    END IF;
                    IF TG_OP = 'DELETE' THEN RETURN OLD; ELSE RETURN NEW; END IF;
                END $$;

                CREATE OR REPLACE FUNCTION compliance.guard_immutable_evidence() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    RAISE EXCEPTION '% is immutable compliance evidence.', TG_TABLE_NAME USING ERRCODE = '55000';
                END $$;

                CREATE OR REPLACE FUNCTION compliance.validate_assessment_scope() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_TABLE_NAME = 'control_evaluations' OR TG_TABLE_NAME = 'findings' THEN
                        IF NOT EXISTS (
                            SELECT 1 FROM compliance.assessments a
                            JOIN compliance.control_points c ON c.tenant_id = a.tenant_id AND c.standard_version_id = a.standard_version_id
                            WHERE a.tenant_id = NEW.tenant_id AND a.id = NEW.assessment_id AND c.id = NEW.control_point_id
                        ) THEN RAISE EXCEPTION 'Assessment and control must use the same tenant and standard version.' USING ERRCODE = '23514'; END IF;
                    ELSIF TG_TABLE_NAME = 'evidence_links' THEN
                        IF NOT EXISTS (
                            SELECT 1 FROM compliance.control_evaluations e
                            JOIN compliance.assessments a ON a.tenant_id = e.tenant_id AND a.id = e.assessment_id
                            JOIN evidence.attachments attachment ON attachment.tenant_id = e.tenant_id AND attachment.id = NEW.evidence_attachment_id
                            JOIN production.crop_zones z ON z.tenant_id = attachment.tenant_id AND z.id = attachment.crop_zone_id
                            JOIN production.seasons season ON season.tenant_id = z.tenant_id AND season.id = z.season_id AND season.farm_id = a.farm_id
                            WHERE e.tenant_id = NEW.tenant_id AND e.id = NEW.evaluation_id
                        ) THEN RAISE EXCEPTION 'Compliance evidence must belong to the assessed farm.' USING ERRCODE = '23514'; END IF;
                    END IF;
                    RETURN NEW;
                END $$;

                CREATE TRIGGER control_points_draft_guard BEFORE INSERT OR UPDATE OR DELETE ON compliance.control_points
                    FOR EACH ROW EXECUTE FUNCTION compliance.guard_standard_content();
                CREATE TRIGGER evidence_requirements_draft_guard BEFORE INSERT OR UPDATE OR DELETE ON compliance.evidence_requirements
                    FOR EACH ROW EXECUTE FUNCTION compliance.guard_standard_content();
                CREATE TRIGGER control_evaluations_scope BEFORE INSERT ON compliance.control_evaluations
                    FOR EACH ROW EXECUTE FUNCTION compliance.validate_assessment_scope();
                CREATE TRIGGER findings_scope BEFORE INSERT ON compliance.findings
                    FOR EACH ROW EXECUTE FUNCTION compliance.validate_assessment_scope();
                CREATE TRIGGER evidence_links_scope BEFORE INSERT ON compliance.evidence_links
                    FOR EACH ROW EXECUTE FUNCTION compliance.validate_assessment_scope();
                CREATE TRIGGER control_evaluations_immutable BEFORE UPDATE OR DELETE ON compliance.control_evaluations
                    FOR EACH ROW EXECUTE FUNCTION compliance.guard_immutable_evidence();
                CREATE TRIGGER evidence_links_immutable BEFORE UPDATE OR DELETE ON compliance.evidence_links
                    FOR EACH ROW EXECUTE FUNCTION compliance.guard_immutable_evidence();

                DO $$
                DECLARE table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY['standard_versions','control_points','evidence_requirements','assessments',
                        'control_evaluations','evidence_links','findings','corrective_actions'] LOOP
                        EXECUTE format('ALTER TABLE compliance.%I ENABLE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('ALTER TABLE compliance.%I FORCE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('CREATE POLICY tenant_isolation ON compliance.%I USING (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid) WITH CHECK (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid)', table_name);
                    END LOOP;
                    FOREACH table_name IN ARRAY ARRAY['hosted_suppliers','supplier_invitations','contracts','contract_allocations','supplier_evidence_requests'] LOOP
                        EXECUTE format('ALTER TABLE supply.%I ENABLE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('ALTER TABLE supply.%I FORCE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('CREATE POLICY tenant_isolation ON supply.%I USING (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid) WITH CHECK (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid)', table_name);
                    END LOOP;
                END $$;

                GRANT USAGE ON SCHEMA compliance TO murimios_app;
                GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA compliance TO murimios_app;
                GRANT SELECT, INSERT, UPDATE, DELETE ON supply.hosted_suppliers, supply.supplier_invitations,
                    supply.contracts, supply.contract_allocations, supply.supplier_evidence_requests TO murimios_app;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP FUNCTION IF EXISTS compliance.validate_assessment_scope() CASCADE;
                DROP FUNCTION IF EXISTS compliance.guard_immutable_evidence() CASCADE;
                DROP FUNCTION IF EXISTS compliance.guard_standard_content() CASCADE;
                """);
            migrationBuilder.DropTable(
                name: "contract_allocations",
                schema: "supply");

            migrationBuilder.DropTable(
                name: "corrective_actions",
                schema: "compliance");

            migrationBuilder.DropTable(
                name: "evidence_links",
                schema: "compliance");

            migrationBuilder.DropTable(
                name: "evidence_requirements",
                schema: "compliance");

            migrationBuilder.DropTable(
                name: "supplier_evidence_requests",
                schema: "supply");

            migrationBuilder.DropTable(
                name: "supplier_invitations",
                schema: "supply");

            migrationBuilder.DropTable(
                name: "findings",
                schema: "compliance");

            migrationBuilder.DropTable(
                name: "control_evaluations",
                schema: "compliance");

            migrationBuilder.DropTable(
                name: "contracts",
                schema: "supply");

            migrationBuilder.DropTable(
                name: "assessments",
                schema: "compliance");

            migrationBuilder.DropTable(
                name: "control_points",
                schema: "compliance");

            migrationBuilder.DropTable(
                name: "hosted_suppliers",
                schema: "supply");

            migrationBuilder.DropTable(
                name: "standard_versions",
                schema: "compliance");
        }
    }
}
