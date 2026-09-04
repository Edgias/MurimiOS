using EdgIQ.MurimiOS.Domain.TenantAccount;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.TenantAccount;
using TenantAccount = global::EdgIQ.MurimiOS.Domain.TenantAccount.TenantAccount;
internal static class TenantAccountConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var account = modelBuilder.Entity<TenantAccount>();

        account.ToTable("tenant_accounts", "saas"); 
context.ConfigureTenantEntity(account);

        account.Property(x => x.LegalName).HasMaxLength(240);

        account.Property(x => x.DisplayName).HasMaxLength(200);

        account.Property(x => x.Slug).HasMaxLength(80);

        account.Property(x => x.CountryCode).HasMaxLength(2).IsFixedLength();

        account.Property(x => x.BillingCurrency).HasMaxLength(3).IsFixedLength();

        account.Property(x => x.TimeZone).HasMaxLength(80);

        account.Property(x => x.ProvisionedBy).HasMaxLength(240);

        account.Property(x => x.SuspensionReason).HasMaxLength(1_000);

        account.HasIndex(x => x.TenantId).IsUnique();

        account.HasIndex(x => x.Slug).IsUnique();

    }
}
