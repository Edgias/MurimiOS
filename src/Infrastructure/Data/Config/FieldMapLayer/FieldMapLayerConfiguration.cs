using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.FieldMapLayer;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.FieldMapLayer;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using FieldMapLayer = global::EdgIQ.MurimiOS.Domain.FieldMapLayer.FieldMapLayer;
internal static class FieldMapLayerConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var layer = modelBuilder.Entity<FieldMapLayer>();

        layer.ToTable("map_layers", "insights");

        context.ConfigureTenantEntity(layer);

        layer.Property(x => x.Name).HasMaxLength(200);

        layer.Property(x => x.SourceType).HasMaxLength(100);

        layer.Property(x => x.GeoJson).HasColumnType("jsonb");

        layer.Property(x => x.StyleJson).HasColumnType("jsonb");

        layer.HasIndex(x => new { x.TenantId, x.SourceType, x.SourceId, x.Kind }).IsUnique();

        layer.HasIndex(x => new { x.TenantId, x.FieldId, x.Kind, x.ValidFrom });

        layer.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        layer.HasOne<FarmField>().WithMany().HasForeignKey(x => new { x.TenantId, x.FieldId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        layer.HasOne<FieldBoundaryVersion>().WithMany().HasForeignKey(x => new { x.TenantId, x.BoundaryVersionId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
