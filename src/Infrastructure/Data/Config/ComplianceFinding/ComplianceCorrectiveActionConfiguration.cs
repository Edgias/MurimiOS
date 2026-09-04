using EdgIQ.MurimiOS.Domain.ComplianceFinding;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.ComplianceFinding;
using ComplianceCorrectiveAction = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceCorrectiveAction;
using ComplianceFinding = global::EdgIQ.MurimiOS.Domain.ComplianceFinding.ComplianceFinding;
internal static class ComplianceCorrectiveActionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var action = modelBuilder.Entity<ComplianceCorrectiveAction>();

        action.ToTable("corrective_actions", "compliance");

        context.ConfigureTenantEntity(action);

        action.Property(x => x.Action).HasMaxLength(4_000);

        action.Property(x => x.Owner).HasMaxLength(240);

        action.Property(x => x.CompletionNotes).HasMaxLength(4_000);

        action.Property(x => x.VerifiedBy).HasMaxLength(240);

        action.Property(x => x.RejectionReason).HasMaxLength(2_000);

        action.HasIndex(x => new { x.TenantId, x.FindingId, x.Status });

        action.HasIndex(x => new { x.TenantId, x.Status, x.DueOn });

        action.HasOne<ComplianceFinding>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FindingId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
