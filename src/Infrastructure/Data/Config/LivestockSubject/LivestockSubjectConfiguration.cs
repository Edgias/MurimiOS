using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockSubject;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
internal static class LivestockSubjectConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var subject = modelBuilder.Entity<LivestockSubject>();

        subject.ToTable("subjects", "livestock");

        context.ConfigureTenantEntity(subject);

        subject.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_subjects_tenant_id_id");

        subject.Property(x => x.Name).HasMaxLength(200);

        subject.Property(x => x.Species).HasMaxLength(80);

        subject.Property(x => x.OfficialIdentifier).HasMaxLength(120);

        subject.HasIndex(x => new { x.TenantId, x.OfficialIdentifier })
            .IsUnique()
            .HasFilter("official_identifier IS NOT NULL");

        subject.HasOne<Farm>()
            .WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id })
            .OnDelete(DeleteBehavior.Restrict);

    }
}
