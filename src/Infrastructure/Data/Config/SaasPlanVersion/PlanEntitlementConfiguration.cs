using EdgIQ.MurimiOS.Domain.SaasPlanVersion;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SaasPlanVersion;
using PlanEntitlement = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.PlanEntitlement;
using SaasPlanVersion = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanVersion;
internal static class PlanEntitlementConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var entitlement = modelBuilder.Entity<PlanEntitlement>();

        entitlement.ToTable("plan_entitlements", "saas"); 
context.ConfigureTenantEntity(entitlement);

        entitlement.Property(x => x.Key).HasMaxLength(120);

        entitlement.Property(x => x.IncludedAmount).HasPrecision(20, 4);

        entitlement.Property(x => x.Unit).HasMaxLength(40);

        entitlement.HasIndex(x => new { x.TenantId, x.PlanVersionId, x.Key }).IsUnique();

        entitlement.HasOne<SaasPlanVersion>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.PlanVersionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
