using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropPlanning.ReplaceDraft;
using static EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CropPlanningEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.ReplaceDraft;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ReplaceDraftEndpoint
{
    internal static void Map(IEndpointRouteBuilder planning)
    {
        planning.MapPut("/plans/{planId:guid}/draft", async Task<IResult> (Guid planId, ReplaceCropPlanDraftRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropPlanning.ReplaceDraft.ReplaceDraftCommand(planId, new global::EdgIQ.MurimiOS.Application.CropPlanning.ReplaceDraft.ReplaceCropPlanDraftRequest(request.Allocations.Select(item => new global::EdgIQ.MurimiOS.Application.CropPlanning.ReplaceDraft.CropPlanAllocationDraft(item.Id, item.FieldId, item.BoundaryVersionId, item.CropZoneId, item.Crop, item.Variety, item.PlannedArea, item.AreaUnit, item.TargetYield, item.TargetYieldUnit, item.TargetPricePerCanonicalUnit, item.PlannedSowingDate, item.PlannedHarvestDate)).ToArray(), request.Operations.Select(item => new global::EdgIQ.MurimiOS.Application.CropPlanning.ReplaceDraft.PlannedCropOperationDraft(item.Id, item.AllocationId, item.OperationType, item.PlannedStart, item.PlannedEnd, item.Sequence, item.Notes)).ToArray(), request.Resources.Select(item => new global::EdgIQ.MurimiOS.Application.CropPlanning.ReplaceDraft.CropPlanResourceDraft(item.Id, item.AllocationId, item.PlannedOperationId, item.ResourceType, item.AgriculturalInputId, item.ResourceName, item.Quantity, item.Unit, item.UnitCostPerCanonicalUnit)).ToArray())), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.PlanningWrite);

    }
}
