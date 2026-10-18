using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class FixComplianceStandardContentGuard : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION compliance.guard_standard_content() RETURNS trigger LANGUAGE plpgsql AS $$
                DECLARE standard_status integer; entity_tenant uuid; parent_standard uuid; parent_control uuid;
                BEGIN
                    IF TG_TABLE_NAME = 'control_points' THEN
                        IF TG_OP = 'DELETE' THEN entity_tenant := OLD.tenant_id; parent_standard := OLD.standard_version_id;
                        ELSE entity_tenant := NEW.tenant_id; parent_standard := NEW.standard_version_id; END IF;
                        SELECT status INTO standard_status FROM compliance.standard_versions
                        WHERE tenant_id = entity_tenant AND id = parent_standard;
                    ELSIF TG_TABLE_NAME = 'evidence_requirements' THEN
                        IF TG_OP = 'DELETE' THEN entity_tenant := OLD.tenant_id; parent_control := OLD.control_point_id;
                        ELSE entity_tenant := NEW.tenant_id; parent_control := NEW.control_point_id; END IF;
                        SELECT s.status INTO standard_status
                        FROM compliance.standard_versions s
                        JOIN compliance.control_points c ON c.tenant_id = s.tenant_id AND c.standard_version_id = s.id
                        WHERE c.tenant_id = entity_tenant AND c.id = parent_control;
                    ELSE
                        RAISE EXCEPTION 'Unsupported table % for standard-content guard.', TG_TABLE_NAME USING ERRCODE = '55000';
                    END IF;
                    IF standard_status IS DISTINCT FROM 1 THEN
                        RAISE EXCEPTION 'Published or retired compliance standard content is immutable.' USING ERRCODE = '55000';
                    END IF;
                    IF TG_OP = 'DELETE' THEN RETURN OLD; ELSE RETURN NEW; END IF;
                END $$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Do not restore the defective trigger body when rolling back this marker migration.
        }
    }
}
