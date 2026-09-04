using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.SoilZone;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SoilZone;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;

using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using SoilZone = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZone;
using SoilZoneBoundaryVersion = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZoneBoundaryVersion;
internal static class SoilZoneBoundaryVersionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var soilBoundary = modelBuilder.Entity<SoilZoneBoundaryVersion>();

        soilBoundary.ToTable("soil_zone_boundary_versions", "farm");

        context.ConfigureTenantEntity(soilBoundary);

        soilBoundary.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_soil_zone_boundaries_tenant_id_id");

        soilBoundary.Property(x => x.Boundary).HasColumnType("geography (multipolygon, 4326)");

        soilBoundary.Property(x => x.AreaHa).HasPrecision(14, 4)
            .HasComputedColumnSql("ST_Area(boundary) / 10000.0", stored: true);

        soilBoundary.Property(x => x.Source).HasMaxLength(80);

        soilBoundary.HasIndex(x => new { x.TenantId, x.SoilZoneId, x.ValidFrom });

        soilBoundary.HasOne<SoilZone>().WithMany().HasForeignKey(x => new { x.TenantId, x.SoilZoneId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        soilBoundary.HasOne<FieldBoundaryVersion>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FieldBoundaryVersionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
