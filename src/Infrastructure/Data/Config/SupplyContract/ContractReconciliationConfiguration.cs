using EdgIQ.MurimiOS.Domain.SupplyContract;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SupplyContract;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using ContractPriceRule = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractPriceRule;
using ContractReconciliation = global::EdgIQ.MurimiOS.Domain.SupplyContract.ContractReconciliation;
using SupplyContract = global::EdgIQ.MurimiOS.Domain.SupplyContract.SupplyContract;
internal static class ContractReconciliationConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var reconciliation = modelBuilder.Entity<ContractReconciliation>();

        reconciliation.ToTable("contract_reconciliations", "supply");

        context.ConfigureTenantEntity(reconciliation);

        reconciliation.Property(x => x.Reference).HasMaxLength(120);

        reconciliation.Property(x => x.FulfilledQuantity).HasPrecision(18, 4);

        reconciliation.Property(x => x.AcceptedQuantity).HasPrecision(18, 4);

        reconciliation.Property(x => x.RejectedQuantity).HasPrecision(18, 4);

        reconciliation.Property(x => x.QuantityUnit).HasMaxLength(30);

        reconciliation.Property(x => x.UnitPrice).HasPrecision(18, 6);

        reconciliation.Property(x => x.GrossAmount).HasPrecision(18, 2);

        reconciliation.Property(x => x.QualityAdjustmentAmount).HasPrecision(18, 2);

        reconciliation.Property(x => x.OtherAdjustmentAmount).HasPrecision(18, 2);

        reconciliation.Property(x => x.PayableAmount).HasPrecision(18, 2);

        reconciliation.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        reconciliation.Property(x => x.Notes).HasMaxLength(2_000);

        reconciliation.Property(x => x.ReconciledBy).HasMaxLength(240);

        reconciliation.HasIndex(x => new { x.TenantId, x.ContractId, x.Revision }).IsUnique();

        reconciliation.HasIndex(x => new { x.TenantId, x.ContractId, x.Reference }).IsUnique();

        reconciliation.HasOne<SupplyContract>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.ContractId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        reconciliation.HasOne<ContractPriceRule>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.PriceRuleId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
