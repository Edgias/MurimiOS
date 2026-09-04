using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.SaasPlanVersion;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.AddEntitlement;
using EntitlementEnforcement = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.EntitlementEnforcement;
using EntitlementKind = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.EntitlementKind;
public sealed record CreatePlanEntitlementRequest(Guid? Id, string Key, EntitlementKind Kind, decimal? IncludedAmount, string Unit, EntitlementEnforcement Enforcement);
