using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateApprovalPolicy;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateApprovalPolicy;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateApprovalPolicyEndpoint
{
    internal static void Map(IEndpointRouteBuilder enterprise)
    {
        enterprise.MapPost("/approval-policies", async Task<IResult> (CreateApprovalPolicyRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.CreateApprovalPolicy.CreateApprovalPolicyCommand(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.CreateApprovalPolicy.CreateApprovalPolicyRequest(request.Id, request.Code, request.Name, request.OrganisationNodeId, request.ActionType, request.ApproverPermission, request.RequiredApprovals)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.EnterpriseApprovalManage);

    }
}
