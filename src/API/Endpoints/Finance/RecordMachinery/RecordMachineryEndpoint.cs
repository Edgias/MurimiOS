using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Finance.RecordMachinery;
using static EdgIQ.MurimiOS.API.Endpoints.Finance.FinanceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Finance.RecordMachinery;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class RecordMachineryEndpoint
{
    internal static void Map(IEndpointRouteBuilder finance)
    {
        finance.MapPost("/farms/{farmId:guid}/machinery-entries", async Task<IResult> (Guid farmId, RecordMachineryCostRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Finance.RecordMachinery.RecordMachineryCommand(farmId, new global::EdgIQ.MurimiOS.Application.Finance.RecordMachinery.RecordMachineryCostRequest(request.Id, request.TransactionId, request.CostCentreId, request.FieldOperationId, request.CropZoneId, request.LivestockSubjectId, request.MachineReference, request.OccurredAt, request.DurationHours, request.HourlyRate, request.AdditionalCost, request.Currency, request.Reference, request.Notes)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FinanceWrite);

    }
}
