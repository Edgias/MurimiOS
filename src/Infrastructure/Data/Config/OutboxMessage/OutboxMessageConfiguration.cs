using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
namespace EdgIQ.MurimiOS.Infrastructure.Data.Config.OutboxMessage;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
internal static class OutboxMessageConfiguration
{
    internal static void Configure(ModelBuilder modelBuilder, PlatformDbContext context)
    {
        var outbox = modelBuilder.Entity<OutboxMessage>();

        outbox.ToTable("outbox_messages", "messaging");

        outbox.HasKey(x => x.Id);

        outbox.Property(x => x.Type).HasMaxLength(200);

        outbox.Property(x => x.Payload).HasColumnType("jsonb");

        outbox.Property(x => x.LastError).HasMaxLength(2_000);

        outbox.HasIndex(x => new
        {
            x.ProcessedAt,
            x.DeadLetteredAt,
            x.AvailableAt,
            x.LockedUntil,
            x.OccurredAt
        }).HasDatabaseName("ix_outbox_messages_dispatch");

        context.ConfigureOutboxMessageFilter(outbox);

    }
}
