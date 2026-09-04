using EdgIQ.MurimiOS.API.Infrastructure;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.TenantMembership;
using Microsoft.EntityFrameworkCore;
using EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.CreateModel;
using static EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.AnalyticsMaturityEndpoints;
namespace EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.CreateModel;
using PlatformPermissions = global::EdgIQ.MurimiOS.Domain.TenantMembership.PlatformPermissions;
internal static class CreateModelEndpoint
{
    internal static void Map(IEndpointRouteBuilder analytics)
    {
        analytics.MapPost("/predictive-models", async Task<IResult> (CreatePredictiveModelRequest request, EdgIQ.Dispatcher.ISender sender, CancellationToken cancellationToken) => (await sender.Send(new global::EdgIQ.MurimiOS.Application.AnalyticsMaturity.CreateModel.CreateModelCommand(new global::EdgIQ.MurimiOS.Application.AnalyticsMaturity.CreateModel.CreatePredictiveModelRequest(request.Id, request.ModelKey, request.ModelVersion, request.Name, request.TargetMetricCode, request.Algorithm, request.FeatureSchemaJson, request.TrainingWindowDays, request.MinimumObservations, request.ValidationMae, request.ValidationMapePercent, request.ValidationDatasetReference)), cancellationToken)).ToHttpResult()).RequireAuthorization(PlatformPermissions.AnalyticsManage);

    }
}
