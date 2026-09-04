using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.FieldOperationRecord;
using EdgIQ.MurimiOS.Domain.CropZone;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.FieldOperationRecord;
using CropZone = global::EdgIQ.MurimiOS.Domain.CropZone.CropZone;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using FieldOperationRecord = global::EdgIQ.MurimiOS.Domain.FieldOperationRecord.FieldOperationRecord;
internal static class FieldOperationRecordConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var operation = modelBuilder.Entity<FieldOperationRecord>();

        operation.ToTable("field_operations", "operations");

        context.ConfigureTenantEntity(operation);

        operation.Property(x => x.OperationType).HasMaxLength(80);

        operation.Property(x => x.Notes).HasMaxLength(2_000);

        operation.Property(x => x.CorrectionReason).HasMaxLength(2_000);

        operation.HasIndex(x => new { x.TenantId, x.FieldId, x.OccurredAt });

        operation.HasIndex(x => new { x.TenantId, x.SourceCommandId }).IsUnique();

        operation.HasOne<FarmField>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FieldId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

        operation.HasOne<FieldBoundaryVersion>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.BoundaryVersionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

        operation.HasOne<CropZone>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.CropZoneId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

        operation.HasOne<FieldOperationRecord>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SupersededById })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
