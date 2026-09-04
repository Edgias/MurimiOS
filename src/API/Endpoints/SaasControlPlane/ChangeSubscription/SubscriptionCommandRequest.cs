using System.Data;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ChangeSubscription;
public sealed record SubscriptionCommandRequest(string Reason, DateTimeOffset? PeriodStartsAt, DateTimeOffset? PeriodEndsAt, DateTimeOffset? CancelAt);
