using EdgIQ.MurimiOS.Domain.LivestockSubject;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockSubject;
using LivestockHealthEvent = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockHealthEvent;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
internal static class LivestockHealthEventConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var healthEvent = modelBuilder.Entity<LivestockHealthEvent>();

        healthEvent.ToTable("health_events", "livestock");

        context.ConfigureTenantEntity(healthEvent);

        healthEvent.Property(x => x.Title).HasMaxLength(200);

        healthEvent.Property(x => x.Diagnosis).HasMaxLength(500);

        healthEvent.Property(x => x.Practitioner).HasMaxLength(200);

        healthEvent.Property(x => x.Notes).HasMaxLength(2_000);

        healthEvent.HasIndex(x => new { x.TenantId, x.SubjectId, x.OccurredAt });

        healthEvent.HasIndex(x => new { x.TenantId, x.SourceCommandId }).IsUnique().HasFilter("source_command_id IS NOT NULL");

        healthEvent.HasOne<LivestockSubject>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SubjectId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
