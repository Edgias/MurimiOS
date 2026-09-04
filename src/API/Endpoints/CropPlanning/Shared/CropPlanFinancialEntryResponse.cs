using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.CropPlan;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Shared;
using FinancialEntryType = global::EdgIQ.MurimiOS.Domain.CropPlan.FinancialEntryType;
public sealed record CropPlanFinancialEntryResponse(Guid Id, Guid PlanId, Guid? AllocationId,
    FinancialEntryType EntryType, string Category, decimal Amount, string Currency,
    DateTimeOffset OccurredAt, string Reference, string? Notes, string RecordedBy);
