using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class FixProductionSeasonFunctionReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                DO $fix$
                DECLARE definition text;
                BEGIN
                    SELECT pg_get_functiondef('analytics.validate_fact_scope()'::regprocedure) INTO definition;
                    EXECUTE replace(definition,'production.growing_seasons','production.seasons');
                    SELECT pg_get_functiondef('compliance.validate_assessment_scope()'::regprocedure) INTO definition;
                    EXECUTE replace(definition,'production.growing_seasons','production.seasons');
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
                    SELECT pg_get_functiondef('analytics.validate_fact_scope()'::regprocedure) INTO definition;
                    EXECUTE replace(definition,'production.seasons','production.growing_seasons');
                    SELECT pg_get_functiondef('compliance.validate_assessment_scope()'::regprocedure) INTO definition;
                    EXECUTE replace(definition,'production.seasons','production.growing_seasons');
                END
                $fix$;
                """);
        }
    }
}
