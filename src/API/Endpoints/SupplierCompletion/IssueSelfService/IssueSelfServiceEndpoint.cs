using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.IssueSelfService;
using static EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.SupplierCompletionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SupplierCompletion.IssueSelfService;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class IssueSelfServiceEndpoint
{
    internal static void Map(IEndpointRouteBuilder portal)
    {
        portal.MapPost("/suppliers/{supplierId:guid}/self-service-actions", async Task<IResult> (Guid supplierId, IssueSupplierSelfServiceRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SupplierCompletion.IssueSelfService.IssueSelfServiceCommand(supplierId, new global::EdgIQ.MurimiOS.Application.SupplierCompletion.IssueSelfService.IssueSupplierSelfServiceRequest(request.Action, request.ClaimedTenantId, request.ExpiresAt)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SupplierPortalManage);

    }
}
