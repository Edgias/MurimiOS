using EdgIQ.MurimiOS.Application.Common.Abstractions;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace EdgIQ.MurimiOS.Infrastructure.Integrations.Livestock;
using Location = global::EdgIQ.MurimiOS.Domain.Location.Location;

using IcarAdeCredentials = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IcarAdeCredentials;
using IcarAdeExchangeResult = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IcarAdeExchangeResult;
using IcarAdeOptions = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IcarAdeOptions;
using IIcarAdeExchangeAdapter = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IIcarAdeExchangeAdapter;

/// <summary>
/// Provider-neutral ICAR ADE JSON delivery. Endpoint and credentials belong to an IntegrationConnection,
/// allowing recording organisations to expose their own ADE-compatible URL without embedding vendor rules.
/// </summary>
public sealed class IcarAdeExchangeAdapter(HttpClient httpClient)
:EdgIQ.MurimiOS.Application.Common.Abstractions.IIcarAdeExchangeAdapter{
    public async Task<IcarAdeExchangeResult> SubmitAsync(IcarAdeOptions options, IcarAdeCredentials credentials,
        Guid submissionId, string payloadJson, CancellationToken cancellationToken)
    {
        string requestHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(payloadJson))).ToLowerInvariant();
        using var request = new HttpRequestMessage(HttpMethod.Post, options.Endpoint);
        request.Headers.TryAddWithoutValidation("Idempotency-Key", submissionId.ToString("N"));
        request.Headers.TryAddWithoutValidation("ICAR-ADE-Version", options.SchemaVersion);
        request.Headers.TryAddWithoutValidation("X-Organisation-Id", options.OrganisationId);
        if (!string.IsNullOrWhiteSpace(credentials.BearerToken))
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", credentials.BearerToken.Trim());
        if (!string.IsNullOrWhiteSpace(credentials.ApiKey))
            request.Headers.TryAddWithoutValidation("X-API-Key", credentials.ApiKey.Trim());
        request.Content = new StringContent(payloadJson, Encoding.UTF8, "application/json");

        try
        {
            using HttpResponseMessage response = await httpClient.SendAsync(request,
                HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            string body = await response.Content.ReadAsStringAsync(cancellationToken);
            return ParseResponse(body, (int)response.StatusCode, response.IsSuccessStatusCode, requestHash,
                response.Headers.Location?.ToString());
        }
        catch (Exception ex) when (ex is HttpRequestException
            || ex is TaskCanceledException && !cancellationToken.IsCancellationRequested)
        {
            return new(false, false, true, null, null, "icar_ade_transport_error", ex.Message,
                JsonSerializer.Serialize(new { error = ex.Message }), requestHash);
        }
    }

    public static IcarAdeExchangeResult ParseResponse(string responseBody, int httpStatus,
        bool successStatus, string requestHash, string? location = null)
    {
        JsonElement root = default;
        bool hasJson = false;
        try
        {
            if (!string.IsNullOrWhiteSpace(responseBody))
            {
                using JsonDocument parsed = JsonDocument.Parse(responseBody);
                root = parsed.RootElement.Clone();
                hasJson = true;
            }
        }
        catch (JsonException ex)
        {
            return successStatus
                ? new(false, false, true, httpStatus, null, "icar_ade_response_invalid", ex.Message,
                    JsonSerializer.Serialize(new { body = responseBody }), requestHash)
                : HttpFailure(httpStatus, responseBody, requestHash);
        }

        string? status = hasJson ? String(root, "status") : null;
        string? reference = hasJson
            ? String(root, "reference") ?? String(root, "submissionId") ?? String(root, "id")
            : null;
        reference ??= location;
        string? code = hasJson ? String(root, "errorCode") ?? String(root, "code") : null;
        string? message = hasJson ? String(root, "errorMessage") ?? String(root, "message") : null;
        string safeBody = hasJson ? root.GetRawText() : "{}";

        if (!successStatus) return HttpFailure(httpStatus, safeBody, requestHash, code, message);
        bool pending = httpStatus == 202 || status?.Equals("submitted", StringComparison.OrdinalIgnoreCase) == true
            || status?.Equals("pending", StringComparison.OrdinalIgnoreCase) == true;
        bool rejected = status?.Equals("rejected", StringComparison.OrdinalIgnoreCase) == true
            || status?.Equals("invalid", StringComparison.OrdinalIgnoreCase) == true;
        if (rejected)
            return new(false, false, false, httpStatus, reference, code ?? "icar_ade_rejected",
                message ?? "The recording organisation rejected the exchange.", safeBody, requestHash);
        if (string.IsNullOrWhiteSpace(reference))
            return new(false, false, true, httpStatus, null, "icar_ade_reference_missing",
                "The recording organisation did not return a submission reference.", safeBody, requestHash);
        return new(!pending, pending, false, httpStatus, reference, null, null, safeBody, requestHash);
    }

    private static IcarAdeExchangeResult HttpFailure(int status, string body, string hash,
        string? code = null, string? message = null)
    {
        bool retriable = status is 408 or 425 or 429 or >= 500;
        return new(false, false, retriable, status, null, code ?? "icar_ade_http_error",
            message ?? $"The recording organisation returned HTTP {status}.",
            body.TrimStart().StartsWith('{') ? body : JsonSerializer.Serialize(new { body }), hash);
    }

    private static string? String(JsonElement root, string name) => root.ValueKind == JsonValueKind.Object
        && root.TryGetProperty(name, out JsonElement value) && value.ValueKind == JsonValueKind.String
            ? value.GetString()?.Trim() : null;
}
