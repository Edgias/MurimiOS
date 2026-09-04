using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;
using EdgIQ.MurimiOS.Domain.TraceEvent;

using EdgIQ.MurimiOS.Domain.Delivery;

namespace EdgIQ.MurimiOS.Application.Common.Traceability;
using Delivery = global::EdgIQ.MurimiOS.Domain.Delivery.Delivery;
using TraceEvent = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEvent;
using TraceEventObject = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventObject;
using TraceEventType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceEventType;
using TraceObjectRole = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectRole;
using TraceObjectType = global::EdgIQ.MurimiOS.Domain.TraceEvent.TraceObjectType;

public static partial class EpcisInterop
{
    public const string EpcisVersion = "2.0.1";
    public const string SchemaVersion = "2.0";
    public const string CbvVersion = "2.0.0";
    public const string ContextUri = "https://ref.gs1.org/standards/epcis/epcis-context.jsonld";
    public const int MaximumCaptureEvents = 5_000;

    private static readonly HashSet<string> EventTypes =
        ["ObjectEvent", "AggregationEvent", "AssociationEvent", "TransformationEvent", "TransactionEvent"];
    private static readonly HashSet<string> Actions = ["ADD", "OBSERVE", "DELETE"];
    private static readonly HashSet<string> BusinessSteps =
    [
        "accepting", "arriving", "assembling", "collecting", "commissioning", "consigning",
        "creating_class_instance", "cycle_counting", "decommissioning", "departing", "destroying",
        "dispensing", "encoding", "entering_exiting", "holding", "inspecting", "loading", "other",
        "packing", "picking", "receiving", "removing", "sampling", "sensor_reporting", "shipping",
        "staging_outbound", "stock_taking", "stocking", "storing", "transforming", "transporting",
        "unloading", "unpacking", "void_shipping"
    ];
    private static readonly HashSet<string> Dispositions =
    [
        "active", "available", "completeness_inferred", "completeness_verified", "conformant",
        "container_closed", "container_open", "damaged", "destroyed", "dispensed", "disposed",
        "encoded", "expired", "in_progress", "inactive", "in_transit", "mismatch_class",
        "mismatch_instance", "mismatch_quantity", "needs_replacement", "no_pedigree_match",
        "non_conformant", "partially_dispensed", "recalled", "reserved", "retail_sold",
        "returned", "stolen", "unavailable", "unknown"
    ];

    public static EpcisCaptureValidation ValidateCapture(ReadOnlySpan<byte> utf8)
    {
        var issues = new List<EpcisValidationIssue>();
        JsonDocument document;
        try { document = JsonDocument.Parse(utf8.ToArray()); }
        catch (JsonException exception)
        {
            return new(null, null, null, null, [], [new("invalid_json", "$", exception.Message)]);
        }
        using (document)
        {
            JsonElement root = document.RootElement;
            if (root.ValueKind != JsonValueKind.Object)
                return new(null, null, null, null, [], [new("document_type_invalid", "$", "An EPCIS document must be a JSON object.")]);
            RequiredString(root, "type", "$", issues, value => value == "EPCISDocument",
                "type must be EPCISDocument.");
            RequiredString(root, "schemaVersion", "$", issues, value => value == SchemaVersion,
                $"schemaVersion must be {SchemaVersion}.");
            if (!root.TryGetProperty("@context", out JsonElement context)
                || context.ValueKind is not (JsonValueKind.String or JsonValueKind.Array or JsonValueKind.Object))
                issues.Add(new("context_required", "$.@context", "EPCIS JSON/JSON-LD requires a context."));
            if (root.TryGetProperty("creationDate", out JsonElement creation)
                && (creation.ValueKind != JsonValueKind.String || !DateTimeOffset.TryParse(creation.GetString(), CultureInfo.InvariantCulture,
                    DateTimeStyles.RoundtripKind, out _)))
                issues.Add(new("creation_date_invalid", "$.creationDate", "creationDate must be an ISO 8601 timestamp."));

            JsonElement eventList = default;
            if (!root.TryGetProperty("epcisBody", out JsonElement body) || body.ValueKind != JsonValueKind.Object
                || !body.TryGetProperty("eventList", out eventList) || eventList.ValueKind != JsonValueKind.Array)
                issues.Add(new("event_list_required", "$.epcisBody.eventList", "epcisBody.eventList must be an array."));

            var events = new List<EpcisCapturedEnvelope>();
            if (eventList.ValueKind == JsonValueKind.Array)
            {
                if (eventList.GetArrayLength() == 0 || eventList.GetArrayLength() > MaximumCaptureEvents)
                    issues.Add(new("event_count_invalid", "$.epcisBody.eventList", $"Capture requires 1-{MaximumCaptureEvents} events."));
                int index = 0;
                foreach (JsonElement item in eventList.EnumerateArray())
                {
                    ValidateEvent(item, index++, issues, events);
                }
            }

            string? id = OptionalAbsoluteUri(root, "id", "$.id", issues);
            string? sender = OptionalText(root, "sender", 500, "$.sender", issues);
            string? receiver = OptionalText(root, "receiver", 500, "$.receiver", issues);
            return new(root.Clone(), id, sender, receiver, events, issues);
        }
    }

    public static JsonObject InternalEvent(Guid tenantId, TraceEvent traceEvent,
        IReadOnlyCollection<TraceEventObject> objects)
    {
        string eventType = traceEvent.EventType is TraceEventType.Transformation or TraceEventType.Aggregation
            ? "TransformationEvent" : traceEvent.EventType == TraceEventType.Transaction ? "TransactionEvent" : "ObjectEvent";
        var result = new JsonObject
        {
            ["eventID"] = $"urn:uuid:{traceEvent.Id:D}",
            ["type"] = eventType,
            ["eventTime"] = traceEvent.OccurredAt.ToString("O", CultureInfo.InvariantCulture),
            ["recordTime"] = traceEvent.RecordedAt.ToString("O", CultureInfo.InvariantCulture),
            ["eventTimeZoneOffset"] = traceEvent.OccurredAt.ToString("zzz", CultureInfo.InvariantCulture),
            ["bizStep"] = BusinessStep(traceEvent.BusinessStep),
            ["disposition"] = Disposition(traceEvent.Disposition),
            ["bizLocation"] = new JsonObject { ["id"] = Location(tenantId, traceEvent.FarmId, "farm") },
            ["https://murimios.io/epcis/sourceType"] = traceEvent.SourceType,
            ["https://murimios.io/epcis/sourceID"] = $"urn:uuid:{traceEvent.SourceId:D}"
        };
        if (traceEvent.LocationId is { } locationId)
            result["readPoint"] = new JsonObject { ["id"] = Location(tenantId, locationId, traceEvent.LocationType) };

        if (eventType == "TransformationEvent")
        {
            AddObjects(result, "inputEPCList", "inputQuantityList", tenantId,
                objects.Where(x => x.Role == TraceObjectRole.Input));
            AddObjects(result, "outputEPCList", "outputQuantityList", tenantId,
                objects.Where(x => x.Role == TraceObjectRole.Output));
            result["transformationID"] = $"urn:uuid:{traceEvent.SourceId:D}";
        }
        else
        {
            AddObjects(result, "epcList", "quantityList", tenantId,
                objects.Where(x => x.Role is TraceObjectRole.Subject or TraceObjectRole.Input or TraceObjectRole.Output));
            result["action"] = eventType == "ObjectEvent" && objects.Any(x => x.Role == TraceObjectRole.Output)
                ? "ADD" : "OBSERVE";
            if (eventType == "TransactionEvent")
            {
                result["bizTransactionList"] = new JsonArray(new JsonObject
                {
                    ["type"] = traceEvent.BusinessStep.Equals("shipping", StringComparison.OrdinalIgnoreCase) ? "desadv"
                        : traceEvent.BusinessStep.Equals("receiving", StringComparison.OrdinalIgnoreCase) ? "recadv" : "upevt",
                    ["bizTransaction"] = Transaction(traceEvent.TransactionReference, traceEvent.SourceId)
                });
                TraceEventObject? parent = objects.FirstOrDefault(x => x.ObjectType == TraceObjectType.Delivery);
                if (parent is not null) result["parentID"] = ObjectIdentifier(tenantId, parent.ObjectType, parent.ObjectId);
            }
        }
        return result;
    }

    public static JsonObject QueryDocument(Guid id, DateTimeOffset createdAt, IEnumerable<JsonNode> events) => new()
    {
        ["@context"] = new JsonArray(ContextUri, new JsonObject { ["murimi"] = "https://murimios.io/epcis/" }),
        ["id"] = $"urn:uuid:{id:D}",
        ["type"] = "EPCISQueryDocument",
        ["schemaVersion"] = SchemaVersion,
        ["creationDate"] = createdAt.ToString("O", CultureInfo.InvariantCulture),
        ["epcisBody"] = new JsonObject
        {
            ["queryResults"] = new JsonObject
            {
                ["queryName"] = "SimpleEventQuery",
                ["resultsBody"] = new JsonObject { ["eventList"] = new JsonArray(events.Select(x => x.DeepClone()).ToArray()) }
            }
        }
    };

    public static string BusinessStep(string value)
    {
        string trimmed = value.Trim();
        if (System.Uri.TryCreate(trimmed, UriKind.Absolute, out _)) return trimmed;
        string normalized = Normalize(value);
        if (normalized == "harvesting") return "commissioning";
        if (normalized == "crop_to_feed") return "creating_class_instance";
        return BusinessSteps.Contains(normalized) ? normalized : Custom("bizstep", normalized);
    }

    public static string Disposition(string value)
    {
        string trimmed = value.Trim();
        if (System.Uri.TryCreate(trimmed, UriKind.Absolute, out _)) return trimmed;
        string normalized = Normalize(value);
        if (normalized == "harvested") return "active";
        if (normalized == "feed_available" || normalized == "received") return "available";
        return Dispositions.Contains(normalized) ? normalized : Custom("disposition", normalized);
    }

    public static string ObjectIdentifier(Guid tenantId, TraceObjectType type, Guid id) =>
        $"urn:murimios:{tenantId:N}:trace-object:{type.ToString().ToLowerInvariant()}:{id:N}";

    private static void ValidateEvent(JsonElement item, int index, List<EpcisValidationIssue> issues,
        List<EpcisCapturedEnvelope> events)
    {
        string path = $"$.epcisBody.eventList[{index}]";
        if (item.ValueKind != JsonValueKind.Object) { issues.Add(new("event_invalid", path, "An event must be an object.")); return; }
        string? type = GetRequired(item, "type", path, issues);
        if (type is null || !EventTypes.Contains(type)) issues.Add(new("event_type_invalid", path + ".type", "Unsupported EPCIS event type."));
        string? eventTimeText = GetRequired(item, "eventTime", path, issues);
        DateTimeOffset eventTime = default;
        if (eventTimeText is null || !DateTimeOffset.TryParse(eventTimeText, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out eventTime))
            issues.Add(new("event_time_invalid", path + ".eventTime", "eventTime must be an ISO 8601 timestamp."));
        string? offset = GetRequired(item, "eventTimeZoneOffset", path, issues);
        if (offset is null || !TimeZoneOffset().IsMatch(offset))
            issues.Add(new("event_timezone_invalid", path + ".eventTimeZoneOffset", "eventTimeZoneOffset must be ±hh:mm and no greater than 14:00."));
        if (type is "ObjectEvent" or "AggregationEvent" or "AssociationEvent" or "TransactionEvent")
        {
            string? action = GetRequired(item, "action", path, issues);
            if (action is null || !Actions.Contains(action)) issues.Add(new("event_action_invalid", path + ".action", "action must be ADD, OBSERVE, or DELETE."));
        }
        string? bizStep = Vocabulary(item, "bizStep", path, BusinessSteps, "BizStep", issues);
        string? disposition = Vocabulary(item, "disposition", path, Dispositions, "Disp", issues);
        DateTimeOffset? recordTime = null;
        if (item.TryGetProperty("recordTime", out JsonElement record))
        {
            if (record.ValueKind != JsonValueKind.String || !DateTimeOffset.TryParse(record.GetString(), CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind, out DateTimeOffset parsed)) issues.Add(new("record_time_invalid", path + ".recordTime", "recordTime must be an ISO 8601 timestamp."));
            else recordTime = parsed;
        }
        List<string> identifiers = Identifiers(item, path, issues);
        string raw = item.GetRawText();
        string hash = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(raw)));
        string? eventId = OptionalAbsoluteUri(item, "eventID", path + ".eventID", issues)
            ?? $"ni:///sha-256;{hash}?ver=CBV2.0";
        if (type is not null && eventTime != default && eventId is not null)
            events.Add(new(eventId, type, eventTime, recordTime, bizStep, disposition, identifiers, hash, raw));
    }

    private static List<string> Identifiers(JsonElement item, string path, List<EpcisValidationIssue> issues)
    {
        var result = new HashSet<string>(StringComparer.Ordinal);
        foreach (string name in new[] { "epcList", "childEPCs", "inputEPCList", "outputEPCList" })
            if (item.TryGetProperty(name, out JsonElement list)) AddUriArray(list, path + "." + name, result, issues);
        foreach (string name in new[] { "quantityList", "childQuantityList", "inputQuantityList", "outputQuantityList" })
            if (item.TryGetProperty(name, out JsonElement list))
            {
                if (list.ValueKind != JsonValueKind.Array) issues.Add(new("identifier_list_invalid", path + "." + name, "Quantity identifiers must be an array."));
                else foreach (JsonElement quantity in list.EnumerateArray())
                {
                    if (quantity.ValueKind != JsonValueKind.Object || !quantity.TryGetProperty("epcClass", out JsonElement epcClass)
                        || epcClass.ValueKind != JsonValueKind.String || !System.Uri.TryCreate(epcClass.GetString(), UriKind.Absolute, out _))
                        issues.Add(new("epc_class_invalid", path + "." + name, "Every quantity element requires an absolute epcClass URI."));
                    else result.Add(epcClass.GetString()!);
                }
            }
        if (item.TryGetProperty("parentID", out JsonElement parent))
        {
            if (parent.ValueKind != JsonValueKind.String || !System.Uri.TryCreate(parent.GetString(), UriKind.Absolute, out _))
                issues.Add(new("parent_id_invalid", path + ".parentID", "parentID must be an absolute URI."));
            else result.Add(parent.GetString()!);
        }
        return result.Order().ToList();
    }

    private static void AddUriArray(JsonElement list, string path, HashSet<string> target, List<EpcisValidationIssue> issues)
    {
        if (list.ValueKind != JsonValueKind.Array) { issues.Add(new("identifier_list_invalid", path, "Identifiers must be an array.")); return; }
        foreach (JsonElement identifier in list.EnumerateArray())
            if (identifier.ValueKind != JsonValueKind.String || !System.Uri.TryCreate(identifier.GetString(), UriKind.Absolute, out _))
                issues.Add(new("identifier_invalid", path, "Every identifier must be an absolute URI."));
            else target.Add(identifier.GetString()!);
    }

    private static void AddObjects(JsonObject target, string epcProperty, string quantityProperty, Guid tenantId,
        IEnumerable<TraceEventObject> source)
    {
        TraceEventObject[] objects = source.ToArray();
        JsonNode[] epcs = objects.Where(x => x.Quantity is null)
            .Select(x => (JsonNode)JsonValue.Create(ObjectIdentifier(tenantId, x.ObjectType, x.ObjectId))!).ToArray();
        JsonNode[] quantities = objects.Where(x => x.Quantity is not null).Select(x => (JsonNode)new JsonObject
        {
            ["epcClass"] = ObjectIdentifier(tenantId, x.ObjectType, x.ObjectId),
            ["quantity"] = x.Quantity,
            ["uom"] = Unit(x.Unit!)
        }).ToArray();
        if (epcs.Length > 0) target[epcProperty] = new JsonArray(epcs);
        if (quantities.Length > 0) target[quantityProperty] = new JsonArray(quantities);
    }

    private static string Unit(string unit) => unit.ToUpperInvariant() switch
    {
        "KG" => "KGM",
        "L" => "LTR",
        "EACH" or "HEAD" => "C62",
        _ => throw new ArgumentException($"No EPCIS UN/CEFACT unit mapping exists for '{unit}'.")
    };
    private static string Location(Guid tenantId, Guid id, string type) =>
        $"urn:murimios:{tenantId:N}:location:{Normalize(type)}:{id:N}";
    private static string Transaction(string? reference, Guid sourceId) =>
        string.IsNullOrWhiteSpace(reference) ? $"urn:uuid:{sourceId:D}" : $"urn:murimios:biz-transaction:{Uri.EscapeDataString(reference.Trim())}";
    private static string Custom(string vocabulary, string value) => $"urn:murimios:cbv:{vocabulary}:{Uri.EscapeDataString(value)}";
    private static string Normalize(string value) => value.Trim().ToLowerInvariant().Replace('-', '_').Replace(' ', '_');

    private static string? Vocabulary(JsonElement item, string property, string path, HashSet<string> known,
        string cbvType, List<EpcisValidationIssue> issues)
    {
        if (!item.TryGetProperty(property, out JsonElement value)) return null;
        if (value.ValueKind != JsonValueKind.String) { issues.Add(new("vocabulary_invalid", path + "." + property, "Vocabulary value must be a string.")); return null; }
        string text = value.GetString()!;
        string compact = Normalize(text);
        if (known.Contains(compact)) return compact;
        if (System.Uri.TryCreate(text, UriKind.Absolute, out _)) return text;
        issues.Add(new("vocabulary_invalid", path + "." + property,
            $"Use a CBV {cbvType} bare value or an absolute custom URI."));
        return text;
    }

    private static string? GetRequired(JsonElement item, string property, string path, List<EpcisValidationIssue> issues)
    {
        if (!item.TryGetProperty(property, out JsonElement value) || value.ValueKind != JsonValueKind.String || string.IsNullOrWhiteSpace(value.GetString()))
        { issues.Add(new("property_required", path + "." + property, property + " is required.")); return null; }
        return value.GetString();
    }
    private static void RequiredString(JsonElement item, string property, string path, List<EpcisValidationIssue> issues,
        Func<string, bool> predicate, string message)
    {
        string? value = GetRequired(item, property, path, issues);
        if (value is not null && !predicate(value)) issues.Add(new("property_invalid", path + "." + property, message));
    }
    private static string? OptionalAbsoluteUri(JsonElement item, string property, string path, List<EpcisValidationIssue> issues)
    {
        if (!item.TryGetProperty(property, out JsonElement value)) return null;
        if (value.ValueKind != JsonValueKind.String || !System.Uri.TryCreate(value.GetString(), UriKind.Absolute, out _))
        { issues.Add(new("uri_invalid", path, property + " must be an absolute URI.")); return null; }
        return value.GetString();
    }
    private static string? OptionalText(JsonElement item, string property, int maximum, string path, List<EpcisValidationIssue> issues)
    {
        if (!item.TryGetProperty(property, out JsonElement value)) return null;
        if (value.ValueKind != JsonValueKind.String || value.GetString()!.Length > maximum)
        { issues.Add(new("text_invalid", path, property + $" must be a string no longer than {maximum} characters.")); return null; }
        return value.GetString();
    }

    [GeneratedRegex("^([+-])((0[0-9]|1[0-3]):[0-5][0-9]|14:00)$", RegexOptions.CultureInvariant)]
    private static partial Regex TimeZoneOffset();
}
