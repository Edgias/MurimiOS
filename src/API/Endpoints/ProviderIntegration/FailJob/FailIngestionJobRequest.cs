using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.FailJob;
public sealed record FailIngestionJobRequest(Guid LeaseId, string ErrorCode, string ErrorMessage,
    DateTimeOffset? RetryAt, int? HttpStatusCode, int RequestUnits, decimal Cost, string? Currency);
