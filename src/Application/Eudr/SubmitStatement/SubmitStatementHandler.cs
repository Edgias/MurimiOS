using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Eudr.SubmitStatement;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;
using EudrDueDiligenceStatement = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceStatement;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class SubmitStatementHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<SubmitStatementCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(SubmitStatementCommand message, CancellationToken cancellationToken) => SubmitStatementAsync(message.statementId, message.request, db, scope, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> SubmitStatementAsync(Guid statementId, SubmitEudrStatementRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        TimeProvider clock, UseCaseContext context, CancellationToken ct)
    {
        EudrDueDiligenceStatement? statement = await db.EudrStatements.SingleOrDefaultAsync(x => x.Id == statementId, ct);
        if (statement is null) return UseCaseResults.NotFound();
        EudrDueDiligenceCase? dueDiligenceCase = await db.EudrCases.SingleOrDefaultAsync(x => x.Id == statement.CaseId, ct);
        if (dueDiligenceCase is null || !await scope.CanAccessFarmAsync(dueDiligenceCase.FarmId, ct)) return UseCaseResults.NotFound();
        try
        {
            DateTimeOffset now = clock.GetUtcNow();
            statement.MarkSubmitted(now, request.SubmissionReference);
            dueDiligenceCase.MarkSubmitted(now, request.SubmissionReference);
            AddRecords(db, tenant, actor, "Submitted", statement.Id, dueDiligenceCase.FarmId,
                "Eudr.StatementSubmitted", new { statement.Id, statement.CaseId, request.SubmissionReference }, now, context);
            await db.SaveChangesAsync(ct);
            return UseCaseResults.Ok(statement);
        }
        catch (ArgumentException e) { return Bad("eudr_submission_invalid", e.Message); }
        catch (InvalidOperationException e) { return Conflict("eudr_submission_transition_invalid", e.Message); }
        catch (DbUpdateException e) { return Conflict("eudr_submission_conflict", DbMessage(e)); }
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

    private static UseCaseResult Conflict(string code, string message) => UseCaseResults.Conflict(new { code, message });

    private static string DbMessage(DbUpdateException e) => e.InnerException?.Message ?? e.Message;


    private static void AddRecords(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        string action, Guid entityId, Guid farmId, string eventType, object payload, DateTimeOffset now, UseCaseContext context)
    {
        string json = JsonSerializer.Serialize(payload, JsonOptions);
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, now, actor.Subject, actor.MembershipId,
            action, "EUDR", entityId, farmId, actor.ScopeOrganisationNodeId, json,
            Activity.Current?.TraceId.ToString() ?? context.User.CorrelationId));
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, eventType, json, now));
    }

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web)
    {
        WriteIndented = false
    };

}
