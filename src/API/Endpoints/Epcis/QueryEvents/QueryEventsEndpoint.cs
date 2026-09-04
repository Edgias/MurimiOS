using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using static EdgIQ.MurimiOS.API.Endpoints.Epcis.EpcisEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Epcis.QueryEvents;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class QueryEventsEndpoint
{
    internal static void Map(IEndpointRouteBuilder epcis)
    {
        epcis.MapGet("/events", async Task<IResult> (Guid farmId, string? eventType, DateTimeOffset? GE_eventTime, DateTimeOffset? LT_eventTime, DateTimeOffset? GE_recordTime, DateTimeOffset? LT_recordTime, string? EQ_action, string? EQ_bizStep, string? EQ_disposition, string? MATCH_anyEPC, int? perPage, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Epcis.QueryEvents.QueryEventsQuery(farmId, eventType, GE_eventTime, LT_eventTime, GE_recordTime, LT_recordTime, EQ_action, EQ_bizStep, EQ_disposition, MATCH_anyEPC, perPage), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.TraceabilityRead);

    }
}
