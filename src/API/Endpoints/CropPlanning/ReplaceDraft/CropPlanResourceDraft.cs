using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.CropPlan;
using EdgIQ.MurimiOS.Domain.Common;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.ReplaceDraft;
using PlannedResourceType = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedResourceType;
public sealed record CropPlanResourceDraft(Guid Id, Guid AllocationId, Guid? PlannedOperationId,
    PlannedResourceType ResourceType, Guid? AgriculturalInputId, string ResourceName,
    decimal Quantity, string Unit, decimal UnitCostPerCanonicalUnit);
