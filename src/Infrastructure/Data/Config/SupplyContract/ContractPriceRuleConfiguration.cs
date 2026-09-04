using EdgIQ.MurimiOS.Domain.SupplyContract;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SupplyContract;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using ContractPriceRule = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractPriceRule;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;
internal static class ContractPriceRuleConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var priceRule = modelBuilder.Entity<ContractPriceRule>();

        priceRule.ToTable("contract_price_rules", "supply");

        context.ConfigureTenantEntity(priceRule);

        priceRule.Property(x => x.MinimumQuantity).HasPrecision(18, 4);

        priceRule.Property(x => x.MaximumQuantity).HasPrecision(18, 4);

        priceRule.Property(x => x.QualityGrade).HasMaxLength(80);

        priceRule.Property(x => x.UnitPrice).HasPrecision(18, 6);

        priceRule.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        priceRule.HasIndex(x => new { x.TenantId, x.ContractId, x.Priority }).IsUnique();

        priceRule.HasIndex(x => new { x.TenantId, x.ContractId, x.EffectiveFrom, x.EffectiveTo });

        priceRule.HasOne<SupplyContract>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.ContractId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
