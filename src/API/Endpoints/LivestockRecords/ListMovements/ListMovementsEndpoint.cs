using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.LivestockRecordsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.ListMovements;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListMovementsEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapGet("/livestock/subjects/{subjectId:guid}/movements", async Task<IResult> (Guid subjectId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockRecords.ListMovements.ListMovementsQuery(subjectId), cancellationToken)).ToHttpResult()).WithTags("Livestock").RequireAuthorization(PlatformPermissions.LivestockRead);

    }
}
