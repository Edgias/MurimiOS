using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.StorageLot;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.StorageLot;
using Crop = global::EdgIQ.MurimiOS.Domain.Crop.Crop;

using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using StorageLot = global::EdgIQ.MurimiOS.Domain.StorageLot.StorageLot;
internal static class StorageLotConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var storageLot = modelBuilder.Entity<StorageLot>();

        storageLot.ToTable("storage_lots", "traceability");

        context.ConfigureTenantEntity(storageLot);

        storageLot.Property(x => x.LotCode).HasMaxLength(120);

        storageLot.Property(x => x.Crop).HasMaxLength(120);

        storageLot.Property(x => x.Variety).HasMaxLength(120);

        storageLot.Property(x => x.Quantity).HasPrecision(18, 4);

        storageLot.Property(x => x.Unit).HasMaxLength(30);

        storageLot.Property(x => x.EnteredQuantity).HasPrecision(18, 4);

        storageLot.Property(x => x.EnteredUnit).HasMaxLength(30);

        storageLot.Property(x => x.DisplayUnit).HasMaxLength(30);

        storageLot.Property(x => x.StorageLocation).HasMaxLength(240);

        storageLot.HasIndex(x => new { x.TenantId, x.LotCode }).IsUnique();

        storageLot.HasIndex(x => new { x.TenantId, x.FarmId, x.PackedAt });

        storageLot.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
