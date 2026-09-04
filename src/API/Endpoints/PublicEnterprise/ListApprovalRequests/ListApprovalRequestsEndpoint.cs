using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ListApprovalRequests;
using DelegatedApprovalStatus = global::EdgIQ.MurimiOS.Domain.DelegatedApprovalRequest.DelegatedApprovalStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListApprovalRequestsEndpoint
{
    internal static void Map(IEndpointRouteBuilder enterprise)
    {
        enterprise.MapGet("/approval-requests", async Task<IResult> (DelegatedApprovalStatus? status, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.ListApprovalRequests.ListApprovalRequestsQuery(status), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.EnterpriseApprove);

    }
}
