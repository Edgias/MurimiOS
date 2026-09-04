using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.EquipmentAsset;
using EdgIQ.MurimiOS.Domain.ConnectedDevice;
using EdgIQ.MurimiOS.Domain.MachineryTelemetryRecord;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.MachineryTelemetryRecord;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;

using ConnectedDevice = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDevice;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using IngestionRecord = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecord;
using MachineryTelemetryRecord = global::EdgIQ.MurimiOS.Domain.MachineryTelemetryRecord.MachineryTelemetryRecord;
internal static class MachineryTelemetryRecordConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var telemetry = modelBuilder.Entity<MachineryTelemetryRecord>();

        telemetry.ToTable("machinery_telemetry", "connected");

        context.ConfigureTenantEntity(telemetry);

        telemetry.HasKey(x => new { x.TenantId, x.Id, x.ObservedAt }).HasName("pk_machinery_telemetry");

        telemetry.Property(x => x.EngineHours).HasPrecision(18, 4);

        telemetry.Property(x => x.SpeedKph).HasPrecision(18, 4);

        telemetry.Property(x => x.FuelLevelPercent).HasPrecision(8, 4);

        telemetry.Property(x => x.State).HasMaxLength(80);

        telemetry.Property(x => x.ExternalRecordId).HasMaxLength(240);

        telemetry.Property(x => x.PayloadSha256).HasMaxLength(64).IsFixedLength();

        telemetry.HasIndex(x => new { x.TenantId, x.EquipmentId, x.ExternalRecordId, x.ObservedAt }).IsUnique();

        telemetry.HasIndex(x => new { x.TenantId, x.FarmId, x.ObservedAt });

        telemetry.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        telemetry.HasOne<EquipmentAsset>().WithMany().HasForeignKey(x => new { x.TenantId, x.EquipmentId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        telemetry.HasOne<ConnectedDevice>().WithMany().HasForeignKey(x => new { x.TenantId, x.DeviceId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        telemetry.HasOne<IngestionRecord>().WithMany().HasForeignKey(x => new { x.TenantId, x.IngestionRecordId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
