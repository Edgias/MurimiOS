using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class GrantProviderIngestionRuntimeAccess : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DO $grant$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        GRANT USAGE ON SCHEMA integrations TO murimios_app;
                        GRANT SELECT, INSERT, UPDATE, DELETE ON ALL TABLES IN SCHEMA integrations TO murimios_app;
                        GRANT USAGE, SELECT, UPDATE ON ALL SEQUENCES IN SCHEMA integrations TO murimios_app;
                        ALTER DEFAULT PRIVILEGES IN SCHEMA integrations
                            GRANT SELECT, INSERT, UPDATE, DELETE ON TABLES TO murimios_app;
                        ALTER DEFAULT PRIVILEGES IN SCHEMA integrations
                            GRANT USAGE, SELECT, UPDATE ON SEQUENCES TO murimios_app;
                    END IF;
                END
                $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DO $revoke$
                BEGIN
                    IF EXISTS (SELECT 1 FROM pg_roles WHERE rolname = 'murimios_app') THEN
                        ALTER DEFAULT PRIVILEGES IN SCHEMA integrations
                            REVOKE SELECT, INSERT, UPDATE, DELETE ON TABLES FROM murimios_app;
                        ALTER DEFAULT PRIVILEGES IN SCHEMA integrations
                            REVOKE USAGE, SELECT, UPDATE ON SEQUENCES FROM murimios_app;
                        REVOKE ALL PRIVILEGES ON ALL TABLES IN SCHEMA integrations FROM murimios_app;
                        REVOKE ALL PRIVILEGES ON ALL SEQUENCES IN SCHEMA integrations FROM murimios_app;
                        REVOKE USAGE ON SCHEMA integrations FROM murimios_app;
                    END IF;
                END
                $revoke$;
                """);
        }
    }
}
