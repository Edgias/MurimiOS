using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentitySecurityAndQuotas : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_events1",
                schema: "traceability",
                table: "events");

            migrationBuilder.EnsureSchema(
                name: "security");

            migrationBuilder.RenameIndex(
                name: "ix_events_tenant_id",
                schema: "traceability",
                table: "events",
                newName: "ix_events_tenant_id1");

            migrationBuilder.AddPrimaryKey(
                name: "pk_events2",
                schema: "traceability",
                table: "events",
                column: "id");

            migrationBuilder.CreateTable(
                name: "credential_rotation_policies",
                schema: "security",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    public_api_client_id = table.Column<Guid>(type: "uuid", nullable: true),
                    maximum_age_days = table.Column<int>(type: "integer", nullable: false),
                    warning_days = table.Column<int>(type: "integer", nullable: false),
                    require_expiry = table.Column<bool>(type: "boolean", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    retired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_credential_rotation_policies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "devices",
                schema: "security",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_key_hash = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    subject = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    platform = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    first_seen_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_seen_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revoked_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    revocation_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_devices1", x => x.id);
                    table.UniqueConstraint("ak_security_devices_tenant_id_id", x => new { x.tenant_id, x.id });
                });

            migrationBuilder.CreateTable(
                name: "rate_limit_policies",
                schema: "security",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    public_api_client_id = table.Column<Guid>(type: "uuid", nullable: true),
                    request_limit = table.Column<int>(type: "integer", nullable: false),
                    window_seconds = table.Column<int>(type: "integer", nullable: false),
                    daily_quota = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    is_active = table.Column<bool>(type: "boolean", nullable: false),
                    retired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_rate_limit_policies", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "recovery_requests",
                schema: "security",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    membership_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    token_prefix = table.Column<string>(type: "character varying(24)", maxLength: 24, nullable: false),
                    token_hash = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    requested_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    maximum_attempts = table.Column<int>(type: "integer", nullable: false),
                    failed_attempts = table.Column<int>(type: "integer", nullable: false),
                    requested_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    verified_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    closed_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_recovery_requests", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "sessions",
                schema: "security",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    session_key_hash = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    subject = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    device_id = table.Column<Guid>(type: "uuid", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    authenticated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    last_seen_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    mfa_satisfied = table.Column<bool>(type: "boolean", nullable: false),
                    authentication_method = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    remote_address_hash = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: true),
                    user_agent_hash = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: true),
                    revoked_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    revoked_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    revocation_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_sessions", x => x.id);
                    table.UniqueConstraint("ak_security_sessions_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_sessions_devices_tenant_id_device_id",
                        columns: x => new { x.tenant_id, x.device_id },
                        principalSchema: "security",
                        principalTable: "devices",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "events",
                schema: "security",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    outcome = table.Column<int>(type: "integer", nullable: false),
                    subject = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    session_id = table.Column<Guid>(type: "uuid", nullable: true),
                    device_id = table.Column<Guid>(type: "uuid", nullable: true),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    correlation_id = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    remote_address_hash = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: true),
                    user_agent_hash = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: true),
                    details_json = table.Column<string>(type: "jsonb", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_events1", x => x.id);
                    table.ForeignKey(
                        name: "fk_events_devices_tenant_id_device_id",
                        columns: x => new { x.tenant_id, x.device_id },
                        principalSchema: "security",
                        principalTable: "devices",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_events_sessions_tenant_id_session_id",
                        columns: x => new { x.tenant_id, x.session_id },
                        principalSchema: "security",
                        principalTable: "sessions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_credential_rotation_policies_tenant_id",
                schema: "security",
                table: "credential_rotation_policies",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_credential_rotation_policies_tenant_id_public_api_client_id",
                schema: "security",
                table: "credential_rotation_policies",
                columns: new[] { "tenant_id", "public_api_client_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_devices_tenant_id_device_key_hash",
                schema: "security",
                table: "devices",
                columns: new[] { "tenant_id", "device_key_hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_devices_tenant_id_subject_status",
                schema: "security",
                table: "devices",
                columns: new[] { "tenant_id", "subject", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_devices_tenant_id1",
                schema: "security",
                table: "devices",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_events_tenant_id",
                schema: "security",
                table: "events",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_events_tenant_id_device_id",
                schema: "security",
                table: "events",
                columns: new[] { "tenant_id", "device_id" });

            migrationBuilder.CreateIndex(
                name: "ix_events_tenant_id_event_type_outcome_occurred_at",
                schema: "security",
                table: "events",
                columns: new[] { "tenant_id", "event_type", "outcome", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_events_tenant_id_occurred_at1",
                schema: "security",
                table: "events",
                columns: new[] { "tenant_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_events_tenant_id_session_id",
                schema: "security",
                table: "events",
                columns: new[] { "tenant_id", "session_id" });

            migrationBuilder.CreateIndex(
                name: "ix_rate_limit_policies_tenant_id",
                schema: "security",
                table: "rate_limit_policies",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_rate_limit_policies_tenant_id_public_api_client_id_is_active",
                schema: "security",
                table: "rate_limit_policies",
                columns: new[] { "tenant_id", "public_api_client_id", "is_active" });

            migrationBuilder.CreateIndex(
                name: "ix_recovery_requests_tenant_id",
                schema: "security",
                table: "recovery_requests",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_recovery_requests_tenant_id_membership_id_status",
                schema: "security",
                table: "recovery_requests",
                columns: new[] { "tenant_id", "membership_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_recovery_requests_tenant_id_token_hash",
                schema: "security",
                table: "recovery_requests",
                columns: new[] { "tenant_id", "token_hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sessions_tenant_id",
                schema: "security",
                table: "sessions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_sessions_tenant_id_device_id",
                schema: "security",
                table: "sessions",
                columns: new[] { "tenant_id", "device_id" });

            migrationBuilder.CreateIndex(
                name: "ix_sessions_tenant_id_session_key_hash",
                schema: "security",
                table: "sessions",
                columns: new[] { "tenant_id", "session_key_hash" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_sessions_tenant_id_subject_status_expires_at",
                schema: "security",
                table: "sessions",
                columns: new[] { "tenant_id", "subject", "status", "expires_at" });

            migrationBuilder.Sql("""
                CREATE TABLE security.request_quota_buckets (
                    tenant_id uuid NOT NULL,
                    scope_type smallint NOT NULL,
                    scope_id uuid NOT NULL,
                    bucket_kind smallint NOT NULL,
                    bucket_start timestamp with time zone NOT NULL,
                    bucket_end timestamp with time zone NOT NULL,
                    request_count integer NOT NULL,
                    CONSTRAINT pk_request_quota_buckets PRIMARY KEY
                        (tenant_id, scope_type, scope_id, bucket_kind, bucket_start),
                    CONSTRAINT ck_request_quota_buckets_scope_type CHECK (scope_type IN (1, 2)),
                    CONSTRAINT ck_request_quota_buckets_bucket_kind CHECK (bucket_kind IN (1, 2)),
                    CONSTRAINT ck_request_quota_buckets_count CHECK (request_count >= 0),
                    CONSTRAINT ck_request_quota_buckets_period CHECK (bucket_end > bucket_start)
                );
                CREATE INDEX ix_request_quota_buckets_bucket_end
                    ON security.request_quota_buckets (bucket_end);

                ALTER TABLE security.rate_limit_policies
                    ADD CONSTRAINT ck_rate_limit_policies_limits
                    CHECK (request_limit > 0 AND window_seconds > 0 AND window_seconds <= 86400 AND daily_quota > 0),
                    ADD CONSTRAINT ck_rate_limit_policies_retirement
                    CHECK ((is_active AND retired_at IS NULL) OR (NOT is_active AND retired_at IS NOT NULL));
                CREATE UNIQUE INDEX ux_rate_limit_policies_active_tenant_default
                    ON security.rate_limit_policies (tenant_id)
                    WHERE public_api_client_id IS NULL AND is_active;
                CREATE UNIQUE INDEX ux_rate_limit_policies_active_client
                    ON security.rate_limit_policies (tenant_id, public_api_client_id)
                    WHERE public_api_client_id IS NOT NULL AND is_active;

                ALTER TABLE security.credential_rotation_policies
                    ADD CONSTRAINT ck_credential_rotation_policies_days
                    CHECK (maximum_age_days > 0 AND warning_days >= 0 AND warning_days < maximum_age_days),
                    ADD CONSTRAINT ck_credential_rotation_policies_retirement
                    CHECK ((is_active AND retired_at IS NULL) OR (NOT is_active AND retired_at IS NOT NULL));
                CREATE UNIQUE INDEX ux_credential_rotation_policies_active_tenant_default
                    ON security.credential_rotation_policies (tenant_id)
                    WHERE public_api_client_id IS NULL AND is_active;
                CREATE UNIQUE INDEX ux_credential_rotation_policies_active_client
                    ON security.credential_rotation_policies (tenant_id, public_api_client_id)
                    WHERE public_api_client_id IS NOT NULL AND is_active;

                ALTER TABLE security.devices
                    ADD CONSTRAINT ck_devices_status CHECK (status IN (1, 2)),
                    ADD CONSTRAINT ck_devices_hash CHECK (device_key_hash ~ '^[0-9A-Fa-f]{64}$'),
                    ADD CONSTRAINT ck_devices_revocation CHECK (
                        (status = 1 AND revoked_at IS NULL AND revoked_by IS NULL AND revocation_reason IS NULL)
                        OR (status = 2 AND revoked_at IS NOT NULL AND revoked_by IS NOT NULL AND revocation_reason IS NOT NULL));
                ALTER TABLE security.sessions
                    ADD CONSTRAINT ck_sessions_status CHECK (status IN (1, 2, 3)),
                    ADD CONSTRAINT ck_sessions_hash CHECK (session_key_hash ~ '^[0-9A-Fa-f]{64}$'),
                    ADD CONSTRAINT ck_sessions_period CHECK (expires_at > authenticated_at),
                    ADD CONSTRAINT ck_sessions_revocation CHECK (
                        (status = 2 AND revoked_at IS NOT NULL AND revoked_by IS NOT NULL AND revocation_reason IS NOT NULL)
                        OR (status IN (1, 3) AND revoked_at IS NULL AND revoked_by IS NULL AND revocation_reason IS NULL));
                ALTER TABLE security.events
                    ADD CONSTRAINT ck_security_events_outcome CHECK (outcome IN (1, 2, 3)),
                    ADD CONSTRAINT ck_security_events_details_json CHECK (jsonb_typeof(details_json) = 'object');
                ALTER TABLE security.recovery_requests
                    ADD CONSTRAINT ck_recovery_requests_status CHECK (status IN (1, 2, 3, 4, 5)),
                    ADD CONSTRAINT ck_recovery_requests_hash CHECK (token_hash ~ '^[0-9A-Fa-f]{64}$'),
                    ADD CONSTRAINT ck_recovery_requests_attempts CHECK (
                        maximum_attempts > 0 AND failed_attempts >= 0 AND failed_attempts <= maximum_attempts),
                    ADD CONSTRAINT ck_recovery_requests_period CHECK (expires_at > requested_at);

                DO $rls$
                DECLARE table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY[
                        'rate_limit_policies', 'credential_rotation_policies', 'devices', 'sessions',
                        'events', 'recovery_requests', 'request_quota_buckets']
                    LOOP
                        EXECUTE format('ALTER TABLE security.%I ENABLE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('ALTER TABLE security.%I FORCE ROW LEVEL SECURITY', table_name);
                        EXECUTE format(
                            'CREATE POLICY tenant_isolation ON security.%I USING (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid) WITH CHECK (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid)',
                            table_name);
                    END LOOP;
                END $rls$;

                CREATE OR REPLACE FUNCTION security.consume_request_quota(
                    p_tenant_id uuid,
                    p_client_id uuid,
                    p_now timestamp with time zone,
                    p_request_limit integer,
                    p_window_seconds integer,
                    p_daily_quota integer)
                RETURNS TABLE (
                    allowed boolean,
                    request_remaining integer,
                    daily_remaining integer,
                    retry_after_seconds integer)
                LANGUAGE plpgsql
                SECURITY DEFINER
                SET search_path = pg_catalog, security
                AS $function$
                DECLARE
                    v_scope_type smallint := CASE WHEN p_client_id IS NULL THEN 1 ELSE 2 END;
                    v_scope_id uuid := coalesce(p_client_id, p_tenant_id);
                    v_request_limit integer;
                    v_window_seconds integer;
                    v_daily_quota integer;
                    v_window_start timestamp with time zone;
                    v_window_end timestamp with time zone;
                    v_day_start timestamp with time zone;
                    v_day_end timestamp with time zone;
                    v_window_count integer;
                    v_day_count integer;
                BEGIN
                    IF p_tenant_id IS NULL OR p_now IS NULL THEN
                        RAISE EXCEPTION 'tenant and timestamp are required' USING ERRCODE = '22023';
                    END IF;

                    SELECT policy.request_limit, policy.window_seconds, policy.daily_quota
                    INTO v_request_limit, v_window_seconds, v_daily_quota
                    FROM security.rate_limit_policies AS policy
                    WHERE policy.tenant_id = p_tenant_id
                      AND policy.public_api_client_id IS NOT DISTINCT FROM p_client_id
                      AND policy.is_active
                    ORDER BY policy.created_at DESC, policy.id DESC
                    LIMIT 1;

                    v_request_limit := coalesce(v_request_limit, p_request_limit);
                    v_window_seconds := coalesce(v_window_seconds, p_window_seconds);
                    v_daily_quota := coalesce(v_daily_quota, p_daily_quota);
                    IF v_request_limit <= 0 OR v_window_seconds <= 0 OR v_window_seconds > 86400 OR v_daily_quota <= 0 THEN
                        RAISE EXCEPTION 'quota limits must be positive and window_seconds must not exceed 86400'
                            USING ERRCODE = '22023';
                    END IF;

                    v_window_start := to_timestamp(
                        floor(extract(epoch FROM p_now) / v_window_seconds) * v_window_seconds);
                    v_window_end := v_window_start + make_interval(secs => v_window_seconds);
                    v_day_start := date_trunc('day', p_now AT TIME ZONE 'UTC') AT TIME ZONE 'UTC';
                    v_day_end := v_day_start + interval '1 day';

                    PERFORM pg_advisory_xact_lock(hashtextextended(
                        p_tenant_id::text || ':' || v_scope_type::text || ':' || v_scope_id::text, 0));

                    SELECT bucket.request_count INTO v_window_count
                    FROM security.request_quota_buckets AS bucket
                    WHERE bucket.tenant_id = p_tenant_id
                      AND bucket.scope_type = v_scope_type
                      AND bucket.scope_id = v_scope_id
                      AND bucket.bucket_kind = 1
                      AND bucket.bucket_start = v_window_start;
                    SELECT bucket.request_count INTO v_day_count
                    FROM security.request_quota_buckets AS bucket
                    WHERE bucket.tenant_id = p_tenant_id
                      AND bucket.scope_type = v_scope_type
                      AND bucket.scope_id = v_scope_id
                      AND bucket.bucket_kind = 2
                      AND bucket.bucket_start = v_day_start;

                    v_window_count := coalesce(v_window_count, 0);
                    v_day_count := coalesce(v_day_count, 0);
                    IF v_window_count >= v_request_limit OR v_day_count >= v_daily_quota THEN
                        allowed := false;
                        request_remaining := greatest(v_request_limit - v_window_count, 0);
                        daily_remaining := greatest(v_daily_quota - v_day_count, 0);
                        retry_after_seconds := greatest(1, ceil(extract(epoch FROM
                            CASE WHEN v_day_count >= v_daily_quota THEN v_day_end - p_now
                                 ELSE v_window_end - p_now END))::integer);
                        RETURN NEXT;
                        RETURN;
                    END IF;

                    INSERT INTO security.request_quota_buckets
                        (tenant_id, scope_type, scope_id, bucket_kind, bucket_start, bucket_end, request_count)
                    VALUES
                        (p_tenant_id, v_scope_type, v_scope_id, 1, v_window_start, v_window_end, 1),
                        (p_tenant_id, v_scope_type, v_scope_id, 2, v_day_start, v_day_end, 1)
                    ON CONFLICT (tenant_id, scope_type, scope_id, bucket_kind, bucket_start)
                    DO UPDATE SET request_count = security.request_quota_buckets.request_count + 1;

                    allowed := true;
                    request_remaining := v_request_limit - v_window_count - 1;
                    daily_remaining := v_daily_quota - v_day_count - 1;
                    retry_after_seconds := 0;
                    RETURN NEXT;
                END
                $function$;

                REVOKE ALL ON FUNCTION security.consume_request_quota(uuid,uuid,timestamptz,integer,integer,integer)
                    FROM PUBLIC;
                DO $grant$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        GRANT USAGE ON SCHEMA security TO murimios_app;
                        GRANT SELECT, INSERT, UPDATE ON
                            security.rate_limit_policies, security.credential_rotation_policies,
                            security.devices, security.sessions, security.recovery_requests
                            TO murimios_app;
                        GRANT SELECT, INSERT ON security.events TO murimios_app;
                        GRANT EXECUTE ON FUNCTION security.consume_request_quota(uuid,uuid,timestamptz,integer,integer,integer)
                            TO murimios_app;
                    END IF;
                END $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $grant$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        REVOKE EXECUTE ON FUNCTION security.consume_request_quota(uuid,uuid,timestamptz,integer,integer,integer)
                            FROM murimios_app;
                    END IF;
                END $grant$;
                DROP FUNCTION IF EXISTS security.consume_request_quota(uuid,uuid,timestamptz,integer,integer,integer);
                DROP TABLE IF EXISTS security.request_quota_buckets;
                """);

            migrationBuilder.DropTable(
                name: "credential_rotation_policies",
                schema: "security");

            migrationBuilder.DropTable(
                name: "events",
                schema: "security");

            migrationBuilder.DropTable(
                name: "rate_limit_policies",
                schema: "security");

            migrationBuilder.DropTable(
                name: "recovery_requests",
                schema: "security");

            migrationBuilder.DropTable(
                name: "sessions",
                schema: "security");

            migrationBuilder.DropTable(
                name: "devices",
                schema: "security");

            migrationBuilder.DropPrimaryKey(
                name: "pk_events2",
                schema: "traceability",
                table: "events");

            migrationBuilder.RenameIndex(
                name: "ix_events_tenant_id1",
                schema: "traceability",
                table: "events",
                newName: "ix_events_tenant_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_events1",
                schema: "traceability",
                table: "events",
                column: "id");
        }
    }
}
