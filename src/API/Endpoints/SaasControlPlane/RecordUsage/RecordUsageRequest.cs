using System.Data;
using System.Text.Json;

using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.SaasControlPlane.RecordUsage;
public sealed record RecordUsageRequest(Guid? Id, string IdempotencyKey, string MetricCode, decimal Quantity, string Unit, DateTimeOffset? OccurredAt, string Source, JsonElement Dimensions);
