using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockGrazingPlan;
using EdgIQ.MurimiOS.Domain.LivestockHolding;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockGrazingPlan;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using LivestockGrazingPlan = global::EdgIQ.MurimiOS.Domain.LivestockGrazingPlan.LivestockGrazingPlan;
using LivestockHolding = global::EdgIQ.MurimiOS.Domain.LivestockHolding.LivestockHolding;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
internal static class LivestockGrazingPlanConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var grazing = modelBuilder.Entity<LivestockGrazingPlan>();

        grazing.ToTable("grazing_plans", "livestock");

        context.ConfigureTenantEntity(grazing);

        grazing.Property(x => x.AvailableDryMatterKg).HasPrecision(18, 4);

        grazing.Property(x => x.Notes).HasMaxLength(2_000);

        grazing.Property(x => x.CreatedBy).HasMaxLength(240);

        grazing.HasIndex(x => new { x.TenantId, x.FarmId, x.StartsAt, x.EndsAt });

        grazing.HasIndex(x => new { x.TenantId, x.HoldingId, x.Status, x.StartsAt, x.EndsAt });

        grazing.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        grazing.HasOne<LivestockSubject>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.GroupSubjectId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        grazing.HasOne<LivestockHolding>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.HoldingId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
