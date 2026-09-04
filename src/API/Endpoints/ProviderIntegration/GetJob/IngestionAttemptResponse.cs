using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using EdgIQ.MurimiOS.Domain.IngestionJob;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.GetJob;
using IngestionAttemptOutcome = global::EdgIQ.MurimiOS.Domain.IngestionJob.IngestionAttemptOutcome;
public sealed record IngestionAttemptResponse(Guid Id, int AttemptNumber, DateTimeOffset StartedAt,
    DateTimeOffset CompletedAt, IngestionAttemptOutcome Outcome, int? HttpStatusCode, int RequestUnits,
    decimal Cost, string? Currency, string? ErrorCode, string? ErrorMessage);
