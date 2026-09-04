using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Finance.RecordLabour;
using static EdgIQ.MurimiOS.API.Endpoints.Finance.FinanceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Finance.RecordLabour;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class RecordLabourEndpoint
{
    internal static void Map(IEndpointRouteBuilder finance)
    {
        finance.MapPost("/farms/{farmId:guid}/labour-entries", async Task<IResult> (Guid farmId, RecordLabourCostRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Finance.RecordLabour.RecordLabourCommand(farmId, new global::EdgIQ.MurimiOS.Application.Finance.RecordLabour.RecordLabourCostRequest(request.Id, request.TransactionId, request.CostCentreId, request.FieldOperationId, request.CropZoneId, request.LivestockSubjectId, request.WorkerReference, request.OccurredAt, request.DurationHours, request.HourlyRate, request.Currency, request.Reference, request.Notes)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FinanceWrite);

    }
}
