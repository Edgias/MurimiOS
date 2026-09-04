using System.Data;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.CropPlanning;
using ApproveEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Approve.ApproveEndpoint;
using CompareEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Compare.CompareEndpoint;
using CreateApplicationRuleEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CreateApplicationRule.CreateApplicationRuleEndpoint;
using CreateRevisionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CreateRevision.CreateRevisionEndpoint;
using GetPlanEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.GetPlan.GetPlanEndpoint;
using GetRotationEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.GetRotation.GetRotationEndpoint;
using GetVarianceEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.GetVariance.GetVarianceEndpoint;
using ListApplicationRulesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.ListApplicationRules.ListApplicationRulesEndpoint;
using ListFinancialEntriesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.ListFinancialEntries.ListFinancialEntriesEndpoint;
using RecordFinancialEntryEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.RecordFinancialEntry.RecordFinancialEntryEndpoint;
using ReplaceDraftEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.ReplaceDraft.ReplaceDraftEndpoint;


public static class CropPlanningEndpoints
{
    public static void MapCropPlanningEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder planning = api.MapGroup("/crop-planning").WithTags("Crop Planning");
        global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.ListPlans.ListPlansEndpoint.Map(planning);
        global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CreatePlan.CreatePlanEndpoint.Map(planning);
        global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.GetPlan.GetPlanEndpoint.Map(planning);
        global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.ReplaceDraft.ReplaceDraftEndpoint.Map(planning);
        global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Submit.SubmitEndpoint.Map(planning);
        global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Approve.ApproveEndpoint.Map(planning);
        global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Reject.RejectEndpoint.Map(planning);
        global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CreateRevision.CreateRevisionEndpoint.Map(planning);
        global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.GetVariance.GetVarianceEndpoint.Map(planning);
        global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.Compare.CompareEndpoint.Map(planning);
        global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.GetRotation.GetRotationEndpoint.Map(planning);
        global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.ListApplicationRules.ListApplicationRulesEndpoint.Map(planning);
        global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.CreateApplicationRule.CreateApplicationRuleEndpoint.Map(planning);
        global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.ListFinancialEntries.ListFinancialEntriesEndpoint.Map(planning);
        global::EdgIQ.MurimiOS.API.Endpoints.CropPlanning.RecordFinancialEntry.RecordFinancialEntryEndpoint.Map(planning);
    }
}
