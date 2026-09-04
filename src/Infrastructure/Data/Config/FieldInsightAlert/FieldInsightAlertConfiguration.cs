using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.FieldInsightAlert;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.FieldInsightAlert;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using FieldInsightAlert = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.FieldInsightAlert;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
internal static class FieldInsightAlertConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var alert = modelBuilder.Entity<FieldInsightAlert>();

        alert.ToTable("alerts", "insights");

        context.ConfigureTenantEntity(alert);

        alert.Property(x => x.AlertKey).HasMaxLength(200);

        alert.Property(x => x.SourceType).HasMaxLength(100);

        alert.Property(x => x.Title).HasMaxLength(200);

        alert.Property(x => x.Message).HasMaxLength(2_000);

        alert.Property(x => x.AcknowledgedBy).HasMaxLength(240);

        alert.Property(x => x.ResolvedBy).HasMaxLength(240);

        alert.Property(x => x.Resolution).HasMaxLength(2_000);

        alert.Property(x => x.DismissedBy).HasMaxLength(240);

        alert.Property(x => x.DismissalReason).HasMaxLength(2_000);

        alert.HasIndex(x => new { x.TenantId, x.AlertKey, x.Status }).IsUnique().HasFilter("status IN (1,2)");

        alert.HasIndex(x => new { x.TenantId, x.FarmId, x.Status, x.Severity, x.DetectedAt });

        alert.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        alert.HasOne<FarmField>().WithMany().HasForeignKey(x => new { x.TenantId, x.FieldId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        alert.HasOne<FieldBoundaryVersion>().WithMany().HasForeignKey(x => new { x.TenantId, x.BoundaryVersionId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        alert.HasOne<WorkOrder>().WithMany().HasForeignKey(x => new { x.TenantId, x.FollowUpWorkOrderId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
