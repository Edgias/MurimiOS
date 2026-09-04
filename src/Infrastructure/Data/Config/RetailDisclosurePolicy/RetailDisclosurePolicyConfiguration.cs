using EdgIQ.MurimiOS.Domain.RetailDisclosurePolicy;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.RetailDisclosurePolicy;
using RetailDisclosurePolicy = global::EdgIQ.MurimiOS.Domain.RetailDisclosurePolicy.RetailDisclosurePolicy;
internal static class RetailDisclosurePolicyConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var policy = modelBuilder.Entity<RetailDisclosurePolicy>();

        policy.ToTable("disclosure_policies", "retail"); 
context.ConfigureTenantEntity(policy);

        policy.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_disclosure_policies_tenant_id_id");

        policy.Property(x => x.Name).HasMaxLength(200); 
policy.Property(x => x.PublicBrandName).HasMaxLength(200);

        policy.Property(x => x.Attributes).HasColumnType("text[]"); 
policy.Property(x => x.FarmIds).HasColumnType("uuid[]");

        policy.Property(x => x.Locale).HasMaxLength(40); 
policy.Property(x => x.AuthoredBy).HasMaxLength(240);

        policy.HasIndex(x => new { x.TenantId, x.SeriesId, x.PolicyVersion }).IsUnique();

        policy.HasIndex(x => new { x.TenantId, x.Status });

    }
}
