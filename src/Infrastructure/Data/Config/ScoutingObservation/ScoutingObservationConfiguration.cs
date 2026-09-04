using EdgIQ.MurimiOS.Domain.EvidenceAttachment;
using EdgIQ.MurimiOS.Domain.FarmField;
using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;
using EdgIQ.MurimiOS.Domain.CropZone;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.ScoutingObservation;
using Location = global::EdgIQ.MurimiOS.Domain.Location.Location;

using CropZone = global::EdgIQ.MurimiOS.Domain.CropZone.CropZone;
using EvidenceAttachment = global::EdgIQ.MurimiOS.Domain.EvidenceAttachment.EvidenceAttachment;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using FarmField = global::EdgIQ.MurimiOS.Domain.FarmField.FarmField;
using FieldBoundaryVersion = global::EdgIQ.MurimiOS.Domain.FarmField.FieldBoundaryVersion;
using ScoutingObservation = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingObservation;
internal static class ScoutingObservationConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var scouting = modelBuilder.Entity<ScoutingObservation>();

        scouting.ToTable("scouting_observations", "insights");

        context.ConfigureTenantEntity(scouting);

        scouting.Property(x => x.IssueCode).HasMaxLength(80);

        scouting.Property(x => x.Title).HasMaxLength(200);

        scouting.Property(x => x.Notes).HasMaxLength(4_000);

        scouting.Property(x => x.RecordedBy).HasMaxLength(240);

        scouting.Property(x => x.Location).HasColumnType("geometry(Point,4326)");

        scouting.HasIndex(x => new { x.TenantId, x.FieldId, x.ObservedAt });

        scouting.HasIndex(x => new { x.TenantId, x.SourceCommandId }).IsUnique().HasFilter("source_command_id IS NOT NULL");

        scouting.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        scouting.HasOne<FarmField>().WithMany().HasForeignKey(x => new { x.TenantId, x.FieldId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        scouting.HasOne<FieldBoundaryVersion>().WithMany().HasForeignKey(x => new { x.TenantId, x.BoundaryVersionId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        scouting.HasOne<CropZone>().WithMany().HasForeignKey(x => new { x.TenantId, x.CropZoneId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        scouting.HasOne<EvidenceAttachment>().WithMany().HasForeignKey(x => new { x.TenantId, x.EvidenceId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
