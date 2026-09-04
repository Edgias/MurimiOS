using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.SupplyRelationship;
using EdgIQ.MurimiOS.Domain.DataGrant;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.SupplyNetwork.ReadSharedHarvestLots;
using CrossTenantAccessAudit = global::EdgIQ.MurimiOS.Domain.DataGrant.CrossTenantAccessAudit;
using DataGrant = global::EdgIQ.MurimiOS.Domain.DataGrant.DataGrant;
using SupplyDataCategories = global::EdgIQ.MurimiOS.Domain.DataGrant.SupplyDataCategories;
using SupplyRelationship = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationship;
using SupplyRelationshipStatus = global::EdgIQ.MurimiOS.Domain.SupplyRelationship.SupplyRelationshipStatus;

public sealed class ReadSharedHarvestLotsHandler(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<ReadSharedHarvestLotsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ReadSharedHarvestLotsQuery message, CancellationToken cancellationToken) => ReadSharedHarvestLotsAsync(message.grantId, message.purpose, db, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> ReadSharedHarvestLotsAsync(Guid grantId, string purpose,
        IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock,
        UseCaseContext context, CancellationToken cancellationToken)
    {
        GrantAccessContext? access = await LoadGrantAccessAsync(grantId, SupplyDataCategories.HarvestLots, purpose,
            db, tenant, clock, cancellationToken);
        if (access is null) return UseCaseResults.NotFound();
        if (!access.Allowed)
        {
            await AuditReadAsync(access.Grant, tenant, actor, purpose, SupplyDataCategories.HarvestLots, null, 0,
                "Denied", db, clock, context, cancellationToken);
            return UseCaseResults.Json(new { code = "data_grant_not_effective" }, statusCode: 403);
        }
        GrantedHarvestLotProjection[] rows = await db.Set<GrantedHarvestLotProjection>()
            .FromSqlInterpolated($"SELECT * FROM supply.read_granted_harvest_lots({grantId})")
            .AsNoTracking().ToArrayAsync(cancellationToken);
        await db.Entry(access.Grant).ReloadAsync(cancellationToken);
        await db.Entry(access.Relationship).ReloadAsync(cancellationToken);
        bool stillAllowed = access.Grant.IsEffective(tenant.TenantId, purpose, SupplyDataCategories.HarvestLots, clock.GetUtcNow())
            && access.Relationship.Status == SupplyRelationshipStatus.Active;
        await AuditReadAsync(access.Grant, tenant, actor, purpose, SupplyDataCategories.HarvestLots, null,
            stillAllowed ? rows.Length : 0, stillAllowed ? "Allowed" : "Denied", db, clock, context, cancellationToken);
        return stillAllowed ? UseCaseResults.Ok(rows) : UseCaseResults.Json(new { code = "data_grant_revoked" }, statusCode: 403);
    }


    private static async Task<GrantAccessContext?> LoadGrantAccessAsync(Guid grantId, string category, string purpose,
        IPlatformDbContext db, ITenantContext tenant, TimeProvider clock, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(purpose)) return null;
        DataGrant? grant = await db.DataGrants.SingleOrDefaultAsync(x => x.Id == grantId
            && (x.ProducerTenantId == tenant.TenantId || x.RecipientTenantId == tenant.TenantId), cancellationToken);
        if (grant is null) return null;
        SupplyRelationship? relationship = await db.SupplyRelationships.SingleOrDefaultAsync(x => x.Id == grant.RelationshipId, cancellationToken);
        if (relationship is null) return null;
        bool allowed = relationship.Status == SupplyRelationshipStatus.Active
            && grant.IsEffective(tenant.TenantId, purpose, category, clock.GetUtcNow());
        return new GrantAccessContext(grant, relationship, allowed);
    }


    private static async Task AuditReadAsync(DataGrant grant, ITenantContext tenant, IRequestActorContext actor,
        string purpose, string category, Guid? resourceId, int resultCount, string outcome, IPlatformDbContext db,
        TimeProvider clock, UseCaseContext context, CancellationToken cancellationToken)
    {
        db.CrossTenantAccessAudits.Add(new CrossTenantAccessAudit(Guid.NewGuid(), grant.ProducerTenantId,
            grant.RecipientTenantId, grant.Id, tenant.TenantId, actor.Subject, purpose, category, resourceId,
            clock.GetUtcNow(), resultCount, outcome, Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        await db.SaveChangesAsync(cancellationToken);
    }

    private sealed record GrantAccessContext(DataGrant Grant, SupplyRelationship Relationship, bool Allowed);

}
