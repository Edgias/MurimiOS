using EdgIQ.MurimiOS.Domain.VeterinaryMedicineStockLot;
using EdgIQ.MurimiOS.Domain.VeterinaryMedicine;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.VeterinaryMedicineStockLot;
using VeterinaryMedicine = global::EdgIQ.MurimiOS.Domain.VeterinaryMedicine.VeterinaryMedicine;
using VeterinaryMedicineStockLot = global::EdgIQ.MurimiOS.Domain.VeterinaryMedicineStockLot.VeterinaryMedicineStockLot;
internal static class VeterinaryMedicineStockLotConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var medicineStock = modelBuilder.Entity<VeterinaryMedicineStockLot>();

        medicineStock.ToTable("medicine_stock_lots", "livestock");

        context.ConfigureTenantEntity(medicineStock);

        medicineStock.Property(x => x.BatchNumber).HasMaxLength(120);

        medicineStock.Property(x => x.InitialQuantity).HasPrecision(18, 4);

        medicineStock.Property(x => x.QuantityOnHand).HasPrecision(18, 4);

        medicineStock.Property(x => x.EnteredInitialQuantity).HasPrecision(18, 4);

        medicineStock.Property(x => x.EnteredUnit).HasMaxLength(30);

        medicineStock.Property(x => x.DisplayUnit).HasMaxLength(30);

        medicineStock.HasIndex(x => new { x.TenantId, x.BatchNumber }).IsUnique();

        medicineStock.HasIndex(x => new { x.TenantId, x.MedicineId, x.ReceivedOn });

        medicineStock.HasOne<VeterinaryMedicine>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.MedicineId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
