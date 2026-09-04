using EdgIQ.MurimiOS.Domain.ApiRateLimitPolicy;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.ApiRateLimitPolicy;
using ApiRateLimitPolicy = global::EdgIQ.MurimiOS.Domain.ApiRateLimitPolicy.ApiRateLimitPolicy;
internal static class ApiRateLimitPolicyConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var rate = modelBuilder.Entity<ApiRateLimitPolicy>();

        rate.ToTable("rate_limit_policies", "security"); 
context.ConfigureTenantEntity(rate);

        rate.Property(x => x.CreatedBy).HasMaxLength(240);

        rate.HasIndex(x => new { x.TenantId, x.PublicApiClientId, x.IsActive });

    }
}
