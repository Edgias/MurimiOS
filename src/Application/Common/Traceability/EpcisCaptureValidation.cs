using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace EdgIQ.MurimiOS.Application.Common.Traceability;
public sealed record EpcisCaptureValidation(JsonElement? Document, string? DocumentId, string? Sender,
    string? Receiver, IReadOnlyList<EpcisCapturedEnvelope> Events, IReadOnlyList<EpcisValidationIssue> Issues);
