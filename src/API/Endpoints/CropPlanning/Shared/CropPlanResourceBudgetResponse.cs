using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.CropPlan;
using EdgIQ.MurimiOS.Domain.Common;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Shared;
using PlannedResourceType = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedResourceType;
public sealed record CropPlanResourceBudgetResponse(Guid Id, Guid PlanId, Guid AllocationId,
    Guid? PlannedOperationId, PlannedResourceType ResourceType, Guid? AgriculturalInputId,
    string ResourceName, decimal Quantity, string Unit, decimal EnteredQuantity, string EnteredUnit,
    decimal UnitCostPerCanonicalUnit, string Currency, decimal PlannedCost);
