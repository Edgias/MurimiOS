using EdgIQ.MurimiOS.Domain.TransparencyToken;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.TransparencyToken;
using TransparencyAccessLog = global::EdgIQ.MurimiOS.Domain.TransparencyToken.TransparencyAccessLog;
using TransparencyToken = global::EdgIQ.MurimiOS.Domain.TransparencyToken.TransparencyToken;
internal static class TransparencyAccessLogConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var transparencyLog = modelBuilder.Entity<TransparencyAccessLog>();

        transparencyLog.ToTable("transparency_access_log", "retail"); 
context.ConfigureTenantEntity(transparencyLog);

        transparencyLog.Property(x => x.Outcome).HasMaxLength(40); 
transparencyLog.Property(x => x.CorrelationId).HasMaxLength(160);

        transparencyLog.Property(x => x.RemoteAddressHash).HasMaxLength(64).IsFixedLength(); 
transparencyLog.Property(x => x.UserAgentHash).HasMaxLength(64).IsFixedLength();

        transparencyLog.HasIndex(x => new { x.TenantId, x.TokenId, x.OccurredAt });

        transparencyLog.HasOne<TransparencyToken>().WithMany().HasForeignKey(x => new { x.TenantId, x.TokenId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
