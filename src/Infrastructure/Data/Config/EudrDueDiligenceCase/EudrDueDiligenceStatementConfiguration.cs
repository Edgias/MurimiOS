using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.EudrDueDiligenceCase;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;
using EudrDueDiligenceStatement = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceStatement;
internal static class EudrDueDiligenceStatementConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var statement = modelBuilder.Entity<EudrDueDiligenceStatement>();

        statement.ToTable("statements", "eudr");

        context.ConfigureTenantEntity(statement);

        statement.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_eudr_statements_tenant_id_id");

        statement.Property(x => x.SchemaVersion).HasMaxLength(80);

        statement.Property(x => x.PayloadJson).HasColumnType("jsonb");

        statement.Property(x => x.PayloadSha256).HasMaxLength(64);

        statement.Property(x => x.AssembledBy).HasMaxLength(240);

        statement.Property(x => x.SubmissionReference).HasMaxLength(200);

        statement.HasIndex(x => new { x.TenantId, x.CaseId, x.StatementVersion }).IsUnique();

        statement.HasIndex(x => new { x.TenantId, x.PayloadSha256 });

        statement.HasOne<EudrDueDiligenceCase>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.CaseId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
