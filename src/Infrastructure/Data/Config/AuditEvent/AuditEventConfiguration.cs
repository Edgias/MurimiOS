using EdgIQ.MurimiOS.Domain.AuditEvent;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.AuditEvent;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
internal static class AuditEventConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var audit = modelBuilder.Entity<AuditEvent>();

        audit.ToTable("events", "audit");

        audit.HasKey(x => x.Id);

        audit.Property(x => x.Id).ValueGeneratedNever();

        audit.Property(x => x.ActorSubject).HasMaxLength(240);

        audit.Property(x => x.Action).HasMaxLength(40);

        audit.Property(x => x.EntityType).HasMaxLength(240);

        audit.Property(x => x.Changes).HasColumnType("jsonb");

        audit.Property(x => x.CorrelationId).HasMaxLength(160);

        audit.HasIndex(x => new { x.TenantId, x.OccurredAt });

        audit.HasIndex(x => new { x.TenantId, x.EntityType, x.EntityId, x.OccurredAt });

        audit.HasIndex(x => new { x.TenantId, x.FarmId, x.OccurredAt });

        audit.HasIndex(x => new { x.TenantId, x.OrganisationNodeId, x.OccurredAt });

        context.ConfigureAuditEventFilter(audit);

    }
}
