using EdgIQ.MurimiOS.Domain.EvidenceAttachment;
using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.EudrDueDiligenceCase;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;

using EudrDeforestationScreening = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDeforestationScreening;
using EudrPlotEvidence = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrPlotEvidence;
using EvidenceAttachment = global::EdgIQ.MurimiOS.Domain.EvidenceAttachment.EvidenceAttachment;
internal static class EudrDeforestationScreeningConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var screening = modelBuilder.Entity<EudrDeforestationScreening>();

        screening.ToTable("deforestation_screenings", "eudr");

        context.ConfigureTenantEntity(screening);

        screening.Property(x => x.ProviderCode).HasMaxLength(80);

        screening.Property(x => x.DatasetName).HasMaxLength(200);

        screening.Property(x => x.DatasetVersion).HasMaxLength(80);

        screening.Property(x => x.CoveragePercent).HasPrecision(6, 3);

        screening.Property(x => x.TreeCoverLossHa).HasPrecision(14, 4);

        screening.Property(x => x.OverlapHa).HasPrecision(14, 4);

        screening.Property(x => x.ConfidencePercent).HasPrecision(6, 3);

        screening.Property(x => x.SourceReference).HasMaxLength(1_000);

        screening.Property(x => x.ResultJson).HasColumnType("jsonb");

        screening.Property(x => x.EvidenceSha256).HasMaxLength(64);

        screening.Property(x => x.ScreenedBy).HasMaxLength(240);

        screening.HasIndex(x => new { x.TenantId, x.PlotEvidenceId, x.ScreenedAt });

        screening.HasIndex(x => new { x.TenantId, x.EvidenceSha256 });

        screening.HasOne<EudrPlotEvidence>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.PlotEvidenceId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        screening.HasOne<EvidenceAttachment>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.EvidenceAttachmentId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
