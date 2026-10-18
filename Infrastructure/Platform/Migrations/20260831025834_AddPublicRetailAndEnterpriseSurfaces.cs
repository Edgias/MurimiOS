using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddPublicRetailAndEnterpriseSurfaces : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "external_api");

            migrationBuilder.EnsureSchema(
                name: "enterprise");

            migrationBuilder.EnsureSchema(
                name: "retail");

            migrationBuilder.CreateTable(
                name: "approval_policies",
                schema: "enterprise",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    organisation_node_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action_type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    approver_permission = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    required_approvals = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    authored_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    authored_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    retired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_approval_policies", x => x.id);
                    table.UniqueConstraint("ak_approval_policies_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_approval_policies_org_nodes_tenant_id_organisation_node_id",
                        columns: x => new { x.tenant_id, x.organisation_node_id },
                        principalSchema: "platform",
                        principalTable: "org_nodes",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "clients",
                schema: "external_api",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    scopes = table.Column<string[]>(type: "text[]", nullable: false),
                    farm_ids = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    registered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    last_used_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revoked_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_clients", x => x.id);
                    table.UniqueConstraint("ak_external_api_clients_tenant_id_id", x => new { x.tenant_id, x.id });
                });

            migrationBuilder.CreateTable(
                name: "disclosure_policies",
                schema: "retail",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    series_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    public_brand_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    attributes = table.Column<string[]>(type: "text[]", nullable: false),
                    farm_ids = table.Column<Guid[]>(type: "uuid[]", nullable: false),
                    data_grant_id = table.Column<Guid>(type: "uuid", nullable: true),
                    policy_version = table.Column<int>(type: "integer", nullable: false),
                    previous_policy_id = table.Column<Guid>(type: "uuid", nullable: true),
                    locale = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    authored_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    authored_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    published_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    retired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_disclosure_policies", x => x.id);
                    table.UniqueConstraint("ak_disclosure_policies_tenant_id_id", x => new { x.tenant_id, x.id });
                });

            migrationBuilder.CreateTable(
                name: "tenant_placements",
                schema: "enterprise",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    region = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    database_key = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    mode = table.Column<int>(type: "integer", nullable: false),
                    residency_country = table.Column<string>(type: "character(2)", fixedLength: true, maxLength: 2, nullable: false),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    effective_from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    effective_to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_placements", x => x.id);
                    table.UniqueConstraint("ak_tenant_placements_tenant_id_id", x => new { x.tenant_id, x.id });
                });

            migrationBuilder.CreateTable(
                name: "webhook_subscriptions",
                schema: "external_api",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    endpoint = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: false),
                    event_types = table.Column<string[]>(type: "text[]", nullable: false),
                    signing_key_hash = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    secret_version = table.Column<int>(type: "integer", nullable: false),
                    maximum_attempts = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    registered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    disabled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_webhook_subscriptions", x => x.id);
                    table.UniqueConstraint("ak_webhook_subscriptions_tenant_id_id", x => new { x.tenant_id, x.id });
                });

            migrationBuilder.CreateTable(
                name: "approval_requests",
                schema: "enterprise",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    policy_id = table.Column<Guid>(type: "uuid", nullable: false),
                    organisation_node_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: true),
                    target_type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action_type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    request_json = table.Column<string>(type: "jsonb", nullable: false),
                    required_approvals = table.Column<int>(type: "integer", nullable: false),
                    approval_count = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    requested_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    requested_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    decided_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_approval_requests", x => x.id);
                    table.UniqueConstraint("ak_approval_requests_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_approval_requests_approval_policies_tenant_id_policy_id",
                        columns: x => new { x.tenant_id, x.policy_id },
                        principalSchema: "enterprise",
                        principalTable: "approval_policies",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_approval_requests_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_approval_requests_org_nodes_tenant_id_organisation_node_id",
                        columns: x => new { x.tenant_id, x.organisation_node_id },
                        principalSchema: "platform",
                        principalTable: "org_nodes",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "access_log",
                schema: "external_api",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    scope = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    method = table.Column<string>(type: "character varying(12)", maxLength: 12, nullable: false),
                    path = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    status_code = table.Column<int>(type: "integer", nullable: false),
                    correlation_id = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    remote_address_hash = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_access_log", x => x.id);
                    table.ForeignKey(
                        name: "fk_access_log_clients_tenant_id_client_id",
                        columns: x => new { x.tenant_id, x.client_id },
                        principalSchema: "external_api",
                        principalTable: "clients",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "credentials",
                schema: "external_api",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    credential_version = table.Column<int>(type: "integer", nullable: false),
                    key_prefix = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    key_hash = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    valid_from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    issued_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    issued_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    previous_credential_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    ended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    superseded_by_credential_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_credentials", x => x.id);
                    table.ForeignKey(
                        name: "fk_credentials_clients_tenant_id_client_id",
                        columns: x => new { x.tenant_id, x.client_id },
                        principalSchema: "external_api",
                        principalTable: "clients",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transparency_tokens",
                schema: "retail",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    policy_id = table.Column<Guid>(type: "uuid", nullable: false),
                    object_type = table.Column<int>(type: "integer", nullable: false),
                    object_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_prefix = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    token_hash = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    issued_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    issued_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transparency_tokens", x => x.id);
                    table.UniqueConstraint("ak_transparency_tokens_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_transparency_tokens_disclosure_policies_tenant_id_policy_id",
                        columns: x => new { x.tenant_id, x.policy_id },
                        principalSchema: "retail",
                        principalTable: "disclosure_policies",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "tenant_migration_jobs",
                schema: "enterprise",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<int>(type: "integer", nullable: false),
                    source_placement_id = table.Column<Guid>(type: "uuid", nullable: false),
                    destination_region = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    destination_database_key = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    destination_mode = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    requested_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    requested_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    artifact_object_key = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    artifact_sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: true),
                    manifest_json = table.Column<string>(type: "jsonb", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    failure_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    failure_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_migration_jobs", x => x.id);
                    table.ForeignKey(
                        name: "fk_tenant_migration_jobs_tenant_placements_tenant_id_source_pl",
                        columns: x => new { x.tenant_id, x.source_placement_id },
                        principalSchema: "enterprise",
                        principalTable: "tenant_placements",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "webhook_deliveries",
                schema: "external_api",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_id = table.Column<Guid>(type: "uuid", nullable: false),
                    outbox_message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    available_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    attempts = table.Column<int>(type: "integer", nullable: false),
                    maximum_attempts = table.Column<int>(type: "integer", nullable: false),
                    lease_id = table.Column<Guid>(type: "uuid", nullable: true),
                    leased_until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    delivered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    response_status = table.Column<int>(type: "integer", nullable: true),
                    response_body_excerpt = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    last_error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_webhook_deliveries", x => x.id);
                    table.ForeignKey(
                        name: "fk_webhook_deliveries_webhook_subscriptions_tenant_id_subscrip",
                        columns: x => new { x.tenant_id, x.subscription_id },
                        principalSchema: "external_api",
                        principalTable: "webhook_subscriptions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "approval_decisions",
                schema: "enterprise",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    request_id = table.Column<Guid>(type: "uuid", nullable: false),
                    membership_id = table.Column<Guid>(type: "uuid", nullable: false),
                    actor_subject = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    decision = table.Column<int>(type: "integer", nullable: false),
                    reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    decided_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_approval_decisions", x => x.id);
                    table.ForeignKey(
                        name: "fk_approval_decisions_approval_requests_tenant_id_request_id",
                        columns: x => new { x.tenant_id, x.request_id },
                        principalSchema: "enterprise",
                        principalTable: "approval_requests",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_approval_decisions_memberships_tenant_id_membership_id",
                        columns: x => new { x.tenant_id, x.membership_id },
                        principalSchema: "access",
                        principalTable: "memberships",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "transparency_access_log",
                schema: "retail",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_id = table.Column<Guid>(type: "uuid", nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    outcome = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    correlation_id = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    remote_address_hash = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: true),
                    user_agent_hash = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_transparency_access_log", x => x.id);
                    table.ForeignKey(
                        name: "fk_transparency_access_log_transparency_tokens_tenant_id_token",
                        columns: x => new { x.tenant_id, x.token_id },
                        principalSchema: "retail",
                        principalTable: "transparency_tokens",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_access_log_tenant_id",
                schema: "external_api",
                table: "access_log",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_access_log_tenant_id_client_id_occurred_at",
                schema: "external_api",
                table: "access_log",
                columns: new[] { "tenant_id", "client_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_approval_decisions_tenant_id",
                schema: "enterprise",
                table: "approval_decisions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_approval_decisions_tenant_id_membership_id",
                schema: "enterprise",
                table: "approval_decisions",
                columns: new[] { "tenant_id", "membership_id" });

            migrationBuilder.CreateIndex(
                name: "ix_approval_decisions_tenant_id_request_id_membership_id",
                schema: "enterprise",
                table: "approval_decisions",
                columns: new[] { "tenant_id", "request_id", "membership_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_approval_policies_tenant_id",
                schema: "enterprise",
                table: "approval_policies",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_approval_policies_tenant_id_code_status",
                schema: "enterprise",
                table: "approval_policies",
                columns: new[] { "tenant_id", "code", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_approval_policies_tenant_id_organisation_node_id",
                schema: "enterprise",
                table: "approval_policies",
                columns: new[] { "tenant_id", "organisation_node_id" });

            migrationBuilder.CreateIndex(
                name: "ix_approval_requests_tenant_id",
                schema: "enterprise",
                table: "approval_requests",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_approval_requests_tenant_id_farm_id",
                schema: "enterprise",
                table: "approval_requests",
                columns: new[] { "tenant_id", "farm_id" });

            migrationBuilder.CreateIndex(
                name: "ix_approval_requests_tenant_id_organisation_node_id",
                schema: "enterprise",
                table: "approval_requests",
                columns: new[] { "tenant_id", "organisation_node_id" });

            migrationBuilder.CreateIndex(
                name: "ix_approval_requests_tenant_id_policy_id",
                schema: "enterprise",
                table: "approval_requests",
                columns: new[] { "tenant_id", "policy_id" });

            migrationBuilder.CreateIndex(
                name: "ix_approval_requests_tenant_id_status_expires_at",
                schema: "enterprise",
                table: "approval_requests",
                columns: new[] { "tenant_id", "status", "expires_at" });

            migrationBuilder.CreateIndex(
                name: "ix_approval_requests_tenant_id_target_type_target_id_action_ty",
                schema: "enterprise",
                table: "approval_requests",
                columns: new[] { "tenant_id", "target_type", "target_id", "action_type", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_clients_tenant_id",
                schema: "external_api",
                table: "clients",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_clients_tenant_id_status",
                schema: "external_api",
                table: "clients",
                columns: new[] { "tenant_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_credentials_key_hash",
                schema: "external_api",
                table: "credentials",
                column: "key_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_credentials_tenant_id",
                schema: "external_api",
                table: "credentials",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_credentials_tenant_id_client_id_credential_version",
                schema: "external_api",
                table: "credentials",
                columns: new[] { "tenant_id", "client_id", "credential_version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_disclosure_policies_tenant_id",
                schema: "retail",
                table: "disclosure_policies",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_disclosure_policies_tenant_id_series_id_policy_version",
                schema: "retail",
                table: "disclosure_policies",
                columns: new[] { "tenant_id", "series_id", "policy_version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_disclosure_policies_tenant_id_status",
                schema: "retail",
                table: "disclosure_policies",
                columns: new[] { "tenant_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_tenant_migration_jobs_tenant_id",
                schema: "enterprise",
                table: "tenant_migration_jobs",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_tenant_migration_jobs_tenant_id_source_placement_id",
                schema: "enterprise",
                table: "tenant_migration_jobs",
                columns: new[] { "tenant_id", "source_placement_id" });

            migrationBuilder.CreateIndex(
                name: "ix_tenant_migration_jobs_tenant_id_status_requested_at",
                schema: "enterprise",
                table: "tenant_migration_jobs",
                columns: new[] { "tenant_id", "status", "requested_at" });

            migrationBuilder.CreateIndex(
                name: "ix_tenant_placements_tenant_id",
                schema: "enterprise",
                table: "tenant_placements",
                column: "tenant_id",
                unique: true,
                filter: "is_primary AND effective_to IS NULL");

            migrationBuilder.CreateIndex(
                name: "ix_transparency_access_log_tenant_id",
                schema: "retail",
                table: "transparency_access_log",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_transparency_access_log_tenant_id_token_id_occurred_at",
                schema: "retail",
                table: "transparency_access_log",
                columns: new[] { "tenant_id", "token_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_transparency_tokens_tenant_id",
                schema: "retail",
                table: "transparency_tokens",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_transparency_tokens_tenant_id_policy_id_object_id_status",
                schema: "retail",
                table: "transparency_tokens",
                columns: new[] { "tenant_id", "policy_id", "object_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_transparency_tokens_token_hash",
                schema: "retail",
                table: "transparency_tokens",
                column: "token_hash",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_webhook_deliveries_status_available_at_leased_until",
                schema: "external_api",
                table: "webhook_deliveries",
                columns: new[] { "status", "available_at", "leased_until" });

            migrationBuilder.CreateIndex(
                name: "ix_webhook_deliveries_tenant_id",
                schema: "external_api",
                table: "webhook_deliveries",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_webhook_deliveries_tenant_id_subscription_id_outbox_message",
                schema: "external_api",
                table: "webhook_deliveries",
                columns: new[] { "tenant_id", "subscription_id", "outbox_message_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_webhook_subscriptions_tenant_id",
                schema: "external_api",
                table: "webhook_subscriptions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_webhook_subscriptions_tenant_id_status",
                schema: "external_api",
                table: "webhook_subscriptions",
                columns: new[] { "tenant_id", "status" });

            migrationBuilder.Sql("""
                ALTER TABLE external_api.clients ADD CONSTRAINT ck_api_client_status CHECK (status BETWEEN 1 AND 3);
                ALTER TABLE external_api.clients ADD CONSTRAINT ck_api_client_scopes CHECK (cardinality(scopes) > 0 AND scopes <@ ARRAY['farms.read','harvest.read','trace.read','analytics.read']::text[]);
                ALTER TABLE external_api.credentials ADD CONSTRAINT ck_api_credential_status CHECK (status BETWEEN 1 AND 3);
                ALTER TABLE external_api.credentials ADD CONSTRAINT ck_api_credential_hash CHECK (key_hash ~ '^[0-9a-f]{64}$' AND credential_version > 0 AND (expires_at IS NULL OR expires_at > valid_from));
                ALTER TABLE external_api.webhook_subscriptions ADD CONSTRAINT ck_webhook_contract CHECK (status BETWEEN 1 AND 3 AND endpoint ~ '^https://' AND cardinality(event_types) > 0 AND signing_key_hash ~ '^[0-9a-f]{64}$' AND maximum_attempts BETWEEN 1 AND 20);
                ALTER TABLE external_api.webhook_deliveries ADD CONSTRAINT ck_webhook_delivery CHECK (status BETWEEN 1 AND 5 AND attempts >= 0 AND maximum_attempts BETWEEN 1 AND 20 AND jsonb_typeof(payload) IS NOT NULL);
                ALTER TABLE retail.disclosure_policies ADD CONSTRAINT ck_disclosure_policy CHECK (status BETWEEN 1 AND 3 AND policy_version > 0 AND cardinality(attributes) > 0 AND attributes <@ ARRAY['origin','crop','variety','lotCode','harvestedAt','quantity','quality','compliance','lineage']::text[]);
                ALTER TABLE retail.transparency_tokens ADD CONSTRAINT ck_transparency_token CHECK (status BETWEEN 1 AND 2 AND object_type = 2 AND token_hash ~ '^[0-9a-f]{64}$' AND (expires_at IS NULL OR expires_at > issued_at));
                ALTER TABLE enterprise.approval_policies ADD CONSTRAINT ck_approval_policy CHECK (status BETWEEN 1 AND 2 AND required_approvals BETWEEN 1 AND 20);
                ALTER TABLE enterprise.approval_requests ADD CONSTRAINT ck_approval_request CHECK (status BETWEEN 1 AND 5 AND required_approvals BETWEEN 1 AND 20 AND approval_count BETWEEN 0 AND required_approvals AND expires_at > requested_at AND jsonb_typeof(request_json) IS NOT NULL);
                ALTER TABLE enterprise.approval_decisions ADD CONSTRAINT ck_approval_decision CHECK (decision BETWEEN 1 AND 2);
                ALTER TABLE enterprise.tenant_placements ADD CONSTRAINT ck_tenant_placement CHECK (mode BETWEEN 1 AND 2 AND char_length(residency_country)=2 AND (effective_to IS NULL OR effective_to > effective_from));
                ALTER TABLE enterprise.tenant_migration_jobs ADD CONSTRAINT ck_tenant_migration CHECK (kind BETWEEN 1 AND 4 AND status BETWEEN 1 AND 8 AND destination_mode BETWEEN 1 AND 2 AND (artifact_sha256 IS NULL OR artifact_sha256 ~ '^[0-9a-f]{64}$'));

                CREATE OR REPLACE FUNCTION external_api.authenticate_api_key(p_key_hash text)
                RETURNS TABLE(tenant_id uuid,client_id uuid,scopes text[],farm_ids uuid[])
                LANGUAGE plpgsql VOLATILE SECURITY DEFINER SET search_path=pg_catalog,external_api AS $fn$
                BEGIN
                    RETURN QUERY
                    WITH authorized AS (
                        SELECT c.tenant_id,c.id,c.scopes,c.farm_ids
                        FROM external_api.credentials k JOIN external_api.clients c ON c.tenant_id=k.tenant_id AND c.id=k.client_id
                        WHERE k.key_hash=lower(p_key_hash) AND k.status=1 AND c.status=1
                          AND k.valid_from<=statement_timestamp() AND (k.expires_at IS NULL OR k.expires_at>statement_timestamp())
                        LIMIT 1
                    ), touched AS (
                        UPDATE external_api.clients c SET last_used_at=statement_timestamp()
                        FROM authorized a WHERE c.tenant_id=a.tenant_id AND c.id=a.id RETURNING c.id
                    ) SELECT a.tenant_id,a.id,a.scopes,a.farm_ids FROM authorized a LEFT JOIN touched t ON t.id=a.id;
                END $fn$;

                CREATE OR REPLACE FUNCTION retail.resolve_transparency_token(p_token_hash text)
                RETURNS TABLE(tenant_id uuid,token_id uuid,policy_id uuid,object_id uuid,data_grant_id uuid,public_brand_name text,locale text,attributes text[],farm_ids uuid[])
                LANGUAGE sql STABLE SECURITY DEFINER SET search_path=pg_catalog,retail,supply AS $fn$
                    SELECT t.tenant_id,t.id,p.id,t.object_id,p.data_grant_id,p.public_brand_name,p.locale,p.attributes,p.farm_ids
                    FROM retail.transparency_tokens t JOIN retail.disclosure_policies p ON p.tenant_id=t.tenant_id AND p.id=t.policy_id
                    WHERE t.token_hash=lower(p_token_hash) AND t.status=1 AND p.status=2
                      AND (t.expires_at IS NULL OR t.expires_at>statement_timestamp())
                      AND (p.data_grant_id IS NULL OR EXISTS (
                          SELECT 1 FROM supply.data_grants g JOIN supply.relationships r ON r.id=g.relationship_id
                          WHERE g.id=p.data_grant_id AND g.recipient_tenant_id=t.tenant_id AND g.status=1 AND r.status=2
                            AND g.downstream_disclosure_allowed AND g.valid_from<=statement_timestamp()
                            AND (g.valid_to IS NULL OR g.valid_to>statement_timestamp())
                            AND 'harvest-lots'=ANY(g.entity_categories)
                            AND (NOT ('lineage'=ANY(p.attributes)) OR 'traceability'=ANY(g.entity_categories))))
                    LIMIT 1
                $fn$;

                CREATE OR REPLACE FUNCTION external_api.enqueue_webhook_delivery()
                RETURNS trigger LANGUAGE plpgsql SECURITY DEFINER SET search_path=pg_catalog,external_api AS $fn$
                BEGIN
                    INSERT INTO external_api.webhook_deliveries(id,subscription_id,outbox_message_id,event_type,payload,occurred_at,available_at,status,attempts,maximum_attempts,tenant_id,created_at_utc,updated_at_utc,version)
                    SELECT gen_random_uuid(),s.id,NEW.id,NEW.type,NEW.payload,NEW.occurred_at,NEW.occurred_at,1,0,s.maximum_attempts,NEW.tenant_id,statement_timestamp(),statement_timestamp(),1
                    FROM external_api.webhook_subscriptions s
                    WHERE s.tenant_id=NEW.tenant_id AND s.status=1 AND (NEW.type=ANY(s.event_types) OR '*'=ANY(s.event_types))
                    ON CONFLICT (tenant_id,subscription_id,outbox_message_id) DO NOTHING;
                    RETURN NEW;
                END $fn$;
                CREATE TRIGGER outbox_enqueue_webhooks AFTER INSERT ON messaging.outbox_messages FOR EACH ROW EXECUTE FUNCTION external_api.enqueue_webhook_delivery();

                CREATE OR REPLACE FUNCTION external_api.claim_webhook_deliveries(p_worker uuid,p_batch integer,p_lock_seconds integer,p_now timestamptz)
                RETURNS TABLE(id uuid,tenant_id uuid,subscription_id uuid,endpoint text,signing_key_hash text,event_type text,payload text,occurred_at timestamptz,attempt integer,lease_id uuid)
                LANGUAGE plpgsql VOLATILE SECURITY DEFINER SET search_path=pg_catalog,external_api AS $fn$
                BEGIN
                    IF p_worker IS NULL OR p_batch NOT BETWEEN 1 AND 200 OR p_lock_seconds NOT BETWEEN 5 AND 3600 THEN RAISE EXCEPTION 'Invalid webhook claim parameters.' USING ERRCODE='22023'; END IF;
                    RETURN QUERY WITH candidates AS (
                        SELECT d.id FROM external_api.webhook_deliveries d JOIN external_api.webhook_subscriptions s ON s.tenant_id=d.tenant_id AND s.id=d.subscription_id
                        WHERE s.status=1 AND ((d.status IN (1,3) AND d.available_at<=p_now) OR (d.status=2 AND d.leased_until<=p_now))
                        ORDER BY d.available_at,d.occurred_at FOR UPDATE OF d SKIP LOCKED LIMIT p_batch
                    ), claimed AS (
                        UPDATE external_api.webhook_deliveries d SET status=2,lease_id=p_worker,leased_until=p_now+make_interval(secs=>p_lock_seconds),attempts=d.attempts+1,updated_at_utc=p_now,version=d.version+1
                        FROM candidates c WHERE d.id=c.id RETURNING d.*
                    ) SELECT c.id,c.tenant_id,c.subscription_id,s.endpoint::text,s.signing_key_hash::text,c.event_type::text,c.payload::text,c.occurred_at,c.attempts,c.lease_id
                      FROM claimed c JOIN external_api.webhook_subscriptions s ON s.tenant_id=c.tenant_id AND s.id=c.subscription_id;
                END $fn$;

                CREATE OR REPLACE FUNCTION external_api.complete_webhook_delivery(p_id uuid,p_lease uuid,p_at timestamptz,p_status integer,p_excerpt text)
                RETURNS boolean LANGUAGE sql VOLATILE SECURITY DEFINER SET search_path=pg_catalog,external_api AS $fn$
                    WITH changed AS (UPDATE external_api.webhook_deliveries SET status=4,delivered_at=p_at,response_status=p_status,response_body_excerpt=left(p_excerpt,2000),last_error=NULL,lease_id=NULL,leased_until=NULL,updated_at_utc=p_at,version=version+1 WHERE id=p_id AND status=2 AND lease_id=p_lease RETURNING 1) SELECT EXISTS(SELECT 1 FROM changed)
                $fn$;
                CREATE OR REPLACE FUNCTION external_api.fail_webhook_delivery(p_id uuid,p_lease uuid,p_at timestamptz,p_retry timestamptz,p_status integer,p_error text,p_excerpt text)
                RETURNS boolean LANGUAGE sql VOLATILE SECURITY DEFINER SET search_path=pg_catalog,external_api AS $fn$
                    WITH changed AS (UPDATE external_api.webhook_deliveries SET status=CASE WHEN attempts>=maximum_attempts THEN 5 ELSE 3 END,available_at=CASE WHEN attempts>=maximum_attempts THEN available_at ELSE p_retry END,response_status=p_status,response_body_excerpt=left(p_excerpt,2000),last_error=left(p_error,2000),lease_id=NULL,leased_until=NULL,updated_at_utc=p_at,version=version+1 WHERE id=p_id AND status=2 AND lease_id=p_lease RETURNING 1) SELECT EXISTS(SELECT 1 FROM changed)
                $fn$;

                CREATE OR REPLACE FUNCTION external_api.guard_credential_lifecycle() RETURNS trigger LANGUAGE plpgsql AS $fn$
                BEGIN
                    IF TG_OP='DELETE' THEN RAISE EXCEPTION 'API credentials are retained.' USING ERRCODE='55000'; END IF;
                    IF (to_jsonb(NEW)-ARRAY['status','ended_at','superseded_by_credential_id','updated_at_utc','version']) IS DISTINCT FROM (to_jsonb(OLD)-ARRAY['status','ended_at','superseded_by_credential_id','updated_at_utc','version']) OR OLD.status<>1 OR NEW.status NOT IN (2,3) THEN RAISE EXCEPTION 'API credentials may only be superseded or revoked.' USING ERRCODE='55000'; END IF; RETURN NEW;
                END $fn$;
                CREATE TRIGGER api_credentials_lifecycle BEFORE UPDATE OR DELETE ON external_api.credentials FOR EACH ROW EXECUTE FUNCTION external_api.guard_credential_lifecycle();
                CREATE TRIGGER api_access_log_immutable BEFORE UPDATE OR DELETE ON external_api.access_log FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER transparency_access_log_immutable BEFORE UPDATE OR DELETE ON retail.transparency_access_log FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER approval_decisions_immutable BEFORE UPDATE OR DELETE ON enterprise.approval_decisions FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();

                DO $rls$ DECLARE item record; BEGIN
                    FOR item IN SELECT * FROM (VALUES
                        ('external_api','clients'),('external_api','credentials'),('external_api','access_log'),('external_api','webhook_subscriptions'),('external_api','webhook_deliveries'),
                        ('retail','disclosure_policies'),('retail','transparency_tokens'),('retail','transparency_access_log'),
                        ('enterprise','approval_policies'),('enterprise','approval_requests'),('enterprise','approval_decisions'),('enterprise','tenant_placements'),('enterprise','tenant_migration_jobs')) AS t(schema_name,table_name)
                    LOOP EXECUTE format('ALTER TABLE %I.%I ENABLE ROW LEVEL SECURITY',item.schema_name,item.table_name); EXECUTE format('ALTER TABLE %I.%I FORCE ROW LEVEL SECURITY',item.schema_name,item.table_name); EXECUTE format('CREATE POLICY tenant_isolation ON %I.%I USING (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid) WITH CHECK (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid)',item.schema_name,item.table_name); END LOOP;
                END $rls$;

                REVOKE ALL ON FUNCTION external_api.authenticate_api_key(text) FROM PUBLIC;
                REVOKE ALL ON FUNCTION retail.resolve_transparency_token(text) FROM PUBLIC;
                REVOKE ALL ON FUNCTION external_api.claim_webhook_deliveries(uuid,integer,integer,timestamptz) FROM PUBLIC;
                REVOKE ALL ON FUNCTION external_api.complete_webhook_delivery(uuid,uuid,timestamptz,integer,text) FROM PUBLIC;
                REVOKE ALL ON FUNCTION external_api.fail_webhook_delivery(uuid,uuid,timestamptz,timestamptz,integer,text,text) FROM PUBLIC;
                DO $grant$ BEGIN IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname='murimios_app') THEN
                    GRANT USAGE ON SCHEMA external_api,retail,enterprise TO murimios_app;
                    GRANT SELECT,INSERT,UPDATE,DELETE ON ALL TABLES IN SCHEMA external_api,retail,enterprise TO murimios_app;
                    GRANT EXECUTE ON FUNCTION external_api.authenticate_api_key(text),retail.resolve_transparency_token(text),external_api.claim_webhook_deliveries(uuid,integer,integer,timestamptz),external_api.complete_webhook_delivery(uuid,uuid,timestamptz,integer,text),external_api.fail_webhook_delivery(uuid,uuid,timestamptz,timestamptz,integer,text,text) TO murimios_app;
                END IF; END $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP FUNCTION IF EXISTS external_api.fail_webhook_delivery(uuid,uuid,timestamptz,timestamptz,integer,text,text);
                DROP FUNCTION IF EXISTS external_api.complete_webhook_delivery(uuid,uuid,timestamptz,integer,text);
                DROP FUNCTION IF EXISTS external_api.claim_webhook_deliveries(uuid,integer,integer,timestamptz);
                DROP FUNCTION IF EXISTS external_api.enqueue_webhook_delivery() CASCADE;
                DROP FUNCTION IF EXISTS retail.resolve_transparency_token(text);
                DROP FUNCTION IF EXISTS external_api.authenticate_api_key(text);
                DROP FUNCTION IF EXISTS external_api.guard_credential_lifecycle() CASCADE;
                """);
            migrationBuilder.DropTable(
                name: "access_log",
                schema: "external_api");

            migrationBuilder.DropTable(
                name: "approval_decisions",
                schema: "enterprise");

            migrationBuilder.DropTable(
                name: "credentials",
                schema: "external_api");

            migrationBuilder.DropTable(
                name: "tenant_migration_jobs",
                schema: "enterprise");

            migrationBuilder.DropTable(
                name: "transparency_access_log",
                schema: "retail");

            migrationBuilder.DropTable(
                name: "webhook_deliveries",
                schema: "external_api");

            migrationBuilder.DropTable(
                name: "approval_requests",
                schema: "enterprise");

            migrationBuilder.DropTable(
                name: "clients",
                schema: "external_api");

            migrationBuilder.DropTable(
                name: "tenant_placements",
                schema: "enterprise");

            migrationBuilder.DropTable(
                name: "transparency_tokens",
                schema: "retail");

            migrationBuilder.DropTable(
                name: "webhook_subscriptions",
                schema: "external_api");

            migrationBuilder.DropTable(
                name: "approval_policies",
                schema: "enterprise");

            migrationBuilder.DropTable(
                name: "disclosure_policies",
                schema: "retail");
        }
    }
}
