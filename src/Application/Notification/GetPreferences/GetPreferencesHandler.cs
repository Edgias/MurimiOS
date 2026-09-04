using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Notification.GetPreferences;

public sealed class GetPreferencesHandler(IPlatformDbContext db, IRequestActorContext actor) : IRequestHandler<GetPreferencesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(GetPreferencesQuery message, CancellationToken cancellationToken) => GetPreferencesAsync(db, actor, cancellationToken);

    private static async Task<UseCaseResult>GetPreferencesAsync(
        IPlatformDbContext db,
        IRequestActorContext actor,
        CancellationToken cancellationToken)
    {
        if (actor.MembershipId is not { } membershipId) return UseCaseResults.Forbid();
        NotificationPreferenceResponse[] preferences = await db.NotificationPreferences
            .Where(x => x.MembershipId == membershipId)
            .OrderBy(x => x.Category)
            .Select(x => new NotificationPreferenceResponse(
                x.Category, x.InAppEnabled, x.EmailEnabled, x.SmsEnabled, x.Version))
            .ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(preferences);
    }


}
