using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IngestionJob;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.Shared;
using IngestionQuality = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionQuality;
using IngestionRecordStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecordStatus;
public sealed record IngestionRecordResponse(Guid Id, Guid JobId, Guid FeedId, Guid FarmId,
    string ExternalRecordId, int SchemaVersion, DateTimeOffset ObservedAt, DateTimeOffset ReceivedAt,
    DateTimeOffset? SourceModifiedAt, string ContentSha256, IngestionQuality Quality,
    IngestionRecordStatus Status, DateTimeOffset? ProcessedAt, string? RejectionCode,
    string? RejectionReason, JsonElement? Payload);
