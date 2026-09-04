using EdgIQ.MurimiOS.API.Infrastructure;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateKpi;
using static EdgIQ.MurimiOS.API.Endpoints.Analytics.AnalyticsEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateKpi;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateKpiEndpoint
{
    internal static void Map(IEndpointRouteBuilder analytics)
    {
        analytics.MapPost("/kpis", async Task<IResult> (CreateKpiDefinitionRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.Analytics.CreateKpi.CreateKpiCommand(new global::EdgIQ.MurimiOS.Application.Analytics.CreateKpi.CreateKpiDefinitionRequest(request.Id, request.SeriesId, request.Code, request.Name, request.Description, request.Formula, request.Unit, request.TimeGrain, request.Owner, request.Exclusions, request.QualityRule, request.Version)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AnalyticsManage);

    }
}
