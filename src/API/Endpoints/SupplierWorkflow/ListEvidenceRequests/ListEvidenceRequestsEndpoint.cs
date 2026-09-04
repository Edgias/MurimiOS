using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.SupplierWorkflowEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplierWorkflow.ListEvidenceRequests;
using SupplierEvidenceRequestStatus = global::EdgIQ.MurimiOS.Domain.SupplierEvidenceRequest.SupplierEvidenceRequestStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListEvidenceRequestsEndpoint
{
    internal static void Map(IEndpointRouteBuilder portal)
    {

        portal.MapGet("/evidence-requests", async Task<IResult> (SupplierEvidenceRequestStatus? status, Guid? hostedSupplierId, Guid? relationshipId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplierWorkflow.ListEvidenceRequests.ListEvidenceRequestsQuery(status, hostedSupplierId, relationshipId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SupplierPortalRead);

    }
}
