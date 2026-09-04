using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.SupplierWorkflowEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ChangeContract;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using SupplierCommandRequest = global::EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.Shared.SupplierCommandRequest;
internal static class ChangeContractEndpoint
{
    internal static void Map(IEndpointRouteBuilder portal)
    {
        portal.MapPost("/contracts/{contractId:guid}/{command:regex(^activate|complete|terminate$)}", async Task<IResult> (Guid contractId, string command, SupplierCommandRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplierWorkflow.ChangeContract.ChangeContractCommand(contractId, command, new global::EdgIQ.MurimiOS.Application.SupplierWorkflow.ChangeContract.SupplierCommandRequest(request.Notes, request.ClaimedTenantId)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SupplierPortalManage);

    }
}
