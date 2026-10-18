using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddScoutingRiskFilters : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "issue_code",
                schema: "insights",
                table: "risk_rules",
                type: "character varying(80)",
                maxLength: 80,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "scouting_category",
                schema: "insights",
                table: "risk_rules",
                type: "integer",
                nullable: true);

            migrationBuilder.Sql(
                """
                ALTER TABLE insights.risk_rules ADD CONSTRAINT ck_risk_rule_scouting_filter CHECK (
                    (scouting_category IS NULL OR scouting_category BETWEEN 1 AND 8)
                    AND ((scouting_category IS NULL AND issue_code IS NULL) OR metric = 8));

                CREATE OR REPLACE FUNCTION insights.guard_risk_rule_filter_update() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF OLD.scouting_category IS DISTINCT FROM NEW.scouting_category
                       OR OLD.issue_code IS DISTINCT FROM NEW.issue_code THEN
                        RAISE EXCEPTION 'Risk-rule scouting filters are immutable; create a new version.' USING ERRCODE = '55000';
                    END IF;
                    RETURN NEW;
                END $$;
                CREATE TRIGGER risk_rule_filter_guard BEFORE UPDATE ON insights.risk_rules
                    FOR EACH ROW EXECUTE FUNCTION insights.guard_risk_rule_filter_update();
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP TRIGGER IF EXISTS risk_rule_filter_guard ON insights.risk_rules;
                DROP FUNCTION IF EXISTS insights.guard_risk_rule_filter_update();
                ALTER TABLE insights.risk_rules DROP CONSTRAINT IF EXISTS ck_risk_rule_scouting_filter;
                """);

            migrationBuilder.DropColumn(
                name: "issue_code",
                schema: "insights",
                table: "risk_rules");

            migrationBuilder.DropColumn(
                name: "scouting_category",
                schema: "insights",
                table: "risk_rules");
        }
    }
}
