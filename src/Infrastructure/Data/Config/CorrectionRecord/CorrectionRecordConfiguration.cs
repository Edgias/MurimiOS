using EdgIQ.MurimiOS.Domain.CorrectionRecord;
using EdgIQ.MurimiOS.Domain.Farm;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.CorrectionRecord;
using CorrectionRecord = global::EdgIQ.MurimiOS.Domain.CorrectionRecord.CorrectionRecord;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
internal static class CorrectionRecordConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var correction = modelBuilder.Entity<CorrectionRecord>();

        correction.ToTable("corrections", "audit");

        context.ConfigureTenantEntity(correction);

        correction.Property(x => x.EntityType).HasMaxLength(240);

        correction.Property(x => x.Reason).HasMaxLength(2_000);

        correction.Property(x => x.CorrectedBy).HasMaxLength(240);

        correction.HasIndex(x => new { x.TenantId, x.EntityType, x.OriginalEntityId }).IsUnique();

        correction.HasIndex(x => new { x.TenantId, x.EntityType, x.ReplacementEntityId }).IsUnique();

        correction.HasIndex(x => new { x.TenantId, x.FarmId, x.CorrectedAt });

        correction.HasOne<Farm>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
