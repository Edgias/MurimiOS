using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.FieldIntelligenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.ResolveAlert;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using AlertDecisionRequest = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Shared.AlertDecisionRequest;
internal static class ResolveAlertEndpoint
{
    internal static void Map(IEndpointRouteBuilder insights)
    {
        insights.MapPost("/alerts/{alertId:guid}/resolve", async Task<IResult> (Guid alertId, AlertDecisionRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FieldIntelligence.ResolveAlert.ResolveAlertCommand(alertId, new global::EdgIQ.MurimiOS.Application.FieldIntelligence.ResolveAlert.AlertDecisionRequest(request.Reason)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FieldIntelligenceWrite);

    }
}
