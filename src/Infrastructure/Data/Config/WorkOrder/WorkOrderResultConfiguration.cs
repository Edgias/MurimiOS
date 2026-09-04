using EdgIQ.MurimiOS.Domain.WorkOrder;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.WorkOrder;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
using WorkOrderResult = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrderResult;
internal static class WorkOrderResultConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var result = modelBuilder.Entity<WorkOrderResult>();

        result.ToTable("work_order_results", "operations");

        context.ConfigureTenantEntity(result);

        result.Property(x => x.Notes).HasMaxLength(2_000);

        result.Property(x => x.RecordedBy).HasMaxLength(240);

        result.HasIndex(x => new { x.TenantId, x.WorkOrderId, x.ResultType, x.ResultId }).IsUnique();

        result.HasIndex(x => new { x.TenantId, x.ResultType, x.ResultId });

        result.HasOne<WorkOrder>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.WorkOrderId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
