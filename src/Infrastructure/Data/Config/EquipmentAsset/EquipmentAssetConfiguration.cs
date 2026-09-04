using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.EquipmentAsset;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.EquipmentAsset;
using EquipmentAsset = global::EdgIQ.MurimiOS.Domain.EquipmentAsset.EquipmentAsset;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
internal static class EquipmentAssetConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var equipment = modelBuilder.Entity<EquipmentAsset>();

        equipment.ToTable("equipment", "connected");

        context.ConfigureTenantEntity(equipment);

        equipment.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_connected_equipment_tenant_id_id");

        equipment.Property(x => x.Code).HasMaxLength(60);

        equipment.Property(x => x.Name).HasMaxLength(200);

        equipment.Property(x => x.Manufacturer).HasMaxLength(120);

        equipment.Property(x => x.Model).HasMaxLength(120);

        equipment.Property(x => x.SerialNumber).HasMaxLength(160);

        equipment.HasIndex(x => new { x.TenantId, x.FarmId, x.Code }).IsUnique();

        equipment.HasIndex(x => new { x.TenantId, x.FarmId, x.Status, x.Type });

        equipment.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
