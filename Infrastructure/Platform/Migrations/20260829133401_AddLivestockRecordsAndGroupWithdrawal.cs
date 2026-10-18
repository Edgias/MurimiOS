using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddLivestockRecordsAndGroupWithdrawal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "feed_batches",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    batch_code = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    feed_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    initial_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    quantity_on_hand = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    entered_initial_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    entered_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    display_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    received_on = table.Column<DateOnly>(type: "date", nullable: false),
                    expires_on = table.Column<DateOnly>(type: "date", nullable: true),
                    supplier = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_feed_batches", x => x.id);
                    table.UniqueConstraint("ak_feed_batches_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_feed_batches_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "group_memberships",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    group_id = table.Column<Guid>(type: "uuid", nullable: false),
                    member_subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    joined_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    left_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_group_memberships", x => x.id);
                    table.ForeignKey(
                        name: "fk_group_memberships_subjects_tenant_id_group_id",
                        columns: x => new { x.tenant_id, x.group_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_group_memberships_subjects_tenant_id_member_subject_id",
                        columns: x => new { x.tenant_id, x.member_subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "health_events",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    diagnosis = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    practitioner = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: true),
                    source_command_id = table.Column<Guid>(type: "uuid", nullable: true),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_health_events", x => x.id);
                    table.ForeignKey(
                        name: "fk_health_events_subjects_tenant_id_subject_id",
                        columns: x => new { x.tenant_id, x.subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "holdings",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_holdings", x => x.id);
                    table.UniqueConstraint("ak_holdings_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_holdings_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "identifiers",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    identifier_namespace = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    value = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: false),
                    valid_from = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    valid_to = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    is_primary = table.Column<bool>(type: "boolean", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_identifiers", x => x.id);
                    table.ForeignKey(
                        name: "fk_identifiers_subjects_tenant_id_subject_id",
                        columns: x => new { x.tenant_id, x.subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "feed_allocations",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    feed_batch_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    entered_quantity = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    entered_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    display_unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    source_command_id = table.Column<Guid>(type: "uuid", nullable: true),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_feed_allocations", x => x.id);
                    table.ForeignKey(
                        name: "fk_feed_allocations_feed_batches_tenant_id_feed_batch_id",
                        columns: x => new { x.tenant_id, x.feed_batch_id },
                        principalSchema: "livestock",
                        principalTable: "feed_batches",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_feed_allocations_subjects_tenant_id_subject_id",
                        columns: x => new { x.tenant_id, x.subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "movements",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    from_holding_id = table.Column<Guid>(type: "uuid", nullable: true),
                    to_holding_id = table.Column<Guid>(type: "uuid", nullable: true),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    reference = table.Column<string>(type: "character varying(160)", maxLength: 160, nullable: true),
                    source_command_id = table.Column<Guid>(type: "uuid", nullable: true),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_movements1", x => x.id);
                    table.ForeignKey(
                        name: "fk_movements_holdings_tenant_id_from_holding_id",
                        columns: x => new { x.tenant_id, x.from_holding_id },
                        principalSchema: "livestock",
                        principalTable: "holdings",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_movements_holdings_tenant_id_to_holding_id",
                        columns: x => new { x.tenant_id, x.to_holding_id },
                        principalSchema: "livestock",
                        principalTable: "holdings",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_movements_subjects_tenant_id_subject_id",
                        columns: x => new { x.tenant_id, x.subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_feed_allocations_tenant_id",
                schema: "livestock",
                table: "feed_allocations",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_feed_allocations_tenant_id_feed_batch_id",
                schema: "livestock",
                table: "feed_allocations",
                columns: new[] { "tenant_id", "feed_batch_id" });

            migrationBuilder.CreateIndex(
                name: "ix_feed_allocations_tenant_id_source_command_id",
                schema: "livestock",
                table: "feed_allocations",
                columns: new[] { "tenant_id", "source_command_id" },
                unique: true,
                filter: "source_command_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_feed_allocations_tenant_id_subject_id_occurred_at",
                schema: "livestock",
                table: "feed_allocations",
                columns: new[] { "tenant_id", "subject_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_feed_batches_tenant_id",
                schema: "livestock",
                table: "feed_batches",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_feed_batches_tenant_id_farm_id_batch_code",
                schema: "livestock",
                table: "feed_batches",
                columns: new[] { "tenant_id", "farm_id", "batch_code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_group_memberships_tenant_id",
                schema: "livestock",
                table: "group_memberships",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_group_memberships_tenant_id_group_id_member_subject_id_join",
                schema: "livestock",
                table: "group_memberships",
                columns: new[] { "tenant_id", "group_id", "member_subject_id", "joined_at" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_group_memberships_tenant_id_member_subject_id_joined_at_lef",
                schema: "livestock",
                table: "group_memberships",
                columns: new[] { "tenant_id", "member_subject_id", "joined_at", "left_at" });

            migrationBuilder.CreateIndex(
                name: "ix_health_events_tenant_id",
                schema: "livestock",
                table: "health_events",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_health_events_tenant_id_source_command_id",
                schema: "livestock",
                table: "health_events",
                columns: new[] { "tenant_id", "source_command_id" },
                unique: true,
                filter: "source_command_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_health_events_tenant_id_subject_id_occurred_at",
                schema: "livestock",
                table: "health_events",
                columns: new[] { "tenant_id", "subject_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_holdings_tenant_id",
                schema: "livestock",
                table: "holdings",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_holdings_tenant_id_farm_id_code",
                schema: "livestock",
                table: "holdings",
                columns: new[] { "tenant_id", "farm_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_identifiers_tenant_id",
                schema: "livestock",
                table: "identifiers",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_identifiers_tenant_id_identifier_namespace_value",
                schema: "livestock",
                table: "identifiers",
                columns: new[] { "tenant_id", "identifier_namespace", "value" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_identifiers_tenant_id_subject_id_is_primary_valid_to",
                schema: "livestock",
                table: "identifiers",
                columns: new[] { "tenant_id", "subject_id", "is_primary", "valid_to" });

            migrationBuilder.CreateIndex(
                name: "ix_movements_tenant_id_from_holding_id",
                schema: "livestock",
                table: "movements",
                columns: new[] { "tenant_id", "from_holding_id" });

            migrationBuilder.CreateIndex(
                name: "ix_movements_tenant_id_source_command_id1",
                schema: "livestock",
                table: "movements",
                columns: new[] { "tenant_id", "source_command_id" },
                unique: true,
                filter: "source_command_id IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "ix_movements_tenant_id_subject_id_occurred_at",
                schema: "livestock",
                table: "movements",
                columns: new[] { "tenant_id", "subject_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_movements_tenant_id_to_holding_id",
                schema: "livestock",
                table: "movements",
                columns: new[] { "tenant_id", "to_holding_id" });

            migrationBuilder.CreateIndex(
                name: "ix_movements_tenant_id1",
                schema: "livestock",
                table: "movements",
                column: "tenant_id");

            migrationBuilder.Sql(
                """
                DO $rls$
                DECLARE table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY[
                        'feed_allocations', 'feed_batches', 'group_memberships', 'health_events',
                        'holdings', 'identifiers', 'movements'
                    ] LOOP
                        EXECUTE format('ALTER TABLE livestock.%I ENABLE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('ALTER TABLE livestock.%I FORCE ROW LEVEL SECURITY', table_name);
                        EXECUTE format(
                            'CREATE POLICY tenant_isolation ON livestock.%I USING (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid) WITH CHECK (tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid)',
                            table_name);
                    END LOOP;
                END
                $rls$;

                CREATE UNIQUE INDEX ux_identifiers_active_primary
                    ON livestock.identifiers (tenant_id, subject_id)
                    WHERE is_primary AND valid_to IS NULL;
                CREATE UNIQUE INDEX ux_group_memberships_active
                    ON livestock.group_memberships (tenant_id, group_id, member_subject_id)
                    WHERE left_at IS NULL;

                CREATE TRIGGER livestock_movements_immutable
                    BEFORE UPDATE OR DELETE ON livestock.movements
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER livestock_health_events_immutable
                    BEFORE UPDATE OR DELETE ON livestock.health_events
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER livestock_feed_allocations_immutable
                    BEFORE UPDATE OR DELETE ON livestock.feed_allocations
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER livestock_treatments_immutable
                    BEFORE UPDATE OR DELETE ON livestock.treatments
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();
                CREATE TRIGGER livestock_dispatches_immutable
                    BEFORE UPDATE OR DELETE ON livestock.dispatches
                    FOR EACH ROW EXECUTE FUNCTION audit.reject_record_mutation();

                CREATE OR REPLACE FUNCTION livestock.allow_membership_close_only()
                RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_OP = 'DELETE'
                       OR OLD.left_at IS NOT NULL
                       OR NEW.left_at IS NULL
                       OR NEW.left_at < OLD.joined_at
                       OR (to_jsonb(NEW) - ARRAY['left_at', 'updated_at_utc', 'version'])
                          IS DISTINCT FROM
                          (to_jsonb(OLD) - ARRAY['left_at', 'updated_at_utc', 'version']) THEN
                        RAISE EXCEPTION 'group membership may only be closed once' USING ERRCODE = '55000';
                    END IF;
                    RETURN NEW;
                END;
                $$;
                CREATE TRIGGER livestock_group_membership_close_only
                    BEFORE UPDATE OR DELETE ON livestock.group_memberships
                    FOR EACH ROW EXECUTE FUNCTION livestock.allow_membership_close_only();

                CREATE OR REPLACE FUNCTION livestock.allow_identifier_retire_only()
                RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_OP = 'DELETE'
                       OR OLD.valid_to IS NOT NULL
                       OR NEW.valid_to IS NULL
                       OR NEW.valid_to < OLD.valid_from
                       OR (to_jsonb(NEW) - ARRAY['valid_to', 'updated_at_utc', 'version'])
                          IS DISTINCT FROM
                          (to_jsonb(OLD) - ARRAY['valid_to', 'updated_at_utc', 'version']) THEN
                        RAISE EXCEPTION 'identifier may only be retired once' USING ERRCODE = '55000';
                    END IF;
                    RETURN NEW;
                END;
                $$;
                CREATE TRIGGER livestock_identifier_retire_only
                    BEFORE UPDATE OR DELETE ON livestock.identifiers
                    FOR EACH ROW EXECUTE FUNCTION livestock.allow_identifier_retire_only();

                CREATE OR REPLACE FUNCTION livestock.allow_feed_issue_only()
                RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF TG_OP = 'DELETE'
                       OR NEW.quantity_on_hand < 0
                       OR NEW.quantity_on_hand > OLD.quantity_on_hand
                       OR (to_jsonb(NEW) - ARRAY['quantity_on_hand', 'updated_at_utc', 'version'])
                          IS DISTINCT FROM
                          (to_jsonb(OLD) - ARRAY['quantity_on_hand', 'updated_at_utc', 'version']) THEN
                        RAISE EXCEPTION 'feed batch may only decrease through allocation' USING ERRCODE = '55000';
                    END IF;
                    RETURN NEW;
                END;
                $$;
                CREATE TRIGGER livestock_feed_batch_issue_only
                    BEFORE UPDATE OR DELETE ON livestock.feed_batches
                    FOR EACH ROW EXECUTE FUNCTION livestock.allow_feed_issue_only();
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP TRIGGER IF EXISTS livestock_treatments_immutable ON livestock.treatments;
                DROP TRIGGER IF EXISTS livestock_dispatches_immutable ON livestock.dispatches;
                DROP TRIGGER IF EXISTS livestock_group_membership_close_only ON livestock.group_memberships;
                DROP TRIGGER IF EXISTS livestock_identifier_retire_only ON livestock.identifiers;
                DROP TRIGGER IF EXISTS livestock_feed_batch_issue_only ON livestock.feed_batches;
                DROP FUNCTION IF EXISTS livestock.allow_membership_close_only();
                DROP FUNCTION IF EXISTS livestock.allow_identifier_retire_only();
                DROP FUNCTION IF EXISTS livestock.allow_feed_issue_only();
                """);

            migrationBuilder.DropTable(
                name: "feed_allocations",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "group_memberships",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "health_events",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "identifiers",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "movements",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "feed_batches",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "holdings",
                schema: "livestock");
        }
    }
}
