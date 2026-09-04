using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.FarmField;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
internal static class FarmFieldConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var field = modelBuilder.Entity<FarmField>();

        field.ToTable("fields", "farm");

        context.ConfigureTenantEntity(field);

        field.Property(x => x.Name).HasMaxLength(200);

        field.HasIndex(x => new { x.TenantId, x.FarmId, x.Name }).IsUnique();

        field.HasOne<Farm>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
