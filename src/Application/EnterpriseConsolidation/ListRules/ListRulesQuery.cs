using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.EnterpriseConsolidation.ListRules;
using EliminationRuleStatus = global::EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule.EliminationRuleStatus;
using IntercompanyEliminationRule = global::EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule.IntercompanyEliminationRule;

public sealed record ListRulesQuery(Guid? organisationNodeId, EliminationRuleStatus? status) : IRequest<UseCaseResult>;
