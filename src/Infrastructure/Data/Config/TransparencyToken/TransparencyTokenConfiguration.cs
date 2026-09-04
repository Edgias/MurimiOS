using EdgIQ.MurimiOS.Domain.RetailDisclosurePolicy;
using EdgIQ.MurimiOS.Domain.TransparencyToken;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.TransparencyToken;
using RetailDisclosurePolicy = global::EdgIQ.MurimiOS.Domain.RetailDisclosurePolicy.RetailDisclosurePolicy;
using TransparencyToken = global::EdgIQ.MurimiOS.Domain.TransparencyToken.TransparencyToken;
internal static class TransparencyTokenConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var token = modelBuilder.Entity<TransparencyToken>();

        token.ToTable("transparency_tokens", "retail"); 
context.ConfigureTenantEntity(token);

        token.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_transparency_tokens_tenant_id_id");

        token.Property(x => x.TokenPrefix).HasMaxLength(24); 
token.Property(x => x.TokenHash).HasMaxLength(64).IsFixedLength();

        token.Property(x => x.IssuedBy).HasMaxLength(240); 
token.HasIndex(x => x.TokenHash).IsUnique();

        token.HasIndex(x => new { x.TenantId, x.PolicyId, x.ObjectId, x.Status });

        token.HasOne<RetailDisclosurePolicy>().WithMany().HasForeignKey(x => new { x.TenantId, x.PolicyId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
