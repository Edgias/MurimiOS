using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.ReplaceDraft;
public sealed record CropPlanAllocationDraft(Guid Id, Guid FieldId, Guid BoundaryVersionId, Guid? CropZoneId,
    string Crop, string? Variety, decimal PlannedArea, string AreaUnit, decimal TargetYield,
    string TargetYieldUnit, decimal TargetPricePerCanonicalUnit, DateOnly PlannedSowingDate, DateOnly PlannedHarvestDate);
