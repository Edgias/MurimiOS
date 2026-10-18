using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantMembershipAuthorization : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "access");

            migrationBuilder.CreateTable(
                name: "memberships",
                schema: "access",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    external_subject = table.Column<string>(type: "character varying(240)", maxLength: 240, nullable: false),
                    display_name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    email = table.Column<string>(type: "character varying(320)", maxLength: 320, nullable: true),
                    role = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    scope_organisation_node_id = table.Column<Guid>(type: "uuid", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_memberships", x => x.id);
                    table.UniqueConstraint("ak_memberships_tenant_id_id", x => new { x.tenant_id, x.id });
                    table.ForeignKey(
                        name: "fk_memberships_organisation_nodes_tenant_id_scope_organisation",
                        columns: x => new { x.tenant_id, x.scope_organisation_node_id },
                        principalSchema: "platform",
                        principalTable: "org_nodes",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "permission_grants",
                schema: "access",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    membership_id = table.Column<Guid>(type: "uuid", nullable: false),
                    permission = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_permission_grants", x => x.id);
                    table.ForeignKey(
                        name: "fk_permission_grants_memberships_tenant_id_membership_id",
                        columns: x => new { x.tenant_id, x.membership_id },
                        principalSchema: "access",
                        principalTable: "memberships",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_memberships_tenant_id",
                schema: "access",
                table: "memberships",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_memberships_tenant_id_external_subject",
                schema: "access",
                table: "memberships",
                columns: new[] { "tenant_id", "external_subject" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_memberships_tenant_id_scope_organisation_node_id",
                schema: "access",
                table: "memberships",
                columns: new[] { "tenant_id", "scope_organisation_node_id" });

            migrationBuilder.CreateIndex(
                name: "ix_memberships_tenant_id_status_role",
                schema: "access",
                table: "memberships",
                columns: new[] { "tenant_id", "status", "role" });

            migrationBuilder.CreateIndex(
                name: "ix_permission_grants_tenant_id",
                schema: "access",
                table: "permission_grants",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_permission_grants_tenant_id_membership_id_permission",
                schema: "access",
                table: "permission_grants",
                columns: new[] { "tenant_id", "membership_id", "permission" },
                unique: true);

            migrationBuilder.Sql(
                """
                ALTER TABLE access.memberships ENABLE ROW LEVEL SECURITY;
                ALTER TABLE access.memberships FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON access.memberships
                    USING (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid)
                    WITH CHECK (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid);

                ALTER TABLE access.permission_grants ENABLE ROW LEVEL SECURITY;
                ALTER TABLE access.permission_grants FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON access.permission_grants
                    USING (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid)
                    WITH CHECK (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid);
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "permission_grants",
                schema: "access");

            migrationBuilder.DropTable(
                name: "memberships",
                schema: "access");
        }
    }
}
