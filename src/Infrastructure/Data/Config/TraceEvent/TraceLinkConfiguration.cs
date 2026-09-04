using EdgIQ.MurimiOS.Domain.TraceEvent;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.TraceEvent;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;
using TraceLink = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceLink;
internal static class TraceLinkConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var traceLink = modelBuilder.Entity<TraceLink>();

        traceLink.ToTable("links", "traceability");

        context.ConfigureTenantEntity(traceLink);

        traceLink.HasIndex(x => new { x.TenantId, x.EventId, x.FromType, x.FromId, x.ToType, x.ToId }).IsUnique();

        traceLink.HasIndex(x => new { x.TenantId, x.FromType, x.FromId });

        traceLink.HasIndex(x => new { x.TenantId, x.ToType, x.ToId });

        traceLink.HasOne<TraceEvent>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.EventId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
