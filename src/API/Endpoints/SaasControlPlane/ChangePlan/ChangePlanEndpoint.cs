using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.SaasControlPlaneEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ChangePlan;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ChangePlanEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/plans/{planId:guid}/{command:regex(^publish|retire$)}", async Task<IResult> (Guid planId, string command, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.ChangePlan.ChangePlanCommand(planId, command), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SaasManage, PlatformOperatorPolicy);

    }
}
