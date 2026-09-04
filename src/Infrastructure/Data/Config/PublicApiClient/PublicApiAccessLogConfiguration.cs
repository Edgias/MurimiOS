using EdgIQ.MurimiOS.Domain.PublicApiClient;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.PublicApiClient;
using PublicApiAccessLog = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiAccessLog;
using PublicApiClient = global::EdgIQ.MurimiOS.Domain.PublicApiClient.PublicApiClient;
internal static class PublicApiAccessLogConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var apiLog = modelBuilder.Entity<PublicApiAccessLog>();

        apiLog.ToTable("access_log", "external_api"); 
context.ConfigureTenantEntity(apiLog);

        apiLog.Property(x => x.Scope).HasMaxLength(80); 
apiLog.Property(x => x.Method).HasMaxLength(12);

        apiLog.Property(x => x.Path).HasMaxLength(500); 
apiLog.Property(x => x.CorrelationId).HasMaxLength(160);

        apiLog.Property(x => x.RemoteAddressHash).HasMaxLength(64).IsFixedLength();

        apiLog.HasIndex(x => new { x.TenantId, x.ClientId, x.OccurredAt });

        apiLog.HasOne<PublicApiClient>().WithMany().HasForeignKey(x => new { x.TenantId, x.ClientId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
