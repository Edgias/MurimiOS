using System.Data;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;

namespace EdgIQ.MurimiOS.API.Endpoints.LivestockExchange.ListDeadlines;
public sealed record RegulatoryDeadlineResponse(string ExchangeType, Guid RecordId, Guid FarmId, Guid SubjectId,
    string ProviderCode, DateTimeOffset DueAt, bool IsOverdue, string Status);
