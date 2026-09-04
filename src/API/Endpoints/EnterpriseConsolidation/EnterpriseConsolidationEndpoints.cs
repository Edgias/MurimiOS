using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation;
using ApproveRuleEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.ApproveRule.ApproveRuleEndpoint;
using CloseCompanyEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CloseCompany.CloseCompanyEndpoint;
using CreateCompanyEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CreateCompany.CreateCompanyEndpoint;
using CreateConsolidationEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CreateConsolidation.CreateConsolidationEndpoint;
using CreateRuleEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CreateRule.CreateRuleEndpoint;
using GetConsolidationEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.GetConsolidation.GetConsolidationEndpoint;
using ListCompaniesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.ListCompanies.ListCompaniesEndpoint;
using ListRulesEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.ListRules.ListRulesEndpoint;
using RetireRuleEndpoint = global::EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.RetireRule.RetireRuleEndpoint;


public static class EnterpriseConsolidationEndpoints
{
    public static void Map(RouteGroupBuilder api)
    {
        RouteGroupBuilder enterprise = api.MapGroup("/enterprise").WithTags("Enterprise consolidation");
        global::EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.ListCompanies.ListCompaniesEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CreateCompany.CreateCompanyEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CloseCompany.CloseCompanyEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.ListRules.ListRulesEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CreateRule.CreateRuleEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.ApproveRule.ApproveRuleEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.RetireRule.RetireRuleEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CreateConsolidation.CreateConsolidationEndpoint.Map(enterprise);
        global::EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.GetConsolidation.GetConsolidationEndpoint.Map(enterprise);
    }
}
