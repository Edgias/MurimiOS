
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Analytics;
using BenchmarkEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.Benchmark.BenchmarkEndpoint;
using CancelProjectionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.CancelProjection.CancelProjectionEndpoint;
using ChangeKpiEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.ChangeKpi.ChangeKpiEndpoint;
using CreateKpiEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateKpi.CreateKpiEndpoint;
using CreateProjectionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateProjection.CreateProjectionEndpoint;
using CreateRetentionPolicyEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateRetentionPolicy.CreateRetentionPolicyEndpoint;
using CreateScheduleEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateSchedule.CreateScheduleEndpoint;
using CropComparisonEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.CropComparison.CropComparisonEndpoint;
using DisableRetentionPolicyEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.DisableRetentionPolicy.DisableRetentionPolicyEndpoint;
using EnrollBenchmarkEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.EnrollBenchmark.EnrollBenchmarkEndpoint;
using FarmDashboardEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.FarmDashboard.FarmDashboardEndpoint;
using ListEnrollmentsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.ListEnrollments.ListEnrollmentsEndpoint;
using ListKpisEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.ListKpis.ListKpisEndpoint;
using ListProjectionsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.ListProjections.ListProjectionsEndpoint;
using ListRetentionPoliciesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.ListRetentionPolicies.ListRetentionPoliciesEndpoint;
using ListSchedulesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.ListSchedules.ListSchedulesEndpoint;
using OrganisationDashboardEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.OrganisationDashboard.OrganisationDashboardEndpoint;
using PauseScheduleEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.PauseSchedule.PauseScheduleEndpoint;
using QueryFactsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.QueryFacts.QueryFactsEndpoint;
using ResumeScheduleEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.ResumeSchedule.ResumeScheduleEndpoint;
using RevokeEnrollmentEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Analytics.RevokeEnrollment.RevokeEnrollmentEndpoint;


public static class AnalyticsEndpoints
{
    public static void MapAnalyticsEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder analytics = api.MapGroup("/analytics").WithTags("Analytics");
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.ListKpis.ListKpisEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateKpi.CreateKpiEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.ChangeKpi.ChangeKpiEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.ListProjections.ListProjectionsEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateProjection.CreateProjectionEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.CancelProjection.CancelProjectionEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.QueryFacts.QueryFactsEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.FarmDashboard.FarmDashboardEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.OrganisationDashboard.OrganisationDashboardEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.CropComparison.CropComparisonEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.ListEnrollments.ListEnrollmentsEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.EnrollBenchmark.EnrollBenchmarkEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.RevokeEnrollment.RevokeEnrollmentEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.Benchmark.BenchmarkEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.ListSchedules.ListSchedulesEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateSchedule.CreateScheduleEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.PauseSchedule.PauseScheduleEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.ResumeSchedule.ResumeScheduleEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.ListRetentionPolicies.ListRetentionPoliciesEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.CreateRetentionPolicy.CreateRetentionPolicyEndpoint.Map(analytics);
        global::EdgIQ.MurimiOS.API.Endpoints.Analytics.DisableRetentionPolicy.DisableRetentionPolicyEndpoint.Map(analytics);
    }
}
