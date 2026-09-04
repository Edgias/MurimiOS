using EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun;
using EdgIQ.MurimiOS.Domain.Farm;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.AnalyticsProjectionRun;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using AnalyticsProjectionRun = global::EdgIQ.MurimiOS.Domain.AnalyticsProjectionRun.AnalyticsProjectionRun;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
internal static class AnalyticsProjectionRunConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var run = modelBuilder.Entity<AnalyticsProjectionRun>();

        run.ToTable("projection_runs", "analytics");

        context.ConfigureTenantEntity(run);

        run.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_analytics_projection_runs_tenant_id_id");

        run.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        run.Property(x => x.ProjectionVersion).HasMaxLength(60);

        run.Property(x => x.RequestedBy).HasMaxLength(240);

        run.Property(x => x.TriggerType).HasMaxLength(80);

        run.Property(x => x.DataQualityPercent).HasPrecision(8, 4);

        run.Property(x => x.ErrorCode).HasMaxLength(80);

        run.Property(x => x.ErrorMessage).HasMaxLength(2_000);

        run.HasIndex(x => new { x.TenantId, x.FarmId, x.PeriodStart, x.PeriodEnd, x.ProjectionVersion, x.Revision }).IsUnique();

        run.HasIndex(x => new { x.TenantId, x.Status, x.ScheduledFor });

        run.HasIndex(x => new { x.TenantId, x.FarmId, x.PeriodEnd, x.SupersededAt });

        run.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
