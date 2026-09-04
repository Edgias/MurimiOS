using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using static EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.FieldIntelligenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.AcknowledgeAlert;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class AcknowledgeAlertEndpoint
{
    internal static void Map(IEndpointRouteBuilder insights)
    {
        insights.MapPost("/alerts/{alertId:guid}/acknowledge", async Task<IResult> (Guid alertId, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FieldIntelligence.AcknowledgeAlert.AcknowledgeAlertCommand(alertId), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FieldIntelligenceWrite);

    }
}
