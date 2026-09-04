using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SiteInfrastructureAsset;
using Location = global::EdgIQ.MurimiOS.Domain.Location.Location;

using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using SiteInfrastructureAsset = global::EdgIQ.MurimiOS.Domain.SiteInfrastructureAsset.SiteInfrastructureAsset;
internal static class SiteInfrastructureAssetConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var asset = modelBuilder.Entity<SiteInfrastructureAsset>();

        asset.ToTable("infrastructure_assets", "farm");

        context.ConfigureTenantEntity(asset);

        asset.Property(x => x.Code).HasMaxLength(80);

        asset.Property(x => x.Name).HasMaxLength(200);

        asset.Property(x => x.Location).HasColumnType("geography (point, 4326)");

        asset.Property(x => x.Capacity).HasPrecision(18, 4);

        asset.Property(x => x.CapacityUnit).HasMaxLength(30);

        asset.Property(x => x.DetailsJson).HasColumnType("jsonb");

        asset.Property(x => x.DecommissionedBy).HasMaxLength(240);

        asset.Property(x => x.DecommissionReason).HasMaxLength(2_000);

        asset.HasIndex(x => new { x.TenantId, x.FarmId, x.Code }).IsUnique();

        asset.HasIndex(x => new { x.TenantId, x.FarmId, x.Status, x.Type });

        asset.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        asset.HasOne<FarmField>().WithMany().HasForeignKey(x => new { x.TenantId, x.FieldId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
