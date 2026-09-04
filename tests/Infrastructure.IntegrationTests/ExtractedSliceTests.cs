using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Application.Common.Results;
using EdgIQ.MurimiOS.Application.Organisation.Create;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.OrganisationNode;
using EdgIQ.MurimiOS.Infrastructure.Data;
using EdgIQ.MurimiOS.Infrastructure.Data.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.Infrastructure.IntegrationTests;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using OrganisationNodeType = global::EdgIQ.MurimiOS.Domain.OrganisationNode.OrganisationNodeType;
using PlatformDbContext = global::EdgIQ.MurimiOS.Infrastructure.Data.PlatformDbContext;
using IOrganisationScopeAccess = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IOrganisationScopeAccess;
using ITenantContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ITenantContext;
using ResolvedOrganisationScope = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ResolvedOrganisationScope;
using UseCaseResult = global::EdgIQ.MurimiOS.Application.Common.Results.UseCaseResult;
using ContentResult = global::EdgIQ.MurimiOS.Application.Common.Results.ContentResult;
using CreateCommand = global::EdgIQ.MurimiOS.Application.Organisation.Create.CreateCommand;
using CreateHandler = global::EdgIQ.MurimiOS.Application.Organisation.Create.CreateHandler;
using CreateOrganisationNodeRequest = global::EdgIQ.MurimiOS.Application.Organisation.Create.CreateOrganisationNodeRequest;
using OrganisationNodeResponse = global::EdgIQ.MurimiOS.Application.Organisation.Create.OrganisationNodeResponse;

public sealed class ExtractedSliceTests
{
    [Fact]
    public async Task Organisation_command_uses_aggregate_and_preserves_created_location_and_tenant()
    {
        Guid tenantId = Guid.NewGuid();
        await using var db = Database(tenantId);
        var handler = new CreateHandler(db, new Scope(new(null, null, null, null)));
        UseCaseResult result = await handler.Handle(new CreateCommand(
            new CreateOrganisationNodeRequest(null, "Farm holding", OrganisationNodeType.Organisation)), CancellationToken.None);

        var created = Assert.IsType<ValueResult<OrganisationNodeResponse>>(result);
        Assert.Equal("Created", created.Kind);
        Assert.Equal($"/api/v1/organisation/nodes/{created.Value!.Id}", created.Location);
        var node = await db.OrganisationNodes.SingleAsync();
        Assert.Equal(created.Value.Id, node.Id);
        Assert.Equal(tenantId, node.TenantId);
        Assert.Equal($"node_{node.Id:N}", node.Path);
    }

    [Fact]
    public async Task Scoped_organisation_command_still_rejects_root_creation_without_writing()
    {
        await using var db = Database(Guid.NewGuid());
        Guid root = Guid.NewGuid();
        var handler = new CreateHandler(db, new Scope(new(root, "root", new HashSet<Guid>(), new HashSet<Guid> { root })));
        UseCaseResult result = await handler.Handle(new CreateCommand(
            new CreateOrganisationNodeRequest(null, "Other holding", OrganisationNodeType.Organisation)), CancellationToken.None);

        Assert.Equal(403, result.Accept(new StatusVisitor()));
        Assert.Empty(await db.OrganisationNodes.ToArrayAsync());
    }

    [Fact]
    public async Task Audit_query_preserves_scope_filter_cursor_and_result_shape()
    {
        Guid tenantId = Guid.NewGuid(), farmId = Guid.NewGuid(), otherFarmId = Guid.NewGuid();
        DateTimeOffset now = DateTimeOffset.Parse("2026-09-01T10:00:00Z");
        await using var db = Database(tenantId);
        AuditEvent Event(Guid farm, int minutes) => new(Guid.NewGuid(), tenantId, now.AddMinutes(minutes),
            "operator", null, "Created", "CropPlan", Guid.NewGuid(), farm, null, "{}", "correlation");
        var expected = Event(farmId, -1);
        db.AuditEvents.AddRange(expected, Event(otherFarmId, -2), Event(farmId, 1));
        await db.SaveChangesAsync();
        var handler = new EdgIQ.MurimiOS.Application.Audit.List.ListHandler(db,
            new Scope(new(Guid.NewGuid(), "root", new HashSet<Guid> { farmId }, new HashSet<Guid>())));
        UseCaseResult result = await handler.Handle(new EdgIQ.MurimiOS.Application.Audit.List.ListQuery(
            now, "CropPlan", null, 10), CancellationToken.None);

        var rows = Assert.IsType<ValueResult<EdgIQ.MurimiOS.Application.Audit.List.AuditEventResponse[]>>(result);
        Assert.Equal(expected.Id, Assert.Single(rows.Value!).Id);
    }

    private static PlatformDbContext Database(Guid tenantId) => new(
        new DbContextOptionsBuilder<PlatformDbContext>().UseInMemoryDatabase(Guid.NewGuid().ToString()).Options,
        new Tenant(tenantId));

    private sealed record Tenant(Guid TenantId) : ITenantContext { public bool IsResolved => true; }
    private sealed class Scope(ResolvedOrganisationScope scope) : IOrganisationScopeAccess
    {
        public Task<ResolvedOrganisationScope> ResolveAsync(CancellationToken cancellationToken) => Task.FromResult(scope);
        public Task<bool> CanAccessFarmAsync(Guid farmId, CancellationToken cancellationToken) => Task.FromResult(scope.AllowsFarm(farmId));
    }
    private sealed class StatusVisitor : IUseCaseResultVisitor<int?>
    {
        public int? Visit<T>(ValueResult<T> result) => result.Status;
        public int? Visit(ContentResult result) => result.Status;
    }
}
