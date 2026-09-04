using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Finance.ReconcileAccounting;
using static EdgIQ.MurimiOS.API.Endpoints.Finance.FinanceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Finance.ReconcileAccounting;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ReconcileAccountingEndpoint
{
    internal static void Map(IEndpointRouteBuilder finance)
    {
        finance.MapPost("/accounting-imports/{batchId:guid}/reconcile", async Task<IResult> (Guid batchId, ReconcileAccountingRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Finance.ReconcileAccounting.ReconcileAccountingCommand(batchId, new global::EdgIQ.MurimiOS.Application.Finance.ReconcileAccounting.ReconcileAccountingRequest(request.AmountTolerance, request.DateWindowDays)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FinanceReconcile);

    }
}
