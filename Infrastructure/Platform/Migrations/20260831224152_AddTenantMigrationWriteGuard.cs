using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddTenantMigrationWriteGuard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION enterprise.tenant_migration_write_blocked(p_tenant_id uuid)
                RETURNS boolean LANGUAGE sql STABLE SECURITY DEFINER SET search_path=pg_catalog,enterprise AS $fn$
                    SELECT EXISTS(SELECT 1 FROM enterprise.tenant_migration_jobs j
                        WHERE j.tenant_id=p_tenant_id AND j.status BETWEEN 2 AND 5)
                $fn$;
                REVOKE ALL ON FUNCTION enterprise.tenant_migration_write_blocked(uuid) FROM PUBLIC;
                DO $grant$ BEGIN IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname='murimios_app') THEN
                    GRANT EXECUTE ON FUNCTION enterprise.tenant_migration_write_blocked(uuid) TO murimios_app;
                END IF; END $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS enterprise.tenant_migration_write_blocked(uuid);");
        }
    }
}
