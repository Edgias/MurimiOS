using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.IntegrationConnection;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;
using IntegrationCredential = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationCredential;
internal static class IntegrationCredentialConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var credential = modelBuilder.Entity<IntegrationCredential>();

        credential.ToTable("credential_versions", "integrations");

        context.ConfigureTenantEntity(credential);

        credential.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_integration_credentials_tenant_id_id");

        credential.Property(x => x.SecretProvider).HasMaxLength(80);

        credential.Property(x => x.ProtectedReference).HasMaxLength(4_000);

        credential.Property(x => x.Fingerprint).HasMaxLength(64);

        credential.Property(x => x.CreatedBy).HasMaxLength(240);

        credential.Property(x => x.RevokedBy).HasMaxLength(240);

        credential.HasIndex(x => new { x.TenantId, x.ConnectionId, x.CredentialVersion }).IsUnique();

        credential.HasIndex(x => new { x.TenantId, x.ConnectionId, x.Status });

        credential.HasOne<IntegrationConnection>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.ConnectionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        credential.HasOne<IntegrationCredential>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.PreviousCredentialId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        credential.HasOne<IntegrationCredential>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SupersededByCredentialId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
