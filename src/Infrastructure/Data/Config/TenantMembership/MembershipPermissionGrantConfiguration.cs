using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.TenantMembership;
using MembershipPermissionGrant = global::EdgIQ.MurimiOS.Domain.TenantMembership.MembershipPermissionGrant;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;
internal static class MembershipPermissionGrantConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var grant = modelBuilder.Entity<MembershipPermissionGrant>();

        grant.ToTable("permission_grants", "access");

        context.ConfigureTenantEntity(grant);

        grant.Property(x => x.Permission).HasMaxLength(120);

        grant.HasIndex(x => new { x.TenantId, x.MembershipId, x.Permission }).IsUnique();

        grant.HasOne<TenantMembership>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.MembershipId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Cascade);

    }
}
