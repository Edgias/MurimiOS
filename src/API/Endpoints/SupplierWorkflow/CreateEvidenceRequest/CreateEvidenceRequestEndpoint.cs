using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.CreateEvidenceRequest;
using static EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.SupplierWorkflowEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.CreateEvidenceRequest;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateEvidenceRequestEndpoint
{
    internal static void Map(IEndpointRouteBuilder portal)
    {
        portal.MapPost("/evidence-requests", async Task<IResult> (CreateSupplierEvidenceRequestRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplierWorkflow.CreateEvidenceRequest.CreateEvidenceRequestCommand(new global::EdgIQ.MurimiOS.Application.SupplierWorkflow.CreateEvidenceRequest.CreateSupplierEvidenceRequestRequest(request.Id, request.RelationshipId, request.HostedSupplierId, request.ContractId, request.StandardVersionId, request.ControlPointId, request.Title, request.Description, request.DueOn)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SupplierPortalManage);

    }
}
