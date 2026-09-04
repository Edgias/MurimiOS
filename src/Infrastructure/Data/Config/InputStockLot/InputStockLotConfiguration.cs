using EdgIQ.MurimiOS.Domain.InputStockLot;
using EdgIQ.MurimiOS.Domain.AgriculturalInput;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.InputStockLot;
using AgriculturalInput = global::EdgIQ.MurimiOS.Domain.AgriculturalInput.AgriculturalInput;
using InputStockLot = global::EdgIQ.MurimiOS.Domain.InputStockLot.InputStockLot;
internal static class InputStockLotConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var stockLot = modelBuilder.Entity<InputStockLot>();

        stockLot.ToTable("input_stock_lots", "inventory");

        context.ConfigureTenantEntity(stockLot);

        stockLot.Property(x => x.LotCode).HasMaxLength(120);

        stockLot.Property(x => x.InitialQuantity).HasPrecision(18, 4);

        stockLot.Property(x => x.QuantityOnHand).HasPrecision(18, 4);

        stockLot.Property(x => x.EnteredInitialQuantity).HasPrecision(18, 4);

        stockLot.Property(x => x.EnteredUnit).HasMaxLength(30);

        stockLot.Property(x => x.DisplayUnit).HasMaxLength(30);

        stockLot.HasIndex(x => new { x.TenantId, x.LotCode }).IsUnique();

        stockLot.HasIndex(x => new { x.TenantId, x.InputId, x.ReceivedOn });

        stockLot.HasOne<AgriculturalInput>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.InputId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
