using System.Text.Json;

namespace EdgIQ.MurimiOS.Application.Common.Abstractions;

public sealed record ProviderIngestionResult(
    IReadOnlyList<ProviderRecord> Records,
    string? CheckpointJson,
    int? HttpStatusCode = 200,
    int RequestUnits = 1,
    int? RateLimitRemaining = null,
    DateTimeOffset? RateLimitResetAt = null);
