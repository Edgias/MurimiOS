using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.NotificationPreference;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Notification.PutPreference;
using NotificationPreference = global::EdgIQ.MurimiOS.Domain.NotificationPreference.NotificationPreference;


public sealed class PutPreferenceHandler(IPlatformDbContext db, IRequestActorContext actor) : IRequestHandler<PutPreferenceCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(PutPreferenceCommand message, CancellationToken cancellationToken) => PutPreferenceAsync(message.category, message.request, db, actor, cancellationToken);

    private static async Task<UseCaseResult>PutPreferenceAsync(
        string category,
        UpdateNotificationPreferenceRequest request,
        IPlatformDbContext db,
        IRequestActorContext actor,
        CancellationToken cancellationToken)
    {
        if (actor.MembershipId is not { } membershipId) return UseCaseResults.Forbid();
        try
        {
            string normalized = category.Trim().ToLowerInvariant();
            NotificationPreference? preference = await db.NotificationPreferences.SingleOrDefaultAsync(
                x => x.MembershipId == membershipId && x.Category == normalized,
                cancellationToken);
            if (preference is null)
            {
                preference = new NotificationPreference(
                    Guid.NewGuid(), membershipId, normalized,
                    request.InAppEnabled, request.EmailEnabled, request.SmsEnabled);
                db.NotificationPreferences.Add(preference);
            }
            else
            {
                preference.SetChannels(request.InAppEnabled, request.EmailEnabled, request.SmsEnabled);
            }
            await db.SaveChangesAsync(cancellationToken);
            return UseCaseResults.Ok(new NotificationPreferenceResponse(
                preference.Category,
                preference.InAppEnabled,
                preference.EmailEnabled,
                preference.SmsEnabled,
                preference.Version));
        }
        catch (ArgumentException exception)
        {
            return UseCaseResults.BadRequest(new { code = "notification_preference_invalid", message = exception.Message });
        }
    }


}
