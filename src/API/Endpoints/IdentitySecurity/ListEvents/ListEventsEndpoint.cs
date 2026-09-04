using EdgIQ.MurimiOS.API.Infrastructure;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.SecurityEvent;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Traceability.ListEvents;
using static EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.IdentitySecurityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.IdentitySecurity.ListEvents;
using SecurityEventOutcome = global::EdgIQ.MurimiOS.Domain.SecurityEvent.SecurityEventOutcome;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListEventsEndpoint
{
    internal static void Map(IEndpointRouteBuilder group)
    {
        group.MapGet("/events", async Task<IResult> (string? eventType, SecurityEventOutcome? outcome, DateTimeOffset? from, DateTimeOffset? to, int? limit, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.IdentitySecurity.ListEvents.ListEventsQuery(eventType, outcome, from, to, limit), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.SecurityRead);

    }
}
