using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ReplayJob;
public sealed record ReplayIngestionJobRequest(Guid? Id, string IdempotencyKey, DateTimeOffset? AvailableAt,
    int? MaximumAttempts, DateTimeOffset? WindowStart, DateTimeOffset? WindowEnd);
