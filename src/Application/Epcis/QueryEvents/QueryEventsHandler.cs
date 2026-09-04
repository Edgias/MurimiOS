using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using EdgIQ.MurimiOS.Application.Common.Abstractions;
using EdgIQ.MurimiOS.Domain.AuditEvent;
using EdgIQ.MurimiOS.Domain.EpcisExchange;
using EdgIQ.MurimiOS.Domain.TraceEvent;
using EdgIQ.MurimiOS.Application.Common.Traceability;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using EdgIQ.MurimiOS.Application.Common.Results;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.Epcis.QueryEvents;
using AuditEvent = global::EdgIQ.MurimiOS.Domain.AuditEvent.AuditEvent;
using EpcisCapturedEvent = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisCapturedEvent;
using EpcisExchange = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisExchange;
using EpcisExchangeDirection = global::EdgIQ.MurimiOS.Domain.EpcisExchange.EpcisExchangeDirection;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;
using TraceEventObject = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventObject;

public sealed class QueryEventsHandler(IResponseMetadata response, IPlatformDbContext db, IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor, TimeProvider clock) : IRequestHandler<QueryEventsQuery, UseCaseResult>
{
    public Task<UseCaseResult> Handle(QueryEventsQuery message, CancellationToken cancellationToken) => QueryEventsAsync(message.farmId, message.eventType, message.GE_eventTime, message.LT_eventTime, message.GE_recordTime, message.LT_recordTime, message.EQ_action, message.EQ_bizStep, message.EQ_disposition, message.MATCH_anyEPC, message.perPage, response, db, scope, tenant, actor, clock, cancellationToken);

    private static async Task<UseCaseResult> QueryEventsAsync(Guid farmId, string? eventType,
        DateTimeOffset? GE_eventTime, DateTimeOffset? LT_eventTime, DateTimeOffset? GE_recordTime,
        DateTimeOffset? LT_recordTime, string? EQ_action, string? EQ_bizStep, string? EQ_disposition,
        string? MATCH_anyEPC, int? perPage, IResponseMetadata response, IPlatformDbContext db,
        IOrganisationScopeAccess scope, ITenantContext tenant, IRequestActorContext actor,
        TimeProvider clock, CancellationToken ct)
    {
        if (!await scope.CanAccessFarmAsync(farmId, ct)) return UseCaseResults.NotFound();
        if (GE_eventTime is { } eventFrom && LT_eventTime is { } eventTo && eventFrom >= eventTo
            || GE_recordTime is { } recordFrom && LT_recordTime is { } recordTo && recordFrom >= recordTo)
            return Bad("epcis_query_range_invalid", "A lower time bound must precede its upper bound.");
        int take = Math.Clamp(perPage ?? 50, 1, 100);
        IQueryable<TraceEvent> internalQuery = db.TraceEvents.AsNoTracking().Where(x => x.FarmId == farmId);
        if (GE_eventTime is { } ge) internalQuery = internalQuery.Where(x => x.OccurredAt >= ge);
        if (LT_eventTime is { } lt) internalQuery = internalQuery.Where(x => x.OccurredAt < lt);
        if (GE_recordTime is { } gr) internalQuery = internalQuery.Where(x => x.RecordedAt >= gr);
        if (LT_recordTime is { } lr) internalQuery = internalQuery.Where(x => x.RecordedAt < lr);
        TraceEvent[] internalEvents = await internalQuery.OrderByDescending(x => x.OccurredAt).Take(1_000).ToArrayAsync(ct);
        Guid[] internalIds = internalEvents.Select(x => x.Id).ToArray();
        TraceEventObject[] objects = await db.TraceEventObjects.AsNoTracking().Where(x => internalIds.Contains(x.EventId)).ToArrayAsync(ct);

        IQueryable<EpcisCapturedEvent> capturedQuery = db.EpcisCapturedEvents.AsNoTracking().Where(x => x.FarmId == farmId);
        if (GE_eventTime is { } cge) capturedQuery = capturedQuery.Where(x => x.EventTime >= cge);
        if (LT_eventTime is { } clt) capturedQuery = capturedQuery.Where(x => x.EventTime < clt);
        if (GE_recordTime is { } cgr) capturedQuery = capturedQuery.Where(x => x.RecordTime >= cgr);
        if (LT_recordTime is { } clr) capturedQuery = capturedQuery.Where(x => x.RecordTime < clr);
        EpcisCapturedEvent[] captured = await capturedQuery.OrderByDescending(x => x.EventTime).Take(1_000).ToArrayAsync(ct);

        var candidates = new List<JsonObject>();
        candidates.AddRange(internalEvents.Select(x => EpcisInterop.InternalEvent(tenant.TenantId, x,
            objects.Where(o => o.EventId == x.Id).ToArray())));
        candidates.AddRange(captured.Select(x => JsonNode.Parse(x.PayloadJson)!.AsObject()));
        string[] types = Values(eventType); string[] actions = Values(EQ_action);
        string[] steps = Values(EQ_bizStep).Select(EpcisInterop.BusinessStep).ToArray();
        string[] dispositions = Values(EQ_disposition).Select(EpcisInterop.Disposition).ToArray();
        string[] epcs = Values(MATCH_anyEPC);
        JsonObject[] selected = candidates.Where(x => Match(x, "type", types)
                && Match(x, "action", actions) && Match(x, "bizStep", steps)
                && Match(x, "disposition", dispositions) && MatchIdentifiers(x, epcs))
            .GroupBy(x => x["eventID"]?.GetValue<string>() ?? x.ToJsonString()).Select(x => x.First())
            .OrderByDescending(x => DateTimeOffset.Parse(x["eventTime"]!.GetValue<string>())).Take(take).ToArray();

        Guid queryId = Guid.NewGuid(); DateTimeOffset now = clock.GetUtcNow();
        JsonObject document = EpcisInterop.QueryDocument(queryId, now, selected);
        string json = document.ToJsonString(JsonOptions);
        if (Encoding.UTF8.GetByteCount(json) > MaximumDocumentBytes)
            return TooLarge("The query result exceeds the ten-megabyte EPCIS document limit; reduce perPage or add filters.");
        string hash = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(json)));
        var exchange = new EpcisExchange(queryId, farmId, EpcisExchangeDirection.QueryResult,
            "EPCISQueryDocument", $"urn:uuid:{queryId:D}", EpcisInterop.SchemaVersion, EpcisInterop.CbvVersion,
            hash, selected.Length,
            selected.Length == 0 ? null : selected.Min(EventTime), selected.Length == 0 ? null : selected.Max(EventTime),
            null, null, json, now, actor.Subject);
        db.EpcisExchanges.Add(exchange);
        AddAudit(db, tenant, actor, now, "EpcisQueryExported", exchange.Id, farmId,
            new { exchange.Id, exchange.Sha256, exchange.EventCount, eventType, GE_eventTime, LT_eventTime,
                GE_recordTime, LT_recordTime, EQ_action, EQ_bizStep, EQ_disposition, MATCH_anyEPC });
        await db.SaveChangesAsync(ct);
        StandardHeaders(response);
        response["GS1-Next-Page-Token-Expires"] = now.AddMinutes(15).ToString("O");
        return UseCaseResults.Text(json, "application/json", Encoding.UTF8, 200);
    }

    private const int MaximumDocumentBytes = 10_000_000;

    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);


    private static bool Match(JsonObject item, string property, string[] expected) => expected.Length == 0
        || item[property] is JsonValue value && expected.Contains(value.GetValue<string>(), StringComparer.OrdinalIgnoreCase);

    private static bool MatchIdentifiers(JsonObject item, string[] expected)
    {
        if (expected.Length == 0) return true;
        var actual = new HashSet<string>(StringComparer.Ordinal);
        foreach (string name in new[] { "epcList", "childEPCs", "inputEPCList", "outputEPCList" })
            if (item[name] is JsonArray epcs) foreach (JsonNode? epc in epcs) if (epc is JsonValue value) actual.Add(value.GetValue<string>());
        foreach (string name in new[] { "quantityList", "childQuantityList", "inputQuantityList", "outputQuantityList" })
            if (item[name] is JsonArray quantities) foreach (JsonNode? quantity in quantities)
                if (quantity?["epcClass"] is JsonValue value) actual.Add(value.GetValue<string>());
        if (item["parentID"] is JsonValue parent) actual.Add(parent.GetValue<string>());
        return expected.Any(actual.Contains);
    }

    private static DateTimeOffset EventTime(JsonObject item) => DateTimeOffset.Parse(item["eventTime"]!.GetValue<string>());

    private static string[] Values(string? value) => string.IsNullOrWhiteSpace(value) ? []
        : value.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);


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

    private static UseCaseResult TooLarge(string message = "An EPCIS document cannot exceed ten megabytes.") =>
        UseCaseResults.Json(new { code = "epcis_document_too_large", message }, statusCode: 413);

    private static UseCaseResult Bad(string code, string message) => UseCaseResults.BadRequest(new { code, message });

}
