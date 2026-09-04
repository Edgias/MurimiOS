using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.IssueTransparencyToken;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.IssueTransparencyToken;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class IssueTransparencyTokenEndpoint
{
    internal static void Map(IEndpointRouteBuilder retail)
    {
        retail.MapPost("/policies/{policyId:guid}/tokens", async Task<IResult> (Guid policyId, IssueTransparencyTokenRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.IssueTransparencyToken.IssueTransparencyTokenCommand(policyId, new global::EdgIQ.MurimiOS.Application.PublicEnterprise.IssueTransparencyToken.IssueTransparencyTokenRequest(request.Id, request.HarvestLotId, request.ExpiresAt)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.RetailTransparencyManage);

    }
}
