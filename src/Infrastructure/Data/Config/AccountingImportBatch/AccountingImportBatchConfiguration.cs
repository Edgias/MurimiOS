using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.AccountingImportBatch;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.AccountingImportBatch;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using AccountingImportBatch = global::EdgIQ.MurimiOS.Domain.AccountingImportBatch.AccountingImportBatch;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
internal static class AccountingImportBatchConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var import = modelBuilder.Entity<AccountingImportBatch>();

        import.ToTable("accounting_imports", "finance");

        context.ConfigureTenantEntity(import);

        import.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_finance_accounting_imports_tenant_id_id");

        import.Property(x => x.Provider).HasMaxLength(80);

        import.Property(x => x.ExternalBatchId).HasMaxLength(160);

        import.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        import.Property(x => x.ImportedBy).HasMaxLength(240);

        import.Property(x => x.ReconciledBy).HasMaxLength(240);

        import.HasIndex(x => new { x.TenantId, x.FarmId, x.Provider, x.ExternalBatchId }).IsUnique();

        import.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
