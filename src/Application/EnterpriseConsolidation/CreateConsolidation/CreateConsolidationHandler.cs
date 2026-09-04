using System.Diagnostics;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun;
using EdgIQ.MurimiOS.Application.Common.Finance;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.EnterpriseConsolidation.CreateConsolidation;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using CurrencyCode = global::EdgIQ.MurimiOS.Domain.Common.CurrencyCode;
using EnterpriseConsolidationRun = global::EdgIQ.MurimiOS.Domain.EnterpriseConsolidationRun.EnterpriseConsolidationRun;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class CreateConsolidationHandler(IPlatformDbContext db, EnterpriseConsolidationEngine engine, IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock, UseCaseContext context) : IRequestHandler<CreateConsolidationCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateConsolidationCommand message, CancellationToken cancellationToken) => CreateConsolidationAsync(message.organisationNodeId, message.request, db, engine, scope, actor, tenant, clock, context, cancellationToken);

    private static async Task<UseCaseResult> CreateConsolidationAsync(Guid organisationNodeId,
        CreateEnterpriseConsolidationRequest request, IPlatformDbContext db, EnterpriseConsolidationEngine engine,
        IOrganisationScopeAccess scope, IRequestActorContext actor, ITenantContext tenant, TimeProvider clock,
        UseCaseContext context, CancellationToken ct)
    {
        if (!(await scope.ResolveAsync(ct)).AllowsNode(organisationNodeId)) return UseCaseResults.NotFound();
        string currency;
        try { currency = new CurrencyCode(request.Currency).Value; }
        catch (ArgumentException ex) { return Bad("currency_invalid", ex.Message); }
        try
        {
            DateTimeOffset now = clock.GetUtcNow(); Guid runId = Id(request.Id);
            EnterpriseConsolidationBuild result = await engine.BuildAsync(runId, organisationNodeId,
                request.PeriodStart, request.PeriodEnd, currency, actor.Subject, now, db, ct);
            db.EnterpriseConsolidationRuns.Add(result.Run); db.EnterpriseEliminationEntries.AddRange(result.Entries);
            AddRecords(db, tenant, actor, "EnterpriseConsolidationCompleted", "Enterprise.ConsolidationCompleted",
                nameof(EnterpriseConsolidationRun), runId, organisationNodeId,
                new { result.Run.Id, result.Run.PeriodStart, result.Run.PeriodEnd, result.Run.Currency,
                    result.Run.GrossExpenses, result.Run.GrossRevenue, result.Run.EliminatedExpenses,
                    result.Run.EliminatedRevenue, result.Run.ConsolidatedExpenses, result.Run.ConsolidatedRevenue,
                    result.Run.ConsolidatedMargin, result.Run.ExceptionCount, result.Run.SourceSha256 }, now, context);
            await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/enterprise/consolidations/{runId}", new { result.Run, result.Entries });
        }
        catch (ArgumentException ex) { return Bad("consolidation_invalid", ex.Message); }
        catch (InvalidOperationException ex) { return Conflict("consolidation_unavailable", ex.Message); }
    }


    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string action, string eventType, string entityType, Guid entityId, Guid organisationNodeId,
        object payload, DateTimeOffset now, UseCaseContext context)
    {
        string json = JsonSerializer.Serialize(payload);
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, entityType, entityId, null, organisationNodeId, json,
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now));
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

}
