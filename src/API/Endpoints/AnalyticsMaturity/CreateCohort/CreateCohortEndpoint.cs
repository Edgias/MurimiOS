using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.CreateCohort;
using static EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.AnalyticsMaturityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.CreateCohort;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateCohortEndpoint
{
    internal static void Map(IEndpointRouteBuilder analytics)
    {
        analytics.MapPost("/benchmark-cohorts", async Task<IResult> (CreateBenchmarkCohortRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.AnalyticsMaturity.CreateCohort.CreateCohortCommand(new global::EdgIQ.MurimiOS.Application.AnalyticsMaturity.CreateCohort.CreateBenchmarkCohortRequest(request.Id, request.CohortKey, request.DefinitionVersion, request.Name, request.Description, request.InclusionCriteriaJson, request.MinimumCohortSize, request.EffectiveFrom, request.EffectiveTo, request.Owner)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AnalyticsManage);

    }
}
