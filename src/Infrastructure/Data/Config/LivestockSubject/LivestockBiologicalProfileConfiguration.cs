using EdgIQ.MurimiOS.Domain.LivestockSubject;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockSubject;
using LivestockBiologicalProfile = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockBiologicalProfile;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
internal static class LivestockBiologicalProfileConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var biologicalProfile = modelBuilder.Entity<LivestockBiologicalProfile>();

        biologicalProfile.ToTable("biological_profiles", "livestock");

        context.ConfigureTenantEntity(biologicalProfile);

        biologicalProfile.Property(x => x.Breed).HasMaxLength(120);

        biologicalProfile.HasIndex(x => new { x.TenantId, x.SubjectId }).IsUnique();

        biologicalProfile.HasOne<LivestockSubject>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SubjectId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        biologicalProfile.HasOne<LivestockSubject>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.DamId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        biologicalProfile.HasOne<LivestockSubject>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SireId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
