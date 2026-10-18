using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class FixFieldIntelligenceScopeGuard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION insights.validate_field_boundary_scope() RETURNS trigger LANGUAGE plpgsql AS $$
                BEGIN
                    IF NOT EXISTS (SELECT 1 FROM farm.fields f WHERE f.tenant_id = NEW.tenant_id
                        AND f.id = NEW.field_id AND f.farm_id = NEW.farm_id) THEN
                        RAISE EXCEPTION 'Field must belong to the intelligence record farm.' USING ERRCODE = '23514';
                    END IF;
                    IF NOT EXISTS (SELECT 1 FROM farm.field_boundary_versions b WHERE b.tenant_id = NEW.tenant_id
                        AND b.id = NEW.boundary_version_id AND b.field_id = NEW.field_id) THEN
                        RAISE EXCEPTION 'Boundary snapshot must belong to the intelligence record field.' USING ERRCODE = '23514';
                    END IF;
                    IF TG_TABLE_NAME = 'scouting_observations' THEN
                        IF NEW.crop_zone_id IS NOT NULL AND NOT EXISTS (SELECT 1 FROM production.crop_zones z
                            WHERE z.tenant_id = NEW.tenant_id AND z.id = NEW.crop_zone_id AND z.field_id = NEW.field_id) THEN
                            RAISE EXCEPTION 'Scouting crop zone must belong to its field.' USING ERRCODE = '23514';
                        END IF;
                        IF NEW.evidence_id IS NOT NULL AND NOT EXISTS (
                            SELECT 1 FROM evidence.attachments e JOIN production.crop_zones z
                              ON z.tenant_id = e.tenant_id AND z.id = e.crop_zone_id
                            WHERE e.tenant_id = NEW.tenant_id AND e.id = NEW.evidence_id AND z.field_id = NEW.field_id) THEN
                            RAISE EXCEPTION 'Scouting evidence must belong to a crop zone on its field.' USING ERRCODE = '23514';
                        END IF;
                    ELSIF TG_TABLE_NAME = 'risk_assessments' THEN
                        IF NOT EXISTS (SELECT 1 FROM insights.risk_rules r WHERE r.tenant_id = NEW.tenant_id
                            AND r.id = NEW.rule_id AND r.rule_version = NEW.rule_version) THEN
                            RAISE EXCEPTION 'Risk assessment rule version does not match its rule.' USING ERRCODE = '23514';
                        END IF;
                    ELSIF TG_TABLE_NAME = 'alerts' THEN
                        IF NEW.follow_up_work_order_id IS NOT NULL AND NOT EXISTS (
                            SELECT 1 FROM operations.work_orders w WHERE w.tenant_id = NEW.tenant_id
                              AND w.id = NEW.follow_up_work_order_id AND w.farm_id = NEW.farm_id AND w.field_id = NEW.field_id) THEN
                            RAISE EXCEPTION 'Alert follow-up work order must target the same farm and field.' USING ERRCODE = '23514';
                        END IF;
                    END IF;
                    RETURN NEW;
                END $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
