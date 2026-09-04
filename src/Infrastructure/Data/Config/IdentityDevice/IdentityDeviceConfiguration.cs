using EdgIQ.MurimiOS.Domain.IdentityDevice;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.IdentityDevice;
using IdentityDevice = global::EdgIQ.MurimiOS.Domain.IdentityDevice.IdentityDevice;
internal static class IdentityDeviceConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var device = modelBuilder.Entity<IdentityDevice>();

        device.ToTable("devices", "security"); 
context.ConfigureTenantEntity(device);

        device.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_security_devices_tenant_id_id");

        device.Property(x => x.DeviceKeyHash).HasMaxLength(64).IsFixedLength();

        device.Property(x => x.Subject).HasMaxLength(240);

        device.Property(x => x.Name).HasMaxLength(200);

        device.Property(x => x.Platform).HasMaxLength(80);

        device.Property(x => x.RevokedBy).HasMaxLength(240);

        device.Property(x => x.RevocationReason).HasMaxLength(1_000);

        device.HasIndex(x => new { x.TenantId, x.DeviceKeyHash }).IsUnique();

        device.HasIndex(x => new { x.TenantId, x.Subject, x.Status });

    }
}
