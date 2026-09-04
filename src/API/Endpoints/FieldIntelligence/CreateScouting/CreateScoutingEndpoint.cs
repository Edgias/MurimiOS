using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.CreateScouting;
using static EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.FieldIntelligenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.CreateScouting;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateScoutingEndpoint
{
    internal static void Map(IEndpointRouteBuilder insights)
    {
        insights.MapPost("/fields/{fieldId:guid}/scouting", async Task<IResult> (Guid fieldId, CreateScoutingObservationRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FieldIntelligence.CreateScouting.CreateScoutingCommand(fieldId, new global::EdgIQ.MurimiOS.Application.FieldIntelligence.CreateScouting.CreateScoutingObservationRequest(request.Id, request.BoundaryVersionId, request.CropZoneId, request.Category, request.Severity, request.IssueCode, request.Title, request.Notes, request.ObservedAt, request.Latitude, request.Longitude, request.EvidenceId, request.SourceCommandId)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FieldIntelligenceWrite);

    }
}
