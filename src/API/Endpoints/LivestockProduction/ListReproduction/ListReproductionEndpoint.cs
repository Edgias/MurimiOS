using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.LivestockProductionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.ListReproduction;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListReproductionEndpoint
{
    internal static void Map(IEndpointRouteBuilder livestock)
    {
        livestock.MapGet("/subjects/{subjectId:guid}/reproduction-events", async Task<IResult> (Guid subjectId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockProduction.ListReproduction.ListReproductionQuery(subjectId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.LivestockRead);

    }
}
