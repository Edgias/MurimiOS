using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.ReplaceDraft;
public sealed record ReplaceCropPlanDraftRequest(IReadOnlyList<CropPlanAllocationDraft> Allocations,
    IReadOnlyList<PlannedCropOperationDraft> Operations, IReadOnlyList<CropPlanResourceDraft> Resources);
