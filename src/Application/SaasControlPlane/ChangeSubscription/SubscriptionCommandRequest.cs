using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.ChangeSubscription;
using TenantSubscription = global::EdgIQ.MurimiOS.Domain.TenantSubscription.TenantSubscription;
public sealed record SubscriptionCommandRequest(string Reason, DateTimeOffset? PeriodStartsAt, DateTimeOffset? PeriodEndsAt, DateTimeOffset? CancelAt);
