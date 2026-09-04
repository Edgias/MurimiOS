using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
namespace EdgIQ.MurimiOS.Application.Audit.List;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;


public sealed class ListHandler(IPlatformDbContext db, IOrganisationScopeAccess scopeAccess) : IRequestHandler<ListQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListQuery message, CancellationToken cancellationToken) => ListAsync(message.before, message.entityType, message.entityId, message.limit, db, scopeAccess, cancellationToken);

    private static async Task<UseCaseResult>ListAsync(
        DateTimeOffset? before,
        string? entityType,
        Guid? entityId,
        int? limit,
        IPlatformDbContext db,
        IOrganisationScopeAccess scopeAccess,
        CancellationToken cancellationToken)
    {
        int pageSize = Math.Clamp(limit ?? 100, 1, 250);
        ResolvedOrganisationScope scope = await scopeAccess.ResolveAsync(cancellationToken);
        IQueryable<AuditEvent> query = db.AuditEvents.AsNoTracking();
        if (!scope.IsUnrestricted)
        {
            query = query.Where(item =>
                item.FarmId != null && scope.FarmIds!.Contains(item.FarmId.Value)
                || item.OrganisationNodeId != null && scope.OrganisationNodeIds!.Contains(item.OrganisationNodeId.Value));
        }
        if (before is { } cursor) query = query.Where(item => item.OccurredAt < cursor);
        if (!string.IsNullOrWhiteSpace(entityType)) query = query.Where(item => item.EntityType == entityType.Trim());
        if (entityId is { } id && id != Guid.Empty) query = query.Where(item => item.EntityId == id);

        AuditEvent[] events = await query
            .OrderByDescending(item => item.OccurredAt)
            .ThenByDescending(item => item.Id)
            .Take(pageSize)
            .ToArrayAsync(cancellationToken);
        AuditEventResponse[] results = events.Select(item => new AuditEventResponse(
                item.Id,
                item.OccurredAt,
                item.ActorSubject,
                item.MembershipId,
                item.Action,
                item.EntityType,
                item.EntityId,
                item.FarmId,
                item.OrganisationNodeId,
                JsonSerializer.Deserialize<JsonElement>(item.Changes),
                item.CorrelationId))
            .ToArray();
        return UseCaseResults.Ok(results);
    }


}
