using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.ConnectedDevice;
using EdgIQ.MurimiOS.Domain.SensorReading;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SensorReading;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;

using ConnectedDevice = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDevice;
using SensorChannel = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.SensorChannel;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using IngestionRecord = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecord;
using SensorReading = global::EdgIQ.MurimiOS.Domain.SensorReading.SensorReading;
internal static class SensorReadingConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var reading = modelBuilder.Entity<SensorReading>();

        reading.ToTable("sensor_readings", "connected");

        context.ConfigureTenantEntity(reading);

        reading.HasKey(x => new { x.TenantId, x.Id, x.ObservedAt }).HasName("pk_sensor_readings");

        reading.Property(x => x.Value).HasPrecision(24, 8);

        reading.Property(x => x.MetricCode).HasMaxLength(120);

        reading.Property(x => x.Unit).HasMaxLength(40);

        reading.Property(x => x.ExternalReadingId).HasMaxLength(240);

        reading.Property(x => x.PayloadSha256).HasMaxLength(64).IsFixedLength();

        reading.HasIndex(x => new { x.TenantId, x.DeviceId, x.ChannelId, x.ExternalReadingId, x.ObservedAt }).IsUnique();

        reading.HasIndex(x => new { x.TenantId, x.FarmId, x.MetricCode, x.ObservedAt });

        reading.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        reading.HasOne<ConnectedDevice>().WithMany().HasForeignKey(x => new { x.TenantId, x.DeviceId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        reading.HasOne<SensorChannel>().WithMany().HasForeignKey(x => new { x.TenantId, x.ChannelId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        reading.HasOne<IngestionRecord>().WithMany().HasForeignKey(x => new { x.TenantId, x.IngestionRecordId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
