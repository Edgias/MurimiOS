using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SyncClient;
using SyncCommandLog = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncCommandLog;
internal static class SyncCommandLogConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var command = modelBuilder.Entity<SyncCommandLog>();

        command.ToTable("command_log", "sync");

        context.ConfigureTenantEntity(command);

        command.Property(x => x.CommandType).HasMaxLength(120);

        command.Property(x => x.ErrorCode).HasMaxLength(80);

        command.Property(x => x.ErrorMessage).HasMaxLength(2_000);

        command.HasIndex(x => new { x.TenantId, x.DeviceId, x.Sequence }).IsUnique();

    }
}
