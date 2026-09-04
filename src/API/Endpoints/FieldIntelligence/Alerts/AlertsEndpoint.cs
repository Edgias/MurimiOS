using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;
using EdgIQ.MurimiOS.Domain.FieldInsightAlert;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using static EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.FieldIntelligenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Alerts;
using InsightAlertStatus = global::EdgIQ.MurimiOS.Domain.FieldInsightAlert.InsightAlertStatus;
using InsightSeverity = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.InsightSeverity;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class AlertsEndpoint
{
    internal static void Map(IEndpointRouteBuilder insights)
    {

        insights.MapGet("/alerts", async Task<IResult> (Guid? farmId, Guid? fieldId, InsightAlertStatus? status, InsightSeverity? minimumSeverity, int? limit, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FieldIntelligence.Alerts.AlertsQuery(farmId, fieldId, status, minimumSeverity, limit), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FieldIntelligenceRead);

    }
}
