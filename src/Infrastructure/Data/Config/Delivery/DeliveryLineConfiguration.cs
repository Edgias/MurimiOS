using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.Delivery;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.Delivery;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using DeliveryLine = global::EdgIQ.MurimiOS.Domain.Delivery.DeliveryLine;
internal static class DeliveryLineConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var deliveryLine = modelBuilder.Entity<DeliveryLine>();

        deliveryLine.ToTable("delivery_lines", "traceability");

        context.ConfigureTenantEntity(deliveryLine);

        deliveryLine.Property(x => x.Quantity).HasPrecision(18, 4);

        deliveryLine.Property(x => x.Unit).HasMaxLength(30);

        deliveryLine.Property(x => x.EnteredQuantity).HasPrecision(18, 4);

        deliveryLine.Property(x => x.EnteredUnit).HasMaxLength(30);

        deliveryLine.HasIndex(x => new { x.TenantId, x.DeliveryId, x.SourceType, x.SourceId }).IsUnique();

        deliveryLine.HasIndex(x => new { x.TenantId, x.SourceType, x.SourceId });

        deliveryLine.HasOne<Delivery>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.DeliveryId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
