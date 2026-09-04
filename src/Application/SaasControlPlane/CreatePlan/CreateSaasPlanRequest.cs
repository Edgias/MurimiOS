using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantSubscription;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.CreatePlan;
using TenantSubscription = global::EdgIQ.MurimiOS.Domain.TenantSubscription.TenantSubscription;

using SaasPlanVersion = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanVersion;
using BillingInterval = global::EdgIQ.MurimiOS.Domain.TenantSubscription.BillingInterval;
public sealed record CreateSaasPlanRequest(Guid? Id, Guid? SeriesId, string Code, string Name, int PlanVersion, BillingInterval BillingInterval, decimal BasePrice, string Currency, DateTimeOffset EffectiveFrom, DateTimeOffset? EffectiveTo);
