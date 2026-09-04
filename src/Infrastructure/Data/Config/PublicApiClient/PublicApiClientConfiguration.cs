using EdgIQ.MurimiOS.Domain.PublicApiClient;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.PublicApiClient;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;
internal static class PublicApiClientConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var client = modelBuilder.Entity<PublicApiClient>();

        client.ToTable("clients", "external_api"); 
context.ConfigureTenantEntity(client);

        client.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_external_api_clients_tenant_id_id");

        client.Property(x => x.Name).HasMaxLength(200); 
client.Property(x => x.Scopes).HasColumnType("text[]");

        client.Property(x => x.FarmIds).HasColumnType("uuid[]"); 
client.Property(x => x.CreatedBy).HasMaxLength(240);

        client.Property(x => x.RevokedBy).HasMaxLength(240); 
client.HasIndex(x => new { x.TenantId, x.Status });

    }
}
