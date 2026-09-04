using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ChangeEvidenceRequest;
using static EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.SupplierWorkflowEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ChangeEvidenceRequest;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ChangeEvidenceRequestEndpoint
{
    internal static void Map(IEndpointRouteBuilder portal)
    {
        portal.MapPost("/evidence-requests/{requestId:guid}/{command:regex(^submit|accept|reject|cancel$)}", async Task<IResult> (Guid requestId, string command, SupplierEvidenceCommandRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplierWorkflow.ChangeEvidenceRequest.ChangeEvidenceRequestCommand(requestId, command, new global::EdgIQ.MurimiOS.Application.SupplierWorkflow.ChangeEvidenceRequest.SupplierEvidenceCommandRequest(request.EvidenceAttachmentId, request.SupplierReference, request.Notes)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SupplierPortalManage);

    }
}
