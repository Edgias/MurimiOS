using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.CompleteJob;
public sealed record CompleteIngestionJobRequest(Guid LeaseId, JsonElement? Checkpoint, int? HttpStatusCode,
    int RequestUnits, decimal Cost, string? Currency, int? RateLimitRemaining, DateTimeOffset? RateLimitResetAt);
