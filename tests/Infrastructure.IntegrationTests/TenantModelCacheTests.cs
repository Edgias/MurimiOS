using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Domain.SyncClient;
using EdgIQ.MurimiOS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using OrganisationNodeType = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNodeType;
using OrganisationNode = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNode;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;
using SyncChange = global::EdgIQ.MurimiOS.Domain.SyncClient.SyncChange;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;

public sealed class TenantModelCacheTests
{
    [Fact]
    public async Task Shared_model_uses_each_contexts_tenant_for_all_filter_shapes()
    {
        var options = new DbContextOptionsBuilder<PlatformDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        Guid first = Guid.NewGuid(), second = Guid.NewGuid();
        await using var a = new PlatformDbContext(options, new Tenant(first));
        await using var b = new PlatformDbContext(options, new Tenant(second));
        foreach (var (db, tenantId) in new[] { (a, first), (b, second) })
        {
            Guid id = Guid.NewGuid();
            db.OrganisationNodes.Add(new OrganisationNode(id, "Holding", OrganisationNodeType.Organisation, $"node_{id:N}"));
            db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenantId, DateTimeOffset.UtcNow,
                "test", null, "Created", "OrganisationNode", id, null, null, "{}", "test"));
            db.SyncChanges.Add(new SyncChange(tenantId, id, "OrganisationNode", id, "Upsert", "{}", DateTimeOffset.UtcNow));
            db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenantId, "Created", "{}", DateTimeOffset.UtcNow));
            await db.SaveChangesAsync();
        }
        Assert.Same(a.Model, b.Model);
        foreach (var (db, tenantId) in new[] { (a, first), (b, second) })
        {
            Assert.Equal(tenantId, (await db.OrganisationNodes.SingleAsync()).TenantId);
            Assert.Equal(tenantId, (await db.AuditEvents.SingleAsync()).TenantId);
            Assert.Equal(tenantId, (await db.SyncChanges.SingleAsync()).TenantId);
            Assert.Equal(tenantId, (await db.OutboxMessages.SingleAsync()).TenantId);
        }
    }

    private sealed record Tenant(Guid TenantId) : ITenantContext { public bool IsResolved => true; }
}
