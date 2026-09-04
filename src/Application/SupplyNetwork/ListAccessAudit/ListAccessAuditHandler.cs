using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.DataGrant;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplyNetwork.ListAccessAudit;
using DataGrant = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrant;

using CrossTenantAccessAudit = global::EdgIQ.MurimiOS.Domain.DataGrant.CrossTenantAccessAudit;

public sealed class ListAccessAuditHandler(IPlatformDbContext db, ITenantContext tenant) : IRequestHandler<ListAccessAuditQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ListAccessAuditQuery message, CancellationToken cancellationToken) => ListAccessAuditAsync(message.grantId, message.limit, db, tenant, cancellationToken);

    private static async Task<UseCaseResult> ListAccessAuditAsync(Guid? grantId, int? limit, IPlatformDbContext db,
        ITenantContext tenant, CancellationToken cancellationToken)
    {
        int take = Math.Clamp(limit ?? 200, 1, 500);
        IQueryable<CrossTenantAccessAudit> query = db.CrossTenantAccessAudits.AsNoTracking()
            .Where(x => x.ProducerTenantId == tenant.TenantId || x.RecipientTenantId == tenant.TenantId);
        if (grantId is { } id) query = query.Where(x => x.GrantId == id);
        CrossTenantAccessAuditResponse[] rows = await query.OrderByDescending(x => x.OccurredAt).Take(take)
            .Select(x => new CrossTenantAccessAuditResponse(x.Id, x.ProducerTenantId, x.RecipientTenantId, x.GrantId,
                x.ActorTenantId, x.ActorSubject, x.Purpose, x.EntityCategory, x.ResourceId, x.OccurredAt,
                x.ResultCount, x.Outcome, x.CorrelationId)).ToArrayAsync(cancellationToken);
        return UseCaseResults.Ok(rows);
    }


}
