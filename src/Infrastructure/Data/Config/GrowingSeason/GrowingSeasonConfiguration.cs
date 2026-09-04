using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.GrowingSeason;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.GrowingSeason;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using GrowingSeason = global::EdgIQ.MurimiOS.Domain.GrowingSeason.GrowingSeason;
internal static class GrowingSeasonConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var season = modelBuilder.Entity<GrowingSeason>();

        season.ToTable("seasons", "production");

        context.ConfigureTenantEntity(season);

        season.Property(x => x.Name).HasMaxLength(120);

        season.HasIndex(x => new { x.TenantId, x.FarmId, x.Name }).IsUnique();

        season.HasIndex(x => new { x.TenantId, x.FarmId, x.StartsOn, x.EndsOn });

        season.HasOne<Farm>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
