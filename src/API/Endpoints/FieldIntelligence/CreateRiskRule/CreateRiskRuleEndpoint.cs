using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.ScoutingObservation;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.CreateRiskRule;
using static EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.FieldIntelligenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.CreateRiskRule;
using ScoutingCategory = global::EdgIQ.MurimiOS.Domain.ScoutingObservation.ScoutingCategory;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateRiskRuleEndpoint
{
    internal static void Map(IEndpointRouteBuilder insights)
    {
        insights.MapPost("/risk-rules", async Task<IResult> (CreateFieldRiskRuleRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FieldIntelligence.CreateRiskRule.CreateRiskRuleCommand(new global::EdgIQ.MurimiOS.Application.FieldIntelligence.CreateRiskRule.CreateFieldRiskRuleRequest(request.Id, request.RuleKey, request.RuleVersion, request.Name, request.Crop, request.Metric, request.Aggregation, request.Comparison, request.ScoutingCategory, request.IssueCode, request.Threshold, request.LookbackHours, request.MinimumSamples, request.Severity, request.Message, request.EffectiveFrom, request.EffectiveTo)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FieldIntelligenceEvaluate);

    }
}
