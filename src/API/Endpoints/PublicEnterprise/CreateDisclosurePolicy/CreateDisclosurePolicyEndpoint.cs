using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateDisclosurePolicy;
using static EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.PublicEnterpriseEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.PublicEnterprise.CreateDisclosurePolicy;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateDisclosurePolicyEndpoint
{
    internal static void Map(IEndpointRouteBuilder retail)
    {
        retail.MapPost("/policies", async Task<IResult> (CreateDisclosurePolicyRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.CreateDisclosurePolicy.CreateDisclosurePolicyCommand(new global::EdgIQ.MurimiOS.Application.PublicEnterprise.CreateDisclosurePolicy.CreateDisclosurePolicyRequest(request.Id, request.Name, request.PublicBrandName, request.Attributes, request.FarmIds, request.DataGrantId, request.PreviousPolicyId, request.Locale)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.RetailTransparencyManage);

    }
}
