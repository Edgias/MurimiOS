using EdgIQ.MurimiOS.Domain.ApiCredentialRotationPolicy;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.ApiCredentialRotationPolicy;
using ApiCredentialRotationPolicy = global::EdgIQ.MurimiOS.Domain.ApiCredentialRotationPolicy.ApiCredentialRotationPolicy;
internal static class ApiCredentialRotationPolicyConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var rotation = modelBuilder.Entity<ApiCredentialRotationPolicy>();

        rotation.ToTable("credential_rotation_policies", "security"); 
context.ConfigureTenantEntity(rotation);

        rotation.Property(x => x.CreatedBy).HasMaxLength(240);

        rotation.HasIndex(x => new { x.TenantId, x.PublicApiClientId, x.IsActive });

    }
}
