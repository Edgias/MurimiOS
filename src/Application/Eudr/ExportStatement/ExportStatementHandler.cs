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
namespace EdgIQ.MurimiOS.Application.Eudr.ExportStatement;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;
using EudrDueDiligenceStatement = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceStatement;
using EudrStatementExport = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrStatementExport;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class ExportStatementHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<ExportStatementCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(ExportStatementCommand message, CancellationToken cancellationToken) => ExportStatementAsync(message.statementId, db, scope, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> ExportStatementAsync(Guid statementId, IPlatformDbContext db,
        IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock,
        UseCaseContext context, CancellationToken ct)
    {
        EudrDueDiligenceStatement? statement = await db.EudrStatements.SingleOrDefaultAsync(x => x.Id == statementId, ct);
        if (statement is null) return UseCaseResults.NotFound();
        EudrDueDiligenceCase? dueDiligenceCase = await db.EudrCases.SingleOrDefaultAsync(x => x.Id == statement.CaseId, ct);
        if (dueDiligenceCase is null || !await scope.CanAccessFarmAsync(dueDiligenceCase.FarmId, ct)) return UseCaseResults.NotFound();
        byte[] bytes = Encoding.UTF8.GetBytes(statement.PayloadJson);
        string digest = Hash(bytes);
        if (!digest.Equals(statement.PayloadSha256, StringComparison.Ordinal))
            return UseCaseResults.Problem("Stored DDS payload integrity verification failed.", statusCode: 500);
        try
        {
            DateTimeOffset now = clock.GetUtcNow();
            statement.MarkExported(now);
            dueDiligenceCase.MarkExported(now);
            var export = new EudrStatementExport(Guid.NewGuid(), statementId, "json", digest, now, actor.Subject);
            db.EudrStatementExports.Add(export);
            AddRecords(db, tenant, actor, "Exported", statement.Id, dueDiligenceCase.FarmId,
                "Eudr.StatementExported", new { statementId = statement.Id, exportId = export.Id, export.Format, export.Sha256 }, now, context);
            await db.SaveChangesAsync(ct);
            string safeCaseNumber = new string(dueDiligenceCase.CaseNumber.Select(c => char.IsLetterOrDigit(c) || c is '-' or '_' ? c : '-').ToArray());
            return UseCaseResults.File(bytes, "application/json", $"eudr-dds-{safeCaseNumber}-v{statement.StatementVersion}.json");
        }
        catch (InvalidOperationException e) { return Conflict("eudr_export_transition_invalid", e.Message); }
        catch (DbUpdateException e) { return Conflict("eudr_export_conflict", DbMessage(e)); }
    }


    private static Guid Id(Guid? value) => value is { } id && id != Guid.Empty ? id : Guid.NewGuid();

    private static string Hash(string value) => Hash(Encoding.UTF8.GetBytes(value));

    private static string Hash(byte[] value) => Convert.ToHexString(SHA256.HashData(value)).ToLowerInvariant();

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
