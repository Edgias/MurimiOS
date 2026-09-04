using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.IntegrationConnection;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;
internal static class IntegrationConnectionConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var connection = modelBuilder.Entity<IntegrationConnection>();

        connection.ToTable("connections", "integrations");

        context.ConfigureTenantEntity(connection);

        connection.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_integration_connections_tenant_id_id");

        connection.Property(x => x.ProviderCode).HasMaxLength(80);

        connection.Property(x => x.Name).HasMaxLength(200);

        connection.Property(x => x.ConfigurationJson).HasColumnType("jsonb");

        connection.Property(x => x.CreatedBy).HasMaxLength(240);

        connection.Property(x => x.LastErrorCode).HasMaxLength(80);

        connection.Property(x => x.LastErrorMessage).HasMaxLength(2_000);

        connection.Property(x => x.DisabledBy).HasMaxLength(240);

        connection.HasIndex(x => new { x.TenantId, x.ProviderCode, x.Name }).IsUnique();

        connection.HasIndex(x => new { x.TenantId, x.Category, x.Status, x.Health });

    }
}
