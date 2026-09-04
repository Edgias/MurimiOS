using EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack;
using System.Diagnostics;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.Compliance;
using AddControlEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.AddControl.AddControlEndpoint;
using AddRequirementEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.AddRequirement.AddRequirementEndpoint;
using AutoEvaluateEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.AutoEvaluate.AutoEvaluateEndpoint;
using ChangeAssessmentEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.ChangeAssessment.ChangeAssessmentEndpoint;
using ChangeCorrectiveActionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.ChangeCorrectiveAction.ChangeCorrectiveActionEndpoint;
using ChangeStandardEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.ChangeStandard.ChangeStandardEndpoint;
using CreateAssessmentEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.CreateAssessment.CreateAssessmentEndpoint;
using CreateCorrectiveActionEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.CreateCorrectiveAction.CreateCorrectiveActionEndpoint;
using CreateStandardEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.CreateStandard.CreateStandardEndpoint;
using DashboardEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.Dashboard.DashboardEndpoint;
using EvaluateControlEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.EvaluateControl.EvaluateControlEndpoint;
using GetAssessmentEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.GetAssessment.GetAssessmentEndpoint;
using GetStandardEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.GetStandard.GetStandardEndpoint;
using InstallStandardPackEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.InstallStandardPack.InstallStandardPackEndpoint;
using ListAssessmentsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.ListAssessments.ListAssessmentsEndpoint;
using ListFindingsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.ListFindings.ListFindingsEndpoint;
using ListStandardPacksEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.ListStandardPacks.ListStandardPacksEndpoint;
using ListStandardsEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.ListStandards.ListStandardsEndpoint;
using WaiveFindingEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.Compliance.WaiveFinding.WaiveFindingEndpoint;

using ComplianceAuditPackEndpoints = global::EdgIQ.MurimiOS.API.Endpoints.ComplianceAuditPack.ComplianceAuditPackEndpoints;

public static class ComplianceEndpoints
{
    public static void MapComplianceEndpoints(RouteGroupBuilder api)
    {
        RouteGroupBuilder standards = api.MapGroup("/compliance/standards").WithTags("Compliance");
        global::EdgIQ.MurimiOS.API.Endpoints.Compliance.ListStandards.ListStandardsEndpoint.Map(standards);
        global::EdgIQ.MurimiOS.API.Endpoints.Compliance.CreateStandard.CreateStandardEndpoint.Map(standards);
        global::EdgIQ.MurimiOS.API.Endpoints.Compliance.GetStandard.GetStandardEndpoint.Map(standards);
        global::EdgIQ.MurimiOS.API.Endpoints.Compliance.AddControl.AddControlEndpoint.Map(standards);
        global::EdgIQ.MurimiOS.API.Endpoints.Compliance.AddRequirement.AddRequirementEndpoint.Map(standards);
        global::EdgIQ.MurimiOS.API.Endpoints.Compliance.ChangeStandard.ChangeStandardEndpoint.Map(standards);
        global::EdgIQ.MurimiOS.API.Endpoints.Compliance.ListStandardPacks.ListStandardPacksEndpoint.Map(standards);
        global::EdgIQ.MurimiOS.API.Endpoints.Compliance.InstallStandardPack.InstallStandardPackEndpoint.Map(standards);

        RouteGroupBuilder farms = api.MapGroup("/farms/{farmId:guid}/compliance").WithTags("Compliance");
        global::EdgIQ.MurimiOS.API.Endpoints.Compliance.ListAssessments.ListAssessmentsEndpoint.Map(farms);
        global::EdgIQ.MurimiOS.API.Endpoints.Compliance.CreateAssessment.CreateAssessmentEndpoint.Map(farms);
        global::EdgIQ.MurimiOS.API.Endpoints.Compliance.GetAssessment.GetAssessmentEndpoint.Map(farms);
        global::EdgIQ.MurimiOS.API.Endpoints.Compliance.ChangeAssessment.ChangeAssessmentEndpoint.Map(farms);
        global::EdgIQ.MurimiOS.API.Endpoints.Compliance.EvaluateControl.EvaluateControlEndpoint.Map(farms);
        global::EdgIQ.MurimiOS.API.Endpoints.Compliance.AutoEvaluate.AutoEvaluateEndpoint.Map(farms);
        global::EdgIQ.MurimiOS.API.Endpoints.Compliance.Dashboard.DashboardEndpoint.Map(farms);
        global::EdgIQ.MurimiOS.API.Endpoints.Compliance.ListFindings.ListFindingsEndpoint.Map(farms);
        ComplianceAuditPackEndpoints.Map(farms);

        RouteGroupBuilder findings = api.MapGroup("/compliance/findings").WithTags("Compliance");
        global::EdgIQ.MurimiOS.API.Endpoints.Compliance.CreateCorrectiveAction.CreateCorrectiveActionEndpoint.Map(findings);
        global::EdgIQ.MurimiOS.API.Endpoints.Compliance.WaiveFinding.WaiveFindingEndpoint.Map(findings);
        global::EdgIQ.MurimiOS.API.Endpoints.Compliance.ChangeCorrectiveAction.ChangeCorrectiveActionEndpoint.Map(findings);
    }
}
