using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddSaasControlPlane : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "saas");

            migrationBuilder.CreateTable(
                name: "billing_accounts",
                schema: "saas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    provider = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    provider_customer_reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    billing_email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: false),
                    tax_identifier = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_billing_accounts", x => x.id);
                    table.UniqueConstraint("ak_saas_billing_accounts_tenant_id_id", x => new { x.tenant_id, x.id });
                });

            migrationBuilder.CreateTable(
                name: "plan_versions",
                schema: "saas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    series_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    plan_version = table.Column<int>(type: "integer", nullable: false),
                    billing_interval = table.Column<int>(type: "integer", nullable: false),
                    base_price = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    effective_from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    effective_to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    status = table.Column<int>(type: "integer", nullable: false),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    published_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    retired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plan_versions", x => x.id);
                    table.UniqueConstraint("ak_saas_plan_versions_tenant_id_id", x => new { x.tenant_id, x.id });
                });

            migrationBuilder.CreateTable(
                name: "tenant_accounts",
                schema: "saas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    legal_name = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    display_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    slug = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    country_code = table.Column<string>(type: "character(2)", fixedLength: true, maxLength: 2, nullable: false),
                    billing_currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    time_zone = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    provisioned_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    provisioned_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    activated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    suspended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    suspension_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tenant_accounts", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usage_events",
                schema: "saas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    idempotency_key = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    metric_code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(20,4)", precision: 20, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    source = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    dimensions_json = table.Column<string>(type: "jsonb", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usage_events", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "usage_periods",
                schema: "saas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    metric_code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    unit = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    period_starts_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    period_ends_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(20,4)", precision: 20, scale: 4, nullable: false),
                    last_metered_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_usage_periods", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "plan_entitlements",
                schema: "saas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    key = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    kind = table.Column<int>(type: "integer", nullable: false),
                    included_amount = table.Column<decimal>(type: "numeric(20,4)", precision: 20, scale: 4, nullable: true),
                    unit = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    enforcement = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_plan_entitlements", x => x.id);
                    table.ForeignKey(
                        name: "fk_plan_entitlements_plan_versions_tenant_id_plan_version_id",
                        columns: x => new { x.tenant_id, x.plan_version_id },
                        principalSchema: "saas",
                        principalTable: "plan_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subscriptions",
                schema: "saas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    plan_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    starts_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    current_period_starts_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    current_period_ends_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    trial_ends_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    provider_subscription_reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    created_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    cancel_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    ended_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    status_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subscriptions", x => x.id);
                    table.UniqueConstraint("ak_saas_subscriptions_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_subscriptions_plan_versions_tenant_id_plan_version_id",
                        columns: x => new { x.tenant_id, x.plan_version_id },
                        principalSchema: "saas",
                        principalTable: "plan_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "invoices",
                schema: "saas",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    billing_account_id = table.Column<Guid>(type: "uuid", nullable: false),
                    subscription_id = table.Column<Guid>(type: "uuid", nullable: true),
                    invoice_number = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    provider_invoice_reference = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    subtotal = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    tax = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    total = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    issued_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    due_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    paid_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    closed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    status_reason = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_invoices", x => x.id);
                    table.ForeignKey(
                        name: "fk_invoices_billing_accounts_tenant_id_billing_account_id",
                        columns: x => new { x.tenant_id, x.billing_account_id },
                        principalSchema: "saas",
                        principalTable: "billing_accounts",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_invoices_subscriptions_tenant_id_subscription_id",
                        columns: x => new { x.tenant_id, x.subscription_id },
                        principalSchema: "saas",
                        principalTable: "subscriptions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_billing_accounts_provider_provider_customer_reference",
                schema: "saas",
                table: "billing_accounts",
                columns: new[] { "provider", "provider_customer_reference" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_billing_accounts_tenant_id",
                schema: "saas",
                table: "billing_accounts",
                column: "tenant_id",
                unique: true,
                filter: "status <> 3");

            migrationBuilder.CreateIndex(
                name: "ix_invoices_tenant_id",
                schema: "saas",
                table: "invoices",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_invoices_tenant_id_billing_account_id",
                schema: "saas",
                table: "invoices",
                columns: new[] { "tenant_id", "billing_account_id" });

            migrationBuilder.CreateIndex(
                name: "ix_invoices_tenant_id_invoice_number",
                schema: "saas",
                table: "invoices",
                columns: new[] { "tenant_id", "invoice_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_invoices_tenant_id_provider_invoice_reference",
                schema: "saas",
                table: "invoices",
                columns: new[] { "tenant_id", "provider_invoice_reference" },
                unique: true,
                filter: "provider_invoice_reference IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_invoices_tenant_id_status_due_at",
                schema: "saas",
                table: "invoices",
                columns: new[] { "tenant_id", "status", "due_at" });

            migrationBuilder.CreateIndex(
                name: "ix_invoices_tenant_id_subscription_id",
                schema: "saas",
                table: "invoices",
                columns: new[] { "tenant_id", "subscription_id" });

            migrationBuilder.CreateIndex(
                name: "ix_plan_entitlements_tenant_id",
                schema: "saas",
                table: "plan_entitlements",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_entitlements_tenant_id_plan_version_id_key",
                schema: "saas",
                table: "plan_entitlements",
                columns: new[] { "tenant_id", "plan_version_id", "key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_plan_versions_tenant_id",
                schema: "saas",
                table: "plan_versions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_plan_versions_tenant_id_code_status",
                schema: "saas",
                table: "plan_versions",
                columns: new[] { "tenant_id", "code", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_plan_versions_tenant_id_series_id_plan_version",
                schema: "saas",
                table: "plan_versions",
                columns: new[] { "tenant_id", "series_id", "plan_version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_tenant_id",
                schema: "saas",
                table: "subscriptions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_tenant_id_plan_version_id",
                schema: "saas",
                table: "subscriptions",
                columns: new[] { "tenant_id", "plan_version_id" });

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_tenant_id_provider_subscription_reference",
                schema: "saas",
                table: "subscriptions",
                columns: new[] { "tenant_id", "provider_subscription_reference" },
                unique: true,
                filter: "provider_subscription_reference IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_subscriptions_tenant_id_status",
                schema: "saas",
                table: "subscriptions",
                columns: new[] { "tenant_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_tenant_accounts_slug",
                schema: "saas",
                table: "tenant_accounts",
                column: "slug",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_tenant_accounts_tenant_id",
                schema: "saas",
                table: "tenant_accounts",
                column: "tenant_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_usage_events_tenant_id",
                schema: "saas",
                table: "usage_events",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_usage_events_tenant_id_idempotency_key",
                schema: "saas",
                table: "usage_events",
                columns: new[] { "tenant_id", "idempotency_key" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_usage_events_tenant_id_metric_code_occurred_at",
                schema: "saas",
                table: "usage_events",
                columns: new[] { "tenant_id", "metric_code", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_usage_periods_tenant_id",
                schema: "saas",
                table: "usage_periods",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_usage_periods_tenant_id_metric_code_period_starts_at_period",
                schema: "saas",
                table: "usage_periods",
                columns: new[] { "tenant_id", "metric_code", "period_starts_at", "period_ends_at" },
                unique: true);

            migrationBuilder.Sql("""
                ALTER TABLE saas.tenant_accounts
                    ADD CONSTRAINT ck_saas_tenant_account_status CHECK (status BETWEEN 1 AND 4),
                    ADD CONSTRAINT ck_saas_tenant_account_country CHECK (country_code ~ '^[A-Z]{2}$'),
                    ADD CONSTRAINT ck_saas_tenant_account_currency CHECK (billing_currency ~ '^[A-Z]{3}$');
                ALTER TABLE saas.plan_versions
                    ADD CONSTRAINT ck_saas_plan_status CHECK (status BETWEEN 1 AND 3),
                    ADD CONSTRAINT ck_saas_plan_interval CHECK (billing_interval BETWEEN 1 AND 2),
                    ADD CONSTRAINT ck_saas_plan_price CHECK (base_price >= 0),
                    ADD CONSTRAINT ck_saas_plan_version CHECK (plan_version > 0),
                    ADD CONSTRAINT ck_saas_plan_dates CHECK (effective_to IS NULL OR effective_to > effective_from),
                    ADD CONSTRAINT ck_saas_plan_currency CHECK (currency ~ '^[A-Z]{3}$');
                ALTER TABLE saas.plan_entitlements
                    ADD CONSTRAINT ck_saas_entitlement_kind CHECK (kind BETWEEN 1 AND 2),
                    ADD CONSTRAINT ck_saas_entitlement_enforcement CHECK (enforcement BETWEEN 1 AND 3),
                    ADD CONSTRAINT ck_saas_entitlement_amount CHECK (
                        (kind=1 AND included_amount IS NULL) OR (kind=2 AND included_amount >= 0));
                ALTER TABLE saas.subscriptions
                    ADD CONSTRAINT ck_saas_subscription_status CHECK (status BETWEEN 1 AND 6),
                    ADD CONSTRAINT ck_saas_subscription_period CHECK (
                        current_period_starts_at >= starts_at AND current_period_ends_at > current_period_starts_at),
                    ADD CONSTRAINT ck_saas_subscription_trial CHECK (
                        trial_ends_at IS NULL OR (trial_ends_at > starts_at AND trial_ends_at <= current_period_ends_at));
                ALTER TABLE saas.usage_events
                    ADD CONSTRAINT ck_saas_usage_event_quantity CHECK (quantity > 0),
                    ADD CONSTRAINT ck_saas_usage_event_dimensions CHECK (jsonb_typeof(dimensions_json)='object');
                ALTER TABLE saas.usage_periods
                    ADD CONSTRAINT ck_saas_usage_period CHECK (period_ends_at > period_starts_at AND quantity > 0);
                ALTER TABLE saas.billing_accounts
                    ADD CONSTRAINT ck_saas_billing_account_status CHECK (status BETWEEN 1 AND 3);
                ALTER TABLE saas.invoices
                    ADD CONSTRAINT ck_saas_invoice_status CHECK (status BETWEEN 1 AND 5),
                    ADD CONSTRAINT ck_saas_invoice_totals CHECK (subtotal >= 0 AND tax >= 0 AND total=subtotal+tax),
                    ADD CONSTRAINT ck_saas_invoice_dates CHECK (due_at >= issued_at),
                    ADD CONSTRAINT ck_saas_invoice_currency CHECK (currency ~ '^[A-Z]{3}$');

                CREATE UNIQUE INDEX ux_saas_published_plan_code ON saas.plan_versions(tenant_id,code) WHERE status=2;
                CREATE UNIQUE INDEX ux_saas_current_subscription ON saas.subscriptions(tenant_id) WHERE status IN (1,2,3,4);

                DO $rls$
                DECLARE table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY[
                        'tenant_accounts','plan_versions','plan_entitlements','subscriptions',
                        'usage_events','usage_periods','billing_accounts','invoices'] LOOP
                        EXECUTE format('ALTER TABLE saas.%I ENABLE ROW LEVEL SECURITY',table_name);
                        EXECUTE format('ALTER TABLE saas.%I FORCE ROW LEVEL SECURITY',table_name);
                        EXECUTE format('CREATE POLICY tenant_isolation ON saas.%I USING (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid) WITH CHECK (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid)',table_name);
                    END LOOP;
                END
                $rls$;

                DO $grant$ BEGIN IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname='murimios_app') THEN
                    GRANT USAGE ON SCHEMA saas TO murimios_app;
                    GRANT SELECT,INSERT,UPDATE ON ALL TABLES IN SCHEMA saas TO murimios_app;
                    REVOKE DELETE ON ALL TABLES IN SCHEMA saas FROM murimios_app;
                    REVOKE UPDATE ON saas.usage_events FROM murimios_app;
                END IF; END $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $grant$ BEGIN IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname='murimios_app') THEN
                    REVOKE ALL PRIVILEGES ON ALL TABLES IN SCHEMA saas FROM murimios_app;
                    REVOKE USAGE ON SCHEMA saas FROM murimios_app;
                END IF; END $grant$;
                """);
            migrationBuilder.DropTable(
                name: "invoices",
                schema: "saas");

            migrationBuilder.DropTable(
                name: "plan_entitlements",
                schema: "saas");

            migrationBuilder.DropTable(
                name: "tenant_accounts",
                schema: "saas");

            migrationBuilder.DropTable(
                name: "usage_events",
                schema: "saas");

            migrationBuilder.DropTable(
                name: "usage_periods",
                schema: "saas");

            migrationBuilder.DropTable(
                name: "billing_accounts",
                schema: "saas");

            migrationBuilder.DropTable(
                name: "subscriptions",
                schema: "saas");

            migrationBuilder.DropTable(
                name: "plan_versions",
                schema: "saas");
        }
    }
}
