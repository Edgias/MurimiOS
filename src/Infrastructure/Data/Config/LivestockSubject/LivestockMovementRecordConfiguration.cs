using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockHolding;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockSubject;
using LivestockHolding = global::EdgIQ.MurimiOS.Domain.LivestockHolding.LivestockHolding;
using LivestockMovementRecord = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockMovementRecord;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
internal static class LivestockMovementRecordConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var movement = modelBuilder.Entity<LivestockMovementRecord>();

        movement.ToTable("movements", "livestock");

        context.ConfigureTenantEntity(movement);

        movement.Property(x => x.Reference).HasMaxLength(160);

        movement.Property(x => x.Notes).HasMaxLength(2_000);

        movement.HasIndex(x => new { x.TenantId, x.SubjectId, x.OccurredAt });

        movement.HasIndex(x => new { x.TenantId, x.SourceCommandId }).IsUnique().HasFilter("source_command_id IS NOT NULL");

        movement.HasOne<LivestockSubject>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SubjectId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        movement.HasOne<LivestockHolding>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FromHoldingId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        movement.HasOne<LivestockHolding>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.ToHoldingId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
