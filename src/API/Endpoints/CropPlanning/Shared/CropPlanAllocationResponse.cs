using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Shared;
public sealed record CropPlanAllocationResponse(Guid Id, Guid PlanId, Guid FieldId, Guid BoundaryVersionId,
    Guid? CropZoneId, string Crop, string? Variety, decimal PlannedAreaHa, decimal EnteredArea,
    string EnteredAreaUnit, decimal TargetYield, string TargetYieldUnit, decimal EnteredTargetYield,
    string EnteredTargetYieldUnit, decimal TargetPricePerCanonicalUnit, string Currency,
    decimal PlannedRevenue, DateOnly PlannedSowingDate, DateOnly PlannedHarvestDate);
