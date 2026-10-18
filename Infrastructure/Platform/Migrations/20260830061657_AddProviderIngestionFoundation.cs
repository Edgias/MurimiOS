using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddProviderIngestionFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "integrations");

            migrationBuilder.CreateTable(
                name: "connections",
                schema: "integrations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    category = table.Column<int>(type: "integer", nullable: false),
                    configuration_json = table.Column<string>(type: "jsonb", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    health = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    activated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_succeeded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_failed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_error_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    last_error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    rate_limit_reset_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    rate_limit_remaining = table.Column<int>(type: "integer", nullable: true),
                    disabled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    disabled_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_connections", x => x.id);
                    table.UniqueConstraint("ak_integration_connections_tenant_id_id", x => new { x.tenant_id, x.id });
                });

            migrationBuilder.CreateTable(
                name: "credential_versions",
                schema: "integrations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    connection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    credential_version = table.Column<int>(type: "integer", nullable: false),
                    secret_provider = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    protected_reference = table.Column<string>(type: "character varying(4000)", maxLength: 4000, nullable: false),
                    fingerprint = table.Column<string>(type: "character varying(64)", maxLength: 64, nullable: false),
                    valid_from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    previous_credential_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    superseded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    superseded_by_credential_id = table.Column<Guid>(type: "uuid", nullable: true),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revoked_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_credential_versions", x => x.id);
                    table.UniqueConstraint("ak_integration_credentials_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_credential_versions_connections_tenant_id_connection_id",
                        columns: x => new { x.tenant_id, x.connection_id },
                        principalSchema: "integrations",
                        principalTable: "connections",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_credential_versions_credential_versions_tenant_id_previous_",
                        columns: x => new { x.tenant_id, x.previous_credential_id },
                        principalSchema: "integrations",
                        principalTable: "credential_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_credential_versions_credential_versions_tenant_id_supersede",
                        columns: x => new { x.tenant_id, x.superseded_by_credential_id },
                        principalSchema: "integrations",
                        principalTable: "credential_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "feeds",
                schema: "integrations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    connection_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feed_type = table.Column<int>(type: "integer", nullable: false),
                    target_type = table.Column<int>(type: "integer", nullable: false),
                    target_id = table.Column<Guid>(type: "uuid", nullable: true),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    external_source_id = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    polling_interval_minutes = table.Column<int>(type: "integer", nullable: false),
                    freshness_threshold_minutes = table.Column<int>(type: "integer", nullable: false),
                    configuration_json = table.Column<string>(type: "jsonb", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    checkpoint_json = table.Column<string>(type: "jsonb", nullable: true),
                    next_poll_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_observed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_received_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_succeeded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_failed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_error_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    last_error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_feeds", x => x.id);
                    table.UniqueConstraint("ak_integration_feeds_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_feeds_connections_tenant_id_connection_id",
                        columns: x => new { x.tenant_id, x.connection_id },
                        principalSchema: "integrations",
                        principalTable: "connections",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_feeds_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ingestion_jobs",
                schema: "integrations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    feed_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<int>(type: "integer", nullable: false),
                    idempotency_key = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    requested_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    requested_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    available_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    attempts = table.Column<int>(type: "integer", nullable: false),
                    maximum_attempts = table.Column<int>(type: "integer", nullable: false),
                    lease_id = table.Column<Guid>(type: "uuid", nullable: true),
                    leased_until = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    last_claimed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    window_start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    window_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    previous_job_id = table.Column<Guid>(type: "uuid", nullable: true),
                    result_checkpoint_json = table.Column<string>(type: "jsonb", nullable: true),
                    last_error_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    last_error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ingestion_jobs", x => x.id);
                    table.UniqueConstraint("ak_ingestion_jobs_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_ingestion_jobs_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ingestion_jobs_feeds_tenant_id_feed_id",
                        columns: x => new { x.tenant_id, x.feed_id },
                        principalSchema: "integrations",
                        principalTable: "feeds",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ingestion_jobs_ingestion_jobs_tenant_id_previous_job_id",
                        columns: x => new { x.tenant_id, x.previous_job_id },
                        principalSchema: "integrations",
                        principalTable: "ingestion_jobs",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ingestion_attempts",
                schema: "integrations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    attempt_number = table.Column<int>(type: "integer", nullable: false),
                    started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    outcome = table.Column<int>(type: "integer", nullable: false),
                    http_status_code = table.Column<int>(type: "integer", nullable: true),
                    request_units = table.Column<int>(type: "integer", nullable: false),
                    cost = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: true),
                    error_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ingestion_attempts", x => x.id);
                    table.ForeignKey(
                        name: "fk_ingestion_attempts_ingestion_jobs_tenant_id_job_id",
                        columns: x => new { x.tenant_id, x.job_id },
                        principalSchema: "integrations",
                        principalTable: "ingestion_jobs",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ingestion_records",
                schema: "integrations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feed_id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_record_id = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    schema_version = table.Column<int>(type: "integer", nullable: false),
                    observed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    received_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    source_modified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    content_sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    payload_json = table.Column<string>(type: "jsonb", nullable: false),
                    quality = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    processed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    rejection_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    rejection_reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_ingestion_records", x => x.id);
                    table.ForeignKey(
                        name: "fk_ingestion_records_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ingestion_records_feeds_tenant_id_feed_id",
                        columns: x => new { x.tenant_id, x.feed_id },
                        principalSchema: "integrations",
                        principalTable: "feeds",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_ingestion_records_ingestion_jobs_tenant_id_job_id",
                        columns: x => new { x.tenant_id, x.job_id },
                        principalSchema: "integrations",
                        principalTable: "ingestion_jobs",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_connections_tenant_id",
                schema: "integrations",
                table: "connections",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_connections_tenant_id_category_status_health",
                schema: "integrations",
                table: "connections",
                columns: new[] { "tenant_id", "category", "status", "health" });

            migrationBuilder.CreateIndex(
                name: "ix_connections_tenant_id_provider_code_name",
                schema: "integrations",
                table: "connections",
                columns: new[] { "tenant_id", "provider_code", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_credential_versions_tenant_id",
                schema: "integrations",
                table: "credential_versions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_credential_versions_tenant_id_connection_id_credential_vers",
                schema: "integrations",
                table: "credential_versions",
                columns: new[] { "tenant_id", "connection_id", "credential_version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_credential_versions_tenant_id_connection_id_status",
                schema: "integrations",
                table: "credential_versions",
                columns: new[] { "tenant_id", "connection_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_credential_versions_tenant_id_previous_credential_id",
                schema: "integrations",
                table: "credential_versions",
                columns: new[] { "tenant_id", "previous_credential_id" });

            migrationBuilder.CreateIndex(
                name: "ix_credential_versions_tenant_id_superseded_by_credential_id",
                schema: "integrations",
                table: "credential_versions",
                columns: new[] { "tenant_id", "superseded_by_credential_id" });

            migrationBuilder.CreateIndex(
                name: "ix_feeds_tenant_id",
                schema: "integrations",
                table: "feeds",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_feeds_tenant_id_connection_id_external_source_id",
                schema: "integrations",
                table: "feeds",
                columns: new[] { "tenant_id", "connection_id", "external_source_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_feeds_tenant_id_farm_id_feed_type_status",
                schema: "integrations",
                table: "feeds",
                columns: new[] { "tenant_id", "farm_id", "feed_type", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_feeds_tenant_id_status_next_poll_at",
                schema: "integrations",
                table: "feeds",
                columns: new[] { "tenant_id", "status", "next_poll_at" });

            migrationBuilder.CreateIndex(
                name: "ix_ingestion_attempts_tenant_id",
                schema: "integrations",
                table: "ingestion_attempts",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_ingestion_attempts_tenant_id_completed_at",
                schema: "integrations",
                table: "ingestion_attempts",
                columns: new[] { "tenant_id", "completed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_ingestion_attempts_tenant_id_job_id_attempt_number",
                schema: "integrations",
                table: "ingestion_attempts",
                columns: new[] { "tenant_id", "job_id", "attempt_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ingestion_jobs_tenant_id",
                schema: "integrations",
                table: "ingestion_jobs",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_ingestion_jobs_tenant_id_farm_id_requested_at",
                schema: "integrations",
                table: "ingestion_jobs",
                columns: new[] { "tenant_id", "farm_id", "requested_at" });

            migrationBuilder.CreateIndex(
                name: "ix_ingestion_jobs_tenant_id_feed_id_idempotency_key",
                schema: "integrations",
                table: "ingestion_jobs",
                columns: new[] { "tenant_id", "feed_id", "idempotency_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ingestion_jobs_tenant_id_previous_job_id",
                schema: "integrations",
                table: "ingestion_jobs",
                columns: new[] { "tenant_id", "previous_job_id" });

            migrationBuilder.CreateIndex(
                name: "ix_ingestion_jobs_tenant_id_status_available_at_leased_until",
                schema: "integrations",
                table: "ingestion_jobs",
                columns: new[] { "tenant_id", "status", "available_at", "leased_until" });

            migrationBuilder.CreateIndex(
                name: "ix_ingestion_records_tenant_id",
                schema: "integrations",
                table: "ingestion_records",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_ingestion_records_tenant_id_farm_id_observed_at",
                schema: "integrations",
                table: "ingestion_records",
                columns: new[] { "tenant_id", "farm_id", "observed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_ingestion_records_tenant_id_feed_id_external_record_id_cont",
                schema: "integrations",
                table: "ingestion_records",
                columns: new[] { "tenant_id", "feed_id", "external_record_id", "content_sha256" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_ingestion_records_tenant_id_job_id_status",
                schema: "integrations",
                table: "ingestion_records",
                columns: new[] { "tenant_id", "job_id", "status" });

            migrationBuilder.Sql(
                """
                ALTER TABLE integrations.connections
                    ADD CONSTRAINT ck_integration_connections_state CHECK (
                        category BETWEEN 1 AND 8 AND status BETWEEN 1 AND 4 AND health BETWEEN 1 AND 4
                        AND (rate_limit_remaining IS NULL OR rate_limit_remaining >= 0)
                        AND ((status = 4) = (disabled_at IS NOT NULL AND disabled_by IS NOT NULL)));
                ALTER TABLE integrations.credential_versions
                    ADD CONSTRAINT ck_integration_credentials_state CHECK (
                        credential_version > 0 AND status BETWEEN 1 AND 3
                        AND (expires_at IS NULL OR expires_at > valid_from)
                        AND ((status = 1 AND superseded_at IS NULL AND superseded_by_credential_id IS NULL AND revoked_at IS NULL AND revoked_by IS NULL)
                          OR (status = 2 AND superseded_at IS NOT NULL AND superseded_by_credential_id IS NOT NULL AND revoked_at IS NULL AND revoked_by IS NULL)
                          OR (status = 3 AND superseded_at IS NULL AND superseded_by_credential_id IS NULL AND revoked_at IS NOT NULL AND revoked_by IS NOT NULL)));
                ALTER TABLE integrations.feeds
                    ADD CONSTRAINT ck_integration_feeds_state CHECK (
                        feed_type BETWEEN 1 AND 8 AND target_type BETWEEN 1 AND 5 AND status BETWEEN 1 AND 3
                        AND polling_interval_minutes BETWEEN 1 AND 43200
                        AND freshness_threshold_minutes BETWEEN 1 AND 129600
                        AND ((target_type = 1 AND target_id IS NULL) OR (target_type <> 1 AND target_id IS NOT NULL)));
                ALTER TABLE integrations.ingestion_jobs
                    ADD CONSTRAINT ck_ingestion_jobs_state CHECK (
                        kind BETWEEN 1 AND 5 AND status BETWEEN 1 AND 6
                        AND maximum_attempts BETWEEN 1 AND 20 AND attempts BETWEEN 0 AND maximum_attempts
                        AND (window_end IS NULL OR window_start IS NULL OR window_end > window_start)
                        AND ((status = 2 AND lease_id IS NOT NULL AND leased_until IS NOT NULL)
                          OR (status <> 2 AND lease_id IS NULL AND leased_until IS NULL))
                        AND ((status IN (3,5,6) AND completed_at IS NOT NULL)
                          OR (status NOT IN (3,5,6) AND completed_at IS NULL)));
                ALTER TABLE integrations.ingestion_attempts
                    ADD CONSTRAINT ck_ingestion_attempts_values CHECK (
                        attempt_number > 0 AND completed_at >= started_at AND outcome BETWEEN 1 AND 3
                        AND (http_status_code IS NULL OR http_status_code BETWEEN 100 AND 599)
                        AND request_units >= 0 AND cost >= 0 AND (cost = 0 OR currency IS NOT NULL));
                ALTER TABLE integrations.ingestion_records
                    ADD CONSTRAINT ck_ingestion_records_state CHECK (
                        schema_version > 0 AND quality BETWEEN 1 AND 4 AND status BETWEEN 1 AND 3
                        AND length(content_sha256) = 64
                        AND ((status = 1 AND processed_at IS NULL AND rejection_code IS NULL AND rejection_reason IS NULL)
                          OR (status = 2 AND processed_at IS NOT NULL AND rejection_code IS NULL AND rejection_reason IS NULL)
                          OR (status = 3 AND processed_at IS NOT NULL AND rejection_code IS NOT NULL AND rejection_reason IS NOT NULL)));

                CREATE OR REPLACE FUNCTION integrations.guard_connection_update() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_OP = 'DELETE' THEN
                        RAISE EXCEPTION 'Integration connections cannot be deleted.' USING ERRCODE = '55000';
                    END IF;
                    IF OLD.tenant_id <> NEW.tenant_id OR OLD.id <> NEW.id OR OLD.provider_code <> NEW.provider_code
                       OR OLD.category <> NEW.category OR OLD.created_at <> NEW.created_at OR OLD.created_by <> NEW.created_by THEN
                        RAISE EXCEPTION 'Integration connection identity and origin are immutable.' USING ERRCODE = '55000';
                    END IF;
                    IF OLD.status = 4 AND NEW IS DISTINCT FROM OLD THEN
                        RAISE EXCEPTION 'Disabled integration connections are immutable.' USING ERRCODE = '55000';
                    END IF;
                    RETURN NEW;
                END $$;
                CREATE TRIGGER integration_connection_guard BEFORE UPDATE OR DELETE ON integrations.connections
                    FOR EACH ROW EXECUTE FUNCTION integrations.guard_connection_update();

                CREATE OR REPLACE FUNCTION integrations.guard_credential_update() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_OP = 'DELETE' THEN
                        RAISE EXCEPTION 'Credential versions cannot be deleted.' USING ERRCODE = '55000';
                    END IF;
                    IF OLD.tenant_id <> NEW.tenant_id OR OLD.id <> NEW.id OR OLD.connection_id <> NEW.connection_id
                       OR OLD.credential_version <> NEW.credential_version OR OLD.secret_provider <> NEW.secret_provider
                       OR OLD.protected_reference <> NEW.protected_reference OR OLD.fingerprint <> NEW.fingerprint
                       OR OLD.valid_from <> NEW.valid_from OR OLD.expires_at IS DISTINCT FROM NEW.expires_at
                       OR OLD.created_at <> NEW.created_at OR OLD.created_by <> NEW.created_by
                       OR OLD.previous_credential_id IS DISTINCT FROM NEW.previous_credential_id THEN
                        RAISE EXCEPTION 'Credential material and version identity are immutable.' USING ERRCODE = '55000';
                    END IF;
                    IF OLD.status <> 1 OR NEW.status NOT IN (2,3) THEN
                        RAISE EXCEPTION 'Only an active credential can transition to superseded or revoked.' USING ERRCODE = '55000';
                    END IF;
                    RETURN NEW;
                END $$;
                CREATE TRIGGER integration_credential_guard BEFORE UPDATE OR DELETE ON integrations.credential_versions
                    FOR EACH ROW EXECUTE FUNCTION integrations.guard_credential_update();

                CREATE OR REPLACE FUNCTION integrations.ensure_single_active_credential() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF (SELECT count(*) FROM integrations.credential_versions c
                        WHERE c.tenant_id = NEW.tenant_id AND c.connection_id = NEW.connection_id AND c.status = 1) > 1 THEN
                        RAISE EXCEPTION 'Only one active credential is allowed per integration connection.' USING ERRCODE = '23505';
                    END IF;
                    RETURN NULL;
                END $$;
                CREATE CONSTRAINT TRIGGER integration_single_active_credential
                    AFTER INSERT OR UPDATE OF status ON integrations.credential_versions
                    DEFERRABLE INITIALLY DEFERRED FOR EACH ROW
                    EXECUTE FUNCTION integrations.ensure_single_active_credential();

                CREATE OR REPLACE FUNCTION integrations.guard_feed_update() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_OP = 'DELETE' THEN
                        RAISE EXCEPTION 'Integration feeds cannot be deleted.' USING ERRCODE = '55000';
                    END IF;
                    IF OLD.tenant_id <> NEW.tenant_id OR OLD.id <> NEW.id OR OLD.connection_id <> NEW.connection_id
                       OR OLD.farm_id <> NEW.farm_id OR OLD.feed_type <> NEW.feed_type OR OLD.target_type <> NEW.target_type
                       OR OLD.target_id IS DISTINCT FROM NEW.target_id OR OLD.external_source_id <> NEW.external_source_id THEN
                        RAISE EXCEPTION 'Integration feed identity, scope, and provider source are immutable.' USING ERRCODE = '55000';
                    END IF;
                    IF OLD.status = 3 AND NEW IS DISTINCT FROM OLD THEN
                        RAISE EXCEPTION 'Disabled integration feeds are immutable.' USING ERRCODE = '55000';
                    END IF;
                    RETURN NEW;
                END $$;
                CREATE TRIGGER integration_feed_guard BEFORE UPDATE OR DELETE ON integrations.feeds
                    FOR EACH ROW EXECUTE FUNCTION integrations.guard_feed_update();

                CREATE OR REPLACE FUNCTION integrations.guard_job_update() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_OP = 'DELETE' THEN
                        RAISE EXCEPTION 'Ingestion jobs cannot be deleted.' USING ERRCODE = '55000';
                    END IF;
                    IF OLD.tenant_id <> NEW.tenant_id OR OLD.id <> NEW.id OR OLD.feed_id <> NEW.feed_id
                       OR OLD.farm_id <> NEW.farm_id OR OLD.kind <> NEW.kind OR OLD.idempotency_key <> NEW.idempotency_key
                       OR OLD.requested_at <> NEW.requested_at OR OLD.requested_by <> NEW.requested_by
                       OR OLD.maximum_attempts <> NEW.maximum_attempts OR OLD.window_start IS DISTINCT FROM NEW.window_start
                       OR OLD.window_end IS DISTINCT FROM NEW.window_end OR OLD.previous_job_id IS DISTINCT FROM NEW.previous_job_id THEN
                        RAISE EXCEPTION 'Ingestion job identity and request are immutable.' USING ERRCODE = '55000';
                    END IF;
                    IF OLD.status IN (3,5,6) THEN
                        RAISE EXCEPTION 'Terminal ingestion jobs are immutable.' USING ERRCODE = '55000';
                    END IF;
                    IF NOT ((OLD.status IN (1,4) AND NEW.status IN (2,6))
                         OR (OLD.status = 2 AND NEW.status IN (2,3,4,5,6))) THEN
                        RAISE EXCEPTION 'Invalid ingestion job transition.' USING ERRCODE = '55000';
                    END IF;
                    RETURN NEW;
                END $$;
                CREATE TRIGGER integration_job_guard BEFORE UPDATE OR DELETE ON integrations.ingestion_jobs
                    FOR EACH ROW EXECUTE FUNCTION integrations.guard_job_update();

                CREATE OR REPLACE FUNCTION integrations.reject_mutation() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    RAISE EXCEPTION 'Ingestion attempts are immutable.' USING ERRCODE = '55000';
                END $$;
                CREATE TRIGGER integration_attempts_immutable BEFORE UPDATE OR DELETE ON integrations.ingestion_attempts
                    FOR EACH ROW EXECUTE FUNCTION integrations.reject_mutation();

                CREATE OR REPLACE FUNCTION integrations.guard_record_update() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_OP = 'DELETE' THEN
                        RAISE EXCEPTION 'Ingestion records cannot be deleted.' USING ERRCODE = '55000';
                    END IF;
                    IF OLD.tenant_id <> NEW.tenant_id OR OLD.id <> NEW.id OR OLD.job_id <> NEW.job_id
                       OR OLD.feed_id <> NEW.feed_id OR OLD.farm_id <> NEW.farm_id
                       OR OLD.external_record_id <> NEW.external_record_id OR OLD.schema_version <> NEW.schema_version
                       OR OLD.observed_at <> NEW.observed_at OR OLD.received_at <> NEW.received_at
                       OR OLD.source_modified_at IS DISTINCT FROM NEW.source_modified_at
                       OR OLD.content_sha256 <> NEW.content_sha256 OR OLD.payload_json <> NEW.payload_json
                       OR OLD.quality <> NEW.quality THEN
                        RAISE EXCEPTION 'Ingestion record provenance and raw payload are immutable.' USING ERRCODE = '55000';
                    END IF;
                    IF OLD.status <> 1 OR NEW.status NOT IN (2,3) THEN
                        RAISE EXCEPTION 'Only a received record can be processed or rejected.' USING ERRCODE = '55000';
                    END IF;
                    RETURN NEW;
                END $$;
                CREATE TRIGGER integration_record_guard BEFORE UPDATE OR DELETE ON integrations.ingestion_records
                    FOR EACH ROW EXECUTE FUNCTION integrations.guard_record_update();

                DO $$
                DECLARE table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY['connections','credential_versions','feeds','ingestion_jobs','ingestion_attempts','ingestion_records'] LOOP
                        EXECUTE format('ALTER TABLE integrations.%I ENABLE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('ALTER TABLE integrations.%I FORCE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('CREATE POLICY tenant_isolation ON integrations.%I USING (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid) WITH CHECK (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid)', table_name);
                    END LOOP;
                END $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP TRIGGER IF EXISTS integration_single_active_credential ON integrations.credential_versions;
                DROP TRIGGER IF EXISTS integration_connection_guard ON integrations.connections;
                DROP TRIGGER IF EXISTS integration_credential_guard ON integrations.credential_versions;
                DROP TRIGGER IF EXISTS integration_feed_guard ON integrations.feeds;
                DROP TRIGGER IF EXISTS integration_job_guard ON integrations.ingestion_jobs;
                DROP TRIGGER IF EXISTS integration_attempts_immutable ON integrations.ingestion_attempts;
                DROP TRIGGER IF EXISTS integration_record_guard ON integrations.ingestion_records;
                DROP FUNCTION IF EXISTS integrations.ensure_single_active_credential();
                DROP FUNCTION IF EXISTS integrations.guard_connection_update();
                DROP FUNCTION IF EXISTS integrations.guard_credential_update();
                DROP FUNCTION IF EXISTS integrations.guard_feed_update();
                DROP FUNCTION IF EXISTS integrations.guard_job_update();
                DROP FUNCTION IF EXISTS integrations.reject_mutation();
                DROP FUNCTION IF EXISTS integrations.guard_record_update();
                """);

            migrationBuilder.DropTable(
                name: "credential_versions",
                schema: "integrations");

            migrationBuilder.DropTable(
                name: "ingestion_attempts",
                schema: "integrations");

            migrationBuilder.DropTable(
                name: "ingestion_records",
                schema: "integrations");

            migrationBuilder.DropTable(
                name: "ingestion_jobs",
                schema: "integrations");

            migrationBuilder.DropTable(
                name: "feeds",
                schema: "integrations");

            migrationBuilder.DropTable(
                name: "connections",
                schema: "integrations");
        }
    }
}
