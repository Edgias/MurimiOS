using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class AddGrantedTransparencyPresentation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION retail.resolve_granted_transparency_presentation(p_token_hash text)
                RETURNS jsonb LANGUAGE sql STABLE SECURITY DEFINER
                SET search_path=pg_catalog,retail,supply,production,farm,traceability,compliance AS $fn$
                    WITH authorized AS (
                        SELECT p.attributes AS policy_attributes,g.attributes AS grant_attributes,g.entity_categories,g.producer_tenant_id,
                               h.id AS harvest_id,h.lot_code,h.quantity,h.unit,h.harvested_at,
                               z.crop,z.variety,s.farm_id,f.name AS farm_name,f.country_code
                        FROM retail.transparency_tokens t
                        JOIN retail.disclosure_policies p ON p.tenant_id=t.tenant_id AND p.id=t.policy_id
                        JOIN supply.data_grants g ON g.id=p.data_grant_id AND g.recipient_tenant_id=t.tenant_id
                        JOIN supply.relationships r ON r.id=g.relationship_id AND r.status=2
                        JOIN production.harvest_lots h ON h.tenant_id=g.producer_tenant_id AND h.id=t.object_id
                        JOIN production.crop_zones z ON z.tenant_id=h.tenant_id AND z.id=h.crop_zone_id
                        JOIN production.seasons s ON s.tenant_id=z.tenant_id AND s.id=z.season_id
                        JOIN farm.farms f ON f.tenant_id=s.tenant_id AND f.id=s.farm_id
                        WHERE t.token_hash=lower(p_token_hash) AND t.status=1 AND p.status=2
                          AND (t.expires_at IS NULL OR t.expires_at>statement_timestamp())
                          AND g.status=1 AND g.downstream_disclosure_allowed AND g.valid_from<=statement_timestamp()
                          AND (g.valid_to IS NULL OR g.valid_to>statement_timestamp())
                          AND 'harvest-lots'=ANY(g.entity_categories) AND s.farm_id=ANY(g.farm_ids)
                          AND (cardinality(g.season_ids)=0 OR s.id=ANY(g.season_ids))
                          AND (cardinality(g.commodities)=0 OR z.crop=ANY(g.commodities))
                    )
                    SELECT jsonb_strip_nulls(jsonb_build_object(
                        'origin',CASE WHEN 'origin'=ANY(a.policy_attributes) AND ('*'=ANY(a.grant_attributes) OR 'origin'=ANY(a.grant_attributes)) THEN jsonb_build_object('farm',a.farm_name,'countryCode',a.country_code,'farmId',a.farm_id) END,
                        'crop',CASE WHEN 'crop'=ANY(a.policy_attributes) AND ('*'=ANY(a.grant_attributes) OR 'crop'=ANY(a.grant_attributes)) THEN to_jsonb(a.crop) END,
                        'variety',CASE WHEN 'variety'=ANY(a.policy_attributes) AND ('*'=ANY(a.grant_attributes) OR 'variety'=ANY(a.grant_attributes)) THEN to_jsonb(a.variety) END,
                        'lotCode',CASE WHEN 'lotCode'=ANY(a.policy_attributes) AND ('*'=ANY(a.grant_attributes) OR 'lotCode'=ANY(a.grant_attributes)) THEN to_jsonb(a.lot_code) END,
                        'harvestedAt',CASE WHEN 'harvestedAt'=ANY(a.policy_attributes) AND ('*'=ANY(a.grant_attributes) OR 'harvestedAt'=ANY(a.grant_attributes)) THEN to_jsonb(a.harvested_at) END,
                        'quantity',CASE WHEN 'quantity'=ANY(a.policy_attributes) AND ('*'=ANY(a.grant_attributes) OR 'quantity'=ANY(a.grant_attributes)) THEN jsonb_build_object('quantity',a.quantity,'unit',a.unit) END,
                        'quality',CASE WHEN 'quality'=ANY(a.policy_attributes) AND 'quality-assessments'=ANY(a.entity_categories) THEN COALESCE((SELECT jsonb_agg(jsonb_build_object('metric',q.metric,'numericResult',q.numeric_result,'textResult',q.text_result,'unit',q.unit,'passed',q.passed,'assessedAt',q.assessed_at) ORDER BY q.assessed_at DESC) FROM traceability.quality_assessments q WHERE q.tenant_id=a.producer_tenant_id AND q.object_type=2 AND q.object_id=a.harvest_id),'[]'::jsonb) END,
                        'compliance',CASE WHEN 'compliance'=ANY(a.policy_attributes) AND 'evidence'=ANY(a.entity_categories) THEN (SELECT jsonb_build_object('scorePercent',c.score_percent,'conforms',c.conforms,'completedAt',c.completed_at) FROM compliance.assessments c WHERE c.tenant_id=a.producer_tenant_id AND c.farm_id=a.farm_id AND c.status=4 ORDER BY c.completed_at DESC LIMIT 1) END
                    )) FROM authorized a
                $fn$;
                REVOKE ALL ON FUNCTION retail.resolve_granted_transparency_presentation(text) FROM PUBLIC;
                DO $grant$ BEGIN IF EXISTS(SELECT 1 FROM pg_roles WHERE rolname='murimios_app') THEN
                    GRANT EXECUTE ON FUNCTION retail.resolve_granted_transparency_presentation(text) TO murimios_app;
                    REVOKE DELETE ON ALL TABLES IN SCHEMA external_api,retail,enterprise FROM murimios_app;
                    REVOKE UPDATE ON external_api.access_log,retail.transparency_access_log,enterprise.approval_decisions FROM murimios_app;
                END IF; END $grant$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("DROP FUNCTION IF EXISTS retail.resolve_granted_transparency_presentation(text);");
        }
    }
}
