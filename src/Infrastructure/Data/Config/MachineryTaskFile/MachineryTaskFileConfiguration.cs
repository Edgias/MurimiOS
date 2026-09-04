using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.EquipmentAsset;
using EdgIQ.MurimiOS.Domain.MachineryTaskFile;
using EdgIQ.MurimiOS.Domain.WorkOrder;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.MachineryTaskFile;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using MachineryTaskFile = global::EdgIQ.MurimiOS.Domain.MachineryTaskFile.MachineryTaskFile;
using WorkOrder = global::EdgIQ.MurimiOS.Domain.WorkOrder.WorkOrder;
internal static class MachineryTaskFileConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var taskFile = modelBuilder.Entity<MachineryTaskFile>();

        taskFile.ToTable("machinery_task_files", "connected");

        context.ConfigureTenantEntity(taskFile);

        taskFile.Property(x => x.FileName).HasMaxLength(255);

        taskFile.Property(x => x.ObjectKey).HasMaxLength(500);

        taskFile.Property(x => x.Sha256).HasMaxLength(64).IsFixedLength();

        taskFile.Property(x => x.MappingVersion).HasMaxLength(60);

        taskFile.Property(x => x.ExternalTaskId).HasMaxLength(240);

        taskFile.Property(x => x.RejectionReason).HasMaxLength(2_000);

        taskFile.HasIndex(x => new { x.TenantId, x.FarmId, x.Status, x.CreatedAt });

        taskFile.HasIndex(x => new { x.TenantId, x.EquipmentId, x.Direction, x.ExternalTaskId }).IsUnique().HasFilter("external_task_id IS NOT NULL");

        taskFile.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        taskFile.HasOne<EquipmentAsset>().WithMany().HasForeignKey(x => new { x.TenantId, x.EquipmentId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        taskFile.HasOne<WorkOrder>().WithMany().HasForeignKey(x => new { x.TenantId, x.WorkOrderId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        taskFile.HasOne<FarmField>().WithMany().HasForeignKey(x => new { x.TenantId, x.FieldId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
