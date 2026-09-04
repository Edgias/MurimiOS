using System.Data;
using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.CropPlanning.ReplaceDraft;
using AgriculturalInput = global::EdgIQ.MurimiOS.Domain.AgriculturalInput.AgriculturalInput;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;

using CropPlan = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlan;
using CropPlanAllocation = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanAllocation;
using CropPlanResourceBudget = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanResourceBudget;
using CropPlanStatus = global::EdgIQ.MurimiOS.Domain.CropPlan.CropPlanStatus;
using PlannedCropOperation = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedCropOperation;
using PlannedResourceType = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedResourceType;
using CropProductApplicationRule = global::EdgIQ.MurimiOS.Domain.CropProductApplicationRule.CropProductApplicationRule;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
public sealed record CropPlanAllocationDraft(Guid Id, Guid FieldId, Guid BoundaryVersionId, Guid? CropZoneId,
    string Crop, string? Variety, decimal PlannedArea, string AreaUnit, decimal TargetYield,
    string TargetYieldUnit, decimal TargetPricePerCanonicalUnit, DateOnly PlannedSowingDate, DateOnly PlannedHarvestDate);
