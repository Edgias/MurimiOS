using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.EnterpriseConsolidation.ListRules;
using EliminationRuleStatus = global::EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule.EliminationRuleStatus;
using IntercompanyEliminationRule = global::EdgIQ.MurimiOS.Domain.IntercompanyEliminationRule.IntercompanyEliminationRule;

public sealed class ListRulesHandler(IPlatformDbContext db, IOrganisationScopeAccess scope) : IRequestHandler<ListRulesQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListRulesQuery message, CancellationToken cancellationToken) => ListRulesAsync(message.organisationNodeId, message.status, db, scope, cancellationToken);

    private static async Task<UseCaseResult> ListRulesAsync(Guid? organisationNodeId, EliminationRuleStatus? status,
        IPlatformDbContext db, IOrganisationScopeAccess scope, CancellationToken ct)
    {
        ResolvedOrganisationScope resolved = await scope.ResolveAsync(ct);
        IQueryable<IntercompanyEliminationRule> query = db.IntercompanyEliminationRules.AsNoTracking();
        if (organisationNodeId is { } nodeId)
        {
            if (!resolved.AllowsNode(nodeId)) return UseCaseResults.NotFound();
            query = query.Where(x => x.ConsolidationOrganisationNodeId == nodeId);
        }
        else if (!resolved.IsUnrestricted) query = query.Where(x => resolved.OrganisationNodeIds!.Contains(x.ConsolidationOrganisationNodeId));
        if (status is { } value) query = query.Where(x => x.Status == value);
        return UseCaseResults.Ok(await query.OrderBy(x => x.RuleKey).ThenByDescending(x => x.RuleVersion).ToArrayAsync(ct));
    }


}
