using System.Data;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.ClaimJob;
using IngestionJobResponse = global::EdgIQ.MurimiOS.API.Endpoints.ProviderIntegration.Shared.IngestionJobResponse;
public sealed record ClaimedIngestionJobResponse(IngestionJobResponse Job, Guid LeaseId, DateTimeOffset LeasedUntil);
