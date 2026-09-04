using System.Text.Json;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record ProviderRecord(
    string ExternalRecordId,
    DateTimeOffset ObservedAt,
    DateTimeOffset? SourceModifiedAt,
    string PayloadJson,
    ProviderNormalizedRecord Normalized);
