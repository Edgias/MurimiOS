using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class CompleteSupplierRiskClaimsAndReconciliation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "last_detached_at",
                schema: "supply",
                table: "hosted_suppliers",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "contract_price_rules",
                schema: "supply",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contract_id = table.Column<Guid>(type: "uuid", nullable: false),
                    priority = table.Column<int>(type: "integer", nullable: false),
                    method = table.Column<int>(type: "integer", nullable: false),
                    minimum_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    maximum_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: true),
                    quality_grade = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    unit_price = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    effective_from = table.Column<DateOnly>(type: "date", nullable: false),
                    effective_to = table.Column<DateOnly>(type: "date", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contract_price_rules", x => x.id);
                    table.UniqueConstraint("ak_contract_price_rules_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_contract_price_rules_contracts_tenant_id_contract_id",
                        columns: x => new { x.tenant_id, x.contract_id },
                        principalSchema: "supply",
                        principalTable: "contracts",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "supplier_risk_assessments",
                schema: "supply",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    relationship_id = table.Column<Guid>(type: "uuid", nullable: true),
                    hosted_supplier_id = table.Column<Guid>(type: "uuid", nullable: true),
                    score = table.Column<int>(type: "integer", nullable: false),
                    band = table.Column<int>(type: "integer", nullable: false),
                    model_version = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    facts_as_of = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    components_json = table.Column<string>(type: "jsonb", nullable: false),
                    inputs_json = table.Column<string>(type: "jsonb", nullable: false),
                    assessed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    assessed_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_supplier_risk_assessments", x => x.id);
                    table.ForeignKey(
                        name: "fk_supplier_risk_assessments_hosted_suppliers_tenant_id_hosted",
                        columns: x => new { x.tenant_id, x.hosted_supplier_id },
                        principalSchema: "supply",
                        principalTable: "hosted_suppliers",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_supplier_risk_assessments_relationships_relationship_id",
                        column: x => x.relationship_id,
                        principalSchema: "supply",
                        principalTable: "relationships",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "supplier_self_service_tokens",
                schema: "supply",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    hosted_supplier_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<int>(type: "integer", nullable: false),
                    claimed_tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    token_hash = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    expires_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    issued_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    issued_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    consumed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    consumer_network_hash = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_supplier_self_service_tokens", x => x.id);
                    table.UniqueConstraint("ak_supplier_self_service_tokens_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_supplier_self_service_tokens_hosted_suppliers_tenant_id_hos",
                        columns: x => new { x.tenant_id, x.hosted_supplier_id },
                        principalSchema: "supply",
                        principalTable: "hosted_suppliers",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "contract_reconciliations",
                schema: "supply",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    contract_id = table.Column<Guid>(type: "uuid", nullable: false),
                    revision = table.Column<int>(type: "integer", nullable: false),
                    reference = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    price_rule_id = table.Column<Guid>(type: "uuid", nullable: true),
                    fulfilled_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    accepted_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    rejected_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    quantity_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    gross_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    quality_adjustment_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    other_adjustment_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    payable_amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    reconciled_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    reconciled_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_contract_reconciliations", x => x.id);
                    table.ForeignKey(
                        name: "fk_contract_reconciliations_contract_price_rules_tenant_id_pri",
                        columns: x => new { x.tenant_id, x.price_rule_id },
                        principalSchema: "supply",
                        principalTable: "contract_price_rules",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_contract_reconciliations_contracts_tenant_id_contract_id",
                        columns: x => new { x.tenant_id, x.contract_id },
                        principalSchema: "supply",
                        principalTable: "contracts",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "hosted_supplier_link_events",
                schema: "supply",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    hosted_supplier_id = table.Column<Guid>(type: "uuid", nullable: false),
                    self_service_token_id = table.Column<Guid>(type: "uuid", nullable: false),
                    action = table.Column<int>(type: "integer", nullable: false),
                    claimed_tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_hosted_supplier_link_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_hosted_supplier_link_events_hosted_suppliers_tenant_id_host",
                        columns: x => new { x.tenant_id, x.hosted_supplier_id },
                        principalSchema: "supply",
                        principalTable: "hosted_suppliers",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_hosted_supplier_link_events_supplier_self_service_tokens_te",
                        columns: x => new { x.tenant_id, x.self_service_token_id },
                        principalSchema: "supply",
                        principalTable: "supplier_self_service_tokens",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_contract_price_rules_tenant_id",
                schema: "supply",
                table: "contract_price_rules",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_contract_price_rules_tenant_id_contract_id_effective_from_e",
                schema: "supply",
                table: "contract_price_rules",
                columns: new[] { "tenant_id", "contract_id", "effective_from", "effective_to" });

            migrationBuilder.CreateIndex(
                name: "ix_contract_price_rules_tenant_id_contract_id_priority",
                schema: "supply",
                table: "contract_price_rules",
                columns: new[] { "tenant_id", "contract_id", "priority" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_contract_reconciliations_tenant_id",
                schema: "supply",
                table: "contract_reconciliations",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_contract_reconciliations_tenant_id_contract_id_reference",
                schema: "supply",
                table: "contract_reconciliations",
                columns: new[] { "tenant_id", "contract_id", "reference" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_contract_reconciliations_tenant_id_contract_id_revision",
                schema: "supply",
                table: "contract_reconciliations",
                columns: new[] { "tenant_id", "contract_id", "revision" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_contract_reconciliations_tenant_id_price_rule_id",
                schema: "supply",
                table: "contract_reconciliations",
                columns: new[] { "tenant_id", "price_rule_id" });

            migrationBuilder.CreateIndex(
                name: "ix_hosted_supplier_link_events_tenant_id",
                schema: "supply",
                table: "hosted_supplier_link_events",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_hosted_supplier_link_events_tenant_id_hosted_supplier_id_oc",
                schema: "supply",
                table: "hosted_supplier_link_events",
                columns: new[] { "tenant_id", "hosted_supplier_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_hosted_supplier_link_events_tenant_id_self_service_token_id",
                schema: "supply",
                table: "hosted_supplier_link_events",
                columns: new[] { "tenant_id", "self_service_token_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_supplier_risk_assessments_relationship_id",
                schema: "supply",
                table: "supplier_risk_assessments",
                column: "relationship_id");

            migrationBuilder.CreateIndex(
                name: "ix_supplier_risk_assessments_tenant_id",
                schema: "supply",
                table: "supplier_risk_assessments",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_supplier_risk_assessments_tenant_id_band_assessed_at",
                schema: "supply",
                table: "supplier_risk_assessments",
                columns: new[] { "tenant_id", "band", "assessed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_supplier_risk_assessments_tenant_id_hosted_supplier_id_asse",
                schema: "supply",
                table: "supplier_risk_assessments",
                columns: new[] { "tenant_id", "hosted_supplier_id", "assessed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_supplier_risk_assessments_tenant_id_relationship_id_assesse",
                schema: "supply",
                table: "supplier_risk_assessments",
                columns: new[] { "tenant_id", "relationship_id", "assessed_at" });

            migrationBuilder.CreateIndex(
                name: "ix_supplier_self_service_tokens_tenant_id",
                schema: "supply",
                table: "supplier_self_service_tokens",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_supplier_self_service_tokens_tenant_id_hosted_supplier_id_s",
                schema: "supply",
                table: "supplier_self_service_tokens",
                columns: new[] { "tenant_id", "hosted_supplier_id", "status", "expires_at" });

            migrationBuilder.CreateIndex(
                name: "ix_supplier_self_service_tokens_tenant_id_token_hash",
                schema: "supply",
                table: "supplier_self_service_tokens",
                columns: new[] { "tenant_id", "token_hash" },
                unique: true);

            migrationBuilder.Sql("""
                ALTER TABLE supply.hosted_suppliers
                    ADD CONSTRAINT ck_hosted_supplier_claim_state CHECK (
                        (status = 3 AND claimed_tenant_id IS NOT NULL AND claimed_at IS NOT NULL) OR
                        (status IN (1,2) AND claimed_tenant_id IS NULL AND claimed_at IS NULL));

                ALTER TABLE supply.supplier_risk_assessments
                    ADD CONSTRAINT ck_supplier_risk_target CHECK ((relationship_id IS NULL) <> (hosted_supplier_id IS NULL)),
                    ADD CONSTRAINT ck_supplier_risk_score_band CHECK (
                        (score BETWEEN 0 AND 24 AND band = 1) OR (score BETWEEN 25 AND 49 AND band = 2) OR
                        (score BETWEEN 50 AND 74 AND band = 3) OR (score BETWEEN 75 AND 100 AND band = 4)),
                    ADD CONSTRAINT ck_supplier_risk_json CHECK (
                        jsonb_typeof(components_json) = 'object' AND jsonb_typeof(inputs_json) = 'object');

                ALTER TABLE supply.supplier_self_service_tokens
                    ADD CONSTRAINT ck_supplier_self_service_action CHECK (action IN (1,2)),
                    ADD CONSTRAINT ck_supplier_self_service_status CHECK (status BETWEEN 1 AND 4),
                    ADD CONSTRAINT ck_supplier_self_service_hash CHECK (token_hash ~ '^[0-9a-f]{64}$'),
                    ADD CONSTRAINT ck_supplier_self_service_expiry CHECK (expires_at > issued_at),
                    ADD CONSTRAINT ck_supplier_self_service_lifecycle CHECK (
                        (status = 1 AND consumed_at IS NULL AND consumer_network_hash IS NULL) OR
                        (status = 2 AND consumed_at IS NOT NULL) OR
                        (status IN (3,4) AND consumed_at IS NOT NULL AND consumer_network_hash IS NULL));

                ALTER TABLE supply.hosted_supplier_link_events
                    ADD CONSTRAINT ck_supplier_link_action CHECK (action IN (1,2));

                ALTER TABLE supply.contract_price_rules
                    ADD CONSTRAINT ck_contract_price_method CHECK (method IN (1,2,3)),
                    ADD CONSTRAINT ck_contract_price_priority CHECK (priority > 0),
                    ADD CONSTRAINT ck_contract_price_range CHECK (
                        minimum_quantity >= 0 AND (maximum_quantity IS NULL OR maximum_quantity > minimum_quantity)),
                    ADD CONSTRAINT ck_contract_price_grade CHECK (
                        (method = 3 AND quality_grade IS NOT NULL) OR (method IN (1,2) AND quality_grade IS NULL)),
                    ADD CONSTRAINT ck_contract_price_value CHECK (unit_price >= 0 AND currency ~ '^[A-Z]{3}$'),
                    ADD CONSTRAINT ck_contract_price_period CHECK (effective_to >= effective_from);

                ALTER TABLE supply.contract_reconciliations
                    ADD CONSTRAINT ck_contract_reconciliation_revision CHECK (revision > 0),
                    ADD CONSTRAINT ck_contract_reconciliation_quantities CHECK (
                        fulfilled_quantity >= 0 AND accepted_quantity >= 0 AND rejected_quantity >= 0
                        AND accepted_quantity + rejected_quantity = fulfilled_quantity),
                    ADD CONSTRAINT ck_contract_reconciliation_amounts CHECK (
                        unit_price >= 0 AND gross_amount = round(accepted_quantity * unit_price, 2)
                        AND payable_amount = round(gross_amount + quality_adjustment_amount + other_adjustment_amount, 2)
                        AND payable_amount >= 0),
                    ADD CONSTRAINT ck_contract_reconciliation_currency CHECK (currency ~ '^[A-Z]{3}$'),
                    ADD CONSTRAINT ck_contract_reconciliation_status CHECK (status IN (1,2));

                CREATE OR REPLACE FUNCTION supply.guard_supplier_self_service_token()
                RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_OP = 'DELETE' THEN RAISE EXCEPTION 'supplier self-service tokens cannot be deleted' USING ERRCODE='55000'; END IF;
                    IF NEW.tenant_id <> OLD.tenant_id OR NEW.id <> OLD.id OR NEW.hosted_supplier_id <> OLD.hosted_supplier_id
                       OR NEW.action <> OLD.action OR NEW.claimed_tenant_id <> OLD.claimed_tenant_id
                       OR NEW.token_hash <> OLD.token_hash OR NEW.expires_at <> OLD.expires_at
                       OR NEW.issued_at <> OLD.issued_at OR NEW.issued_by <> OLD.issued_by
                       OR NEW.created_at_utc <> OLD.created_at_utc THEN
                        RAISE EXCEPTION 'supplier self-service token identity is immutable' USING ERRCODE='55000';
                    END IF;
                    RETURN NEW;
                END $$;

                CREATE TRIGGER supplier_risk_assessments_immutable BEFORE UPDATE OR DELETE ON supply.supplier_risk_assessments
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER hosted_supplier_link_events_immutable BEFORE UPDATE OR DELETE ON supply.hosted_supplier_link_events
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER contract_price_rules_immutable BEFORE UPDATE OR DELETE ON supply.contract_price_rules
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER contract_reconciliations_immutable BEFORE UPDATE OR DELETE ON supply.contract_reconciliations
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER supplier_self_service_tokens_guard BEFORE UPDATE OR DELETE ON supply.supplier_self_service_tokens
                    FOR EACH ROW EXECUTE FUNCTION supply.guard_supplier_self_service_token();

                ALTER TABLE supply.supplier_risk_assessments ENABLE ROW LEVEL SECURITY;
                ALTER TABLE supply.supplier_risk_assessments FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON supply.supplier_risk_assessments
                    USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid)
                    WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);
                ALTER TABLE supply.supplier_self_service_tokens ENABLE ROW LEVEL SECURITY;
                ALTER TABLE supply.supplier_self_service_tokens FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON supply.supplier_self_service_tokens
                    USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid)
                    WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);
                ALTER TABLE supply.hosted_supplier_link_events ENABLE ROW LEVEL SECURITY;
                ALTER TABLE supply.hosted_supplier_link_events FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON supply.hosted_supplier_link_events
                    USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid)
                    WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);
                ALTER TABLE supply.contract_price_rules ENABLE ROW LEVEL SECURITY;
                ALTER TABLE supply.contract_price_rules FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON supply.contract_price_rules
                    USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid)
                    WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);
                ALTER TABLE supply.contract_reconciliations ENABLE ROW LEVEL SECURITY;
                ALTER TABLE supply.contract_reconciliations FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON supply.contract_reconciliations
                    USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid)
                    WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);

                CREATE OR REPLACE FUNCTION supply.resolve_supplier_self_service_token(p_hash text)
                RETURNS TABLE(tenant_id uuid, token_id uuid, supplier_id uuid)
                LANGUAGE sql STABLE SECURITY DEFINER
                SET search_path = pg_catalog, supply
                AS $$
                    SELECT t.tenant_id,t.id,t.hosted_supplier_id
                    FROM supply.supplier_self_service_tokens t
                    WHERE t.token_hash=p_hash
                    LIMIT 1
                $$;
                REVOKE ALL ON FUNCTION supply.resolve_supplier_self_service_token(text) FROM PUBLIC;

                DO $grants$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname='murimios_app') THEN
                        GRANT SELECT,INSERT ON supply.supplier_risk_assessments,supply.hosted_supplier_link_events,
                            supply.contract_price_rules,supply.contract_reconciliations TO murimios_app;
                        REVOKE UPDATE,DELETE ON supply.supplier_risk_assessments,supply.hosted_supplier_link_events,
                            supply.contract_price_rules,supply.contract_reconciliations FROM murimios_app;
                        GRANT SELECT,INSERT,UPDATE ON supply.supplier_self_service_tokens TO murimios_app;
                        REVOKE DELETE ON supply.supplier_self_service_tokens FROM murimios_app;
                        GRANT EXECUTE ON FUNCTION supply.resolve_supplier_self_service_token(text) TO murimios_app;
                    END IF;
                END $grants$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP FUNCTION IF EXISTS supply.resolve_supplier_self_service_token(text);
                DROP FUNCTION IF EXISTS supply.guard_supplier_self_service_token();
                """);

            migrationBuilder.DropTable(
                name: "contract_reconciliations",
                schema: "supply");

            migrationBuilder.DropTable(
                name: "hosted_supplier_link_events",
                schema: "supply");

            migrationBuilder.DropTable(
                name: "supplier_risk_assessments",
                schema: "supply");

            migrationBuilder.DropTable(
                name: "contract_price_rules",
                schema: "supply");

            migrationBuilder.DropTable(
                name: "supplier_self_service_tokens",
                schema: "supply");

            migrationBuilder.DropColumn(
                name: "last_detached_at",
                schema: "supply",
                table: "hosted_suppliers");
        }
    }
}
