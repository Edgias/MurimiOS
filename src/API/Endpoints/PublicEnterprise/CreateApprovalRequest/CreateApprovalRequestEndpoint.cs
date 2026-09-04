using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateApprovalRequest;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateApprovalRequest;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateApprovalRequestEndpoint
{
    internal static void Map(IEndpointRouteBuilder enterprise)
    {
        enterprise.MapPost("/approval-requests", async Task<IResult> (CreateDelegatedApprovalRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.CreateApprovalRequest.CreateApprovalRequestCommand(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.CreateApprovalRequest.CreateDelegatedApprovalRequest(request.Id, request.PolicyId, request.FarmId, request.TargetType, request.TargetId, request.ActionType, request.RequestJson, request.ExpiresAt)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.EnterpriseApprovalManage);

    }
}
