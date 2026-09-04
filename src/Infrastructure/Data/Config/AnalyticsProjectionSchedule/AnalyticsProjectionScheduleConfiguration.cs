using EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule;
using EdgIQ.MurimiOS.Domain.Farm;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.AnalyticsProjectionSchedule;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using AnalyticsProjectionSchedule = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionSchedule.AnalyticsProjectionSchedule;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
internal static class AnalyticsProjectionScheduleConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var schedule = modelBuilder.Entity<AnalyticsProjectionSchedule>();

        schedule.ToTable("projection_schedules", "analytics"); 
context.ConfigureTenantEntity(schedule);

        schedule.Property(x => x.Name).HasMaxLength(200); 
schedule.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        schedule.Property(x => x.ProjectionVersion).HasMaxLength(60); 
schedule.Property(x => x.CreatedBy).HasMaxLength(240);

        schedule.HasIndex(x => new { x.TenantId, x.FarmId, x.Name }).IsUnique();

        schedule.HasIndex(x => new { x.IsActive, x.NextRunAt });

        schedule.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
