using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.LegalCompany;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.FinanceTransaction;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;
using LegalCompany = global::EdgIQ.MurimiOS.Domain.LegalCompany.LegalCompany;
internal static class FinanceTransactionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var transaction = modelBuilder.Entity<FinanceTransaction>();

        transaction.ToTable("transactions", "finance");

        context.ConfigureTenantEntity(transaction);

        transaction.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_finance_transactions_tenant_id_id");

        transaction.Property(x => x.Category).HasMaxLength(120);

        transaction.Property(x => x.Amount).HasPrecision(18, 2);

        transaction.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        transaction.Property(x => x.Reference).HasMaxLength(160);

        transaction.Property(x => x.Counterparty).HasMaxLength(240);

        transaction.Property(x => x.IntercompanyReference).HasMaxLength(160);

        transaction.Property(x => x.Description).HasMaxLength(2_000);

        transaction.Property(x => x.PostedBy).HasMaxLength(240);

        transaction.Ignore(x => x.SignedAmount);

        transaction.HasIndex(x => new { x.TenantId, x.FarmId, x.OccurredAt });

        transaction.HasIndex(x => new { x.TenantId, x.FarmId, x.Reference }).IsUnique();

        transaction.HasIndex(x => new { x.TenantId, x.SourceType, x.SourceId }).IsUnique()
            .HasFilter("source_id IS NOT NULL");

        transaction.HasIndex(x => new { x.TenantId, x.ReversalOfId }).IsUnique()
            .HasFilter("reversal_of_id IS NOT NULL");

        transaction.HasIndex(x => new { x.TenantId, x.CounterpartyLegalCompanyId, x.IntercompanyReference, x.OccurredAt })
            .HasFilter("counterparty_legal_company_id IS NOT NULL");

        transaction.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        transaction.HasOne<FinanceTransaction>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.ReversalOfId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        transaction.HasOne<LegalCompany>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.CounterpartyLegalCompanyId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
