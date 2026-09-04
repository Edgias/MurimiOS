using EdgIQ.MurimiOS.API.Infrastructure;
using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using EdgIQ.MurimiOS.Domain.FieldRiskRule;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using static EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.FieldIntelligenceEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.ListRiskRules;
using RiskRuleStatus = global::EdgIQ.MurimiOS.Domain.FieldRiskRule.RiskRuleStatus;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class ListRiskRulesEndpoint
{
    internal static void Map(IEndpointRouteBuilder insights)
    {

        insights.MapGet("/risk-rules", async Task<IResult> (RiskRuleStatus? status, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.FieldIntelligence.ListRiskRules.ListRiskRulesQuery(status), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.FieldIntelligenceRead);

    }
}
