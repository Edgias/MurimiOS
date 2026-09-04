using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantSubscription;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.CreatePlan;
using BillingInterval = global::EdgIQ.MurimiOS.Domain.TenantSubscription.BillingInterval;
public sealed record CreateSaasPlanRequest(Guid? Id, Guid? SeriesId, string Code, string Name, int PlanVersion, BillingInterval BillingInterval, decimal BasePrice, string Currency, DateTimeOffset EffectiveFrom, DateTimeOffset? EffectiveTo);
