using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.GetJob;
using IngestionJobResponse = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.Shared.IngestionJobResponse;
public sealed record IngestionJobDetailResponse(IngestionJobResponse Job, IReadOnlyList<IngestionAttemptResponse> Attempts,
    int ReceivedRecords, int ProcessedRecords, int RejectedRecords, decimal ProviderCost);
