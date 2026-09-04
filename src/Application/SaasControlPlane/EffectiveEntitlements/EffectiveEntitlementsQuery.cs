using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.EffectiveEntitlements;
using SaasPlanVersion = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanVersion;

using PlanEntitlement = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.PlanEntitlement;
using SubscriptionStatus = global::EdgIQ.MurimiOS.Domain.TenantSubscription.SubscriptionStatus;
using TenantSubscription = global::EdgIQ.MurimiOS.Domain.TenantSubscription.TenantSubscription;
using UsageMeterPeriod = global::EdgIQ.MurimiOS.Domain.UsageMeterPeriod.UsageMeterPeriod;

public sealed record EffectiveEntitlementsQuery() : IRequest<UseCaseResult>;
