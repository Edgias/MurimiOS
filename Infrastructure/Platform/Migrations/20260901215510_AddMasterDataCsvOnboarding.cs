using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddMasterDataCsvOnboarding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "master_data_jobs",
                schema: "imports",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    file_name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    byte_size = table.Column<long>(type: "bigint", nullable: false),
                    sha256 = table.Column<string>(type: "character(64)", fixedLength: true, maxLength: 64, nullable: false),
                    object_key = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    parser_version = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    header_json = table.Column<string>(type: "jsonb", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    row_count = table.Column<int>(type: "integer", nullable: false),
                    ready_row_count = table.Column<int>(type: "integer", nullable: false),
                    error_row_count = table.Column<int>(type: "integer", nullable: false),
                    applied_row_count = table.Column<int>(type: "integer", nullable: false),
                    failed_row_count = table.Column<int>(type: "integer", nullable: false),
                    checkpoint_ordinal = table.Column<int>(type: "integer", nullable: false),
                    validated_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    apply_started_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    applied_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    applied_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: true),
                    failure_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    failure_detail = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_master_data_jobs", x => x.id);
                    table.UniqueConstraint("ak_master_data_jobs_tenant_id_id", x => new { x.tenant_id, x.id });
                });

            migrationBuilder.CreateTable(
                name: "opening_balances",
                schema: "finance",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    account_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    account_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    direction = table.Column<int>(type: "integer", nullable: false),
                    amount = table.Column<decimal>(type: "numeric(18,2)", precision: 18, scale: 2, nullable: false),
                    currency = table.Column<string>(type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    as_of = table.Column<DateOnly>(type: "date", nullable: false),
                    reference = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    imported_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    imported_by = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_opening_balances", x => x.id);
                    table.ForeignKey(
                        name: "fk_opening_balances_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "master_data_rows",
                schema: "imports",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    job_id = table.Column<Guid>(type: "uuid", nullable: false),
                    ordinal = table.Column<int>(type: "integer", nullable: false),
                    raw_json = table.Column<string>(type: "jsonb", nullable: false),
                    normalized_json = table.Column<string>(type: "jsonb", nullable: false),
                    issues_json = table.Column<string>(type: "jsonb", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    applied_entity_type = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    applied_entity_id = table.Column<Guid>(type: "uuid", nullable: true),
                    applied_farm_id = table.Column<Guid>(type: "uuid", nullable: true),
                    applied_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    failure_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    failure_detail = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_master_data_rows", x => x.id);
                    table.ForeignKey(
                        name: "fk_master_data_rows_farms_tenant_id_applied_farm_id",
                        columns: x => new { x.tenant_id, x.applied_farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_master_data_rows_master_data_jobs_tenant_id_job_id",
                        columns: x => new { x.tenant_id, x.job_id },
                        principalSchema: "imports",
                        principalTable: "master_data_jobs",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_master_data_jobs_tenant_id",
                schema: "imports",
                table: "master_data_jobs",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_master_data_jobs_tenant_id_status_created_at_utc",
                schema: "imports",
                table: "master_data_jobs",
                columns: new[] { "tenant_id", "status", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_master_data_jobs_tenant_id_type_sha256",
                schema: "imports",
                table: "master_data_jobs",
                columns: new[] { "tenant_id", "type", "sha256" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_master_data_rows_tenant_id",
                schema: "imports",
                table: "master_data_rows",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_master_data_rows_tenant_id_applied_farm_id",
                schema: "imports",
                table: "master_data_rows",
                columns: new[] { "tenant_id", "applied_farm_id" });

            migrationBuilder.CreateIndex(
                name: "ix_master_data_rows_tenant_id_job_id_ordinal",
                schema: "imports",
                table: "master_data_rows",
                columns: new[] { "tenant_id", "job_id", "ordinal" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_master_data_rows_tenant_id_job_id_status_ordinal",
                schema: "imports",
                table: "master_data_rows",
                columns: new[] { "tenant_id", "job_id", "status", "ordinal" });

            migrationBuilder.CreateIndex(
                name: "ix_opening_balances_tenant_id",
                schema: "finance",
                table: "opening_balances",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_opening_balances_tenant_id_farm_id_account_code_as_of",
                schema: "finance",
                table: "opening_balances",
                columns: new[] { "tenant_id", "farm_id", "account_code", "as_of" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_opening_balances_tenant_id_farm_id_as_of",
                schema: "finance",
                table: "opening_balances",
                columns: new[] { "tenant_id", "farm_id", "as_of" });

            migrationBuilder.Sql("""
                ALTER TABLE imports.master_data_jobs
                    ADD CONSTRAINT ck_master_job_type CHECK (type BETWEEN 1 AND 7),
                    ADD CONSTRAINT ck_master_job_status CHECK (status BETWEEN 1 AND 6),
                    ADD CONSTRAINT ck_master_job_size CHECK (byte_size > 0 AND byte_size <= 20971520),
                    ADD CONSTRAINT ck_master_job_hash CHECK (sha256 ~ '^[0-9a-f]{64}$'),
                    ADD CONSTRAINT ck_master_job_header CHECK (jsonb_typeof(header_json) = 'array'),
                    ADD CONSTRAINT ck_master_job_counts CHECK (
                        row_count >= 0 AND ready_row_count >= 0 AND error_row_count >= 0
                        AND applied_row_count >= 0 AND failed_row_count >= 0 AND checkpoint_ordinal >= 0
                        AND ready_row_count + error_row_count = row_count
                        AND applied_row_count + failed_row_count <= ready_row_count),
                    ADD CONSTRAINT ck_master_job_lifecycle CHECK (
                        (status = 1 AND validated_at IS NULL AND apply_started_at IS NULL AND applied_at IS NULL) OR
                        (status IN (2,3) AND validated_at IS NOT NULL AND apply_started_at IS NULL AND applied_at IS NULL) OR
                        (status = 4 AND validated_at IS NOT NULL AND apply_started_at IS NOT NULL AND applied_by IS NOT NULL AND applied_at IS NULL) OR
                        (status = 5 AND validated_at IS NOT NULL AND apply_started_at IS NOT NULL AND applied_by IS NOT NULL AND applied_at IS NOT NULL) OR
                        (status = 6 AND failure_code IS NOT NULL AND failure_detail IS NOT NULL));

                ALTER TABLE imports.master_data_rows
                    ADD CONSTRAINT ck_master_row_ordinal CHECK (ordinal > 0),
                    ADD CONSTRAINT ck_master_row_status CHECK (status BETWEEN 1 AND 4),
                    ADD CONSTRAINT ck_master_row_json CHECK (
                        jsonb_typeof(raw_json) = 'object' AND jsonb_typeof(normalized_json) = 'object'
                        AND jsonb_typeof(issues_json) = 'array'),
                    ADD CONSTRAINT ck_master_row_lifecycle CHECK (
                        (status IN (1,2) AND applied_entity_type IS NULL AND applied_entity_id IS NULL AND applied_at IS NULL AND failure_code IS NULL AND failure_detail IS NULL) OR
                        (status = 3 AND applied_entity_type IS NOT NULL AND applied_entity_id IS NOT NULL AND applied_at IS NOT NULL AND failure_code IS NULL AND failure_detail IS NULL) OR
                        (status = 4 AND applied_entity_type IS NULL AND applied_entity_id IS NULL AND applied_at IS NULL AND failure_code IS NOT NULL AND failure_detail IS NOT NULL));

                ALTER TABLE finance.opening_balances
                    ADD CONSTRAINT ck_opening_balance_direction CHECK (direction IN (1,2)),
                    ADD CONSTRAINT ck_opening_balance_amount CHECK (amount > 0),
                    ADD CONSTRAINT ck_opening_balance_currency CHECK (currency ~ '^[A-Z]{3}$');

                CREATE OR REPLACE FUNCTION imports.guard_master_data_job_mutation()
                RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_OP = 'DELETE' THEN RAISE EXCEPTION 'master-data import jobs cannot be deleted' USING ERRCODE='55000'; END IF;
                    IF NEW.tenant_id <> OLD.tenant_id OR NEW.id <> OLD.id OR NEW.type <> OLD.type
                       OR NEW.file_name <> OLD.file_name OR NEW.byte_size <> OLD.byte_size OR NEW.sha256 <> OLD.sha256
                       OR NEW.object_key <> OLD.object_key OR NEW.parser_version <> OLD.parser_version
                       OR NEW.header_json <> OLD.header_json OR NEW.created_at_utc <> OLD.created_at_utc THEN
                        RAISE EXCEPTION 'master-data import source identity is immutable' USING ERRCODE='55000';
                    END IF;
                    RETURN NEW;
                END $$;

                CREATE OR REPLACE FUNCTION imports.guard_master_data_row_mutation()
                RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_OP = 'DELETE' THEN RAISE EXCEPTION 'master-data import rows cannot be deleted' USING ERRCODE='55000'; END IF;
                    IF NEW.tenant_id <> OLD.tenant_id OR NEW.id <> OLD.id OR NEW.job_id <> OLD.job_id
                       OR NEW.ordinal <> OLD.ordinal OR NEW.raw_json <> OLD.raw_json
                       OR NEW.normalized_json <> OLD.normalized_json OR NEW.issues_json <> OLD.issues_json
                       OR NEW.created_at_utc <> OLD.created_at_utc THEN
                        RAISE EXCEPTION 'validated master-data import row payloads are immutable' USING ERRCODE='55000';
                    END IF;
                    RETURN NEW;
                END $$;

                CREATE TRIGGER master_data_jobs_guard BEFORE UPDATE OR DELETE ON imports.master_data_jobs
                    FOR EACH ROW EXECUTE FUNCTION imports.guard_master_data_job_mutation();
                CREATE TRIGGER master_data_rows_guard BEFORE UPDATE OR DELETE ON imports.master_data_rows
                    FOR EACH ROW EXECUTE FUNCTION imports.guard_master_data_row_mutation();
                CREATE TRIGGER opening_balances_immutable BEFORE UPDATE OR DELETE ON finance.opening_balances
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();

                ALTER TABLE imports.master_data_jobs ENABLE ROW LEVEL SECURITY;
                ALTER TABLE imports.master_data_jobs FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON imports.master_data_jobs
                    USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid)
                    WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);
                ALTER TABLE imports.master_data_rows ENABLE ROW LEVEL SECURITY;
                ALTER TABLE imports.master_data_rows FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON imports.master_data_rows
                    USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid)
                    WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);
                ALTER TABLE finance.opening_balances ENABLE ROW LEVEL SECURITY;
                ALTER TABLE finance.opening_balances FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON finance.opening_balances
                    USING (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid)
                    WITH CHECK (tenant_id=nullif(current_setting('app.tenant_id',true),'')::uuid);

                DO $grants$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname='murimios_app') THEN
                        GRANT SELECT,INSERT,UPDATE ON imports.master_data_jobs,imports.master_data_rows TO murimios_app;
                        REVOKE DELETE ON imports.master_data_jobs,imports.master_data_rows FROM murimios_app;
                        GRANT SELECT,INSERT ON finance.opening_balances TO murimios_app;
                        REVOKE UPDATE,DELETE ON finance.opening_balances FROM murimios_app;
                    END IF;
                END $grants$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DROP FUNCTION IF EXISTS imports.guard_master_data_row_mutation();
                DROP FUNCTION IF EXISTS imports.guard_master_data_job_mutation();
                """);

            migrationBuilder.DropTable(
                name: "master_data_rows",
                schema: "imports");

            migrationBuilder.DropTable(
                name: "opening_balances",
                schema: "finance");

            migrationBuilder.DropTable(
                name: "master_data_jobs",
                schema: "imports");
        }
    }
}
