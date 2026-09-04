using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockRationPlan;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockRationPlan;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using LivestockRationPlan = global::EdgIQ.MurimiOS.Domain.LivestockRationPlan.LivestockRationPlan;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
internal static class LivestockRationPlanConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var ration = modelBuilder.Entity<LivestockRationPlan>();

        ration.ToTable("ration_plans", "livestock");

        context.ConfigureTenantEntity(ration);

        ration.Property(x => x.Name).HasMaxLength(200);

        ration.Property(x => x.Notes).HasMaxLength(2_000);

        ration.Property(x => x.CreatedBy).HasMaxLength(240);

        ration.HasIndex(x => new { x.TenantId, x.SubjectId, x.Status, x.EffectiveFrom });

        ration.HasIndex(x => new { x.TenantId, x.FarmId, x.EffectiveFrom, x.EffectiveTo });

        ration.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        ration.HasOne<LivestockSubject>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SubjectId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
