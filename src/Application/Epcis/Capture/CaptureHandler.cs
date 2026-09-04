using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.EpcisExchange;
using EdgIQ.MurimiOS.Domain.OutboxMessage;
using EdgIQ.MurimiOS.Application.Common.Traceability;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Epcis.Capture;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using EpcisCapturedEvent = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisCapturedEvent;
using EpcisExchange = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisExchange;
using EpcisExchangeDirection = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisExchangeDirection;
using OutboxMessage = global::EdgIQ.MurimiOS.Domain.OutboxMessage.OutboxMessage;

public sealed class CaptureHandler(IResponseMetadata response, IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock) : IRequestHandler<CaptureCommand, UseCaseResult>
{
    public Task<UseCaseResult> Handle(CaptureCommand message, CancellationToken cancellationToken) => CaptureAsync(message.farmId, message.request, response, db, scope, tenant, actor, clock, cancellationToken);

    private static async Task<UseCaseResult> CaptureAsync(Guid farmId, IncomingContent request, IResponseMetadata response,
        IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        TimeProvider clock, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        if (request.ContentLength is > MaximumDocumentBytes) return TooLarge();
        if (request.ContentType is { Length: > 0 } contentType
            && !contentType.StartsWith("application/json", StringComparison.OrdinalIgnoreCase)
            && !contentType.StartsWith("application/ld+json", StringComparison.OrdinalIgnoreCase))
            return UseCaseResults.StatusCode(415);
        byte[] bytes;
        try { bytes = await ReadLimitedAsync(request.Body, ct); }
        catch (EpcisPayloadTooLargeException) { return TooLarge(); }
        EpcisCaptureValidation validation = EpcisInterop.ValidateCapture(bytes);
        if (validation.Issues.Count > 0)
            return UseCaseResults.BadRequest(new
            {
                type = "epcisException:ValidationException", title = "EPCIS document validation failed.",
                status = 400, errors = validation.Issues
            });

        string hash = Convert.ToHexStringLower(SHA256.HashData(bytes));
        EpcisExchange? existing = await db.EpcisExchanges.AsNoTracking().SingleOrDefaultAsync(x =>
            x.Direction == EpcisExchangeDirection.Capture && x.Sha256 == hash, ct);
        if (existing is not null)
        {
            StandardHeaders(response);
            string existingLocation = $"/api/v1/epcis/capture/{existing.Id}";
            response["Location"] = existingLocation;
            return UseCaseResults.Accepted(existingLocation, Status(existing));
        }

        string[] eventIds = validation.Events.Select(x => x.EventId).ToArray();
        string? duplicateEvent = await db.EpcisCapturedEvents.AsNoTracking().Where(x => eventIds.Contains(x.EventId))
            .Select(x => x.EventId).FirstOrDefaultAsync(ct);
        if (duplicateEvent is not null)
            return UseCaseResults.Conflict(new { code = "epcis_event_exists", message = $"Event {duplicateEvent} was already captured." });

        Guid exchangeId = Guid.NewGuid();
        DateTimeOffset now = clock.GetUtcNow();
        string documentId = validation.DocumentId ?? $"urn:uuid:{exchangeId:D}";
        var exchange = new EpcisExchange(exchangeId, farmId, EpcisExchangeDirection.Capture,
            "EPCISDocument", documentId, EpcisInterop.SchemaVersion, EpcisInterop.CbvVersion, hash,
            validation.Events.Count, validation.Events.Min(x => (DateTimeOffset?)x.EventTime),
            validation.Events.Max(x => (DateTimeOffset?)x.EventTime), validation.Sender, validation.Receiver,
            Encoding.UTF8.GetString(bytes), now, actor.Subject);
        db.EpcisExchanges.Add(exchange);
        db.EpcisCapturedEvents.AddRange(validation.Events.Select(x => new EpcisCapturedEvent(Guid.NewGuid(),
            exchange.Id, farmId, x.EventId, x.EventType, x.EventTime, x.RecordTime, x.BusinessStep,
            x.Disposition, JsonSerializer.Serialize(x.ObjectIdentifiers, JsonOptions), x.Sha256, x.PayloadJson)));
        AddAudit(db, tenant, actor, now, "EpcisCaptured", exchange.Id, farmId,
            new { exchange.Id, exchange.DocumentId, exchange.Sha256, exchange.EventCount, exchange.SchemaVersion, exchange.CbvVersion });
        db.OutboxMessages.Add(new OutboxMessage(Guid.NewGuid(), tenant.TenantId, "Epcis.DocumentCaptured",
            JsonSerializer.Serialize(new { exchange.Id, exchange.FarmId, exchange.DocumentId, exchange.Sha256, exchange.EventCount }, JsonOptions), now));
        try { await db.SaveChangesAsync(ct); }
        catch (DbUpdateException)
        {
            return UseCaseResults.Conflict(new { code = "epcis_capture_conflict", message = "The document or one of its event identifiers already exists." });
        }
        StandardHeaders(response);
        string location = $"/api/v1/epcis/capture/{exchange.Id}";
        response["Location"] = location;
        return UseCaseResults.Accepted(location, Status(exchange));
    }

    private const int MaximumDocumentBytes = 10_000_000;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);


    private static object Status(EpcisExchange x) => new
    {
        captureID = x.Id, createdAt = x.ExchangedAt, finishedAt = x.ExchangedAt,
        running = false, success = true, captureErrorBehaviour = "rollback", errors = Array.Empty<object>(),
        x.DocumentId, x.Sha256, x.EventCount
    };

    private static DateTimeOffset EventTime(JsonObject item) => DateTimeOffset.Parse(item["eventTime"]!.GetValue<string>());


    private static void StandardHeaders(IResponseMetadata response)
    {
        response["GS1-EPCIS-Version"] = EpcisInterop.EpcisVersion;
        response["GS1-CBV-Version"] = EpcisInterop.CbvVersion;
        response["GS1-Extensions"] = "https://murimios.io/epcis/";
    }

    private static void AddAudit(IPlatformDbContext db, ITenantContext tenant, IRequestActorContext actor,
        DateTimeOffset at, string action, Guid id, Guid farmId, object changes) =>
        db.AuditEvents.Add(new AuditEvent(Guid.NewGuid(), tenant.TenantId, at, actor.Subject, actor.MembershipId,
            action, nameof(EpcisExchange), id, farmId, actor.ScopeOrganisationNodeId,
            JsonSerializer.Serialize(changes, JsonOptions), Activity.Current?.TraceId.ToString()));

    private static async Task<byte[]> ReadLimitedAsync(Stream source, CancellationToken ct)
    {
        using var output = new MemoryStream(); byte[] buffer = new byte[64 * 1024];
        while (true)
        {
            int read = await source.ReadAsync(buffer, ct); if (read == 0) break;
            if (output.Length + read > MaximumDocumentBytes) throw new EpcisPayloadTooLargeException();
            await output.WriteAsync(buffer.AsMemory(0, read), ct);
        }
        return output.ToArray();
    }

    private static UseCaseResult TooLarge(string message = "An EPCIS document cannot exceed ten megabytes.") =>
        UseCaseResults.Json(new { code = "epcis_document_too_large", message }, statusCode: 413);

    private sealed class EpcisPayloadTooLargeException : Exception { }

}
