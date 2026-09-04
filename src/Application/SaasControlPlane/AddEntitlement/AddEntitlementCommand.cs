using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SaasControlPlane.AddEntitlement;
using EntitlementEnforcement = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.EntitlementEnforcement;
using EntitlementKind = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.EntitlementKind;
using PlanEntitlement = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.PlanEntitlement;
using SaasPlanStatus = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanStatus;
using SaasPlanVersion = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanVersion;

public sealed record AddEntitlementCommand(Guid planId, CreatePlanEntitlementRequest request) : IRequest<UseCaseResult>;
