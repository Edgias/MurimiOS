using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.CreateAllocation;
using static EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.SupplierWorkflowEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.CreateAllocation;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateAllocationEndpoint
{
    internal static void Map(IEndpointRouteBuilder portal)
    {
        portal.MapPost("/contracts/{contractId:guid}/allocations", async Task<IResult> (Guid contractId, CreateContractAllocationRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplierWorkflow.CreateAllocation.CreateAllocationCommand(contractId, new global::EdgIQ.MurimiOS.Application.SupplierWorkflow.CreateAllocation.CreateContractAllocationRequest(request.Id, request.Reference, request.DeliveryFrom, request.DeliveryTo, request.AllocatedQuantity, request.QuantityUnit)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SupplierPortalManage);

    }
}
