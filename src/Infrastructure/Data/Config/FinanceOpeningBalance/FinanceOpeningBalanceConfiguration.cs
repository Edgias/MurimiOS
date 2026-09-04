using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.FinanceOpeningBalance;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.FinanceOpeningBalance;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FinanceOpeningBalance = global::EdgIQ.MurimiOS.Domain.FinanceOpeningBalance.FinanceOpeningBalance;
internal static class FinanceOpeningBalanceConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var openingBalance = modelBuilder.Entity<FinanceOpeningBalance>();

        openingBalance.ToTable("opening_balances", "finance");

        context.ConfigureTenantEntity(openingBalance);

        openingBalance.Property(x => x.AccountCode).HasMaxLength(80);

        openingBalance.Property(x => x.AccountName).HasMaxLength(200);

        openingBalance.Property(x => x.Amount).HasPrecision(18, 2);

        openingBalance.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        openingBalance.Property(x => x.Reference).HasMaxLength(160);

        openingBalance.Property(x => x.ImportedBy).HasMaxLength(240);

        openingBalance.HasIndex(x => new { x.TenantId, x.FarmId, x.AccountCode, x.AsOf }).IsUnique();

        openingBalance.HasIndex(x => new { x.TenantId, x.FarmId, x.AsOf });

        openingBalance.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
