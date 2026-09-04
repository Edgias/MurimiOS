using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.NotificationPreference;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.NotificationPreference;
using NotificationPreference = global::EdgIQ.MurimiOS.Domain.NotificationPreference.NotificationPreference;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;
internal static class NotificationPreferenceConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var preference = modelBuilder.Entity<NotificationPreference>();

        preference.ToTable("preferences", "notifications");

        context.ConfigureTenantEntity(preference);

        preference.Property(x => x.Category).HasMaxLength(120);

        preference.HasIndex(x => new { x.TenantId, x.MembershipId, x.Category }).IsUnique();

        preference.HasOne<TenantMembership>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.MembershipId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Cascade);

    }
}
