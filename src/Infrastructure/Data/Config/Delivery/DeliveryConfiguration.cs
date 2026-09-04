using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.Delivery;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.Delivery;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
internal static class DeliveryConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var delivery = modelBuilder.Entity<Delivery>();

        delivery.ToTable("deliveries", "traceability");

        context.ConfigureTenantEntity(delivery);

        delivery.Property(x => x.DeliveryNumber).HasMaxLength(120);

        delivery.Property(x => x.Recipient).HasMaxLength(240);

        delivery.Property(x => x.Destination).HasMaxLength(240);

        delivery.Property(x => x.TransactionReference).HasMaxLength(160);

        delivery.HasIndex(x => new { x.TenantId, x.DeliveryNumber }).IsUnique();

        delivery.HasIndex(x => new { x.TenantId, x.FarmId, x.DispatchedAt });

        delivery.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
