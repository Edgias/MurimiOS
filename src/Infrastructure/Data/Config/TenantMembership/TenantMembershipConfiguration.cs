using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.TenantMembership;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;
internal static class TenantMembershipConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var membership = modelBuilder.Entity<TenantMembership>();

        membership.ToTable("memberships", "access");

        context.ConfigureTenantEntity(membership);

        membership.Property(x => x.ExternalSubject).HasMaxLength(240);

        membership.Property(x => x.DisplayName).HasMaxLength(200);

        membership.Property(x => x.Email).HasMaxLength(320);

        membership.HasIndex(x => new { x.TenantId, x.ExternalSubject }).IsUnique();

        membership.HasIndex(x => new { x.TenantId, x.Status, x.Role });

        membership.HasOne<OrganisationNode>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.ScopeOrganisationNodeId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
