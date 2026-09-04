using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase;
using EdgIQ.MurimiOS.Domain.Common;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using Microsoft.EntityFrameworkCore;
using NetTopologySuite.Geometries;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Eudr.CreateCase;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using EudrCommodity = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrCommodity;
using EudrDueDiligenceCase = global::EdgIQ.MurimiOS.Domain.EudrDueDiligenceCase.EudrDueDiligenceCase;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class CreateCaseHandler(IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock, UseCaseContext context) : IRequestHandler<CreateCaseCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CreateCaseCommand message, CancellationToken cancellationToken) => CreateCaseAsync(message.farmId, message.request, db, scope, tenant, actor, clock, context, cancellationToken);

    private static async Task<UseCaseResult> CreateCaseAsync(Guid farmId, CreateEudrCaseRequest request,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        TimeProvider clock, UseCaseContext context, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct) || !await db.Farms.AnyAsync(x => x.Id == farmId, ct))
            return UseCaseResults.NotFound();
        try
        {
            DateTimeOffset now = clock.GetUtcNow();
            var dueDiligenceCase = new EudrDueDiligenceCase(Id(request.Id), farmId, request.CaseNumber,
                request.Commodity, request.ProductDescription, request.HsCode, request.Quantity, request.Unit,
                request.ProductionCountry, request.ProductionFrom, request.ProductionTo, request.OperatorName,
                request.OperatorRegistrationNumber, request.OperatorAddress, request.Market, now, actor.Subject);
            db.EudrCases.Add(dueDiligenceCase);
            AddRecords(db, tenant, actor, "Created", dueDiligenceCase.Id, farmId,
                "Eudr.CaseCreated", new { dueDiligenceCase.Id, dueDiligenceCase.CaseNumber, dueDiligenceCase.Commodity }, now, context);
            await db.SaveChangesAsync(ct);
            return UseCaseResults.Created($"/api/v1/eudr/cases/{dueDiligenceCase.Id}", dueDiligenceCase);
        }
        catch (ArgumentException e) { return Bad("eudr_case_invalid", e.Message); }
        catch (DbUpdateException e) { return Conflict("eudr_case_conflict", DbMessage(e)); }
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
