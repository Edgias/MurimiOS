using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.SaasControlPlaneEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ChangeAccount;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using LifecycleReasonRequest = global::EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.Shared.LifecycleReasonRequest;
internal static class ChangeAccountEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/account/{command:regex(^activate|resume|suspend|close$)}", async Task<IResult> (string command, LifecycleReasonRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.ChangeAccount.ChangeAccountCommand(command, new global::EdgIQ.MurimiOS.Application.SaasControlPlane.ChangeAccount.LifecycleReasonRequest(request.Reason)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SaasManage, PlatformOperatorPolicy);

    }
}
