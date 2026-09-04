using EdgIQ.MurimiOS.Domain.DataGrant;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.DataGrant;
using CrossTenantAccessAudit = global::EdgIQ.MurimiOS.Domain.DataGrant.CrossTenantAccessAudit;
using DataGrant = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrant;
internal static class CrossTenantAccessAuditConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var accessAudit = modelBuilder.Entity<CrossTenantAccessAudit>();

        accessAudit.ToTable("access_audit", "supply");

        accessAudit.HasKey(x => x.Id);

        accessAudit.Property(x => x.Id).ValueGeneratedNever();

        accessAudit.Property(x => x.ActorSubject).HasMaxLength(240);

        accessAudit.Property(x => x.Purpose).HasMaxLength(240);

        accessAudit.Property(x => x.EntityCategory).HasMaxLength(120);

        accessAudit.Property(x => x.Outcome).HasMaxLength(40);

        accessAudit.Property(x => x.CorrelationId).HasMaxLength(160);

        accessAudit.HasIndex(x => new { x.ProducerTenantId, x.OccurredAt });

        accessAudit.HasIndex(x => new { x.RecipientTenantId, x.OccurredAt });

        accessAudit.HasIndex(x => new { x.GrantId, x.OccurredAt });

        accessAudit.HasOne<DataGrant>().WithMany().HasForeignKey(x => x.GrantId).OnDelete(DeleteBehavior.Restrict);

    }
}
