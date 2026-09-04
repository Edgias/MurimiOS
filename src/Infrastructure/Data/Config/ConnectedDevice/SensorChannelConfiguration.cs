using EdgIQ.MurimiOS.Domain.ConnectedDevice;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.ConnectedDevice;
using ConnectedDevice = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDevice;
using SensorChannel = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.SensorChannel;
internal static class SensorChannelConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var channel = modelBuilder.Entity<SensorChannel>();

        channel.ToTable("sensor_channels", "connected");

        context.ConfigureTenantEntity(channel);

        channel.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_sensor_channels_tenant_id_id");

        channel.Property(x => x.ChannelKey).HasMaxLength(120);

        channel.Property(x => x.MetricCode).HasMaxLength(120);

        channel.Property(x => x.Unit).HasMaxLength(40);

        channel.Property(x => x.MinimumValidValue).HasPrecision(24, 8);

        channel.Property(x => x.MaximumValidValue).HasPrecision(24, 8);

        channel.HasIndex(x => new { x.TenantId, x.DeviceId, x.ChannelKey }).IsUnique();

        channel.HasIndex(x => new { x.TenantId, x.MetricCode, x.IsActive });

        channel.HasOne<ConnectedDevice>().WithMany().HasForeignKey(x => new { x.TenantId, x.DeviceId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
