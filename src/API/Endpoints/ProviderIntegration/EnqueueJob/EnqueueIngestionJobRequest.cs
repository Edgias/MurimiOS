using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IngestionJob;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.EnqueueJob;
using IngestionJobKind = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionJobKind;
public sealed record EnqueueIngestionJobRequest(Guid? Id, IngestionJobKind Kind, string IdempotencyKey,
    DateTimeOffset? AvailableAt, int? MaximumAttempts, DateTimeOffset? WindowStart, DateTimeOffset? WindowEnd);
