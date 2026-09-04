using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IngestionJob;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ReceiveRecords;
using IngestionQuality = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionQuality;
public sealed record IngestionRecordRequest(Guid? Id, string ExternalRecordId, int SchemaVersion,
    DateTimeOffset ObservedAt, DateTimeOffset? SourceModifiedAt, IngestionQuality Quality, JsonElement Payload);
