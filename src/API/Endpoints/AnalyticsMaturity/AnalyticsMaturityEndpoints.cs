using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity;
using ApproveCohortEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.ApproveCohort.ApproveCohortEndpoint;
using ApproveModelEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.ApproveModel.ApproveModelEndpoint;
using CreateCohortEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.CreateCohort.CreateCohortEndpoint;
using CreateModelEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.CreateModel.CreateModelEndpoint;
using ForecastAccuracyEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.ForecastAccuracy.ForecastAccuracyEndpoint;
using ListCohortsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.ListCohorts.ListCohortsEndpoint;
using ListDimensionsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.ListDimensions.ListDimensionsEndpoint;
using ListForecastsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.ListForecasts.ListForecastsEndpoint;
using ListModelsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.ListModels.ListModelsEndpoint;
using ListRefreshRequestsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.ListRefreshRequests.ListRefreshRequestsEndpoint;
using RetireCohortEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.RetireCohort.RetireCohortEndpoint;
using RetireModelEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.RetireModel.RetireModelEndpoint;


public static class AnalyticsMaturityEndpoints
{
    public static void Map(RouteGroupBuilder api)
    {
        RouteGroupBuilder analytics = api.MapGroup("/analytics").WithTags("Analytics maturity");
        global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.ListCohorts.ListCohortsEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.CreateCohort.CreateCohortEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.ApproveCohort.ApproveCohortEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.RetireCohort.RetireCohortEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.ListModels.ListModelsEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.CreateModel.CreateModelEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.ApproveModel.ApproveModelEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.RetireModel.RetireModelEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.ListRefreshRequests.ListRefreshRequestsEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.ListDimensions.ListDimensionsEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.ListForecasts.ListForecastsEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.AnalyticsMaturity.ForecastAccuracy.ForecastAccuracyEndpoint.Map(analytics);
    }
}
