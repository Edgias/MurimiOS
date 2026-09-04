using System.Data;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockRecords.GetHistory;
public sealed record LivestockHistoryItem(Guid RecordId, string EventType, DateTimeOffset OccurredAt, string? Summary);
