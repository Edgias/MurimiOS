using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NetTopologySuite.Geometries;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class InitialPlatform : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "sync");

            migrationBuilder.EnsureSchema(
                name: "farm");

            migrationBuilder.EnsureSchema(
                name: "operations");

            migrationBuilder.EnsureSchema(
                name: "livestock");

            migrationBuilder.EnsureSchema(
                name: "platform");

            migrationBuilder.EnsureSchema(
                name: "messaging");

            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:PostgresExtension:ltree", ",,")
                .Annotation("Npgsql:PostgresExtension:postgis", ",,");

            migrationBuilder.Sql("CREATE EXTENSION IF NOT EXISTS btree_gist;");

            migrationBuilder.CreateTable(
                name: "change_log",
                schema: "sync",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    entity_type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    entity_id = table.Column<Guid>(type: "uuid", nullable: false),
                    operation = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    changed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_change_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "command_log",
                schema: "sync",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    device_id = table.Column<Guid>(type: "uuid", nullable: false),
                    sequence = table.Column<long>(type: "bigint", nullable: false),
                    command_type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    schema_version = table.Column<int>(type: "integer", nullable: false),
                    captured_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    processed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    result_id = table.Column<Guid>(type: "uuid", nullable: true),
                    error_code = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: true),
                    error_message = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_command_log", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "org_nodes",
                schema: "platform",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    node_type = table.Column<int>(type: "integer", nullable: false),
                    path = table.Column<string>(type: "ltree", maxLength: 800, nullable: false),
                    parent_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_org_nodes", x => x.id);
                    table.UniqueConstraint("ak_org_nodes_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_org_nodes_org_nodes_tenant_id_parent_id",
                        columns: x => new { x.tenant_id, x.parent_id },
                        principalSchema: "platform",
                        principalTable: "org_nodes",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "outbox_messages",
                schema: "messaging",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    payload = table.Column<string>(type: "jsonb", nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    processed_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    attempts = table.Column<int>(type: "integer", nullable: false),
                    last_error = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_outbox_messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "farms",
                schema: "farm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    organisation_node_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    code = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    country_code = table.Column<string>(type: "character varying(2)", maxLength: 2, nullable: false),
                    time_zone = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_farms", x => x.id);
                    table.UniqueConstraint("ak_farms_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_farms_org_nodes_tenant_id_organisation_node_id",
                        columns: x => new { x.tenant_id, x.organisation_node_id },
                        principalSchema: "platform",
                        principalTable: "org_nodes",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "fields",
                schema: "farm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_fields", x => x.id);
                    table.UniqueConstraint("ak_fields_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_fields_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "subjects",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    farm_id = table.Column<Guid>(type: "uuid", nullable: false),
                    kind = table.Column<int>(type: "integer", nullable: false),
                    name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    species = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    official_identifier = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_subjects", x => x.id);
                    table.UniqueConstraint("ak_subjects_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_subjects_farms_tenant_id_farm_id",
                        columns: x => new { x.tenant_id, x.farm_id },
                        principalSchema: "farm",
                        principalTable: "farms",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "field_boundary_versions",
                schema: "farm",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: false),
                    boundary = table.Column<MultiPolygon>(type: "geography (multipolygon, 4326)", nullable: false),
                    area_ha = table.Column<decimal>(type: "numeric(14,4)", precision: 14, scale: 4, nullable: false, computedColumnSql: "ST_Area(boundary) / 10000.0", stored: true),
                    valid_from = table.Column<DateOnly>(type: "date", nullable: false),
                    valid_to = table.Column<DateOnly>(type: "date", nullable: true),
                    source = table.Column<string>(type: "character varying(40)", maxLength: 40, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_field_boundary_versions", x => x.id);
                    table.UniqueConstraint("ak_field_boundary_versions_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_field_boundary_versions_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "measurements",
                schema: "livestock",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    subject_id = table.Column<Guid>(type: "uuid", nullable: false),
                    metric = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    value = table.Column<decimal>(type: "numeric(18,4)", precision: 18, scale: 4, nullable: false),
                    unit = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    recorded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    source_command_id = table.Column<Guid>(type: "uuid", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_measurements", x => x.id);
                    table.ForeignKey(
                        name: "fk_measurements_subjects_tenant_id_subject_id",
                        columns: x => new { x.tenant_id, x.subject_id },
                        principalSchema: "livestock",
                        principalTable: "subjects",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "field_operations",
                schema: "operations",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    field_id = table.Column<Guid>(type: "uuid", nullable: false),
                    boundary_version_id = table.Column<Guid>(type: "uuid", nullable: false),
                    operation_type = table.Column<string>(type: "character varying(80)", maxLength: 80, nullable: false),
                    occurred_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    recorded_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    source_command_id = table.Column<Guid>(type: "uuid", nullable: false),
                    notes = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_field_operations", x => x.id);
                    table.ForeignKey(
                        name: "fk_field_operations_field_boundary_versions_tenant_id_boundary",
                        columns: x => new { x.tenant_id, x.boundary_version_id },
                        principalSchema: "farm",
                        principalTable: "field_boundary_versions",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "fk_field_operations_fields_tenant_id_field_id",
                        columns: x => new { x.tenant_id, x.field_id },
                        principalSchema: "farm",
                        principalTable: "fields",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "ix_change_log_tenant_id_id",
                schema: "sync",
                table: "change_log",
                columns: new[] { "tenant_id", "id" });

            migrationBuilder.CreateIndex(
                name: "ix_command_log_tenant_id",
                schema: "sync",
                table: "command_log",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_command_log_tenant_id_device_id_sequence",
                schema: "sync",
                table: "command_log",
                columns: new[] { "tenant_id", "device_id", "sequence" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_farms_tenant_id",
                schema: "farm",
                table: "farms",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_farms_tenant_id_code",
                schema: "farm",
                table: "farms",
                columns: new[] { "tenant_id", "code" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_farms_tenant_id_organisation_node_id",
                schema: "farm",
                table: "farms",
                columns: new[] { "tenant_id", "organisation_node_id" });

            migrationBuilder.CreateIndex(
                name: "ix_field_boundary_versions_tenant_id",
                schema: "farm",
                table: "field_boundary_versions",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_field_boundary_versions_tenant_id_field_id_valid_from",
                schema: "farm",
                table: "field_boundary_versions",
                columns: new[] { "tenant_id", "field_id", "valid_from" });

            migrationBuilder.CreateIndex(
                name: "ix_field_operations_tenant_id",
                schema: "operations",
                table: "field_operations",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_field_operations_tenant_id_boundary_version_id",
                schema: "operations",
                table: "field_operations",
                columns: new[] { "tenant_id", "boundary_version_id" });

            migrationBuilder.CreateIndex(
                name: "ix_field_operations_tenant_id_field_id_occurred_at",
                schema: "operations",
                table: "field_operations",
                columns: new[] { "tenant_id", "field_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_field_operations_tenant_id_source_command_id",
                schema: "operations",
                table: "field_operations",
                columns: new[] { "tenant_id", "source_command_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_fields_tenant_id",
                schema: "farm",
                table: "fields",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_fields_tenant_id_farm_id_name",
                schema: "farm",
                table: "fields",
                columns: new[] { "tenant_id", "farm_id", "name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_measurements_tenant_id",
                schema: "livestock",
                table: "measurements",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_measurements_tenant_id_source_command_id",
                schema: "livestock",
                table: "measurements",
                columns: new[] { "tenant_id", "source_command_id" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_measurements_tenant_id_subject_id_occurred_at",
                schema: "livestock",
                table: "measurements",
                columns: new[] { "tenant_id", "subject_id", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_org_nodes_tenant_id",
                schema: "platform",
                table: "org_nodes",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_org_nodes_tenant_id_parent_id",
                schema: "platform",
                table: "org_nodes",
                columns: new[] { "tenant_id", "parent_id" });

            migrationBuilder.CreateIndex(
                name: "ix_org_nodes_tenant_id_path",
                schema: "platform",
                table: "org_nodes",
                columns: new[] { "tenant_id", "path" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_outbox_messages_processed_at_occurred_at",
                schema: "messaging",
                table: "outbox_messages",
                columns: new[] { "processed_at", "occurred_at" });

            migrationBuilder.CreateIndex(
                name: "ix_subjects_tenant_id",
                schema: "livestock",
                table: "subjects",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_subjects_tenant_id_farm_id",
                schema: "livestock",
                table: "subjects",
                columns: new[] { "tenant_id", "farm_id" });

            migrationBuilder.CreateIndex(
                name: "ix_subjects_tenant_id_official_identifier",
                schema: "livestock",
                table: "subjects",
                columns: new[] { "tenant_id", "official_identifier" },
                unique: true,
                filter: "official_identifier IS NOT NULL");

            migrationBuilder.Sql(
                "CREATE INDEX ix_field_boundary_versions_boundary_gist " +
                "ON farm.field_boundary_versions USING gist (boundary);");

            migrationBuilder.Sql(
                "ALTER TABLE farm.field_boundary_versions " +
                "ADD CONSTRAINT ex_field_boundary_versions_validity " +
                "EXCLUDE USING gist (tenant_id WITH =, field_id WITH =, " +
                "daterange(valid_from, valid_to, '[]') WITH &&);");

            migrationBuilder.Sql(
                "CREATE INDEX ix_org_nodes_path_gist " +
                "ON platform.org_nodes USING gist (path);");

            migrationBuilder.Sql(
                """
                DO $rls$
                DECLARE
                    table_name text;
                BEGIN
                    FOREACH table_name IN ARRAY ARRAY[
                        'platform.org_nodes',
                        'farm.farms',
                        'farm.fields',
                        'farm.field_boundary_versions',
                        'operations.field_operations',
                        'livestock.subjects',
                        'livestock.measurements',
                        'sync.command_log',
                        'sync.change_log',
                        'messaging.outbox_messages'
                    ]
                    LOOP
                        EXECUTE format('ALTER TABLE %s ENABLE ROW LEVEL SECURITY', table_name);
                        EXECUTE format('ALTER TABLE %s FORCE ROW LEVEL SECURITY', table_name);
                        EXECUTE format(
                            'CREATE POLICY tenant_isolation ON %s USING (' ||
                            'tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid' ||
                            ') WITH CHECK (' ||
                            'tenant_id = nullif(current_setting(''app.tenant_id'', true), '''')::uuid' ||
                            ')',
                            table_name);
                    END LOOP;
                END
                $rls$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "change_log",
                schema: "sync");

            migrationBuilder.DropTable(
                name: "command_log",
                schema: "sync");

            migrationBuilder.DropTable(
                name: "field_operations",
                schema: "operations");

            migrationBuilder.DropTable(
                name: "measurements",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "outbox_messages",
                schema: "messaging");

            migrationBuilder.DropTable(
                name: "field_boundary_versions",
                schema: "farm");

            migrationBuilder.DropTable(
                name: "subjects",
                schema: "livestock");

            migrationBuilder.DropTable(
                name: "fields",
                schema: "farm");

            migrationBuilder.DropTable(
                name: "farms",
                schema: "farm");

            migrationBuilder.DropTable(
                name: "org_nodes",
                schema: "platform");
        }
    }
}
