using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Edgias.MurimiOS.Infrastructure.Platform.Migrations
{
    /// <inheritdoc />
    public partial class FixTenantMigrationCutoverTimestamp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("""
                CREATE OR REPLACE FUNCTION enterprise.complete_tenant_migration(p_job uuid,p_lease uuid,p_now timestamptz)
                RETURNS boolean LANGUAGE plpgsql VOLATILE SECURITY DEFINER SET search_path=pg_catalog,enterprise AS $fn$
                DECLARE j enterprise.tenant_migration_jobs%ROWTYPE; s enterprise.tenant_placements%ROWTYPE;
                    v_destination uuid; v_cutover timestamptz;
                BEGIN
                    SELECT job.* INTO j FROM enterprise.tenant_migration_jobs job
                    JOIN enterprise.tenant_migration_execution e ON e.job_id=job.id
                    WHERE job.id=p_job AND job.status=5 AND e.lease_id=p_lease AND e.leased_until>p_now FOR UPDATE OF job;
                    IF j.id IS NULL THEN RETURN false; END IF;
                    SELECT * INTO s FROM enterprise.tenant_placements WHERE tenant_id=j.tenant_id AND id=j.source_placement_id FOR UPDATE;
                    SELECT destination_placement_id INTO v_destination FROM enterprise.tenant_migration_execution WHERE job_id=j.id;
                    v_cutover := greatest(p_now,s.effective_from+interval '1 microsecond');
                    UPDATE enterprise.tenant_placements SET is_primary=false,
                        effective_to=coalesce(effective_to,greatest(v_cutover,effective_from+interval '1 microsecond')),
                        updated_at_utc=v_cutover,version=version+1 WHERE tenant_id=j.tenant_id AND is_primary AND effective_to IS NULL;
                    INSERT INTO enterprise.tenant_placements(id,region,database_key,mode,residency_country,is_primary,
                        effective_from,effective_to,tenant_id,created_at_utc,updated_at_utc,version)
                    VALUES(v_destination,j.destination_region,j.destination_database_key,j.destination_mode,s.residency_country,
                        true,v_cutover,NULL,j.tenant_id,v_cutover,v_cutover,1)
                    ON CONFLICT(id) DO UPDATE SET is_primary=true,effective_to=NULL,updated_at_utc=v_cutover,
                        version=enterprise.tenant_placements.version+1;
                    UPDATE enterprise.tenant_migration_jobs SET status=6,completed_at=v_cutover,updated_at_utc=v_cutover,
                        version=version+1 WHERE id=j.id;
                    UPDATE enterprise.tenant_migration_execution SET lease_id=NULL,leased_until=NULL,updated_at=v_cutover WHERE job_id=j.id;
                    RETURN true;
                END $fn$;
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
