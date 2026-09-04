using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.GrowingSeason;
using EdgIQ.MurimiOS.Domain.CropZone;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.CropZone;
using Crop = global::EdgIQ.MurimiOS.Domain.Crop.Crop;

using CropZone = global::EdgIQ.MurimiOS.Domain.CropZone.CropZone;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using GrowingSeason = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeason;
internal static class CropZoneConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var cropZone = modelBuilder.Entity<CropZone>();

        cropZone.ToTable("crop_zones", "production");

        context.ConfigureTenantEntity(cropZone);

        cropZone.Property(x => x.Name).HasMaxLength(160);

        cropZone.Property(x => x.Crop).HasMaxLength(120);

        cropZone.Property(x => x.Variety).HasMaxLength(120);

        cropZone.Property(x => x.Boundary).HasColumnType("geography (multipolygon, 4326)");

        cropZone.Property(x => x.AreaHa)
            .HasPrecision(14, 4)
            .HasComputedColumnSql("ST_Area(boundary) / 10000.0", stored: true);

        cropZone.HasIndex(x => new { x.TenantId, x.SeasonId, x.Name }).IsUnique();

        cropZone.HasIndex(x => new { x.TenantId, x.FieldId, x.SeasonId });

        cropZone.HasOne<GrowingSeason>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SeasonId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

        cropZone.HasOne<FarmField>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FieldId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

        cropZone.HasOne<FieldBoundaryVersion>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.BoundaryVersionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
