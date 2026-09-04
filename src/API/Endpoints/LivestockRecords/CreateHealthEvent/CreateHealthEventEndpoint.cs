using EdgIQ.MurimiOS.API.Infrastructure;
using System.Data;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateHealthEvent;
using static EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.LivestockRecordsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.CreateHealthEvent;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateHealthEventEndpoint
{
    internal static void Map(IEndpointRouteBuilder api)
    {
        api.MapPost("/livestock/subjects/{subjectId:guid}/health-events", async Task<IResult> (Guid subjectId, CreateLivestockHealthEventRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.LivestockRecords.CreateHealthEvent.CreateHealthEventCommand(subjectId, new global::EdgIQ.MurimiOS.Application.LivestockRecords.CreateHealthEvent.CreateLivestockHealthEventRequest(request.Id, request.Type, request.Title, request.OccurredAt, request.Diagnosis, request.Practitioner, request.Notes)), cancellationToken)).ToHttpResult()).WithTags("Livestock").RequireAuthorization(PlatformPermissions.LivestockWrite);

    }
}
