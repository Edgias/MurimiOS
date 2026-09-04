using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.RegularExpressions;

namespace EdgIQ.MurimiOS.Application.Common.Traceability;
public sealed record EpcisCapturedEnvelope(string EventId, string EventType, DateTimeOffset EventTime,
    DateTimeOffset? RecordTime, string? BusinessStep, string? Disposition,
    IReadOnlyList<string> ObjectIdentifiers, string Sha256, string PayloadJson);
