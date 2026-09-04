using EdgIQ.MurimiOS.Domain.LivestockSubject;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockSubject;
using LivestockGroupMembership = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockGroupMembership;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
internal static class LivestockGroupMembershipConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var membership = modelBuilder.Entity<LivestockGroupMembership>();

        membership.ToTable("group_memberships", "livestock");

        context.ConfigureTenantEntity(membership);

        membership.HasIndex(x => new { x.TenantId, x.GroupId, x.MemberSubjectId, x.JoinedAt }).IsUnique();

        membership.HasIndex(x => new { x.TenantId, x.MemberSubjectId, x.JoinedAt, x.LeftAt });

        membership.HasOne<LivestockSubject>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.GroupId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        membership.HasOne<LivestockSubject>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.MemberSubjectId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
