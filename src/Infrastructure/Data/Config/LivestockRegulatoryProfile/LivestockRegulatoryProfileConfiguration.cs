using EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile;
using EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockRegulatoryProfile;
using LivestockRegulatoryProfile = global::EdgIQ.MurimiOS.Domain.LivestockRegulatoryProfile.LivestockRegulatoryProfile;
internal static class LivestockRegulatoryProfileConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var regulatoryProfile = modelBuilder.Entity<LivestockRegulatoryProfile>();

        regulatoryProfile.ToTable("regulatory_profiles", "livestock");

        context.ConfigureTenantEntity(regulatoryProfile);

        regulatoryProfile.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_livestock_regulatory_profiles_tenant_id_id");

        regulatoryProfile.Property(x => x.ProfileKey).HasMaxLength(120);

        regulatoryProfile.Property(x => x.Name).HasMaxLength(200);

        regulatoryProfile.Property(x => x.Jurisdiction).HasMaxLength(20);

        regulatoryProfile.Property(x => x.Species).HasMaxLength(80);

        regulatoryProfile.Property(x => x.ProviderCode).HasMaxLength(80);

        regulatoryProfile.Property(x => x.SchemaVersion).HasMaxLength(40);

        regulatoryProfile.Property(x => x.MediaType).HasMaxLength(120);

        regulatoryProfile.Property(x => x.SourceUri).HasMaxLength(1_000);

        regulatoryProfile.HasIndex(x => new { x.TenantId, x.ProfileKey, x.ProfileVersion }).IsUnique();

        regulatoryProfile.HasIndex(x => new { x.TenantId, x.ExchangeType, x.Jurisdiction, x.Species, x.EffectiveFrom });


        modelBuilder.Entity<EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutoryMovementDocument>().HasOne<LivestockRegulatoryProfile>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.RegulatoryProfileId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
