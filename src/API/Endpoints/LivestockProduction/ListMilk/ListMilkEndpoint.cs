using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using PlatformLivestockTreatment = EdgIQ.MurimiOS.Domain.LivestockTreatment.LivestockTreatment;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.LivestockProductionEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockProduction.ListMilk;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListMilkEndpoint
{
    internal static void Map(IEndpointRouteBuilder livestock)
    {
        livestock.MapGet("/subjects/{subjectId:guid}/milk-records", async Task<IResult> (Guid subjectId, DateTimeOffset? from, DateTimeOffset? to, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockProduction.ListMilk.ListMilkQuery(subjectId, from, to), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.LivestockRead);

    }
}
