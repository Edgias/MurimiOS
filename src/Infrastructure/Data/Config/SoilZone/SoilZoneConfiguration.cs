using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.SoilZone;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SoilZone;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using SoilZone = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZone;
internal static class SoilZoneConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var soilZone = modelBuilder.Entity<SoilZone>();

        soilZone.ToTable("soil_zones", "farm");

        context.ConfigureTenantEntity(soilZone);

        soilZone.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_soil_zones_tenant_id_id");

        soilZone.Property(x => x.Code).HasMaxLength(80);

        soilZone.Property(x => x.Name).HasMaxLength(200);

        soilZone.Property(x => x.Classification).HasMaxLength(120);

        soilZone.Property(x => x.ManagementGuidance).HasMaxLength(4_000);

        soilZone.Property(x => x.RetiredBy).HasMaxLength(240);

        soilZone.HasIndex(x => new { x.TenantId, x.FieldId, x.Code }).IsUnique();

        soilZone.HasIndex(x => new { x.TenantId, x.FarmId, x.Status });

        soilZone.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        soilZone.HasOne<FarmField>().WithMany().HasForeignKey(x => new { x.TenantId, x.FieldId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
