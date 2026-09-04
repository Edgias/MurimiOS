using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.EudrDueDiligenceCase;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;

using EudrDueDiligenceStatement = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceStatement;
using EudrStatementExport = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrStatementExport;
internal static class EudrStatementExportConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var export = modelBuilder.Entity<EudrStatementExport>();

        export.ToTable("statement_exports", "eudr");

        context.ConfigureTenantEntity(export);

        export.Property(x => x.Format).HasMaxLength(20);

        export.Property(x => x.Sha256).HasMaxLength(64);

        export.Property(x => x.ExportedBy).HasMaxLength(240);

        export.HasIndex(x => new { x.TenantId, x.StatementId, x.ExportedAt });

        export.HasOne<EudrDueDiligenceStatement>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.StatementId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
