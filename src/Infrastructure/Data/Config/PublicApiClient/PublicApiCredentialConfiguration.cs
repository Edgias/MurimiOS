using EdgIQ.MurimiOS.Domain.PublicApiClient;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.PublicApiClient;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;
using PublicApiCredential = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiCredential;
internal static class PublicApiCredentialConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var credential = modelBuilder.Entity<PublicApiCredential>();

        credential.ToTable("credentials", "external_api"); 
context.ConfigureTenantEntity(credential);

        credential.Property(x => x.KeyPrefix).HasMaxLength(24); 
credential.Property(x => x.KeyHash).HasMaxLength(64).IsFixedLength();

        credential.Property(x => x.IssuedBy).HasMaxLength(240);

        credential.HasIndex(x => x.KeyHash).IsUnique();

        credential.HasIndex(x => new { x.TenantId, x.ClientId, x.CredentialVersion }).IsUnique();

        credential.HasOne<PublicApiClient>().WithMany().HasForeignKey(x => new { x.TenantId, x.ClientId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
