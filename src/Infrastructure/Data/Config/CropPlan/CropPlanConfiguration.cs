using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.CropPlan;
using EdgIQ.MurimiOS.Domain.GrowingSeason;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.CropPlan;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using GrowingSeason = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeason;
internal static class CropPlanConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var plan = modelBuilder.Entity<CropPlan>();

        plan.ToTable("crop_plans", "planning");

        context.ConfigureTenantEntity(plan);

        plan.Property(x => x.Name).HasMaxLength(200);

        plan.Property(x => x.ScenarioName).HasMaxLength(120);

        plan.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        plan.Property(x => x.CreatedBy).HasMaxLength(240);

        plan.Property(x => x.SubmittedBy).HasMaxLength(240);

        plan.Property(x => x.ApprovedBy).HasMaxLength(240);

        plan.Property(x => x.RejectedBy).HasMaxLength(240);

        plan.Property(x => x.RejectionReason).HasMaxLength(2_000);

        plan.HasIndex(x => new { x.TenantId, x.ScenarioId, x.Revision }).IsUnique();

        plan.HasIndex(x => new { x.TenantId, x.FarmId, x.SeasonId, x.Status });

        plan.HasIndex(x => new { x.TenantId, x.PlanningGroupId, x.Status });

        plan.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        plan.HasOne<GrowingSeason>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SeasonId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        plan.HasOne<CropPlan>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.PreviousPlanId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        plan.HasOne<CropPlan>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SupersededByPlanId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
