using EdgIQ.MurimiOS.Domain.SaasPlanVersion;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SaasPlanVersion;
using Currency = global::EdgIQ.MurimiOS.Domain.Currency.Currency;

using SaasPlanVersion = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanVersion;
internal static class SaasPlanVersionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var plan = modelBuilder.Entity<SaasPlanVersion>();

        plan.ToTable("plan_versions", "saas"); 
context.ConfigureTenantEntity(plan);

        plan.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_saas_plan_versions_tenant_id_id");

        plan.Property(x => x.Code).HasMaxLength(80);

        plan.Property(x => x.Name).HasMaxLength(200);

        plan.Property(x => x.BasePrice).HasPrecision(18, 2);

        plan.Property(x => x.Currency).HasMaxLength(3).IsFixedLength();

        plan.Property(x => x.CreatedBy).HasMaxLength(240);

        plan.HasIndex(x => new { x.TenantId, x.SeriesId, x.PlanVersion }).IsUnique();

        plan.HasIndex(x => new { x.TenantId, x.Code, x.Status });

    }
}
