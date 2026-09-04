using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.AgriculturalInput;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.AgriculturalInput;
using AgriculturalInput = global::EdgIQ.MurimiOS.Domain.AgriculturalInput.AgriculturalInput;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
internal static class AgriculturalInputConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var input = modelBuilder.Entity<AgriculturalInput>();

        input.ToTable("inputs", "inventory");

        context.ConfigureTenantEntity(input);

        input.Property(x => x.Name).HasMaxLength(160);

        input.Property(x => x.Category).HasMaxLength(80);

        input.Property(x => x.Unit).HasMaxLength(30);

        input.Property(x => x.DisplayUnit).HasMaxLength(30);

        input.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_inputs_tenant_id_id");

        input.HasIndex(x => new { x.TenantId, x.FarmId, x.Name }).IsUnique();

        input.HasOne<Farm>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
