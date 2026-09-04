using EdgIQ.MurimiOS.Domain.WorkOrder;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.WorkOrder;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderStatusHistory = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderStatusHistory;
internal static class WorkOrderStatusHistoryConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var history = modelBuilder.Entity<WorkOrderStatusHistory>();

        history.ToTable("work_order_status_history", "operations");

        context.ConfigureTenantEntity(history);

        history.Property(x => x.Actor).HasMaxLength(240);

        history.Property(x => x.Reason).HasMaxLength(2_000);

        history.HasIndex(x => new { x.TenantId, x.WorkOrderId, x.OccurredAt });

        history.HasOne<WorkOrder>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.WorkOrderId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
