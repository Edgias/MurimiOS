using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.ChangeDisclosurePolicy;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ChangeDisclosurePolicyEndpoint
{
    internal static void Map(IEndpointRouteBuilder retail)
    {
        retail.MapPost("/policies/{policyId:guid}/{command:regex(^publish|retire$)}", async Task<IResult> (Guid policyId, string command, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.ChangeDisclosurePolicy.ChangeDisclosurePolicyCommand(policyId, command), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.RetailTransparencyManage);

    }
}
