using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Shared;
public sealed record PlannedCropOperationResponse(Guid Id, Guid PlanId, Guid AllocationId, string OperationType,
    DateOnly PlannedStart, DateOnly PlannedEnd, int Sequence, string? Notes);
