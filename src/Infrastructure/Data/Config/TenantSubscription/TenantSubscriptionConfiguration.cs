using EdgIQ.MurimiOS.Domain.SaasPlanVersion;
using EdgIQ.MurimiOS.Domain.TenantSubscription;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.TenantSubscription;
using SaasPlanVersion = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanVersion;
using TenantSubscription = global::EdgIQ.MurimiOS.Domain.TenantSubscription.TenantSubscription;
internal static class TenantSubscriptionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var subscription = modelBuilder.Entity<TenantSubscription>();

        subscription.ToTable("subscriptions", "saas"); 
context.ConfigureTenantEntity(subscription);

        subscription.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_saas_subscriptions_tenant_id_id");

        subscription.Property(x => x.ProviderSubscriptionReference).HasMaxLength(200);

        subscription.Property(x => x.CreatedBy).HasMaxLength(240);

        subscription.Property(x => x.StatusReason).HasMaxLength(1_000);

        subscription.HasIndex(x => new { x.TenantId, x.Status });

        subscription.HasIndex(x => new { x.TenantId, x.ProviderSubscriptionReference }).IsUnique()
            .HasFilter("provider_subscription_reference IS NOT NULL");

        subscription.HasOne<SaasPlanVersion>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.PlanVersionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
