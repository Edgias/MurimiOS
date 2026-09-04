using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.SyncClient;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.SyncClient;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
internal static class SyncChangeConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var change = modelBuilder.Entity<SyncChange>();

        change.ToTable("change_log", "sync");

        change.HasKey(x => x.Id);

        change.Property(x => x.Id).ValueGeneratedOnAdd();

        change.Property(x => x.EntityType).HasMaxLength(120);

        change.Property(x => x.Operation).HasMaxLength(20);

        change.Property(x => x.Payload).HasColumnType("jsonb");

        change.HasIndex(x => new { x.TenantId, x.Id });

        change.HasIndex(x => new { x.TenantId, x.FarmId, x.Id });

        change.HasOne<Farm>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

        context.ConfigureSyncChangeFilter(change);

    }
}
