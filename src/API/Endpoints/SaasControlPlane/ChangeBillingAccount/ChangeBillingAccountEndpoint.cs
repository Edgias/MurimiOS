using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.SaasControlPlaneEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.ChangeBillingAccount;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ChangeBillingAccountEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapPost("/billing/accounts/{accountId:guid}/{command:regex(^activate|suspend|close$)}", async Task<IResult> (Guid accountId, string command, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.SaasControlPlane.ChangeBillingAccount.ChangeBillingAccountCommand(accountId, command), cancellationToken)).ToHttpResult())
            .RequireAuthorization(PlatformPermissions.SaasBillingManage, PlatformOperatorPolicy);

    }
}
