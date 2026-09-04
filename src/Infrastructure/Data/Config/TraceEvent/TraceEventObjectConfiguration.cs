using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.TraceEvent;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;
using TraceEventObject = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventObject;
internal static class TraceEventObjectConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var traceObject = modelBuilder.Entity<TraceEventObject>();

        traceObject.ToTable("event_objects", "traceability");

        context.ConfigureTenantEntity(traceObject);

        traceObject.Property(x => x.Quantity).HasPrecision(18, 4);

        traceObject.Property(x => x.Unit).HasMaxLength(30);

        traceObject.Property(x => x.EnteredQuantity).HasPrecision(18, 4);

        traceObject.Property(x => x.EnteredUnit).HasMaxLength(30);

        traceObject.HasIndex(x => new { x.TenantId, x.EventId, x.ObjectType, x.ObjectId, x.Role }).IsUnique();

        traceObject.HasIndex(x => new { x.TenantId, x.ObjectType, x.ObjectId });

        traceObject.HasOne<TraceEvent>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.EventId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
