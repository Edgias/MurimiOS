using EdgIQ.MurimiOS.Domain.Farm;
using EdgIQ.MurimiOS.Domain.BusinessDocumentExchange;
using EdgIQ.MurimiOS.Domain.IntegrationConnection;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.BusinessDocumentExchange;
using BusinessDocumentExchange = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.BusinessDocumentExchange;
using Farm = global::EdgIQ.MurimiOS.Domain.Farm.Farm;
using IntegrationConnection = global::EdgIQ.MurimiOS.Domain.IntegrationConnection.IntegrationConnection;
internal static class BusinessDocumentExchangeConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {

        var document = modelBuilder.Entity<BusinessDocumentExchange>();

        document.ToTable("business_documents", "connected");

        context.ConfigureTenantEntity(document);

        document.Property(x => x.ExternalDocumentId).HasMaxLength(240);

        document.Property(x => x.MappingVersion).HasMaxLength(60);

        document.Property(x => x.PayloadJson).HasColumnType("jsonb");

        document.Property(x => x.PayloadSha256).HasMaxLength(64).IsFixedLength();

        document.Property(x => x.RejectionReason).HasMaxLength(2_000);

        document.HasIndex(x => new { x.TenantId, x.ConnectionId, x.DocumentType, x.Direction, x.ExternalDocumentId, x.PayloadSha256 }).IsUnique();

        document.HasIndex(x => new { x.TenantId, x.FarmId, x.Status, x.OccurredAt });

        document.HasOne<IntegrationConnection>().WithMany().HasForeignKey(x => new { x.TenantId, x.ConnectionId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

        document.HasOne<Farm>().WithMany().HasForeignKey(x => new { x.TenantId, x.FarmId })
            .HasPrincipalKey(x => new { x.TenantId, x.Id }).OnDelete(DeleteBehavior.Restrict);

    }
}
