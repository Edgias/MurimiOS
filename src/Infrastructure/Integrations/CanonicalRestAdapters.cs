using EdgIQ.MurimiOS.Application.Common.Abstractions;
using System.Globalization;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.FinanceTransaction;
using EdgIQ.MurimiOS.Domain.BusinessDocumentExchange;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;

namespace EdgIQ.MurimiOS.Infrastructure.Integrations;
using BusinessDocumentExchange = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.BusinessDocumentExchange;
using FinanceTransaction = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceTransaction;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;

using BusinessDocumentType = global::EdgIQ.MurimiOS.Domain.BusinessDocumentExchange.BusinessDocumentType;
using FinanceEntryType = global::EdgIQ.MurimiOS.Domain.FinanceTransaction.FinanceEntryType;
using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using AccountingBatchValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.AccountingBatchValue;
using AccountingLineValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.AccountingLineValue;
using BusinessDocumentValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.BusinessDocumentValue;
using IProviderAdapter = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IProviderAdapter;
using MachineryTelemetryValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.MachineryTelemetryValue;
using ProviderAdapterException = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderAdapterException;
using ProviderIngestionContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderIngestionContext;
using ProviderIngestionResult = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderIngestionResult;
using ProviderRecord = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderRecord;

public abstract class CanonicalRestAdapter(HttpClient httpClient) : IProviderAdapter
{
    public const string Code = "canonical-rest";
    public string ProviderCode => Code;
    public abstract IntegrationFeedType FeedType { get; }

    public async Task<ProviderIngestionResult> IngestAsync(ProviderIngestionContext context, CancellationToken cancellationToken)
    {
        JsonElement connection = ProviderJson.Object(context.ConnectionConfigurationJson, "connection configuration");
        JsonElement feed = ProviderJson.Object(context.FeedConfigurationJson, "feed configuration");
        string endpointText = feed.String("endpoint") ?? connection.String("endpoint")
            ?? throw new ProviderAdapterException("rest_endpoint_missing", "A canonical REST endpoint is required.", false);
        if (!Uri.TryCreate(endpointText, UriKind.Absolute, out Uri? endpoint) || endpoint.Scheme != Uri.UriSchemeHttps)
            throw new ProviderAdapterException("rest_endpoint_invalid", "The canonical REST endpoint must use HTTPS.", false);
        string? cursor = CheckpointCursor(context.CheckpointJson);
        var builder = new UriBuilder(endpoint);
        if (!string.IsNullOrWhiteSpace(cursor))
        {
            string parameter = feed.String("cursorParameter") ?? "cursor";
            builder.Query = string.IsNullOrEmpty(builder.Query) ? $"{Uri.EscapeDataString(parameter)}={Uri.EscapeDataString(cursor)}"
                : $"{builder.Query.TrimStart('?')}&{Uri.EscapeDataString(parameter)}={Uri.EscapeDataString(cursor)}";
        }
        using var request = new HttpRequestMessage(HttpMethod.Get, builder.Uri);
        ApplyCredential(request, context.CredentialSecret);
        using HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        string body = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode) throw Failure(response.StatusCode, body);
        JsonElement root = ProviderJson.Object(body, "canonical REST response");
        IReadOnlyList<ProviderRecord> records = Parse(root, context);
        string? next = root.String("nextCursor");
        return new ProviderIngestionResult(records, next is null ? context.CheckpointJson : JsonSerializer.Serialize(new { cursor = next }),
            (int)response.StatusCode, 1, RateLimit(response), null);
    }

    protected abstract IReadOnlyList<ProviderRecord> Parse(JsonElement root, ProviderIngestionContext context);

    protected static JsonElement[] Array(JsonElement root, string name)
    {
        if (!root.TryGetProperty(name, out JsonElement value) || value.ValueKind != JsonValueKind.Array)
            throw new ProviderAdapterException("rest_response_invalid", $"Canonical response requires array '{name}'.", false);
        return value.EnumerateArray().ToArray();
    }
    protected static DateTimeOffset Time(JsonElement value, string name) => DateTimeOffset.TryParse(value.String(name),
        CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset result) ? result.ToUniversalTime()
        : throw new ProviderAdapterException("rest_time_invalid", $"'{name}' must be an ISO-8601 timestamp.", false);
    protected static DateOnly Date(JsonElement value, string name) => DateOnly.TryParse(value.String(name), CultureInfo.InvariantCulture,
        DateTimeStyles.None, out DateOnly result) ? result : throw new ProviderAdapterException("rest_date_invalid", $"'{name}' must be an ISO date.", false);
    protected static T EnumValue<T>(JsonElement value, string name) where T : struct, Enum => Enum.TryParse(value.String(name), true, out T result)
        ? result : throw new ProviderAdapterException("rest_enum_invalid", $"'{name}' is not a supported {typeof(T).Name}.", false);

    private static void ApplyCredential(HttpRequestMessage request, string? credentialJson)
    {
        if (string.IsNullOrWhiteSpace(credentialJson)) return;
        JsonElement credential = ProviderJson.Object(credentialJson, "credential");
        string scheme = credential.String("scheme") ?? "Bearer";
        string? token = credential.String("token");
        if (!string.IsNullOrWhiteSpace(token)) request.Headers.Authorization = new AuthenticationHeaderValue(scheme, token);
        string? header = credential.String("header"); string? value = credential.String("value");
        if (!string.IsNullOrWhiteSpace(header) && !string.IsNullOrWhiteSpace(value)) request.Headers.TryAddWithoutValidation(header, value);
    }
    private static string? CheckpointCursor(string? json)
    {
        if (string.IsNullOrWhiteSpace(json)) return null;
        return ProviderJson.Object(json, "checkpoint").String("cursor");
    }
    private static int? RateLimit(HttpResponseMessage response) => response.Headers.TryGetValues("X-RateLimit-Remaining", out IEnumerable<string>? values)
        && int.TryParse(values.FirstOrDefault(), out int result) ? result : null;
    private static ProviderAdapterException Failure(HttpStatusCode status, string payload)
    {
        bool retry = (int)status is 408 or 425 or 429 or >= 500;
        string excerpt = payload.Length <= 500 ? payload : payload[..500];
        return new ProviderAdapterException("canonical_rest_failed", $"Canonical REST endpoint returned {(int)status}: {excerpt}", retry, (int)status);
    }
}

public sealed class CanonicalMachineryRestAdapter(HttpClient httpClient) : CanonicalRestAdapter(httpClient)
{
    public override IntegrationFeedType FeedType => IntegrationFeedType.Machinery;
    protected override IReadOnlyList<ProviderRecord> Parse(JsonElement root, ProviderIngestionContext context) => Array(root, "records").Select(item =>
    {
        DateTimeOffset observed = Time(item, "observedAt");
        string id = item.String("id") ?? throw new ProviderAdapterException("rest_record_id_missing", "Machinery record id is required.", false);
        string equipment = item.String("equipmentCode") ?? throw new ProviderAdapterException("rest_equipment_missing", "Equipment code is required.", false);
        var value = new MachineryTelemetryValue(equipment, item.String("deviceKey"), item.Decimal("engineHours"), item.Decimal("speedKph"),
            item.Decimal("fuelLevelPercent"), item.String("state") ?? "unknown", Double(item, "latitude"), Double(item, "longitude"));
        return new ProviderRecord(id, observed, null, item.GetRawText(), value);
    }).ToArray();
    private static double? Double(JsonElement item, string name) => item.TryGetProperty(name, out JsonElement value) && value.TryGetDouble(out double result) ? result : null;
}

public sealed class CanonicalErpRestAdapter(HttpClient httpClient) : CanonicalRestAdapter(httpClient)
{
    public override IntegrationFeedType FeedType => IntegrationFeedType.Erp;
    protected override IReadOnlyList<ProviderRecord> Parse(JsonElement root, ProviderIngestionContext context) => Array(root, "documents").Select(item =>
    {
        string id = item.String("id") ?? throw new ProviderAdapterException("rest_document_id_missing", "ERP document id is required.", false);
        DateTimeOffset occurred = Time(item, "occurredAt");
        JsonElement document = item.TryGetProperty("payload", out JsonElement payload) ? payload : item;
        var value = new BusinessDocumentValue(EnumValue<BusinessDocumentType>(item, "type"), id,
            item.Int32("schemaVersion", 1), item.String("mappingVersion") ?? "canonical-v1", document.GetRawText());
        return new ProviderRecord(id, occurred, null, item.GetRawText(), value);
    }).ToArray();
}

public sealed class CanonicalAccountingRestAdapter(HttpClient httpClient) : CanonicalRestAdapter(httpClient)
{
    public override IntegrationFeedType FeedType => IntegrationFeedType.Accounting;
    protected override IReadOnlyList<ProviderRecord> Parse(JsonElement root, ProviderIngestionContext context) => Array(root, "batches").Select(batch =>
    {
        string id = batch.String("id") ?? throw new ProviderAdapterException("rest_batch_id_missing", "Accounting batch id is required.", false);
        DateOnly start = Date(batch, "statementStart"); DateOnly end = Date(batch, "statementEnd");
        AccountingLineValue[] lines = Array(batch, "lines").Select(line => new AccountingLineValue(
            line.String("id") ?? throw new ProviderAdapterException("rest_line_id_missing", "Accounting line id is required.", false),
            Time(line, "occurredAt"), EnumValue<FinanceEntryType>(line, "entryType"),
            line.Decimal("amount") ?? throw new ProviderAdapterException("rest_amount_missing", "Accounting amount is required.", false),
            line.String("accountCode") ?? throw new ProviderAdapterException("rest_account_missing", "Account code is required.", false),
            line.String("reference"), line.String("description") ?? "Imported accounting line")).ToArray();
        var value = new AccountingBatchValue(id, start, end, batch.String("currency") ?? "USD", lines);
        return new ProviderRecord(id, new DateTimeOffset(end.ToDateTime(TimeOnly.MinValue), TimeSpan.Zero), null, batch.GetRawText(), value);
    }).ToArray();
}
