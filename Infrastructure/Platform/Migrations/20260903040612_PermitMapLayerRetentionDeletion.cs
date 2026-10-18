using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class PermitMapLayerRetentionDeletion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION insights.reject_immutable_mutation() RETURNS trigger LANGUAGE plpgsql AS $function$
                BEGIN
                    IF TG_OP='DELETE' AND current_user<>session_user
                       AND current_setting('app.analytics_retention',true)='on'
                       AND TG_TABLE_NAME='map_layers' THEN
                        RETURN OLD;
                    END IF;
                    RAISE EXCEPTION 'Field-intelligence source, assessment, and projection records are immutable.' USING ERRCODE='55000';
                END $function$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION insights.reject_immutable_mutation() RETURNS trigger LANGUAGE plpgsql AS $function$
                BEGIN
                    RAISE EXCEPTION 'Field-intelligence source, assessment, and projection records are immutable.' USING ERRCODE='55000';
                END $function$;
                """);
        }
    }
}
