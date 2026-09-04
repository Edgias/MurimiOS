using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.SaasPlanVersion;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.SaasControlPlaneEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ListPlans;
using SaasPlanStatus = global::EdgIQ.MurimiOS.Domain.SaasPlanVersion.SaasPlanStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListPlansEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {

        group.MapGet("/plans", async Task<IResult> (SaasPlanStatus? status, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.ListPlans.ListPlansQuery(status), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SaasRead);

    }
}
