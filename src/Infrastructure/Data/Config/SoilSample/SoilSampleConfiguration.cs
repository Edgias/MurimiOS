using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.SoilZone;
using EdgIQ.MurimiOS.Domain.SoilSample;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SoilSample;
using Location = global::EdgIQ.MurimiOS.Domain.Location.Location;

using SoilZone = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZone;

using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using SoilSample = global::EdgIQ.MurimiOS.Domain.SoilSample.SoilSample;
using SoilZoneBoundaryVersion = global::EdgIQ.MurimiOS.Domain.SoilZone.SoilZoneBoundaryVersion;
internal static class SoilSampleConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var soilSample = modelBuilder.Entity<SoilSample>();

        soilSample.ToTable("soil_samples", "farm");

        context.ConfigureTenantEntity(soilSample);

        soilSample.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_soil_samples_tenant_id_id");

        soilSample.Property(x => x.SampleCode).HasMaxLength(100);

        soilSample.Property(x => x.Location).HasColumnType("geography (point, 4326)");

        soilSample.Property(x => x.SampledBy).HasMaxLength(240);

        soilSample.Property(x => x.DepthFromCm).HasPrecision(8, 2);

        soilSample.Property(x => x.DepthToCm).HasPrecision(8, 2);

        soilSample.Property(x => x.LaboratoryName).HasMaxLength(240);

        soilSample.Property(x => x.LaboratoryReference).HasMaxLength(160);

        soilSample.Property(x => x.Notes).HasMaxLength(2_000);

        soilSample.Property(x => x.RejectionReason).HasMaxLength(2_000);

        soilSample.HasIndex(x => new { x.TenantId, x.FarmId, x.SampleCode }).IsUnique();

        soilSample.HasIndex(x => new { x.TenantId, x.FieldId, x.SampledAt });

        soilSample.HasIndex(x => new { x.TenantId, x.Status, x.SampledAt });

        soilSample.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        soilSample.HasOne<FarmField>().WithMany().HasForeignKey(x => new { x.TenantId, x.FieldId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        soilSample.HasOne<FieldBoundaryVersion>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FieldBoundaryVersionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        soilSample.HasOne<SoilZoneBoundaryVersion>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SoilZoneBoundaryVersionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
