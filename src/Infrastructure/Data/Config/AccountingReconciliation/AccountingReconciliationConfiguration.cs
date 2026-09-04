using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using EdgIQ.MurimiOS.Domain.AccountingImportBatch;
using EdgIQ.MurimiOS.Domain.AccountingReconciliation;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.AccountingReconciliation;
using AccountingImportBatch = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingImportBatch;
using AccountingStatementLine = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingStatementLine;
using AccountingReconciliation = global::EdgIQ.MurimiOS.Domain.AccountingReconciliation.AccountingReconciliation;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;
internal static class AccountingReconciliationConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var reconciliation = modelBuilder.Entity<AccountingReconciliation>();

        reconciliation.ToTable("accounting_reconciliations", "finance");

        context.ConfigureTenantEntity(reconciliation);

        reconciliation.Property(x => x.Difference).HasPrecision(18, 2);

        reconciliation.Property(x => x.Tolerance).HasPrecision(18, 2);

        reconciliation.Property(x => x.MatchedBy).HasMaxLength(240);

        reconciliation.Property(x => x.Notes).HasMaxLength(1_000);

        reconciliation.HasIndex(x => new { x.TenantId, x.BatchId, x.StatementLineId }).IsUnique();

        reconciliation.HasIndex(x => new { x.TenantId, x.TransactionId }).IsUnique()
            .HasFilter("transaction_id IS NOT NULL");

        reconciliation.HasOne<AccountingImportBatch>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.BatchId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        reconciliation.HasOne<AccountingStatementLine>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.BatchId, x.StatementLineId })
            .HasPrincipalKey(x => new { x.TenantId, x.BatchId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        reconciliation.HasOne<FinanceTransaction>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.TransactionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
