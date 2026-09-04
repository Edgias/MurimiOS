using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.ReplaceDraft;
public sealed record PlannedCropOperationDraft(Guid Id, Guid AllocationId, string OperationType,
    DateOnly PlannedStart, DateOnly PlannedEnd, int Sequence, string? Notes);
