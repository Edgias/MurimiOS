using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PostEnterpriseApprovalPoliciesPolicyIdGuidRetire;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class PostEnterpriseApprovalPoliciesPolicyIdGuidRetireEndpoint
{
    internal static void Map(IEndpointRouteBuilder enterprise)
    {
        enterprise.MapPost("/approval-policies/{policyId:guid}/retire", async Task<IResult> (Guid policyId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.PostEnterpriseApprovalPoliciesPolicyIdGuidRetire.PostEnterpriseApprovalPoliciesPolicyIdGuidRetireCommand(policyId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.EnterpriseApprovalManage);

    }
}
