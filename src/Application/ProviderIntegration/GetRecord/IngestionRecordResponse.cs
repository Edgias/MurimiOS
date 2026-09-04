using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IngestionJob;
using Microsoft.EntityFrameworkCore;
using EdgIQ.Dispatcher;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
namespace EdgIQ.MurimiOS.Application.ProviderIntegration.GetRecord;
using IngestionJob = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJob;

using IngestionQuality = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionQuality;
using IngestionRecord = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecord;
using IngestionRecordStatus = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionRecordStatus;
public sealed record IngestionRecordResponse(Guid Id, Guid JobId, Guid FeedId, Guid FarmId,
    string ExternalRecordId, int SchemaVersion, DateTimeOffset ObservedAt, DateTimeOffset ReceivedAt,
    DateTimeOffset? SourceModifiedAt, string ContentSha256, IngestionQuality Quality,
    IngestionRecordStatus Status, DateTimeOffset? ProcessedAt, string? RejectionCode,
    string? RejectionReason, JsonElement? Payload);
