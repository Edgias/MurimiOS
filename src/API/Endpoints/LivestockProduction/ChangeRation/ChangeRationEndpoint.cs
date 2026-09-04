using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.LivestockProductionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.ChangeRation;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ChangeRationEndpoint
{
    internal static void Map(IEndpointRouteBuilder livestock)
    {
        livestock.MapPost("/ration-plans/{planId:guid}/{command:regex(^activate|retire$)}", async Task<IResult> (Guid planId, string command, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockProduction.ChangeRation.ChangeRationCommand(planId, command), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}
