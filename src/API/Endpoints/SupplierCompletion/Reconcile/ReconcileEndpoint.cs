using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.Reconcile;
using static EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.SupplierCompletionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.Reconcile;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ReconcileEndpoint
{
    internal static void Map(IEndpointRouteBuilder portal)
    {
        portal.MapPost("/contracts/{contractId:guid}/reconciliations", async Task<IResult> (Guid contractId, ReconcileContractRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplierCompletion.Reconcile.ReconcileCommand(contractId, new global::EdgIQ.MurimiOS.Application.SupplierCompletion.Reconcile.ReconcileContractRequest(request.Id, request.Reference, request.AcceptedQuantity, request.RejectedQuantity, request.QualityGrade, request.QualityAdjustmentAmount, request.OtherAdjustmentAmount, request.EffectiveOn, request.Notes)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SupplierPortalManage);

    }
}
