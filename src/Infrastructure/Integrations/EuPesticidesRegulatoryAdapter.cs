using EdgIQ.MurimiOS.Application.Common.Abstractions;
using System.Globalization;
using System.Net;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IntegrationFeed;

namespace EdgIQ.MurimiOS.Infrastructure.Integrations;
using IntegrationFeed = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeed;

using IntegrationFeedType = global::EdgIQ.MurimiOS.Domain.IntegrationFeed.IntegrationFeedType;
using IProviderAdapter = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IProviderAdapter;
using ProviderAdapterException = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderAdapterException;
using ProviderIngestionContext = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderIngestionContext;
using ProviderIngestionResult = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderIngestionResult;
using ProviderRecord = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ProviderRecord;
using RegulatoryCatalogueItemValue = global::EdgIQ.MurimiOS.Application.Common.Abstractions.RegulatoryCatalogueItemValue;

public sealed class EuPesticidesRegulatoryAdapter(HttpClient httpClient) : IProviderAdapter
{
    public const string DefaultEndpoint = "https://api.datalake.sante.service.ec.europa.eu/sante/pesticides/active-substances-download?format=json&api-version=v3.0";
    public string ProviderCode => "eu-pesticides";
    public IntegrationFeedType FeedType => IntegrationFeedType.Regulatory;

    public async Task<ProviderIngestionResult> IngestAsync(ProviderIngestionContext context, CancellationToken cancellationToken)
    {
        JsonElement connection = ProviderJson.Object(context.ConnectionConfigurationJson, "Connection configuration");
        string endpoint = connection.String("endpoint") ?? DefaultEndpoint;
        using var request = new HttpRequestMessage(HttpMethod.Get, endpoint);
        if (!string.IsNullOrWhiteSpace(context.CheckpointJson))
        {
            JsonElement checkpoint = ProviderJson.Object(context.CheckpointJson, "Feed checkpoint");
            string? etag = checkpoint.String("etag");
            if (!string.IsNullOrWhiteSpace(etag)) request.Headers.TryAddWithoutValidation("If-None-Match", etag);
            if (DateTimeOffset.TryParse(checkpoint.String("lastModified"), out DateTimeOffset modified)) request.Headers.IfModifiedSince = modified;
        }
        using HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        if (response.StatusCode == HttpStatusCode.NotModified)
            return new ProviderIngestionResult([], context.CheckpointJson, (int)response.StatusCode);
        string payload = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode) throw Failure(response.StatusCode, payload);
        using JsonDocument document = JsonDocument.Parse(payload);
        JsonElement items = FindItems(document.RootElement);
        string version = response.Headers.ETag?.Tag?.Trim('"')
            ?? response.Content.Headers.LastModified?.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture)
            ?? context.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        var records = new List<ProviderRecord>();
        foreach (JsonElement item in items.EnumerateArray())
        {
            string? name = GetString(item, "name", "substanceName", "activeSubstanceName", "description");
            string? code = GetString(item, "id", "code", "substanceId", "activeSubstanceId");
            if (string.IsNullOrWhiteSpace(name)) continue;
            code = string.IsNullOrWhiteSpace(code) ? Slug(name) : code.Trim();
            string? status = GetString(item, "status", "approvalStatus", "state");
            DateOnly effectiveFrom = GetDate(item, "approvalDate", "effectiveFrom", "dateOfApproval") ?? DateOnly.FromDateTime(context.Now.UtcDateTime);
            DateOnly? effectiveTo = GetDate(item, "expirationDate", "expiryDate", "effectiveTo");
            if (effectiveTo < effectiveFrom) effectiveTo = null;
            string raw = item.GetRawText();
            string attributes = JsonSerializer.Serialize(new
            {
                approvalStatus = status,
                substanceType = GetString(item, "type", "substanceType"),
                lowRisk = GetBoolean(item, "lowRisk", "isLowRisk"),
                candidateForSubstitution = GetBoolean(item, "candidateForSubstitution", "isCandidateForSubstitution"),
                source = "European Commission EU Pesticides Database",
                sourceVersion = version
            });
            DateTimeOffset observedAt = GetDateTime(item, "lastUpdate", "modifiedAt", "updatedAt") ?? context.Now;
            records.Add(new ProviderRecord(code, observedAt, observedAt, raw,
                new RegulatoryCatalogueItemValue(version, code, name, status, attributes, effectiveFrom, effectiveTo)));
        }
        string? etagValue = response.Headers.ETag?.ToString();
        DateTimeOffset? lastModified = response.Content.Headers.LastModified;
        string checkpointJson = JsonSerializer.Serialize(new { etag = etagValue, lastModified, version, recordCount = records.Count });
        return new ProviderIngestionResult(records, checkpointJson, (int)response.StatusCode);
    }

    private static JsonElement FindItems(JsonElement root)
    {
        if (root.ValueKind == JsonValueKind.Array) return root;
        if (root.ValueKind == JsonValueKind.Object)
        {
            foreach (string name in new[] { "data", "items", "results", "value", "activeSubstances" })
                if (TryGet(root, name, out JsonElement value) && value.ValueKind == JsonValueKind.Array) return value;
        }
        throw new ProviderAdapterException("regulatory_response_invalid", "The EU pesticides response contained no record array.", true);
    }

    private static string? GetString(JsonElement item, params string[] names)
    {
        foreach (string name in names)
            if (TryGet(item, name, out JsonElement value))
                return value.ValueKind == JsonValueKind.String ? value.GetString() : value.ValueKind == JsonValueKind.Number ? value.GetRawText() : null;
        return null;
    }

    private static bool? GetBoolean(JsonElement item, params string[] names)
    {
        foreach (string name in names)
            if (TryGet(item, name, out JsonElement value))
                return value.ValueKind == JsonValueKind.True ? true : value.ValueKind == JsonValueKind.False ? false : null;
        return null;
    }

    private static DateOnly? GetDate(JsonElement item, params string[] names) =>
        DateOnly.TryParse(GetString(item, names), CultureInfo.InvariantCulture, DateTimeStyles.None, out DateOnly value) ? value : null;

    private static DateTimeOffset? GetDateTime(JsonElement item, params string[] names) =>
        DateTimeOffset.TryParse(GetString(item, names), CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out DateTimeOffset value) ? value.ToUniversalTime() : null;

    private static bool TryGet(JsonElement item, string name, out JsonElement value)
    {
        if (item.ValueKind == JsonValueKind.Object)
            foreach (JsonProperty property in item.EnumerateObject())
                if (property.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) { value = property.Value; return true; }
        value = default;
        return false;
    }

    private static string Slug(string value)
    {
        string slug = string.Concat(value.Trim().ToUpperInvariant().Select(character => char.IsLetterOrDigit(character) ? character : '-'));
        while (slug.Contains("--", StringComparison.Ordinal)) slug = slug.Replace("--", "-", StringComparison.Ordinal);
        return slug.Trim('-')[..Math.Min(120, slug.Trim('-').Length)];
    }

    private static ProviderAdapterException Failure(HttpStatusCode status, string payload)
    {
        bool retriable = (int)status is 408 or 425 or 429 || (int)status >= 500;
        string message = payload.Length > 1_000 ? payload[..1_000] : payload;
        return new ProviderAdapterException("regulatory_request_failed", $"EU pesticides API returned {(int)status}: {message}", retriable, (int)status);
    }
}
