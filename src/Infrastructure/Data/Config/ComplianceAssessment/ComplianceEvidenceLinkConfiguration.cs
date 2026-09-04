using EdgIQ.MurimiOS.Domain.ComplianceAssessment;
using EdgIQ.MurimiOS.Domain.EvidenceAttachment;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.ComplianceAssessment;
using ComplianceAssessment = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceAssessment;

using ComplianceControlEvaluation = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceControlEvaluation;
using ComplianceEvidenceLink = global::EdgIQ.MurimiOS.Domain.ComplianceAssessment.ComplianceEvidenceLink;
using EvidenceAttachment = global::EdgIQ.MurimiOS.Domain.EvidenceAttachment.EvidenceAttachment;
internal static class ComplianceEvidenceLinkConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var evidenceLink = modelBuilder.Entity<ComplianceEvidenceLink>();

        evidenceLink.ToTable("evidence_links", "compliance");

        context.ConfigureTenantEntity(evidenceLink);

        evidenceLink.Property(x => x.LinkedBy).HasMaxLength(240);

        evidenceLink.HasIndex(x => new { x.TenantId, x.EvaluationId, x.EvidenceAttachmentId }).IsUnique();

        evidenceLink.HasOne<ComplianceControlEvaluation>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.EvaluationId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        evidenceLink.HasOne<EvidenceAttachment>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.EvidenceAttachmentId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
