using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.NotificationRecord;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Notification.MarkRead;
using NotificationRecord = global::EdgIQ.MurimiOS.Domain.NotificationRecord.NotificationRecord;


public sealed class MarkReadHandler(IPlatformDbContext db, IRequestActorContext actor, TimeProvider timeProvider) : IRequestHandler<MarkReadCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(MarkReadCommand message, CancellationToken cancellationToken) => MarkReadAsync(message.notificationId, db, actor, timeProvider, cancellationToken);

    private static async Task<UseCaseResult>MarkReadAsync(
        Guid notificationId,
        IPlatformDbContext db,
        IRequestActorContext actor,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        if (actor.MembershipId is not { } membershipId) return UseCaseResults.Forbid();
        NotificationRecord? notification = await db.Notifications.SingleOrDefaultAsync(
            x => x.Id == notificationId && x.RecipientMembershipId == membershipId,
            cancellationToken);
        if (notification is null) return UseCaseResults.NotFound();
        notification.MarkRead(timeProvider.GetUtcNow());
        await db.SaveChangesAsync(cancellationToken);
        return UseCaseResults.Ok(ToResponse(notification));
    }


    private static NotificationResponse ToResponse(NotificationRecord notification) => new(
        notification.Id,
        notification.Category,
        notification.Title,
        notification.Body,
        notification.Severity,
        notification.ResourceType,
        notification.ResourceId,
        notification.ActionUrl,
        notification.ReadAt,
        notification.CreatedAtUtc);

}
