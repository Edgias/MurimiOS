using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.Farm;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;
internal static class FarmConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var farm = modelBuilder.Entity<Farm>();

        farm.ToTable("farms", "farm");

        context.ConfigureTenantEntity(farm);

        farm.Property(x => x.Name).HasMaxLength(200);

        farm.Property(x => x.Code).HasMaxLength(40);

        farm.Property(x => x.CountryCode).HasMaxLength(2);

        farm.Property(x => x.TimeZone).HasMaxLength(100);

        farm.HasIndex(x => new { x.TenantId, x.Code }).IsUnique();

        farm.HasOne<OrganisationNode>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.OrganisationNodeId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
