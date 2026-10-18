using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddEnterpriseLegalCompanyConsolidation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "counterparty_legal_company_id",
                schema: "finance",
                table: "transactions",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "intercompany_reference",
                schema: "finance",
                table: "transactions",
                type: "character varying(160)",
                maxLength: 160,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "consolidation_runs",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organisation_node_id = table.Column<Guid>(type: "uuid", nullable: false),
                    period_start = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    period_end = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    gross_expenses = table.Column<decimal>(type: "numeric(28,2)", precision: 28, scale: 2, nullable: false),
                    gross_revenue = table.Column<decimal>(type: "numeric(28,2)", precision: 28, scale: 2, nullable: false),
                    eliminated_expenses = table.Column<decimal>(type: "numeric(28,2)", precision: 28, scale: 2, nullable: false),
                    eliminated_revenue = table.Column<decimal>(type: "numeric(28,2)", precision: 28, scale: 2, nullable: false),
                    consolidated_expenses = table.Column<decimal>(type: "numeric(28,2)", precision: 28, scale: 2, nullable: false),
                    consolidated_revenue = table.Column<decimal>(type: "numeric(28,2)", precision: 28, scale: 2, nullable: false),
                    consolidated_margin = table.Column<decimal>(type: "numeric(28,2)", precision: 28, scale: 2, nullable: false),
                    farm_count = table.Column<int>(type: "integer", nullable: false),
                    company_count = table.Column<int>(type: "integer", nullable: false),
                    source_transaction_count = table.Column<int>(type: "integer", nullable: false),
                    exception_count = table.Column<int>(type: "integer", nullable: false),
                    source_sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    completed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    requested_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_consolidation_runs", x => x.id);
                    table.UniqueConstraint("ak_finance_consolidation_runs_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_consolidation_runs_org_nodes_tenant_id_organisation_node_id",
                        columns: x => new { x.tenant_id, x.organisation_node_id },
                        principalSchema: "platform",
                        principalTable: "org_nodes",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "legal_companies",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organisation_node_id = table.Column<Guid>(type: "uuid", nullable: false),
                    company_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    legal_name = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    registration_number = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    tax_identifier = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    country_code = table.Column<string>(type: "character(2)", fixedLength: true, maxLength: 2, nullable: false),
                    functional_currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_to = table.Column<DateOnly>(type: "date", nullable: true),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
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
                    table.PrimaryKey("pk_legal_companies", x => x.id);
                    table.UniqueConstraint("ak_finance_legal_companies_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_legal_companies_org_nodes_tenant_id_organisation_node_id",
                        columns: x => new { x.tenant_id, x.organisation_node_id },
                        principalSchema: "platform",
                        principalTable: "org_nodes",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "intercompany_elimination_rules",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    consolidation_organisation_node_id = table.Column<Guid>(type: "uuid", nullable: false),
                    seller_company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    buyer_company_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_key = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    rule_version = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    balancing_tolerance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_to = table.Column<DateOnly>(type: "date", nullable: true),
                    created_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    approved_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    approved_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    approval_reference = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    retired_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    retired_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_intercompany_elimination_rules", x => x.id);
                    table.UniqueConstraint("ak_finance_elimination_rules_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_intercompany_elimination_rules_legal_companies_tenant_id_bu",
                        columns: x => new { x.tenant_id, x.buyer_company_id },
                        principalSchema: "finance",
                        principalTable: "legal_companies",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_intercompany_elimination_rules_legal_companies_tenant_id_se",
                        columns: x => new { x.tenant_id, x.seller_company_id },
                        principalSchema: "finance",
                        principalTable: "legal_companies",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_intercompany_elimination_rules_org_nodes_tenant_id_consolid",
                        columns: x => new { x.tenant_id, x.consolidation_organisation_node_id },
                        principalSchema: "platform",
                        principalTable: "org_nodes",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "consolidation_elimination_entries",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    consolidation_run_id = table.Column<Guid>(type: "uuid", nullable: false),
                    rule_id = table.Column<Guid>(type: "uuid", nullable: false),
                    intercompany_reference = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    seller_revenue = table.Column<decimal>(type: "numeric(28,2)", precision: 28, scale: 2, nullable: false),
                    buyer_expense = table.Column<decimal>(type: "numeric(28,2)", precision: 28, scale: 2, nullable: false),
                    eliminated_amount = table.Column<decimal>(type: "numeric(28,2)", precision: 28, scale: 2, nullable: false),
                    difference = table.Column<decimal>(type: "numeric(28,2)", precision: 28, scale: 2, nullable: false),
                    tolerance = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    seller_transaction_ids_json = table.Column<string>(type: "jsonb", nullable: false),
                    buyer_transaction_ids_json = table.Column<string>(type: "jsonb", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_consolidation_elimination_entries", x => x.id);
                    table.ForeignKey(
                        name: "fk_consolidation_elimination_entries_consolidation_runs_tenant",
                        columns: x => new { x.tenant_id, x.consolidation_run_id },
                        principalSchema: "finance",
                        principalTable: "consolidation_runs",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_consolidation_elimination_entries_intercompany_elimination_",
                        columns: x => new { x.tenant_id, x.rule_id },
                        principalSchema: "finance",
                        principalTable: "intercompany_elimination_rules",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_transactions_tenant_id_counterparty_legal_company_id_interc",
                schema: "finance",
                table: "transactions",
                columns: new[] { "tenant_id", "counterparty_legal_company_id", "intercompany_reference", "occurred_at" },
                filter: "counterparty_legal_company_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_consolidation_elimination_entries_tenant_id",
                schema: "finance",
                table: "consolidation_elimination_entries",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_consolidation_elimination_entries_tenant_id_consolidation_r",
                schema: "finance",
                table: "consolidation_elimination_entries",
                columns: new[] { "tenant_id", "consolidation_run_id", "rule_id", "intercompany_reference" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_consolidation_elimination_entries_tenant_id_rule_id",
                schema: "finance",
                table: "consolidation_elimination_entries",
                columns: new[] { "tenant_id", "rule_id" });

            migrationBuilder.CreateIndex(
                name: "ix_consolidation_runs_tenant_id",
                schema: "finance",
                table: "consolidation_runs",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_consolidation_runs_tenant_id_organisation_node_id_period_end",
                schema: "finance",
                table: "consolidation_runs",
                columns: new[] { "tenant_id", "organisation_node_id", "period_end" });

            migrationBuilder.CreateIndex(
                name: "ix_intercompany_elimination_rules_tenant_id",
                schema: "finance",
                table: "intercompany_elimination_rules",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_intercompany_elimination_rules_tenant_id_buyer_company_id",
                schema: "finance",
                table: "intercompany_elimination_rules",
                columns: new[] { "tenant_id", "buyer_company_id" });

            migrationBuilder.CreateIndex(
                name: "ix_intercompany_elimination_rules_tenant_id_consolidation_orga",
                schema: "finance",
                table: "intercompany_elimination_rules",
                columns: new[] { "tenant_id", "consolidation_organisation_node_id", "status" });

            migrationBuilder.CreateIndex(
                name: "ix_intercompany_elimination_rules_tenant_id_rule_key_rule_vers",
                schema: "finance",
                table: "intercompany_elimination_rules",
                columns: new[] { "tenant_id", "rule_key", "rule_version" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_intercompany_elimination_rules_tenant_id_seller_company_id_",
                schema: "finance",
                table: "intercompany_elimination_rules",
                columns: new[] { "tenant_id", "seller_company_id", "buyer_company_id", "currency" },
                unique: true,
                filter: "status = 2");

            migrationBuilder.CreateIndex(
                name: "ix_legal_companies_tenant_id",
                schema: "finance",
                table: "legal_companies",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_legal_companies_tenant_id_company_code",
                schema: "finance",
                table: "legal_companies",
                columns: new[] { "tenant_id", "company_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_legal_companies_tenant_id_country_code_registration_number",
                schema: "finance",
                table: "legal_companies",
                columns: new[] { "tenant_id", "country_code", "registration_number" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_legal_companies_tenant_id_organisation_node_id",
                schema: "finance",
                table: "legal_companies",
                columns: new[] { "tenant_id", "organisation_node_id" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "fk_transactions_legal_companies_tenant_id_counterparty_legal_c",
                schema: "finance",
                table: "transactions",
                columns: new[] { "tenant_id", "counterparty_legal_company_id" },
                principalSchema: "finance",
                principalTable: "legal_companies",
                principalColumns: new[] { "tenant_id", "id" },
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.Sql("""
                ALTER TABLE finance.transactions ADD CONSTRAINT ck_finance_transaction_intercompany
                    CHECK((counterparty_legal_company_id IS NULL AND intercompany_reference IS NULL)
                       OR (counterparty_legal_company_id IS NOT NULL AND nullif(btrim(intercompany_reference),'') IS NOT NULL));
                ALTER TABLE finance.legal_companies ADD CONSTRAINT ck_finance_legal_company_lifecycle
                    CHECK((is_active AND effective_to IS NULL AND closed_at IS NULL AND closed_by IS NULL)
                       OR (NOT is_active AND effective_to IS NOT NULL AND effective_to>=effective_from
                           AND closed_at IS NOT NULL AND nullif(btrim(closed_by),'') IS NOT NULL));
                ALTER TABLE finance.intercompany_elimination_rules ADD CONSTRAINT ck_finance_elimination_rule
                    CHECK(seller_company_id<>buyer_company_id AND rule_version>0 AND balancing_tolerance>=0
                        AND (effective_to IS NULL OR effective_to>=effective_from) AND status BETWEEN 1 AND 3
                        AND ((status=1 AND approved_at IS NULL AND retired_at IS NULL)
                          OR (status=2 AND approved_at IS NOT NULL AND approved_by IS NOT NULL
                              AND approval_reference IS NOT NULL AND retired_at IS NULL)
                          OR (status=3 AND approved_at IS NOT NULL AND retired_at IS NOT NULL AND retired_by IS NOT NULL)));
                ALTER TABLE finance.consolidation_runs ADD CONSTRAINT ck_finance_consolidation_run
                    CHECK(period_end>period_start AND farm_count>=0 AND company_count>0 AND source_transaction_count>=0
                        AND exception_count>=0 AND eliminated_expenses>=0 AND eliminated_revenue>=0
                        AND consolidated_expenses=gross_expenses-eliminated_expenses
                        AND consolidated_revenue=gross_revenue-eliminated_revenue
                        AND consolidated_margin=consolidated_revenue-consolidated_expenses
                        AND source_sha256 ~ '^[0-9a-f]{64}$');
                ALTER TABLE finance.consolidation_elimination_entries ADD CONSTRAINT ck_finance_elimination_entry
                    CHECK(seller_revenue>=0 AND buyer_expense>=0 AND eliminated_amount>=0 AND tolerance>=0
                        AND eliminated_amount=least(seller_revenue,buyer_expense)
                        AND difference=seller_revenue-buyer_expense
                        AND status=CASE WHEN abs(difference)<=tolerance THEN 1 ELSE 2 END
                        AND jsonb_typeof(seller_transaction_ids_json)='array'
                        AND jsonb_typeof(buyer_transaction_ids_json)='array');

                DO $rls$
                DECLARE table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY['legal_companies','intercompany_elimination_rules',
                        'consolidation_runs','consolidation_elimination_entries'] LOOP
                        EXECUTE format('ALTER TABLE finance.%I ENABLE ROW LEVEL SECURITY',table_name);
                        EXECUTE format('ALTER TABLE finance.%I FORCE ROW LEVEL SECURITY',table_name);
                        EXECUTE format('CREATE POLICY tenant_isolation ON finance.%I USING (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid) WITH CHECK (tenant_id=nullif(current_setting(''app.tenant_id'',true),'''')::uuid)',table_name);
                    END LOOP;
                END $rls$;

                CREATE OR REPLACE FUNCTION finance.guard_legal_company() RETURNS trigger
                LANGUAGE plpgsql SET search_path=pg_catalog,finance,platform,public AS $function$
                DECLARE node_kind integer;
                BEGIN
                    IF TG_OP='DELETE' THEN RAISE EXCEPTION 'Legal companies cannot be deleted.' USING ERRCODE='55000'; END IF;
                    IF TG_OP='INSERT' THEN
                        SELECT node_type INTO node_kind FROM platform.org_nodes WHERE tenant_id=NEW.tenant_id AND id=NEW.organisation_node_id;
                        IF node_kind IS DISTINCT FROM 1 THEN RAISE EXCEPTION 'A legal company requires an organisation node.' USING ERRCODE='23514'; END IF;
                        RETURN NEW;
                    END IF;
                    IF (to_jsonb(NEW)-ARRAY['is_active','effective_to','closed_at','closed_by','updated_at_utc','version'])
                       IS DISTINCT FROM
                       (to_jsonb(OLD)-ARRAY['is_active','effective_to','closed_at','closed_by','updated_at_utc','version'])
                       OR NOT OLD.is_active OR NEW.is_active OR NEW.effective_to IS NULL OR NEW.closed_at IS NULL
                       OR nullif(btrim(NEW.closed_by),'') IS NULL OR NEW.version<>OLD.version+1 THEN
                        RAISE EXCEPTION 'A legal company may only transition once from active to closed.' USING ERRCODE='55000';
                    END IF;
                    RETURN NEW;
                END $function$;

                CREATE OR REPLACE FUNCTION finance.guard_elimination_rule() RETURNS trigger
                LANGUAGE plpgsql SET search_path=pg_catalog,finance,platform,public AS $function$
                DECLARE root_path ltree; seller_path ltree; buyer_path ltree;
                BEGIN
                    IF TG_OP='DELETE' THEN RAISE EXCEPTION 'Elimination rules cannot be deleted.' USING ERRCODE='55000'; END IF;
                    IF TG_OP='INSERT' THEN
                        SELECT path INTO root_path FROM platform.org_nodes WHERE tenant_id=NEW.tenant_id AND id=NEW.consolidation_organisation_node_id;
                        SELECT n.path INTO seller_path FROM finance.legal_companies c JOIN platform.org_nodes n
                            ON n.tenant_id=c.tenant_id AND n.id=c.organisation_node_id
                            WHERE c.tenant_id=NEW.tenant_id AND c.id=NEW.seller_company_id AND c.is_active;
                        SELECT n.path INTO buyer_path FROM finance.legal_companies c JOIN platform.org_nodes n
                            ON n.tenant_id=c.tenant_id AND n.id=c.organisation_node_id
                            WHERE c.tenant_id=NEW.tenant_id AND c.id=NEW.buyer_company_id AND c.is_active;
                        IF root_path IS NULL OR seller_path IS NULL OR buyer_path IS NULL
                           OR NOT (seller_path <@ root_path) OR NOT (buyer_path <@ root_path) THEN
                            RAISE EXCEPTION 'Elimination companies must be active descendants of the consolidation scope.' USING ERRCODE='23514';
                        END IF;
                        RETURN NEW;
                    END IF;
                    IF (to_jsonb(NEW)-ARRAY['status','approved_at','approved_by','approval_reference','retired_at','retired_by','updated_at_utc','version'])
                       IS DISTINCT FROM
                       (to_jsonb(OLD)-ARRAY['status','approved_at','approved_by','approval_reference','retired_at','retired_by','updated_at_utc','version'])
                       OR NOT ((OLD.status=1 AND NEW.status=2 AND NEW.approved_at IS NOT NULL AND NEW.approved_by IS NOT NULL
                                AND NEW.approval_reference IS NOT NULL)
                            OR (OLD.status=2 AND NEW.status=3 AND NEW.retired_at IS NOT NULL AND NEW.retired_by IS NOT NULL))
                       OR NEW.version<>OLD.version+1 THEN
                        RAISE EXCEPTION 'Invalid elimination-rule lifecycle transition.' USING ERRCODE='55000';
                    END IF;
                    RETURN NEW;
                END $function$;

                CREATE OR REPLACE FUNCTION finance.validate_intercompany_transaction() RETURNS trigger
                LANGUAGE plpgsql SET search_path=pg_catalog,finance,farm,platform,public AS $function$
                DECLARE owner_company uuid; transaction_date date:=NEW.occurred_at::date;
                BEGIN
                    IF NEW.counterparty_legal_company_id IS NULL THEN RETURN NEW; END IF;
                    IF NOT EXISTS(SELECT 1 FROM finance.legal_companies c WHERE c.tenant_id=NEW.tenant_id
                        AND c.id=NEW.counterparty_legal_company_id AND c.effective_from<=transaction_date
                        AND (c.effective_to IS NULL OR c.effective_to>=transaction_date)) THEN
                        RAISE EXCEPTION 'The intercompany counterparty is not effective on the transaction date.' USING ERRCODE='23514';
                    END IF;
                    SELECT c.id INTO owner_company FROM farm.farms f
                    JOIN platform.org_nodes farm_node ON farm_node.tenant_id=f.tenant_id AND farm_node.id=f.organisation_node_id
                    JOIN finance.legal_companies c ON c.tenant_id=f.tenant_id AND c.effective_from<=transaction_date
                        AND (c.effective_to IS NULL OR c.effective_to>=transaction_date)
                    JOIN platform.org_nodes company_node ON company_node.tenant_id=c.tenant_id
                        AND company_node.id=c.organisation_node_id AND farm_node.path <@ company_node.path
                    WHERE f.tenant_id=NEW.tenant_id AND f.id=NEW.farm_id
                    ORDER BY nlevel(company_node.path) DESC LIMIT 1;
                    IF owner_company IS NULL OR owner_company=NEW.counterparty_legal_company_id THEN
                        RAISE EXCEPTION 'The farm must belong to a different effective legal company.' USING ERRCODE='23514';
                    END IF;
                    RETURN NEW;
                END $function$;

                CREATE TRIGGER finance_legal_company_guard BEFORE INSERT OR UPDATE OR DELETE ON finance.legal_companies
                    FOR EACH ROW EXECUTE FUNCTION finance.guard_legal_company();
                CREATE TRIGGER finance_elimination_rule_guard BEFORE INSERT OR UPDATE OR DELETE ON finance.intercompany_elimination_rules
                    FOR EACH ROW EXECUTE FUNCTION finance.guard_elimination_rule();
                CREATE TRIGGER finance_intercompany_transaction_validate BEFORE INSERT ON finance.transactions
                    FOR EACH ROW EXECUTE FUNCTION finance.validate_intercompany_transaction();
                CREATE TRIGGER finance_consolidation_runs_immutable BEFORE UPDATE OR DELETE ON finance.consolidation_runs
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER finance_elimination_entries_immutable BEFORE UPDATE OR DELETE ON finance.consolidation_elimination_entries
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();

                DO $grant$ BEGIN IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname='murimios_app') THEN
                    GRANT SELECT,INSERT,UPDATE ON finance.legal_companies,finance.intercompany_elimination_rules TO murimios_app;
                    GRANT SELECT,INSERT ON finance.consolidation_runs,finance.consolidation_elimination_entries TO murimios_app;
                END IF; END $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP TRIGGER IF EXISTS finance_intercompany_transaction_validate ON finance.transactions;
                DROP TRIGGER IF EXISTS finance_legal_company_guard ON finance.legal_companies;
                DROP TRIGGER IF EXISTS finance_elimination_rule_guard ON finance.intercompany_elimination_rules;
                DROP TRIGGER IF EXISTS finance_consolidation_runs_immutable ON finance.consolidation_runs;
                DROP TRIGGER IF EXISTS finance_elimination_entries_immutable ON finance.consolidation_elimination_entries;
                DROP FUNCTION IF EXISTS finance.validate_intercompany_transaction();
                DROP FUNCTION IF EXISTS finance.guard_legal_company();
                DROP FUNCTION IF EXISTS finance.guard_elimination_rule();
                """);
            migrationBuilder.DropForeignKey(
                name: "fk_transactions_legal_companies_tenant_id_counterparty_legal_c",
                schema: "finance",
                table: "transactions");

            migrationBuilder.DropTable(
                name: "consolidation_elimination_entries",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "consolidation_runs",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "intercompany_elimination_rules",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "legal_companies",
                schema: "finance");

            migrationBuilder.DropIndex(
                name: "ix_transactions_tenant_id_counterparty_legal_company_id_interc",
                schema: "finance",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "counterparty_legal_company_id",
                schema: "finance",
                table: "transactions");

            migrationBuilder.DropColumn(
                name: "intercompany_reference",
                schema: "finance",
                table: "transactions");
        }
    }
}
