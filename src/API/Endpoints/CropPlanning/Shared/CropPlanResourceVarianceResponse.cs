using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.CropPlan;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Shared;
using PlannedResourceType = global::EdgIQ.MurimiOS.Domain.CropPlan.PlannedResourceType;
public sealed record CropPlanResourceVarianceResponse(Guid ResourceBudgetId, PlannedResourceType ResourceType,
    string ResourceName, Guid? AgriculturalInputId, decimal PlannedQuantity, decimal? ActualQuantity,
    decimal? QuantityVariance, string Unit, decimal PlannedCost, string Currency);
