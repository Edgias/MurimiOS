using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.DecideApproval;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.DecideApproval;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class DecideApprovalEndpoint
{
    internal static void Map(IEndpointRouteBuilder enterprise)
    {
        enterprise.MapPost("/approval-requests/{requestId:guid}/decisions", async Task<IResult> (Guid requestId, ApprovalDecisionRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.DecideApproval.DecideApprovalCommand(requestId, new global::EdgIQ.MurimiOS.Application.PublicEnterprise.DecideApproval.ApprovalDecisionRequest(request.Decision, request.Reason)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.EnterpriseApprove);

    }
}
