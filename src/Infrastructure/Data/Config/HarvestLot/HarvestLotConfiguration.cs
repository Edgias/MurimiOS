using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.HarvestLot;
using EdgIQ.MurimiOS.Domain.CropZone;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.HarvestLot;
using CropZone = global::EdgIQ.MurimiOS.Domain.CropZone.CropZone;
using HarvestLot = global::EdgIQ.MurimiOS.Domain.HarvestLot.HarvestLot;
internal static class HarvestLotConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var harvestLot = modelBuilder.Entity<HarvestLot>();

        harvestLot.ToTable("harvest_lots", "production");

        context.ConfigureTenantEntity(harvestLot);

        harvestLot.Property(x => x.LotCode).HasMaxLength(120);

        harvestLot.Property(x => x.Quantity).HasPrecision(18, 4);

        harvestLot.Property(x => x.Unit).HasMaxLength(30);

        harvestLot.Property(x => x.EnteredQuantity).HasPrecision(18, 4);

        harvestLot.Property(x => x.EnteredUnit).HasMaxLength(30);

        harvestLot.Property(x => x.DisplayUnit).HasMaxLength(30);

        harvestLot.Property(x => x.Notes).HasMaxLength(2_000);

        harvestLot.HasIndex(x => new { x.TenantId, x.LotCode }).IsUnique();

        harvestLot.HasIndex(x => new { x.TenantId, x.CropZoneId, x.HarvestedAt });

        harvestLot.HasIndex(x => new { x.TenantId, x.SourceCommandId }).IsUnique();

        harvestLot.HasOne<CropZone>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.CropZoneId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
