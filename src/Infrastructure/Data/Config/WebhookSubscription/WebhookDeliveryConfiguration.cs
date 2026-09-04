using EdgIQ.MurimiOS.Domain.WebhookSubscription;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.WebhookSubscription;
using WebhookDelivery = global::EdgIQ.MurimiOS.Domain.WebhookSubscription.WebhookDelivery;
using WebhookSubscription = global::EdgIQ.MurimiOS.Domain.WebhookSubscription.WebhookSubscription;
internal static class WebhookDeliveryConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var delivery = modelBuilder.Entity<WebhookDelivery>();

        delivery.ToTable("webhook_deliveries", "external_api"); 
context.ConfigureTenantEntity(delivery);

        delivery.Property(x => x.EventType).HasMaxLength(200); 
delivery.Property(x => x.Payload).HasColumnType("jsonb");

        delivery.Property(x => x.ResponseBodyExcerpt).HasMaxLength(2000); 
delivery.Property(x => x.LastError).HasMaxLength(2000);

        delivery.HasIndex(x => new { x.TenantId, x.SubscriptionId, x.OutboxMessageId }).IsUnique();

        delivery.HasIndex(x => new { x.Status, x.AvailableAt, x.LeasedUntil });

        delivery.HasOne<WebhookSubscription>().WithMany().HasForeignKey(x => new { x.TenantId, x.SubscriptionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
