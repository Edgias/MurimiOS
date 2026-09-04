using EdgIQ.MurimiOS.Domain.BillingAccount;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.BillingAccount;
using BillingAccount = global::EdgIQ.MurimiOS.Domain.BillingAccount.BillingAccount;
internal static class BillingAccountConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var billingAccount = modelBuilder.Entity<BillingAccount>();

        billingAccount.ToTable("billing_accounts", "saas"); 
context.ConfigureTenantEntity(billingAccount);

        billingAccount.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_saas_billing_accounts_tenant_id_id");

        billingAccount.Property(x => x.Provider).HasMaxLength(80);

        billingAccount.Property(x => x.ProviderCustomerReference).HasMaxLength(200);

        billingAccount.Property(x => x.BillingEmail).HasMaxLength(320);

        billingAccount.Property(x => x.TaxIdentifier).HasMaxLength(120);

        billingAccount.HasIndex(x => x.TenantId).IsUnique().HasFilter("status <> 3");

        billingAccount.HasIndex(x => new { x.Provider, x.ProviderCustomerReference }).IsUnique();

    }
}
