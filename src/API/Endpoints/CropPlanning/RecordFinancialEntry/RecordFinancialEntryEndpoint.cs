using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.CropPlanning.RecordFinancialEntry;
using static EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CropPlanningEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning.RecordFinancialEntry;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class RecordFinancialEntryEndpoint
{
    internal static void Map(IEndpointRouteBuilder planning)
    {
        planning.MapPost("/plans/{planId:guid}/financial-entries", async Task<IResult> (Guid planId, RecordCropPlanFinancialEntryRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.CropPlanning.RecordFinancialEntry.RecordFinancialEntryCommand(planId, new global::EdgIQ.MurimiOS.Application.CropPlanning.RecordFinancialEntry.RecordCropPlanFinancialEntryRequest(request.Id, request.AllocationId, request.EntryType, request.Category, request.Amount, request.Currency, request.OccurredAt, request.Reference, request.Notes, request.CostCentreId)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.PlanningWrite);

    }
}
