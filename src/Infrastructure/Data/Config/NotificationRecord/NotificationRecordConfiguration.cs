using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.NotificationRecord;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.NotificationRecord;
using NotificationRecord = global::EdgIQ.MurimiOS.Domain.NotificationRecord.NotificationRecord;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;
internal static class NotificationRecordConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var notification = modelBuilder.Entity<NotificationRecord>();

        notification.ToTable("notifications", "notifications");

        context.ConfigureTenantEntity(notification);

        notification.Property(x => x.Category).HasMaxLength(120);

        notification.Property(x => x.Title).HasMaxLength(200);

        notification.Property(x => x.Body).HasMaxLength(2_000);

        notification.Property(x => x.ResourceType).HasMaxLength(120);

        notification.Property(x => x.ActionUrl).HasMaxLength(500);

        notification.HasIndex(x => new { x.TenantId, x.RecipientMembershipId, x.ReadAt, x.CreatedAtUtc });

        notification.HasIndex(x => new { x.TenantId, x.ResourceType, x.ResourceId });

        notification.HasOne<TenantMembership>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.RecipientMembershipId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Cascade);

    }
}
