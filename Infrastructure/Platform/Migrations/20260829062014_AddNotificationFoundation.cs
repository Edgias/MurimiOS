using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddNotificationFoundation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "notifications");

            migrationBuilder.CreateTable(
                name: "notifications",
                schema: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    recipient_membership_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    body = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    severity = table.Column<int>(type: "integer", nullable: false),
                    resource_type = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: true),
                    resource_id = table.Column<Guid>(type: "uuid", nullable: true),
                    action_url = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    read_at = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: true),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_notifications", x => x.id);
                    table.ForeignKey(
                        name: "fk_notifications_memberships_tenant_id_recipient_membership_id",
                        columns: x => new { x.tenant_id, x.recipient_membership_id },
                        principalSchema: "access",
                        principalTable: "memberships",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "preferences",
                schema: "notifications",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    membership_id = table.Column<Guid>(type: "uuid", nullable: false),
                    category = table.Column<string>(type: "character varying(120)", maxLength: 120, nullable: false),
                    in_app_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    email_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    sms_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    tenant_id = table.Column<Guid>(type: "uuid", nullable: false),
                    created_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    updated_at_utc = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false),
                    version = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_preferences", x => x.id);
                    table.ForeignKey(
                        name: "fk_preferences_memberships_tenant_id_membership_id",
                        columns: x => new { x.tenant_id, x.membership_id },
                        principalSchema: "access",
                        principalTable: "memberships",
                        principalColumns: new[] { "tenant_id", "id" },
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_notifications_tenant_id",
                schema: "notifications",
                table: "notifications",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_notifications_tenant_id_recipient_membership_id_read_at_cre",
                schema: "notifications",
                table: "notifications",
                columns: new[] { "tenant_id", "recipient_membership_id", "read_at", "created_at_utc" });

            migrationBuilder.CreateIndex(
                name: "ix_notifications_tenant_id_resource_type_resource_id",
                schema: "notifications",
                table: "notifications",
                columns: new[] { "tenant_id", "resource_type", "resource_id" });

            migrationBuilder.CreateIndex(
                name: "ix_preferences_tenant_id",
                schema: "notifications",
                table: "preferences",
                column: "tenant_id");

            migrationBuilder.CreateIndex(
                name: "ix_preferences_tenant_id_membership_id_category",
                schema: "notifications",
                table: "preferences",
                columns: new[] { "tenant_id", "membership_id", "category" },
                unique: true);

            migrationBuilder.Sql("""
                ALTER TABLE notifications.notifications ENABLE ROW LEVEL SECURITY;
                ALTER TABLE notifications.notifications FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON notifications.notifications
                    USING (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid)
                    WITH CHECK (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid);

                ALTER TABLE notifications.preferences ENABLE ROW LEVEL SECURITY;
                ALTER TABLE notifications.preferences FORCE ROW LEVEL SECURITY;
                CREATE POLICY tenant_isolation ON notifications.preferences
                    USING (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid)
                    WITH CHECK (tenant_id = nullif(current_setting('app.tenant_id', true), '')::uuid);

                DO $grant$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        GRANT USAGE ON SCHEMA notifications TO murimios_app;
                        GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA notifications TO murimios_app;
                    END IF;
                END
                $grant$;

                CREATE OR REPLACE FUNCTION messaging.outbox_health()
                RETURNS TABLE (
                    pending_count bigint,
                    oldest_pending_at timestamp with time zone,
                    dead_letter_count bigint)
                LANGUAGE sql
                SECURITY DEFINER
                SET search_path = pg_catalog, messaging
                AS $function$
                    SELECT COUNT(*) FILTER (WHERE processed_at IS NULL AND dead_lettered_at IS NULL),
                           MIN(occurred_at) FILTER (WHERE processed_at IS NULL AND dead_lettered_at IS NULL),
                           COUNT(*) FILTER (WHERE dead_lettered_at IS NOT NULL)
                    FROM messaging.outbox_messages;
                $function$;
                REVOKE ALL ON FUNCTION messaging.outbox_health() FROM PUBLIC;
                GRANT EXECUTE ON FUNCTION messaging.outbox_health() TO murimios_app;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS messaging.outbox_health();");

            migrationBuilder.DropTable(
                name: "notifications",
                schema: "notifications");

            migrationBuilder.DropTable(
                name: "preferences",
                schema: "notifications");
        }
    }
}
