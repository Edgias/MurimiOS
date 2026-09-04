using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.InputStockLot;
using EdgIQ.MurimiOS.Domain.AgriculturalInput;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.WorkOrder;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using AgriculturalInput = global::EdgIQ.MurimiOS.Domain.AgriculturalInput.AgriculturalInput;
using InputStockLot = global::EdgIQ.MurimiOS.Domain.InputStockLot.InputStockLot;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderResource = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderResource;
internal static class WorkOrderResourceConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var resource = modelBuilder.Entity<WorkOrderResource>();

        resource.ToTable("work_order_resources", "operations");

        context.ConfigureTenantEntity(resource);

        resource.Property(x => x.ResourceName).HasMaxLength(160);

        resource.Property(x => x.ResourceReference).HasMaxLength(160);

        resource.Property(x => x.Quantity).HasPrecision(18, 4);

        resource.Property(x => x.Unit).HasMaxLength(30);

        resource.Property(x => x.EnteredQuantity).HasPrecision(18, 4);

        resource.Property(x => x.EnteredUnit).HasMaxLength(30);

        resource.Property(x => x.UnitCost).HasPrecision(18, 6);

        resource.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        resource.Ignore(x => x.PlannedCost);

        resource.HasIndex(x => new { x.TenantId, x.WorkOrderId, x.ResourceType });

        resource.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceReference, x.Status });

        resource.HasIndex(x => new { x.TenantId, x.StockLotId, x.Status });

        resource.HasOne<WorkOrder>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.WorkOrderId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        resource.HasOne<AgriculturalInput>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.AgriculturalInputId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        resource.HasOne<InputStockLot>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.StockLotId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
