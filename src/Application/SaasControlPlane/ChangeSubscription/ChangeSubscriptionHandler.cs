using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.TenantSubscription;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.ChangeSubscription;
using TenantSubscription = global::EdgIQ.MurimiOS.Domain.TenantSubscription.TenantSubscription;

public sealed class ChangeSubscriptionHandler(IPlatformDbContext db, TenantDatabaseRoutingScope routing) : IRequestHandler<ChangeSubscriptionCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ChangeSubscriptionCommand message, CancellationToken cancellationToken) => ChangeSubscriptionAsync(message.subscriptionId, message.command, message.request, db, routing, cancellationToken);

    private static async Task<UseCaseResult> ChangeSubscriptionAsync(Guid subscriptionId, string command,
        SubscriptionCommandRequest request, IPlatformDbContext db, TenantDatabaseRoutingScope routing, CancellationToken ct)
    {
        using IDisposable control = routing.EnterControlPlane(); TenantSubscription? subscription = await db.TenantSubscriptions.SingleOrDefaultAsync(x => x.Id == subscriptionId, ct);
        if (subscription is null) return UseCaseResults.NotFound();
        try
        {
            DateTimeOffset now = DateTimeOffset.UtcNow;
            if (command == "activate") subscription.Activate(now);
            else if (command == "past-due") subscription.MarkPastDue(request.Reason);
            else if (command == "suspend") subscription.Suspend(request.Reason);
            else if (command == "renew") subscription.Renew(request.PeriodStartsAt ?? throw new ArgumentException("Period start is required."), request.PeriodEndsAt ?? throw new ArgumentException("Period end is required."));
            else if (command == "schedule-cancellation") subscription.ScheduleCancellation(request.CancelAt ?? subscription.CurrentPeriodEndsAt);
            else subscription.Cancel(now, request.Reason);
            await db.SaveChangesAsync(ct); return UseCaseResults.Ok(subscription);
        }
        catch (Exception e) when (e is InvalidOperationException or ArgumentException) { return Conflict("subscription_transition_invalid", e.Message); }
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
