using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class GrantAuditSchemaAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        GRANT USAGE ON SCHEMA audit TO murimios_app;
                        GRANT SELECT, INSERT ON audit.events, audit.corrections TO murimios_app;
                        ALTER DEFAULT PRIVILEGES IN SCHEMA audit
                            GRANT SELECT, INSERT ON TABLES TO murimios_app;
                    END IF;
                END;
                $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DO $$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        ALTER DEFAULT PRIVILEGES IN SCHEMA audit
                            REVOKE SELECT, INSERT ON TABLES FROM murimios_app;
                        REVOKE ALL ON audit.events, audit.corrections FROM murimios_app;
                        REVOKE USAGE ON SCHEMA audit FROM murimios_app;
                    END IF;
                END;
                $$;
                """);
        }
    }
}
