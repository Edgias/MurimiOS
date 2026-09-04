using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.Common;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ChangeAllocation;
using static EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.SupplierWorkflowEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ChangeAllocation;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ChangeAllocationEndpoint
{
    internal static void Map(IEndpointRouteBuilder portal)
    {
        portal.MapPost("/contracts/{contractId:guid}/allocations/{allocationId:guid}/{command:regex(^fulfil|cancel$)}", async Task<IResult> (Guid contractId, Guid allocationId, string command, AllocationCommandRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplierWorkflow.ChangeAllocation.ChangeAllocationCommand(contractId, allocationId, command, new global::EdgIQ.MurimiOS.Application.SupplierWorkflow.ChangeAllocation.AllocationCommandRequest(request.Quantity)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SupplierPortalManage);

    }
}
