using System.Data;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreateSubscription;
public sealed record CreateTenantSubscriptionRequest(Guid? Id, Guid PlanVersionId, DateTimeOffset StartsAt, DateTimeOffset CurrentPeriodEndsAt, DateTimeOffset? TrialEndsAt, string? ProviderSubscriptionReference);
