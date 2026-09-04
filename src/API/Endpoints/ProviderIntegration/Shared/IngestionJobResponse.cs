using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IngestionJob;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.Shared;
using IngestionJobKind = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJobKind;
using IngestionJobStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJobStatus;
public sealed record IngestionJobResponse(Guid Id, Guid FeedId, Guid FarmId, IngestionJobKind Kind,
    string IdempotencyKey, IngestionJobStatus Status, DateTimeOffset RequestedAt, string RequestedBy,
    DateTimeOffset AvailableAt, int Attempts, int MaximumAttempts, DateTimeOffset? LeasedUntil,
    DateTimeOffset? LastClaimedAt, DateTimeOffset? CompletedAt, DateTimeOffset? WindowStart,
    DateTimeOffset? WindowEnd, Guid? PreviousJobId, JsonElement? ResultCheckpoint,
    string? LastErrorCode, string? LastErrorMessage, uint Version);
