using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.EquipmentAsset;
using EdgIQ.MurimiOS.Domain.ConnectedDevice;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using EdgIQ.MurimiOS.Domain.LivestockHolding;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.ConnectedDevice;
using ConnectedDevice = global::EdgIQ.MurimiOS.Domain.ConnectedDevice.ConnectedDevice;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;
using LivestockHolding = global::EdgIQ.MurimiOS.Domain.LivestockHolding.LivestockHolding;
internal static class ConnectedDeviceConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var device = modelBuilder.Entity<ConnectedDevice>();

        device.ToTable("devices", "connected");

        context.ConfigureTenantEntity(device);

        device.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_connected_devices_tenant_id_id");

        device.Property(x => x.DeviceKey).HasMaxLength(160);

        device.Property(x => x.Name).HasMaxLength(200);

        device.Property(x => x.FirmwareVersion).HasMaxLength(80);

        device.HasIndex(x => new { x.TenantId, x.DeviceKey }).IsUnique();

        device.HasIndex(x => new { x.TenantId, x.FarmId, x.Status, x.Kind });

        device.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        device.HasOne<IntegrationConnection>().WithMany().HasForeignKey(x => new { x.TenantId, x.ConnectionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        device.HasOne<EquipmentAsset>().WithMany().HasForeignKey(x => new { x.TenantId, x.EquipmentId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        device.HasOne<FarmField>().WithMany().HasForeignKey(x => new { x.TenantId, x.FieldId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        device.HasOne<LivestockHolding>().WithMany().HasForeignKey(x => new { x.TenantId, x.HoldingId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
