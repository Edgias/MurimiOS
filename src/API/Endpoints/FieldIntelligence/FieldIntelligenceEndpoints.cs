using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;

namespace EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence;
using AcknowledgeAlertEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.AcknowledgeAlert.AcknowledgeAlertEndpoint;
using ActivateRiskRuleEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.ActivateRiskRule.ActivateRiskRuleEndpoint;
using AlertsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Alerts.AlertsEndpoint;
using CreateFollowUpWorkOrderEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.CreateFollowUpWorkOrder.CreateFollowUpWorkOrderEndpoint;
using CreateRiskRuleEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.CreateRiskRule.CreateRiskRuleEndpoint;
using CreateScoutingEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.CreateScouting.CreateScoutingEndpoint;
using DismissAlertEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.DismissAlert.DismissAlertEndpoint;
using EvaluateRisksEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.EvaluateRisks.EvaluateRisksEndpoint;
using IngestSatelliteEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.IngestSatellite.IngestSatelliteEndpoint;
using IngestWeatherForecastsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.IngestWeatherForecasts.IngestWeatherForecastsEndpoint;
using IngestWeatherObservationsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.IngestWeatherObservations.IngestWeatherObservationsEndpoint;
using ListRiskRulesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.ListRiskRules.ListRiskRulesEndpoint;
using MapLayersEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.MapLayers.MapLayersEndpoint;
using ResolveAlertEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.ResolveAlert.ResolveAlertEndpoint;
using RetireRiskRuleEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.RetireRiskRule.RetireRiskRuleEndpoint;
using RiskAssessmentsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.RiskAssessments.RiskAssessmentsEndpoint;
using SatelliteEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Satellite.SatelliteEndpoint;
using ScoutingEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Scouting.ScoutingEndpoint;
using WeatherEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Weather.WeatherEndpoint;


public static class FieldIntelligenceEndpoints
{

    public static void MapFieldIntelligenceEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder insights = api.MapGroup("/field-intelligence").WithTags("Real-time Field Intelligence");
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Overview.OverviewEndpoint.Map(insights);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Weather.WeatherEndpoint.Map(insights);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Satellite.SatelliteEndpoint.Map(insights);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Scouting.ScoutingEndpoint.Map(insights);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.CreateScouting.CreateScoutingEndpoint.Map(insights);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.MapLayers.MapLayersEndpoint.Map(insights);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.IngestWeatherObservations.IngestWeatherObservationsEndpoint.Map(insights);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.IngestWeatherForecasts.IngestWeatherForecastsEndpoint.Map(insights);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.IngestSatellite.IngestSatelliteEndpoint.Map(insights);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.ListRiskRules.ListRiskRulesEndpoint.Map(insights);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.CreateRiskRule.CreateRiskRuleEndpoint.Map(insights);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.ActivateRiskRule.ActivateRiskRuleEndpoint.Map(insights);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.RetireRiskRule.RetireRiskRuleEndpoint.Map(insights);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.EvaluateRisks.EvaluateRisksEndpoint.Map(insights);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.RiskAssessments.RiskAssessmentsEndpoint.Map(insights);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.Alerts.AlertsEndpoint.Map(insights);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.AcknowledgeAlert.AcknowledgeAlertEndpoint.Map(insights);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.ResolveAlert.ResolveAlertEndpoint.Map(insights);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.DismissAlert.DismissAlertEndpoint.Map(insights);
        global::EdgIQ.MurimiOS.API.Endpoints.FieldIntelligence.CreateFollowUpWorkOrder.CreateFollowUpWorkOrderEndpoint.Map(insights);
    }
}
