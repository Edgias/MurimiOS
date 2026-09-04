using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.LivestockSubject;
using EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.LivestockStatutoryMovementDocument;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using LivestockStatutoryMovementDocument = global::EdgIQ.MurimiOS.Domain.LivestockStatutoryMovementDocument.LivestockStatutoryMovementDocument;
using LivestockMovementRecord = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockMovementRecord;
using LivestockSubject = global::EdgIQ.MurimiOS.Domain.LivestockSubject.LivestockSubject;
internal static class LivestockStatutoryMovementDocumentConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var statutoryDocument = modelBuilder.Entity<LivestockStatutoryMovementDocument>();

        statutoryDocument.ToTable("statutory_movement_documents", "livestock");

        context.ConfigureTenantEntity(statutoryDocument);

        statutoryDocument.HasAlternateKey(x => new { x.TenantId, x.Id }).HasName("ak_statutory_movement_documents_tenant_id_id");

        statutoryDocument.Property(x => x.Jurisdiction).HasMaxLength(20);

        statutoryDocument.Property(x => x.ProviderCode).HasMaxLength(80);

        statutoryDocument.Property(x => x.SchemaVersion).HasMaxLength(40);

        statutoryDocument.Property(x => x.DocumentNumber).HasMaxLength(120);

        statutoryDocument.Property(x => x.SnapshotJson).HasColumnType("jsonb");

        statutoryDocument.Property(x => x.PayloadSha256).HasMaxLength(64).IsFixedLength();

        statutoryDocument.Property(x => x.CreatedBy).HasMaxLength(240);

        statutoryDocument.Property(x => x.DocumentMediaType).HasMaxLength(120);

        statutoryDocument.Property(x => x.RenderedDocument).HasColumnType("text");

        statutoryDocument.Property(x => x.ExternalReference).HasMaxLength(160);

        statutoryDocument.Property(x => x.RejectionCode).HasMaxLength(80);

        statutoryDocument.Property(x => x.RejectionMessage).HasMaxLength(2_000);

        statutoryDocument.HasIndex(x => new { x.TenantId, x.MovementId, x.ProviderCode }).IsUnique();

        statutoryDocument.HasIndex(x => new { x.TenantId, x.Status, x.ReportingDueAt });

        statutoryDocument.HasOne<LivestockMovementRecord>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.MovementId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        statutoryDocument.HasOne<LivestockSubject>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.SubjectId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        statutoryDocument.HasOne<Farm>().WithMany()
            .HasForeignKey(x => new { x.TenantId, x.FarmId }).HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
