using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockMeasurementRecord;
using LivestockMeasurementRecord = global::EdgIQ.MurimiOS.Domain.LivestockMeasurementRecord.LivestockMeasurementRecord;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
internal static class LivestockMeasurementRecordConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var measurement = modelBuilder.Entity<LivestockMeasurementRecord>();

        measurement.ToTable("measurements", "livestock");

        context.ConfigureTenantEntity(measurement);

        measurement.Property(x => x.Metric).HasMaxLength(80);

        measurement.Property(x => x.Unit).HasMaxLength(30);

        measurement.Property(x => x.Value).HasPrecision(18, 4);

        measurement.Property(x => x.EnteredValue).HasPrecision(18, 4);

        measurement.Property(x => x.EnteredUnit).HasMaxLength(30);

        measurement.Property(x => x.DisplayUnit).HasMaxLength(30);

        measurement.Property(x => x.CorrectionReason).HasMaxLength(2_000);

        measurement.HasIndex(x => new { x.TenantId, x.SubjectId, x.OccurredAt });

        measurement.HasIndex(x => new { x.TenantId, x.SourceCommandId }).IsUnique();

        measurement.HasOne<LivestockSubject>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SubjectId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

        measurement.HasOne<LivestockMeasurementRecord>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SupersededById })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
