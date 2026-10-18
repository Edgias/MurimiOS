using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class FixScheduledProviderUuidGeneration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $fix$
                DECLARE definition text;
                BEGIN
                    SELECT pg_get_functiondef('integrations.schedule_due_jobs(timestamptz,integer,integer)'::regprocedure)
                    INTO definition;
                    EXECUTE replace(definition,'public.gen_random_uuid()','pg_catalog.gen_random_uuid()');
                END
                $fix$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $fix$
                DECLARE definition text;
                BEGIN
                    SELECT pg_get_functiondef('integrations.schedule_due_jobs(timestamptz,integer,integer)'::regprocedure)
                    INTO definition;
                    EXECUTE replace(definition,'pg_catalog.gen_random_uuid()','public.gen_random_uuid()');
                END
                $fix$;
                """);
        }
    }
}
