using EdgIQ.MurimiOS.Domain.WebhookSubscription;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.WebhookSubscription;
using WebhookSubscription = global::EdgIQ.MurimiOS.Domain.WebhookSubscription.WebhookSubscription;
internal static class WebhookSubscriptionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var webhook = modelBuilder.Entity<WebhookSubscription>();

        webhook.ToTable("webhook_subscriptions", "external_api"); 
context.ConfigureTenantEntity(webhook);

        webhook.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_webhook_subscriptions_tenant_id_id");

        webhook.Property(x => x.Name).HasMaxLength(200); 
webhook.Property(x => x.Endpoint).HasMaxLength(2048);

        webhook.Property(x => x.EventTypes).HasColumnType("text[]"); 
webhook.Property(x => x.SigningKeyHash).HasMaxLength(64).IsFixedLength();

        webhook.Property(x => x.CreatedBy).HasMaxLength(240); 
webhook.HasIndex(x => new { x.TenantId, x.Status });

    }
}
