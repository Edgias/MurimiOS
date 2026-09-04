using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.TraceEvent;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;
internal static class TraceEventConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var traceEvent = modelBuilder.Entity<TraceEvent>();

        traceEvent.ToTable("events", "traceability");

        context.ConfigureTenantEntity(traceEvent);

        traceEvent.Property(x => x.BusinessStep).HasMaxLength(120);

        traceEvent.Property(x => x.Disposition).HasMaxLength(120);

        traceEvent.Property(x => x.LocationType).HasMaxLength(80);

        traceEvent.Property(x => x.SourceType).HasMaxLength(120);

        traceEvent.Property(x => x.TransactionReference).HasMaxLength(160);

        traceEvent.Property(x => x.PayloadJson).HasColumnType("jsonb");

        traceEvent.HasIndex(x => new { x.TenantId, x.SourceType, x.SourceId }).IsUnique();

        traceEvent.HasIndex(x => new { x.TenantId, x.FarmId, x.OccurredAt });

        traceEvent.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
