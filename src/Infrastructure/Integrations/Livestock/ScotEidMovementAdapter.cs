using EdgIQ.MurimiOS.Application.Common.Abstractions;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Xml.Linq;

namespace EdgIQ.MurimiOS.Infrastructure.Integrations.Livestock;
using IScotEidMovementAdapter = global::EdgIQ.MurimiOS.Application.Common.Abstractions.IScotEidMovementAdapter;
using ScotEidCredentials = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ScotEidCredentials;
using ScotEidMovementSubmission = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ScotEidMovementSubmission;
using ScotEidOptions = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ScotEidOptions;
using ScotEidSubmissionResult = global::EdgIQ.MurimiOS.Application.Common.Abstractions.ScotEidSubmissionResult;

public sealed class ScotEidMovementAdapter(HttpClient httpClient)
:EdgIQ.MurimiOS.Application.Common.Abstractions.IScotEidMovementAdapter{
    private static readonly XNamespace Soap = "http://schemas.xmlsoap.org/soap/envelope/";
    private static readonly XNamespace Service = "http://tempuri.org/";

    public async Task<ScotEidSubmissionResult> SubmitAsync(ScotEidOptions options,
        ScotEidCredentials credentials, ScotEidMovementSubmission movement, CancellationToken cancellationToken)
    {
        string xml = BuildEnvelope(options, credentials, movement, DateTimeOffset.UtcNow);
        string requestHash = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(xml))).ToLowerInvariant();
        using var request = new HttpRequestMessage(HttpMethod.Post, options.Endpoint);
        request.Headers.Authorization = new AuthenticationHeaderValue("Basic",
            Convert.ToBase64String(Encoding.UTF8.GetBytes($"{credentials.Username}:{credentials.Password}")));
        request.Headers.TryAddWithoutValidation("SOAPAction", options.SoapAction);
        request.Content = new StringContent(xml, Encoding.UTF8, "text/xml");
        try
        {
            using HttpResponseMessage response = await httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            string body = await response.Content.ReadAsStringAsync(cancellationToken);
            if (!response.IsSuccessStatusCode)
            {
                bool retriable = (int)response.StatusCode is 408 or 425 or 429 or >= 500;
                return new(false, retriable, (int)response.StatusCode, null, "scoteid_http_error",
                    $"ScotEID returned HTTP {(int)response.StatusCode}.", JsonSerializer.Serialize(new { body }), requestHash);
            }
            return ParseResponse(body, (int)response.StatusCode, requestHash);
        }
        catch (Exception ex) when (ex is HttpRequestException or TaskCanceledException && !cancellationToken.IsCancellationRequested)
        {
            return new(false, true, null, null, "scoteid_transport_error", ex.Message,
                JsonSerializer.Serialize(new { error = ex.Message }), requestHash);
        }
    }

    public static string BuildEnvelope(ScotEidOptions options, ScotEidCredentials credentials,
        ScotEidMovementSubmission movement, DateTimeOffset timestamp)
    {
        static XElement E(string name, object? value) => new(Service + name, value);
        var movementElement = new XElement(Service + "Movement",
            E("Row", 1), E("AnimalID", movement.AnimalId), E("DepartureMainCPH", movement.DepartureMainCph),
            E("DestinationMainCPH", movement.DestinationMainCph), E("DestinationLocation", movement.DestinationLocation),
            E("DepartureDate", movement.DepartureDate.ToString("yyyy-MM-dd")),
            E("MoveDate", movement.MoveDate.ToString("yyyy-MM-dd")), E("UserReference", movement.UserReference));
        var operation = new XElement(Service + "SMECreateCattleMovementsRequest",
            E("ApplicationName", options.ApplicationName), E("ApplicationVersion", options.ApplicationVersion),
            E("ApplicationKey", credentials.ApplicationKey), E("SchemaVersion", options.SchemaVersion),
            E("Timestamp", timestamp.UtcDateTime.ToString("O")),
            new XElement(Service + "Movements", movementElement));
        var document = new XDocument(new XElement(Soap + "Envelope",
            new XAttribute(XNamespace.Xmlns + "soap", Soap), new XAttribute(XNamespace.Xmlns + "svc", Service),
            new XElement(Soap + "Body", operation)));
        return document.ToString(SaveOptions.DisableFormatting);
    }

    public static ScotEidSubmissionResult ParseResponse(string responseXml, int httpStatus, string requestHash)
    {
        try
        {
            XDocument document = XDocument.Parse(responseXml, LoadOptions.None);
            string? status = Value(document, "Status") ?? Value(document, "Result");
            string? reference = Value(document, "MovementReference");
            XElement? error = document.Descendants().FirstOrDefault(x => x.Name.LocalName.Equals("Error", StringComparison.OrdinalIgnoreCase));
            string? code = error?.Attribute("Code")?.Value ?? ChildValue(error, "Code");
            string? message = error?.Attribute("Message")?.Value ?? ChildValue(error, "Message") ?? Value(document, "Message");
            bool accepted = status?.Equals("Success", StringComparison.OrdinalIgnoreCase) == true
                || status?.Equals("Accepted", StringComparison.OrdinalIgnoreCase) == true;
            if (accepted && string.IsNullOrWhiteSpace(reference))
                return new(false, false, httpStatus, null, "scoteid_reference_missing",
                    "ScotEID accepted the request without a movement reference.", JsonSerializer.Serialize(new { status }), requestHash);
            return accepted
                ? new(true, false, httpStatus, reference, null, null, JsonSerializer.Serialize(new { status, movementReference = reference }), requestHash)
                : new(false, false, httpStatus, null, code ?? "scoteid_validation_error", message ?? "ScotEID rejected the movement.",
                    JsonSerializer.Serialize(new { status, code, message }), requestHash);
        }
        catch (Exception ex) when (ex is System.Xml.XmlException or InvalidOperationException)
        {
            return new(false, true, httpStatus, null, "scoteid_response_invalid", ex.Message,
                JsonSerializer.Serialize(new { error = ex.Message }), requestHash);
        }
    }

    private static string? Value(XContainer document, string localName) => document.Descendants()
        .FirstOrDefault(x => x.Name.LocalName.Equals(localName, StringComparison.OrdinalIgnoreCase))?.Value?.Trim();
    private static string? ChildValue(XElement? element, string localName) => element?.Elements()
        .FirstOrDefault(x => x.Name.LocalName.Equals(localName, StringComparison.OrdinalIgnoreCase))?.Value?.Trim();
}
