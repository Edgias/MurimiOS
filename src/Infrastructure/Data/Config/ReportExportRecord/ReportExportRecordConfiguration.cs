using EdgIQ.MurimiOS.Domain.CropZone;
using EdgIQ.MurimiOS.Domain.ReportExportRecord;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.ReportExportRecord;
using CropZone = global::EdgIQ.MurimiOS.Domain.CropZone.CropZone;
using ReportExportRecord = global::EdgIQ.MurimiOS.Domain.ReportExportRecord.ReportExportRecord;
internal static class ReportExportRecordConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var report = modelBuilder.Entity<ReportExportRecord>();

        report.ToTable("exports", "reporting");

        context.ConfigureTenantEntity(report);

        report.Property(x => x.ReportType).HasMaxLength(120);

        report.Property(x => x.Format).HasMaxLength(20);

        report.Property(x => x.Sha256).HasMaxLength(64);

        report.Property(x => x.RequestedBy).HasMaxLength(200);

        report.HasIndex(x => new { x.TenantId, x.CropZoneId, x.GeneratedAt });

        report.HasIndex(x => new { x.TenantId, x.Sha256 });

        report.HasOne<CropZone>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.CropZoneId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
