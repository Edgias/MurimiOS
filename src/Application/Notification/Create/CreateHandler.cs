using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.NotificationRecord;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Notification.Create;
using TenantMembership = global::EdgIQ.MurimiOS.Domain.TenantMembership.TenantMembership;

using NotificationRecord = global::EdgIQ.MurimiOS.Domain.NotificationRecord.NotificationRecord;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using MembershipStatus = global::EdgIQ.MurimiOS.Domain.TenantMembership.MembershipStatus;


public sealed class CreateHandler(IPlatformDbContext db, ITenantContext tenant, TimeProvider timeProvider) : IRequestHandler<CreateCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateCommand message, CancellationToken cancellationToken) => CreateAsync(message.request, db, tenant, timeProvider, cancellationToken);

    private static async Task<UseCaseResult>CreateAsync(
        CreateNotificationRequest request,
        IPlatformDbContext db,
        ITenantContext tenant,
        TimeProvider timeProvider,
        CancellationToken cancellationToken)
    {
        bool recipientExists = await db.TenantMemberships.AnyAsync(
            x => x.Id == request.RecipientMembershipId && x.Status == MembershipStatus.Active,
            cancellationToken);
        if (!recipientExists)
            return UseCaseResults.BadRequest(new { code = "notification_recipient_invalid", message = "An active recipient membership is required." });

        try
        {
            var notification = new NotificationRecord(
                Guid.NewGuid(),
                request.RecipientMembershipId,
                request.Category,
                request.Title,
                request.Body,
                request.Severity,
                request.ResourceType,
                request.ResourceId,
                request.ActionUrl);
            DateTimeOffset occurredAt = timeProvider.GetUtcNow();
            db.Notifications.Add(notification);
            db.OutboxMessages.Add(new OutboxMessage(
                Guid.NewGuid(),
                tenant.TenantId,
                "Notification.Requested",
                JsonSerializer.Serialize(new
                {
                    notification.Id,
                    notification.RecipientMembershipId,
                    notification.Category,
                    notification.Title,
                    notification.Body,
                    notification.Severity,
                    notification.ResourceType,
                    notification.ResourceId,
                    notification.ActionUrl
                }),
                occurredAt));
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Created($"/api/v1/notifications/{notification.Id}", ToResponse(notification));
        }
        catch (ArgumentException exception)
        {
            return UseCaseResults.BadRequest(new { code = "notification_invalid", message = exception.Message });
        }
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
