using EdgIQ.MurimiOS.Domain.Common;



namespace EdgIQ.MurimiOS.Domain.TenantSubscription;
using TenantAccount = global::EdgIQ.MurimiOS.Domain.TenantAccount.TenantAccount;

public sealed class TenantSubscription : TenantEntity
{
    private TenantSubscription() { }

    public TenantSubscription(Guid id, Guid planVersionId, DateTimeOffset startsAt, DateTimeOffset currentPeriodEndsAt,
        DateTimeOffset? trialEndsAt, string? providerSubscriptionReference, DateTimeOffset createdAt, string createdBy) : base(id)
    {
        if (planVersionId == Guid.Empty || currentPeriodEndsAt <= startsAt || trialEndsAt <= startsAt || trialEndsAt > currentPeriodEndsAt)
            throw new ArgumentException("Subscription plan, period, or trial is invalid.");
        PlanVersionId = planVersionId;
        StartsAt = startsAt.ToUniversalTime();
        CurrentPeriodStartsAt = StartsAt;
        CurrentPeriodEndsAt = currentPeriodEndsAt.ToUniversalTime();
        TrialEndsAt = trialEndsAt?.ToUniversalTime();
        ProviderSubscriptionReference = NullIfEmpty(providerSubscriptionReference, 200);
        CreatedAt = createdAt.ToUniversalTime();
        CreatedBy = TenantAccount.Text(createdBy, 240);
        Status = TrialEndsAt.HasValue ? SubscriptionStatus.Trialing : SubscriptionStatus.Active;
    }

    public Guid PlanVersionId { get; private set; }
    public SubscriptionStatus Status { get; private set; }
    public DateTimeOffset StartsAt { get; private set; }
    public DateTimeOffset CurrentPeriodStartsAt { get; private set; }
    public DateTimeOffset CurrentPeriodEndsAt { get; private set; }
    public DateTimeOffset? TrialEndsAt { get; private set; }
    public string? ProviderSubscriptionReference { get; private set; }
    public DateTimeOffset CreatedAt { get; private set; }
    public string CreatedBy { get; private set; } = "";
    public DateTimeOffset? CancelAt { get; private set; }
    public DateTimeOffset? EndedAt { get; private set; }
    public string? StatusReason { get; private set; }

    public void Activate(DateTimeOffset at)
    {
        if (Status is not (SubscriptionStatus.Trialing or SubscriptionStatus.PastDue or SubscriptionStatus.Suspended))
            throw new InvalidOperationException("Subscription cannot be activated from its current state.");
        Status = SubscriptionStatus.Active;
        TrialEndsAt = TrialEndsAt <= at ? at.ToUniversalTime() : TrialEndsAt;
        StatusReason = null;
    }

    public void MarkPastDue(string reason)
    {
        if (Status is not (SubscriptionStatus.Active or SubscriptionStatus.Trialing)) throw new InvalidOperationException("Only a current subscription can become past due.");
        Status = SubscriptionStatus.PastDue;
        StatusReason = TenantAccount.Text(reason, 1_000);
    }

    public void Suspend(string reason)
    {
        if (Status is not (SubscriptionStatus.Active or SubscriptionStatus.Trialing or SubscriptionStatus.PastDue)) throw new InvalidOperationException("Subscription cannot be suspended.");
        Status = SubscriptionStatus.Suspended;
        StatusReason = TenantAccount.Text(reason, 1_000);
    }

    public void Renew(DateTimeOffset periodStartsAt, DateTimeOffset periodEndsAt)
    {
        if (Status is not (SubscriptionStatus.Active or SubscriptionStatus.Trialing) || periodStartsAt < CurrentPeriodEndsAt || periodEndsAt <= periodStartsAt)
            throw new InvalidOperationException("Only a current subscription can advance to a valid next period.");
        CurrentPeriodStartsAt = periodStartsAt.ToUniversalTime();
        CurrentPeriodEndsAt = periodEndsAt.ToUniversalTime();
        if (TrialEndsAt <= periodStartsAt) Status = SubscriptionStatus.Active;
        if (CancelAt <= periodStartsAt) Cancel(periodStartsAt, "Scheduled cancellation reached.");
    }

    public void ScheduleCancellation(DateTimeOffset at)
    {
        if (Status is SubscriptionStatus.Cancelled or SubscriptionStatus.Expired || at < StartsAt)
            throw new InvalidOperationException("A terminal subscription cannot be scheduled for cancellation.");
        CancelAt = at.ToUniversalTime();
    }

    public void Cancel(DateTimeOffset at, string reason)
    {
        if (Status is SubscriptionStatus.Cancelled or SubscriptionStatus.Expired) throw new InvalidOperationException("Subscription is already terminal.");
        Status = SubscriptionStatus.Cancelled;
        EndedAt = at.ToUniversalTime();
        StatusReason = TenantAccount.Text(reason, 1_000);
    }

    private static string? NullIfEmpty(string? value, int maximum)
    {
        string text = TenantAccount.Optional(value, maximum);
        return text.Length == 0 ? null : text;
    }
}
