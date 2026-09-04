using EdgIQ.MurimiOS.Domain.OrganisationNode;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.OrganisationNode;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;
internal static class OrganisationNodeConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var node = modelBuilder.Entity<OrganisationNode>();

        node.ToTable("org_nodes", "platform");

        context.ConfigureTenantEntity(node);

        node.Property(x => x.Name).HasMaxLength(200);

        node.Property(x => x.Path).HasColumnType("ltree").HasMaxLength(800);

        node.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_org_nodes_tenant_id_id");

        node.HasIndex(x => new { x.TenantId, x.Path }).IsUnique();

        node.HasOne<OrganisationNode>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.ParentId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
