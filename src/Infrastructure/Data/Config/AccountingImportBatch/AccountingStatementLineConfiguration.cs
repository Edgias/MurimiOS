using EdgIQ.MurimiOS.Domain.AccountingImportBatch;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.AccountingImportBatch;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using AccountingImportBatch = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingImportBatch;
using AccountingStatementLine = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingStatementLine;
internal static class AccountingStatementLineConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var line = modelBuilder.Entity<AccountingStatementLine>();

        line.ToTable("accounting_statement_lines", "finance");

        context.ConfigureTenantEntity(line);

        line.HasAlternateKey(x => new { x.TenantId, x.BatchId, x.Id }).HasName("ak_finance_statement_lines_tenant_batch_id");

        line.Property(x => x.ExternalLineId).HasMaxLength(160);

        line.Property(x => x.Amount).HasPrecision(18, 2);

        line.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        line.Property(x => x.AccountCode).HasMaxLength(80);

        line.Property(x => x.Reference).HasMaxLength(160);

        line.Property(x => x.Description).HasMaxLength(1_000);

        line.HasIndex(x => new { x.TenantId, x.BatchId, x.ExternalLineId }).IsUnique();

        line.HasOne<AccountingImportBatch>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.BatchId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
