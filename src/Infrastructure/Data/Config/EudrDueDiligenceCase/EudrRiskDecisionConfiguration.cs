using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.EudrDueDiligenceCase;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;
using EudrRiskDecision = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrRiskDecision;
internal static class EudrRiskDecisionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var riskDecision = modelBuilder.Entity<EudrRiskDecision>();

        riskDecision.ToTable("risk_decisions", "eudr");

        context.ConfigureTenantEntity(riskDecision);

        riskDecision.Property(x => x.Rationale).HasMaxLength(8_000);

        riskDecision.Property(x => x.MitigationJson).HasColumnType("jsonb");

        riskDecision.Property(x => x.LegalityEvidenceJson).HasColumnType("jsonb");

        riskDecision.Property(x => x.EvidenceSnapshotJson).HasColumnType("jsonb");

        riskDecision.Property(x => x.EvidenceSha256).HasMaxLength(64);

        riskDecision.Property(x => x.DecidedBy).HasMaxLength(240);

        riskDecision.HasIndex(x => new { x.TenantId, x.CaseId, x.Revision }).IsUnique();

        riskDecision.HasOne<EudrDueDiligenceCase>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.CaseId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
