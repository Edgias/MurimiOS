using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateMovement;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.LivestockRecordsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateMovement;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateMovementEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/livestock/subjects/{subjectId:guid}/movements", async Task<IResult> (Guid subjectId, CreateLivestockMovementRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockRecords.CreateMovement.CreateMovementCommand(subjectId, new global::EdgIQ.MurimiOS.Application.LivestockRecords.CreateMovement.CreateLivestockMovementRequest(request.Id, request.Type, request.FromHoldingId, request.ToHoldingId, request.OccurredAt, request.Reference, request.Notes)), cancellationToken)).ToHttpResult()).WithTags("Livestock").RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}
