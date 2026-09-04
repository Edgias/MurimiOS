using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.InventoryMovement;
using EdgIQ.MurimiOS.Domain.InputStockLot;
using EdgIQ.MurimiOS.Domain.FieldOperationRecord;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.InventoryMovement;
using FieldOperationRecord = global::EdgIQ.MurimiOS.Domain.FieldOperationRecord.FieldOperationRecord;
using InputStockLot = global::EdgIQ.MurimiOS.Domain.InputStockLot.InputStockLot;
using InventoryMovement = global::EdgIQ.MurimiOS.Domain.InventoryMovement.InventoryMovement;
internal static class InventoryMovementConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var movement = modelBuilder.Entity<InventoryMovement>();

        movement.ToTable("movements", "inventory");

        context.ConfigureTenantEntity(movement);

        movement.Property(x => x.Quantity).HasPrecision(18, 4);

        movement.Property(x => x.Unit).HasMaxLength(30);

        movement.Property(x => x.EnteredQuantity).HasPrecision(18, 4);

        movement.Property(x => x.EnteredUnit).HasMaxLength(30);

        movement.Property(x => x.DisplayUnit).HasMaxLength(30);

        movement.Property(x => x.Notes).HasMaxLength(2_000);

        movement.HasIndex(x => new { x.TenantId, x.StockLotId, x.OccurredAt });

        movement.HasIndex(x => new { x.TenantId, x.SourceCommandId })
            .IsUnique()
            .HasFilter("source_command_id IS NOT NULL");

        movement.HasOne<InputStockLot>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.StockLotId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

        movement.HasOne<FieldOperationRecord>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.OperationId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
