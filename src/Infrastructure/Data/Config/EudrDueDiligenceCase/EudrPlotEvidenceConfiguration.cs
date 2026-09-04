using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;
using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.HarvestLot;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.EudrDueDiligenceCase;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;
using EudrPlotEvidence = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrPlotEvidence;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using HarvestLot = global::EdgIQ.MurimiOS.Domain.HarvestLot.HarvestLot;
internal static class EudrPlotEvidenceConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var plot = modelBuilder.Entity<EudrPlotEvidence>();

        plot.ToTable("plot_evidence", "eudr");

        context.ConfigureTenantEntity(plot);

        plot.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_eudr_plot_evidence_tenant_id_id");

        plot.Property(x => x.PlotReference).HasMaxLength(120);

        plot.Property(x => x.AreaHa).HasPrecision(14, 4);

        plot.Property(x => x.BoundaryGeoJson).HasColumnType("jsonb");

        plot.Property(x => x.BoundarySha256).HasMaxLength(64);

        plot.Property(x => x.CapturedBy).HasMaxLength(240);

        plot.HasIndex(x => new { x.TenantId, x.CaseId, x.PlotReference }).IsUnique();

        plot.HasIndex(x => new { x.TenantId, x.CaseId, x.BoundaryVersionId }).IsUnique();

        plot.HasIndex(x => new { x.TenantId, x.BoundarySha256 });

        plot.HasOne<EudrDueDiligenceCase>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.CaseId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        plot.HasOne<FarmField>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FieldId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        plot.HasOne<FieldBoundaryVersion>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.BoundaryVersionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        plot.HasOne<HarvestLot>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.HarvestLotId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
