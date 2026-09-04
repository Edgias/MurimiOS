using System.Diagnostics;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.EnterpriseConsolidation.CreateRule;
public sealed record CreateEliminationRuleRequest(Guid? Id, Guid ConsolidationOrganisationNodeId,
    Guid SellerCompanyId, Guid BuyerCompanyId, string RuleKey, int RuleVersion, string Name,
    string Currency, decimal BalancingTolerance, DateOnly EffectiveFrom, DateOnly? EffectiveTo);
