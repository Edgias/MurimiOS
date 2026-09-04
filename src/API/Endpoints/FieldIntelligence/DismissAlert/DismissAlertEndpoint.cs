using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Shared;
using static EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.FieldIntelligenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.DismissAlert;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
using AlertDecisionRequest = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Shared.AlertDecisionRequest;
internal static class DismissAlertEndpoint
{
    internal static void Map(IEndpointRouteBuilder insights)
    {
        insights.MapPost("/alerts/{alertId:guid}/dismiss", async Task<IResult> (Guid alertId, AlertDecisionRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FieldIntelligence.DismissAlert.DismissAlertCommand(alertId, new global::EdgIQ.MurimiOS.Application.FieldIntelligence.DismissAlert.AlertDecisionRequest(request.Reason)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FieldIntelligenceWrite);

    }
}
