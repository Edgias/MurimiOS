using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.NotificationRecord;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Notification.Get;
using NotificationRecord = global::EdgIQ.MurimiOS.Domain.NotificationRecord.NotificationRecord;


public sealed class GetHandler(IPlatformDbContext db, IRequestActorContext actor) : IRequestHandler<GetQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetQuery message, CancellationToken cancellationToken) => GetAsync(message.unreadOnly, message.limit, db, actor, cancellationToken);

    private static async Task<UseCaseResult>GetAsync(
        bool? unreadOnly,
        int? limit,
        IPlatformDbContext db,
        IRequestActorContext actor,
        CancellationToken cancellationToken)
    {
        if (actor.MembershipId is not { } membershipId) return UseCaseResults.Forbid();
        int pageSize = Math.Clamp(limit ?? 50, 1, 200);
        IQueryable<NotificationRecord> query = db.Notifications
            .Where(x => x.RecipientMembershipId == membershipId);
        if (unreadOnly == true) query = query.Where(x => x.ReadAt == null);

        NotificationResponse[] notifications = await query
            .OrderByDescending(x => x.CreatedAtUtc)
            .ThenByDescending(x => x.Id)
            .Take(pageSize)
            .Select(x => new NotificationResponse(
                x.Id,
                x.Category,
                x.Title,
                x.Body,
                x.Severity,
                x.ResourceType,
                x.ResourceId,
                x.ActionUrl,
                x.ReadAt,
                x.CreatedAtUtc))
            .ToArrayAsync(cancellationToken);
        int unreadCount = await db.Notifications.CountAsync(
            x => x.RecipientMembershipId == membershipId && x.ReadAt == null, cancellationToken);
        return UseCaseResults.Ok(new NotificationInboxResponse(unreadCount, notifications));
    }


}
