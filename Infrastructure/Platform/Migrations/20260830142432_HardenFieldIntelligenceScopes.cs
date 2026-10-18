using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class HardenFieldIntelligenceScopes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                CREATE OR REPLACE FUNCTION insights.validate_record_scope() RETURNS trigger LANGUAGE plpgsql AS $$
                DECLARE expected_feed_type integer;
                BEGIN
                    IF TG_TABLE_NAME = 'weather_locations' THEN
                        IF NEW.field_id IS NOT NULL AND NOT EXISTS (
                            SELECT 1 FROM farm.fields f WHERE f.tenant_id = NEW.tenant_id
                              AND f.id = NEW.field_id AND f.farm_id = NEW.farm_id) THEN
                            RAISE EXCEPTION 'Weather location field must belong to its farm.' USING ERRCODE = '23514';
                        END IF;
                        RETURN NEW;
                    END IF;

                    IF TG_TABLE_NAME IN ('weather_observations', 'weather_forecasts') THEN
                        expected_feed_type := 1;
                        IF NOT EXISTS (SELECT 1 FROM insights.weather_locations l WHERE l.tenant_id = NEW.tenant_id
                            AND l.id = NEW.location_id AND l.farm_id = NEW.farm_id AND l.field_id IS NOT DISTINCT FROM NEW.field_id) THEN
                            RAISE EXCEPTION 'Weather location scope does not match the canonical weather record.' USING ERRCODE = '23514';
                        END IF;
                    ELSIF TG_TABLE_NAME = 'satellite_field_statistics' THEN
                        expected_feed_type := 2;
                        IF NOT EXISTS (SELECT 1 FROM farm.field_boundary_versions b WHERE b.tenant_id = NEW.tenant_id
                            AND b.id = NEW.boundary_version_id AND b.field_id = NEW.field_id) THEN
                            RAISE EXCEPTION 'Satellite boundary snapshot does not belong to the field.' USING ERRCODE = '23514';
                        END IF;
                    END IF;

                    IF expected_feed_type IS NOT NULL THEN
                        IF NOT EXISTS (
                            SELECT 1 FROM integrations.ingestion_records r
                            JOIN integrations.feeds f ON f.tenant_id = r.tenant_id AND f.id = r.feed_id
                            WHERE r.tenant_id = NEW.tenant_id AND r.id = NEW.ingestion_record_id
                              AND r.farm_id = NEW.farm_id AND r.feed_id = NEW.feed_id
                              AND f.feed_type = expected_feed_type
                              AND (f.target_type <> 2 OR f.target_id IS NOT DISTINCT FROM NEW.field_id)) THEN
                            RAISE EXCEPTION 'Ingestion provenance, feed type, farm, or field target does not match the canonical record.' USING ERRCODE = '23514';
                        END IF;
                    END IF;
                    RETURN NEW;
                END $$;

                CREATE TRIGGER weather_location_scope BEFORE INSERT ON insights.weather_locations FOR EACH ROW EXECUTE FUNCTION insights.validate_record_scope();
                CREATE TRIGGER weather_observation_scope BEFORE INSERT ON insights.weather_observations FOR EACH ROW EXECUTE FUNCTION insights.validate_record_scope();
                CREATE TRIGGER weather_forecast_scope BEFORE INSERT ON insights.weather_forecasts FOR EACH ROW EXECUTE FUNCTION insights.validate_record_scope();
                CREATE TRIGGER satellite_statistic_scope BEFORE INSERT ON insights.satellite_field_statistics FOR EACH ROW EXECUTE FUNCTION insights.validate_record_scope();

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
                    ELSIF TG_TABLE_NAME = 'alerts' AND NEW.follow_up_work_order_id IS NOT NULL THEN
                        IF NOT EXISTS (SELECT 1 FROM operations.work_orders w WHERE w.tenant_id = NEW.tenant_id
                            AND w.id = NEW.follow_up_work_order_id AND w.farm_id = NEW.farm_id AND w.field_id = NEW.field_id) THEN
                            RAISE EXCEPTION 'Alert follow-up work order must target the same farm and field.' USING ERRCODE = '23514';
                        END IF;
                    END IF;
                    RETURN NEW;
                END $$;
                CREATE TRIGGER scouting_scope BEFORE INSERT ON insights.scouting_observations FOR EACH ROW EXECUTE FUNCTION insights.validate_field_boundary_scope();
                CREATE TRIGGER risk_assessment_scope BEFORE INSERT ON insights.risk_assessments FOR EACH ROW EXECUTE FUNCTION insights.validate_field_boundary_scope();
                CREATE TRIGGER alert_scope BEFORE INSERT OR UPDATE OF follow_up_work_order_id ON insights.alerts FOR EACH ROW EXECUTE FUNCTION insights.validate_field_boundary_scope();
                CREATE TRIGGER map_layer_scope BEFORE INSERT ON insights.map_layers FOR EACH ROW EXECUTE FUNCTION insights.validate_field_boundary_scope();
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DROP TRIGGER IF EXISTS weather_location_scope ON insights.weather_locations;
                DROP TRIGGER IF EXISTS weather_observation_scope ON insights.weather_observations;
                DROP TRIGGER IF EXISTS weather_forecast_scope ON insights.weather_forecasts;
                DROP TRIGGER IF EXISTS satellite_statistic_scope ON insights.satellite_field_statistics;
                DROP TRIGGER IF EXISTS scouting_scope ON insights.scouting_observations;
                DROP TRIGGER IF EXISTS risk_assessment_scope ON insights.risk_assessments;
                DROP TRIGGER IF EXISTS alert_scope ON insights.alerts;
                DROP TRIGGER IF EXISTS map_layer_scope ON insights.map_layers;
                DROP FUNCTION IF EXISTS insights.validate_record_scope();
                DROP FUNCTION IF EXISTS insights.validate_field_boundary_scope();
                """);
        }
    }
}
